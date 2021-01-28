using Xamarin.Essentials;

namespace DDRTracker.Services
{
    interface IPhotoService
    {
        void SavePhoto(FileResult photo); // Consider putting name as part of parameter.
    }
}