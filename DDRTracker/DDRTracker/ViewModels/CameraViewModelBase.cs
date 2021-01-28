using DDRTracker.Helpers;
using DDRTracker.Services;

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace DDRTracker.InterfaceBases
{
    /// <summary>
    /// This class represents what a page should do when using the Camera. It should be able to take pictures, and pick pictures from the gallery. 
    /// Also supports an option to display the image selected/taken in the app.
    /// TODO: Work on saving images to gallery
    /// </summary>
    public abstract class CameraViewModelBase : ObservableBase
    {
        protected FileResult rawImage;
        #region SelectedImage
        protected ImageSource _selectedImage = null;
        public ImageSource SelectedImage
        {
            get { return _selectedImage; }
            set { SetField(ref _selectedImage, value); }
        }
        #endregion

        #region Commands
        public ICommand TakePictureCommand { get; }
        public ICommand GalleryCommand { get; }
        #endregion

        IPhotoService PhotoAssistant => DependencyService.Get<IPhotoService>(); // Personal class that saves to platform-specific mobile devices storage.

        public CameraViewModelBase()
        {
            TakePictureCommand = new Command(PickPhotoFromCamera);
            GalleryCommand = new Command(PickPhotoFromGallery);
        }

        /// <summary>
        /// Takes a photo for use with the user's camera. If no camera is detected, does nothing.
        /// </summary>
        async void PickPhotoFromCamera()
        {
            if (!MediaPicker.IsCaptureSupported)
            {
                await Shell.Current.DisplayAlert("NO CAMERA", "No camera is available.", "OK");
                return;
            }

            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                SetPhoto(photo);

            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occured with taking a picture from camera.");
                Debug.WriteLine(e.Message);
                await Shell.Current.DisplayAlert("CAMERA", "Something happened while trying to take a picture.", "OK");
            }

        }

        /// <summary>
        /// Picks a photo from the user's gallery. If no photo is chosen, does nothing.
        /// </summary>
        async void PickPhotoFromGallery() 
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                SetPhoto(photo);

            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occured with taking a picture from gallery");
                Debug.WriteLine(e.Message);
                await Shell.Current.DisplayAlert("GALLERY", "Something happened while trying to grab a photo from the gallery.", "OK");
            }

        }

        // Helper function to set the photo in both taking a picture or picking one from the library.
        async void SetPhoto(FileResult photo)
        {
            if (photo == null)
            {
                await Shell.Current.DisplayAlert("NO PHOTO", "Something went wrong with grabbing the photo to use.", "OK");
                return;
            }

            rawImage = photo;

            // Get file stream and set it as photo.
            var newFile = Path.Combine(FileSystem.CacheDirectory, rawImage.FileName);
            Debug.WriteLine(newFile);

            using (var stream = await photo.OpenReadAsync())
            {
                using (var newStream = File.OpenWrite(newFile))
                {
                    await stream.CopyToAsync(newStream);
                }
                SelectedImage = ImageSource.FromFile(newFile);

            }

        }

        /// <summary>
        /// Save the photo obtained from this to the user's gallery.
        /// </summary>
        protected async void SavePhoto()
        {
            PhotoAssistant.SavePhoto(rawImage);
        }
    }
}
