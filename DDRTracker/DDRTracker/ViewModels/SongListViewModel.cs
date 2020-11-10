using DDRTracker.Helpers;
using DDRTracker.Models;
using DDRTracker.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DDRTracker.ViewModels
{
    /// <summary>
    /// ViewModel to display a list of songs from the datastore.
    /// Note: Turn this into a Singleton as there should only be one instance of this class ever at runtime.
    /// </summary>
    class SongListViewModel : DataStoreViewModelBase<Song, string>
    {
        #region SelectedSong
        Song _selectedSong;
        public Song SelectedSong
        {
            get { return _selectedSong; }
            set 
            {
                SetField(ref _selectedSong, value, null, OnSongSelected);
            }
        }
        #endregion

        public ObservableCollection<Song> Songs { get; }

        #region Commands
        public Command LoadSongsCommand { get; }
        public Command<Song> SongTapped { get; }
        #endregion

        public SongListViewModel()
        {
            Title = "Song List";
            Songs = new ObservableCollection<Song>();
            LoadSongsCommand = new Command(async () => await ExecuteLoadSongsCommand());
            SongTapped = new Command<Song>(OnSongSelected);
        }

        /// <summary>
        /// When the page first appears, intialize some fields.
        /// </summary>
        public void OnAppearing()
        {
            IsBusy = true;
            SelectedSong = null;
        }

        /// <summary>
        /// Loads the songs into a viewable list.
        /// </summary>
        /// <returns></returns>
        async Task ExecuteLoadSongsCommand()
        {
            IsBusy = true;

            try
            {
                Songs.Clear();
                var songs = await DataStore.GetAllAsync(true);
                foreach (var song in songs)
                {
                    Songs.Add(song);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not load songs from data store.");
                Debug.WriteLine(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// If a song is selected, move user into a detail page with more info about said song.
        /// </summary>
        /// <param name="song">Selected Song</param>
        async void OnSongSelected(Song song)
        {
            if (song == null) 
            {
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(SongDetailPage)}?{nameof(SongDetailViewModel.SongId)}={song.Id}");
        }
    }
}
