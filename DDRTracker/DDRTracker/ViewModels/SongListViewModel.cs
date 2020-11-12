using DDRTracker.Models;
using DDRTracker.Services;
using DDRTracker.Views;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DDRTracker.ViewModels
{
    /// <summary>
    /// ViewModel to display a list of songs from the datastore. Singleton as there should only be one instance of this class ever at runtime. Master list for a user.
    /// Note: Check my overrides access modifiers.
    /// </summary>
    public sealed class SongListViewModel : ListViewModelBase<Song, string>
    {
        public static SongListViewModel Instance { get; } = new SongListViewModel();

        IDataSource<Song, string> DataStore => DependencyService.Get<IDataSource<Song, string>>();

        private SongListViewModel() {}

        public override async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                ItemList.Clear();

                var songs = await DataStore.GetAllAsync(true);
                foreach (var song in songs)
                {
                    ItemList.Add(song);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not load songs from data store.");
                Debug.WriteLine(e);
                await Shell.Current.DisplayAlert("SONG LIST", "Could not load songs from database.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override async void OnItemSelected(Song item)
        {
            if (item == null) 
            {
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(SongDetailPage)}?{nameof(SongDetailViewModel.SongId)}={item.Id}");
        }

    }
}
