using DDRTracker.InterfaceBases;
using DDRTracker.Models;
using DDRTracker.Services;

using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Xamarin.Forms;

namespace DDRTracker.ViewModels
{
    /// <summary>
    /// ViewModel for a Camera Page in the DDR Tracking Application. Has the ability to take a picture, pick a picture from gallery, 
    /// and process picture data into data that is storeable by the data store. Singleton class. Don't add it to the abstract class.
    /// TODO: Check all of my awaits. If I need them or not.
    /// TODO: Check that my streams are being disposed of properly at the end when they are not in use.
    /// TODO: Remember to cleanup tupleList name and construction. And also learn to write regex better.
    /// TODO: Save file when can't process photo.
    /// </summary>
    public sealed class SongCameraViewModel : CameraViewModelBase
    {
        public static SongCameraViewModel Instance { get; } = new SongCameraViewModel(); // Singleton

        readonly IPhotoProcessor photoAnalyzer;

        public ICommand ProcessCommand { get; }

        readonly (string Key, Regex Rgx, bool AlreadyFound)[] detectFromPhotos;

        IDataSource<Song, string> DataStore => DependencyService.Get<IDataSource<Song, string>>();

        #region IsBusy
        bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetField(ref _isBusy, value); }
        }
        #endregion

        private SongCameraViewModel()
        {
            ProcessCommand = new Command(ProcessPictureInfo);

            photoAnalyzer = new AmazonPhotoProcesssor();

            detectFromPhotos = new (string Key, Regex Rgx, bool AlreadyFound)[]
            {
                ("NAME", new Regex(@"^[\d\.]+\b(.+)"), false), // Learn to make better regexes me ;-;
                ("SCORE", new Regex(@"^MARVELOUS (\d+)"), false)
            };
        }
 
        /// <summary>
        /// Using the image that the user has selected, attempt to analyze the photo and have it store into the database.
        /// </summary>
        async void ProcessPictureInfo()
        {
            if (SelectedImage == null)
            {
                await Shell.Current.DisplayAlert("NO PICTURE", "No picture to process into a song.", "OK");
                return;
            }

            photoAnalyzer.ClearData();
            var resultMap = await photoAnalyzer.ProcessPictureInfoAsync(rawImage, detectFromPhotos);
            Song song = ConvertToSong(resultMap);

            if (song != null) // Put song into database
            {
                bool confirmAddition = await Shell.Current.DisplayAlert("Question", $"Is this info correct?\nSong Name: {song.Name}\nScore: {song.Score}", "YES", "NO"); // Interpolated string
                if (confirmAddition) 
                {
                    await DataStore.AddAsync(song);
                }
            }
            else // There's not enough data to build a song, tell user to retake a better photo or manually input. Save picture if this is the case.
            {
                await Shell.Current.DisplayAlert("FAILURE", "Could not add the song to the database.\nPlease either retake the photo or manually input.", "OK");

                // Save file to gallery
                // Might require me to dig into android and ios specific things.
            }
        }

        /// <summary>
        /// Converts a dictionary into a Song model object. Helper function to parse the photo processor.
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        Song ConvertToSong(IDictionary<string, string> dict)
        {

            if (dict.TryGetValue("NAME", out _) &&
                dict.TryGetValue("SCORE", out _))
            {
                if (int.TryParse(dict["SCORE"], out _))
                {
                    return new Song()
                    {
                        Id = 8,
                        Name = dict["NAME"],
                        Score = int.Parse(dict["SCORE"])
                    };
                } 
                else
                {
                    Debug.WriteLine("Could not turn score string into an int.\n");
                    Debug.WriteLine($"Current score tag is: {dict["SCORE"]}\n");
                }
            }

            return null;

        }
        
    }
}
