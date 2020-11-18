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

            // Setup online database
            app = Realms.Sync.App.Create(Constants.RealmAppID);
            var user = await app.LogInAsync(Credentials.Anonymous());
            var configTwo = new SyncConfiguration("PUBLIC", user);
            using (var realmTwo = await Realm.GetInstanceAsync(configTwo))
            {
                // Grab data from online and put into local database.
                var onlineSongs = realmTwo.All<RealmDataModel>();
                foreach(RealmDataModel rdm in onlineSongs)
                {
                    tmpRealm.Add((tmpRealm) => tmpRealm.Add(new Song() 
                    {
                        Name = rdm.Name,
                        Score = 0,
                        Id = rdm.IntId
                    }))
                }
            }
            
            

        }

        void IntializeSongs()
        {
            try
            {
                realm.Write(() =>
                {
                    realm.Add(new Song { Id = 1, Name = "Sakura Storm", Score = 230 });
                    realm.Add(new Song { Id = 2, Name = "ACES FOR ACES", Score = 1000 });
                    realm.Add(new Song { Id = 3, Name = "Be a Hero!", Score = 0 });
                    realm.Add(new Song { Id = 4, Name = "Emera", Score = 530 });
                });

            } catch (Exception)
            {
                Debug.WriteLine("Something went wrong with intializing initial data in RealmDatabase.");
            };
        }

        public async Task<bool> AddAsync(Song item)
        {
            try
            {
                await realm.WriteAsync((tmpRealm) => tmpRealm.Add(item));
                return true;

            } catch (Exception)
            {
                Debug.WriteLine("Something went wrong with trying to add into the database.");
                return false;
            }
            
        }

        public async Task<IEnumerable<Song>> GetAllAsync(bool forceRefresh = false)
        {
            var songs = realm.All<Song>().AsEnumerable<Song>();
            return await Task.FromResult(songs);

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
            } catch (Exception)
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
        /// <param name="queryString"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Song>> GetByName(string queryString)
        {
            var grabSongs = realm.All<Song>().ToList();
            var searchableSongs = grabSongs.Where((Song s) => s.Name.ToLower().Contains(queryString.ToLower())).ToList();
            return await Task.FromResult(searchableSongs);
        }
    }
}
