using DDRTracker.Helpers;
using DDRTracker.Models;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace DDRTracker.ViewModels
{
    /// <summary>
    /// ViewModel for a Song Detail Page. Allows a user to view a song's information: Name, Score, etc. Able to change existing score data from here.
    /// Note: Consider turning Commands into ICommands and turning them into their respective equivalents here. Mouse over the changeCanExecute to get what the event should really be called.
    /// </summary>
    [QueryProperty(nameof(SongId), nameof(SongId))] // Identifier/Route of this class. First Argument is the public property name from this class. Second argument is the parameter name used in the URL by the navigation.
    class SongDetailViewModel : DataStoreViewModelBase<Song, string>
    {
        #region SongID
        // This is the parameters as given by navigation. Parameters are given in string format and must be converted if data type is different.
        string _songId;
        public string SongId
        {
            set 
            { 
                SetField(ref _songId, value, null, LoadBySongId);
            }
        }
        #endregion

        public int Id { get; set; }
        #region Name
        string _name;
        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }
        #endregion

        #region Score
        int _score;
        public int Score
        {
            get { return _score; }
            set { SetField(ref _score, value); }
        }
        #endregion

        #region Commands
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        #endregion

        public SongDetailViewModel()
        {
            SaveCommand = new Command(OnUpdate);
            CancelCommand = new Command(OnCancel);
        }

        /// <summary>
        /// Goes back to the previous page.
        /// </summary>
        async void OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        /// <summary>
        /// Updates the corresponding song entry with the fields provided. This method should only be called when there are values (0 or not) for the song instance.
        /// </summary>
        async void OnUpdate()
        {
            IsBusy = true;

            try
            {
                Song updatedSong = new Song()
                {
                    Id = Id,
                    Name = Name,
                    Score = Score
                };

                await DataStore.UpdateAsync(updatedSong);

            } 
            catch (Exception)
            {
                Debug.WriteLine("Could not convert string into a corresponding integer.");
                return;
            }
            finally
            {
                IsBusy = false;
            }
            
            await Shell.Current.GoToAsync("..");
        }

        /// <summary>
        /// Setup/Grab the info to populate the page with title, score, etc. of the given song using its ID.
        /// </summary>
        /// <param name="songId">ID of the song you wish to see more details about.</param>
        public async void LoadBySongId(string songId)
        {
            IsBusy = true;

            try
            {
                var song = await DataStore.GetAsync(songId);

                Id = song.Id;
                Name = song.Name;
                Score = song.Score;
            }
            catch(Exception)
            {
                Debug.WriteLine("Failed to Load Song");
                await Application.Current.MainPage.DisplayAlert("No Song", ":( Could not load song with given ID", "OK");
                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsBusy = false;
            }

        }
    }
}
