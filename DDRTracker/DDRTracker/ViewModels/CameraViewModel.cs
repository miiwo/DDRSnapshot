using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using DDRTracker.Helpers;
using DDRTracker.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DDRTracker.ViewModels
{
    /// <summary>
    /// ViewModel for a Camera Page in the DDR Tracking Application. Has the ability to take a picture, pick a picture from gallery, 
    /// and process picture data into data that is storeable by the data store.
    /// Note: Close stream once done with it. When user isn't on page.
    /// </summary>
    class CameraViewModel : DataStoreViewModelBase<Song, string>
    {
        #region ResultImage
        ImageSource _resultImage = null;
        public ImageSource ResultImage
        {
            get { return _resultImage; }
            set { SetField(ref _resultImage, value); }
        }
        #endregion

        AmazonRekognitionClient arClient;

        #region Commands
        public Command TakePictureCommand { get; }
        public Command GalleryCommand { get; }
        public Command ProcessCommand { get; }
        #endregion

        public CameraViewModel()
        {
            TakePictureCommand = new Command(PickPhotoFromCamera);
            GalleryCommand = new Command(PickPhotoFromGallery);
            ProcessCommand = new Command(ProcessPictureInfo);

            //arClient = new AmazonRekognitionClient();
        }

        // Have it return results (a song object)
        /*private async Task<Song> ProcessPicture(byte[] picture) 
        {
            try
            {
                DetectTextRequest req = new DetectTextRequest()
                {
                    Image = { Bytes = picture }
                };
                DetectTextResponse res = await arClient.DetectTextAsync(req);

                // put results into a song object
                Song song = new Song 
                { 
                    Name = res.TextDetections[0].DetectedText, 
                    Score = int.Parse(res.TextDetections[1].DetectedText) 
                };
                return song;

            } catch (Exception)
            {
                Debug.WriteLine("Could not process picture");
            }

            return null;
            
        }*/

        /// <summary>
        /// Takes a photo for use with the user's camera. If no camera is detected, does nothing.
        /// </summary>
        async void PickPhotoFromCamera()
        {
            if (!MediaPicker.IsCaptureSupported)
            {
                await Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera is available", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                
                if (photo == null) //If user cancelled
                {
                    return;
                }

                // set the image to the corresponding thing onto the view.
                var stream = await photo.OpenReadAsync();
                ResultImage = ImageSource.FromStream(() => stream);
            }
            catch (Exception)
            {
                Debug.WriteLine("An error occured with taking a picture");
            }
            finally
            {
                IsBusy = false;
            }

        }
        /// <summary>
        /// Picks a photo from the user's gallery. If no photo is chosen, does nothing.
        /// </summary>
        async void PickPhotoFromGallery()
        {

            IsBusy = true;

            try
            {
                var photo = await MediaPicker.PickPhotoAsync();

                if (photo == null)
                {
                    return;
                }

                // Get file stream and set it as photo.
                var stream = await photo.OpenReadAsync();
                ResultImage = ImageSource.FromStream(() => stream);
            }
            catch (Exception)
            {
                Debug.WriteLine("An error occured with taking a picture");
                return;
            }
            finally
            {
                IsBusy = false;
            }

            
        }
        /// <summary>
        /// Reads a picture and turns the data in it into a Song Model for storage into the collection. If no valid object can be created, notifies the user that it cannot.
        /// </summary>
        void ProcessPictureInfo()
        {
            if (ResultImage == null)
            {
                return;
            }

            // Put song into database.
            //Song song = ProcessPicture(ResultImage);
            // If song is null, pop a dialogue saying cannot process image.
            /*if (Song == null)
            {
                await Application.Current.MainPage.DisplayAlert("No Song", ":( Could not convert photo into song data", "OK");
                return;
            }*/
            //Debug.WriteLine(song); // Check to make sure the results are okay.
            //DataStore.AddAsync(song);
        }
    }
}
