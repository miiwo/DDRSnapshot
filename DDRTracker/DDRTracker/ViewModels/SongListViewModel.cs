using DDRTracker.Models;
using DDRTracker.Services;
using DDRTracker.Views;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace DDRTracker.ViewModels
{
    /// <summary>
    /// ViewModel to display a list of songs from the datastore. Master list for a user.
    /// TODO: Along with SongDetail, have the URI be the objectID instead.
    /// TODO: Make the DataStore a DI.
    /// </summary>
    public sealed class SongListViewModel : ListViewModelBase<Song, string>
    {
        bool shouldUpdateFromCloud = false;

        IDataSource<Song, string> DataStore => DependencyService.Get<IDataSource<Song, string>>();

        public SongListViewModel() 
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                shouldUpdateFromCloud = true;
            }
        }

        public override async Task LoadItems()
        {
            IsBusy = true;

            try
            {
                ItemList.Clear();


                if (shouldUpdateFromCloud)
                {
                    try
                    {
                        await ((SongRealmDataStore)DataStore).UpdateFromOnline();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Error in grabbing songs from the network");
                    }
                    finally
                    {
                        shouldUpdateFromCloud = false;
                    }
                }


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

                var filteredsongs = string.IsNullOrWhiteSpace(query) ? await DataStore.GetAllAsync(true) : await ((SongRealmDataStore)DataStore).GetByName(query);
                foreach(var song in filteredsongs)
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
