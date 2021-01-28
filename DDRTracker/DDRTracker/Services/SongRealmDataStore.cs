using DDRTracker.Models;

using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

using Realms;
using Realms.Sync;

namespace DDRTracker.Services
{
    /// <summary>
    /// Concrete class that implements a data store using MongoDB as a cloud database. Currently stores it in mobile device using a Realm database.
    /// Note: Have this class throw exceptions.
    /// </summary>
    class RealmDataStore : IDataSource<Song, string>
    {
        readonly Realm realm = null;
        readonly Realms.Sync.App app = null;

        public RealmDataStore()
        {
            // Setup offline database
            var config = new RealmConfiguration("ddr-track-starter.realm");
            realm = Realm.GetInstance(config);

            // Test local dummy data
            InitializeSongs();
            // Setup online config
            app = Realms.Sync.App.Create(Constants.RealmAppID);
        }

        public void updateFromOnline()
        {
            
            var user = await app.LogInAsync(Credentials.Anonymous());

            // Connect to database and grab data directly
            var mongoClient = user.GetMongoClient(Constants.atlas_service);
            var dbSong = mongoClient.GetDatabase(Constants.db_name);
            var songList = dbSong.GetCollection<SongMetaData>(Constants.table_name).FindAsync();

            // Fill into local database
            try 
            {
                realm.Write(() => 
                {
                    foreach (SongMetaData smd in songList)
                    {
                        realm.Add(new Song
                        {
                            Name = smd.Name,
                            Score = 0,
                            OId = smd.Id,
                            Id = smd.IntId
                        });
                    }
                });
                
            } 
            catch (Exception e)
            {
                Debug.WriteLine("Something went wrong with putting the songs from the cloud mongoDB into the phone's DB. SongRealmDataStore.UpdateFromOnline");
                Debug.WriteLine(e.ToString());
            } 
            finally
            {
                // Log out once done.
                await user.LogOutAsync();
            }

        }

        /// <summary>
        /// Helper method to help me populate the song list at first.
        /// </summary>
        void InitializeSongs()
        {
            try
            {
                realm.Write(() =>
                {
                    realm.Add(new Song { OId = ObjectID.GenerateNewId(), Id = 1, Name = "Sakura Storm", Score = 230 });
                    realm.Add(new Song { OId = ObjectID.GenerateNewId(), Id = 2, Name = "ACES FOR ACES", Score = 1000 });
                    realm.Add(new Song { OId = ObjectID.GenerateNewId(), Id = 3, Name = "Be a Hero!", Score = 0 });
                    realm.Add(new Song { OId = ObjectID.GenerateNewId(), Id = 4, Name = "Emera", Score = 530 });
                });

            } 
            catch (Exception)
            {
                Debug.WriteLine("Something went wrong with initializing initial data in RealmDatabase.");
            };
        }

        public async Task<bool> AddAsync(Song item)
        {
            try
            {
                await realm.WriteAsync((tmpRealm) => tmpRealm.Add(item));

                return true;
            } 
            catch (Exception)
            {
                Debug.WriteLine("Something went wrong with trying to add into the database.");

                return false;
            }
            
        }

        public async Task<IEnumerable<Song>> GetAllAsync(bool forceRefresh = false)
        {
            try
            {
                var songs = realm.All<Song>().AsEnumerable<Song>();
                return await Task.FromResult(songs);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Something went wrong with getting all the songs from the datastore. SONGREALMDATASTORE.cs");
                Debug.WriteLine(e.Message);

                return Enumerable.Empty<Song>();
            }

        }

        public async Task<Song> GetAsync(string id)
        {
            var song = realm.All<Song>().Where(s => s.Id == int.Parse(id)).First();
            return await Task.FromResult(song);
        }

        public async Task<bool> RemoveAsync(string id)
        {
            try
            {
                await realm.WriteAsync((tmpRealm) =>
                {
                    var song = realm.All<Song>().Where(s => s.Id == int.Parse(id)).First();
                    tmpRealm.Remove(song);
                });

                return true;
            } 
            catch (Exception)
            {
                Debug.WriteLine("Something went wrong with trying to remove from the database.");

                return false;
            }
        }

        public async Task<bool> UpdateAsync(Song item)
        {
            try
            {
                await realm.WriteAsync((tmpRealm) => tmpRealm.Add(item, true));

                return true;
            }
            catch (Exception)
            {
                Debug.WriteLine("Something went wrong with trying to update into the database.");

                return false;
            }
        }

        /// <summary>
        /// Gets all songs that match or contains letters with respect to the query string. This function is case insensitive.
        /// </summary>
        /// <param name="queryString">string to search by</param>
        /// <returns>Collection of songs fulfilling the criteria.</returns>
        public async Task<IEnumerable<Song>> GetByName(string queryString)
        {
            var grabSongs = realm.All<Song>().ToList();
            var searchableSongs = grabSongs.Where((Song s) => s.Name.ToLower().Contains(queryString.ToLower())).ToList();
            
            return await Task.FromResult(searchableSongs);
        }
    }
}
