using Realms;

namespace DDRTracker.Models
{
    /// <summary>
    /// Model for the DDR Tracker Application.
    /// </summary>
    public class Song : RealmObject
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }
        
        public int Score { get; set; }
    }
}
