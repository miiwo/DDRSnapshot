using DDRTracker.Models;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDRTracker.Services
{
    /// <summary>
    /// Concrete class that implements an in-memory repository for handling songs in the DDR Tracking Application. Used to test out that the architecture works.
    /// </summary>
    public class LocalSongDataStore : IDataSource<Song, string>
    {
        private readonly List<Song> songs = null;

        public LocalSongDataStore()
        {
            // Intialize songs in data source
            songs = new List<Song>()
            {
                new Song { Id=1, Name = "Sakura Storm", Score=230 },
                new Song { Id=2, Name = "ACES FOR ACES", Score=1000 },
                new Song { Id=3, Name = "Be a Hero!", Score=0 },
                new Song { Id=4, Name = "Emera", Score=530 }
            };
        }

        public async Task<bool> AddAsync(Song song)
        {
            songs.Add(song);

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<Song>> GetAllAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(songs);
        }

        public async Task<Song> GetAsync(string id)
        {
            int convertToStringId = int.Parse(id);
            return await Task.FromResult(songs.FirstOrDefault(song => song.Id == convertToStringId));
        }

        public async Task<bool> RemoveAsync(string id)
        {
            int convertToStringId = int.Parse(id);
            var song = songs.FirstOrDefault(songInDataStore => songInDataStore.Id == convertToStringId);
            songs.RemoveAll(s => s.Id == convertToStringId); // Consider changing this line to remove first instance

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateAsync(Song song)
        {
            var oldSong = songs.FirstOrDefault(songInDataStore => songInDataStore.Id == song.Id); // Should check what the default value is
            oldSong.Score = song.Score;

            return await Task.FromResult(true);
        }
    }
}
