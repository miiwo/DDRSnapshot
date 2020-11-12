using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DDRTracker.Services
{
    /// <summary>
    /// Interface specifying what a ML photo analyzer should do. This should work if I ever want to implement my own ML.
    /// </summary>
    public interface IPhotoProcessor
    {
        IDictionary<string, string> HashMap { get; }
        Task<IDictionary<string, string>> ProcessPictureInfoAsync(FileResult photo, IEnumerable<(string Key, Regex Rgx, bool AlreadyFound)> tupleList);
        void ClearData();
    }
}
