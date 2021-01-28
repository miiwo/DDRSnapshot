using System.Text.RegularExpressions;

namespace DDRTracker.PhotoProcessors
{
    public struct ProcessorOptions
    {
        public ProcessorOptions(string key, Regex rgx, bool alreadyFound)
        {
            Key = key;
            Rgx = rgx;
            Found = alreadyFound;
        }

        public string Key { get; set; }
        public Regex Rgx { get; set; }
        public bool Found { get; set; }
    }
}