using MongoDB.Bson;
using Realms;

namespace DDRTracker.Models
{
    public class RealmDataModel : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId? Id {get; set; }

        [MapTo("_int_id")]
        public int? IntId {get; set; }

        [MapTo("name")]
        [Required]
        public string Name {get; set; }

        [MapTo("score")]
        public int Score {get; set; }
    }
}