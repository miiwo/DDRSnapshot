using MongoDB.Bson;
using Realms;

namespace DDRTracker.Models
{
    // Consider now that in the cloud database have the stepchart, and have the song only contain pattern/bpm info.
    // ORM of the song meta data obtained from the cloud MongoDB. 
    // Consider removing the MapTo as that's for Realm Sync. Also remove Score from cloud db as its not needed.
    public class SongMetaData : RealmObject
    {
        [PrimaryKey]
        [BsonElement("_id")]
        [MapTo("_id")]
        public ObjectId Id {get; set; }

        [MapTo("_int_id")]
        [BsonElement("id")]
        public int IntId {get; set; }

        [MapTo("name")]
        [BsonElement("name")]
        [Required]
        public string Name {get; set; }

        [MapTo("score")]
        [BsonElement("score")]
        public int Score {get; set; }
    }
}