using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Essentials;

namespace DDRTracker.Services
{
    /// <summary>
    /// Interface specifying what an AI photo analyzer should do. This should work if I ever want to implement my own ML.
    /// </summary>
    public interface IPhotoProcessor
    {
        IDictionary<string, string> HashMap { get; } // Data structure to hold my results. Ensures no need to rerun again unless necessary by the user.

        /// <summary>
        /// Method used to parse data and obtain certain info (as dictated by regexes) to search for.
        /// </summary>
        Task<IDictionary<string, string>> ProcessPictureInfoAsync(FileResult photo, IEnumerable<ProcessorOptions> tupleList);
        
        /// <summary>
        /// Method used to clear the data the photo processor obtained.
        /// </summary>
        void ClearData();
    }
}
