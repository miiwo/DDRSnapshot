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
    /// Note: Put Searchbar behavior thing into viewmodel.
    /// Note: I need to refresh in order for the songs to show up. Fix this somehow.
    /// TODO: Along with SongDetail, have the URI be the objectID instead.
    /// </summary>
    public sealed class SongListViewModel : ListViewModelBase<Song, string>
    {
        public static readonly SongListViewModel Instance { get; } = new SongListViewModel(); // Singleton

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
                Debug.WriteLine(e.Message);
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

        public override async void PerformSearch(string query) 
        {
            IsBusy = true;

            try
            {
                ItemList.Clear();

                var songs = string.IsNullOrWhiteSpace(query) ? await DataStore.GetAllAsync(true) : await ((SongRealmDataStore)DataStore).GetByName(query);
                foreach(var song in songs)
                {
                    ItemList.Add(song);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("SongListViewModel: Could not PerformSearch() with query: '{0}'.", query));
                Debug.WriteLine(e.Message);
            }
            finally 
            {
                IsBusy = false;
            }
        }

    }
}
