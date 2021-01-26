using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Essentials;

namespace DDRTracker.Services
{
    /// <summary>
    /// Proccesses information from a photo into key-value pairs using Amazon's Rekognition system. Decoupled to be used for any photo analyzation.
    /// Note: Consider making this a singleton because I only want one instance connecting at a time. Also because I have limited access to how many times I can scan. If I didn't have a limit, remove Singleton.
    // But still make it lazy because we won't ever really need to use it unless used.
    /// Note: Rewrite code for the tuple list (specifically the readonly problem)
    /// Note: Consider making the tuple into its own class.
    /// Note: Along with whatever is implementing this class. Change how it matches (match groups)
    /// </summary>
    public sealed class AmazonPhotoProcesssor : IPhotoProcessor
    {
        public static readonly Lazy<AmazonPhotoProcessor> Instance { get; } = new Lazy<AmazonPhotoProcessor>(() => new AmazonPhotoProcessor());

        readonly AmazonRekognitionClient arClient;
        public IDictionary<string, string> HashMap { get; }

        readonly float confidenceThreshold; // Consider doing a getter and setter for this if the user ever wants to based on their own pref.

        /// <summary>
        /// Constructor for AmazonPhotoProcessor. Intializes values to be setup when using this class.
        /// </summary>
        private AmazonPhotoProcesssor()
        {
            // Initialize the Amazon Cognito credentials provider
            CognitoAWSCredentials credentials = new CognitoAWSCredentials(
                Constants.AmazonCredentials, // Identity pool ID, stored in a separate class :X
                RegionEndpoint.USEast2 // Region
            );

            arClient = new AmazonRekognitionClient(credentials, RegionEndpoint.USEast2);

            HashMap = new Dictionary<string, string>();

            confidenceThreshold = (float)0.75;
        }

        /// <summary>
        /// Reads a picture and returns the data obtained from it into a dictionary with key-value pairs.
        /// For this specifically, items in a picture are connected (ie. fruit : apple), except its for
        /// (judgement, values) in the game. This will return the appropriate pairing. Until I rework this
        /// to be able to grab both the judgement and its value into one var.
        /// </summary>
        /// <param name="photo">photo file to be analyzed</param>
        /// <param name="tupleList">list containing what to search for in the image</param>
        /// <returns></returns>
        public async Task<IDictionary<string, string>> ProcessPictureInfoAsync(FileResult photo, IEnumerable<(string Key, Regex Rgx, bool AlreadyFound)> tupleList)
        {
            try
            {
                DetectTextRequest req = new DetectTextRequest
                {
                    Image = new Image()
                };

                // Grab image bytes
                using (var memoryStream = new MemoryStream())
                {
                    var stream = await photo.OpenReadAsync();
                    stream.CopyTo(memoryStream);
                    req.Image.Bytes = memoryStream;

                }

                DetectTextResponse res = await arClient.DetectTextAsync(req);

                foreach (TextDetection text in res.TextDetections)
                {
                    if (text.Confidence < confidenceThreshold) // Yeet the words its not confident in
                    {
                        continue;
                    }

                    // REGEX MATCHING TIME (Consider having 2 lists: not found and already found regexes, so it doesn't repeat over)
                    using(var iteratorTuple = tupleList.GetEnumerator()) // Using statement simplfies variables that need to be disposed of at the end.
                    {
                        while (iteratorTuple.MoveNext())
                        {
                            (string Key, Regex Rgx, bool AlreadyFound) = iteratorTuple.Current;

                            // Fix this so I can iterate through while editing the AlreadyFound part of the tuple T-T

                            Match match = Rgx.Match(text.DetectedText);

                            if (match.Success)
                            {
                                if (!HashMap.ContainsKey(Key)) 
                                {
                                    HashMap.Add(Key, match.Groups[1].Value);
                                    Debug.WriteLine($"{Key} IS NOW SET TO: {HashMap[Key]}"); // Debug Statement
                                    AlreadyFound = true; // This does not work because this enumerator is read-only
                                }
                                
                                break;
                            }

                        }

                    }

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could not process picture into a song.\n");
                Debug.WriteLine(e.Message);
            }

            return HashMap; // return this as a read only dictionary
        }

        /// <summary>
        /// Checks if the fields that need to be filled in as given by the user matches with what the processor grabs. 
        /// This is a pre-optimization to ensuring that all fields are filled in. Consider removing it.
        /// </summary>
        /// <param name="targetFields">user provides the key fields that need to be filled</param>
        /// <returns>boolean indicating if all fields have values in them</returns>
        public bool ValidateAllInfo(string[] targetFields)
        {
            foreach(string s in targetFields)
            {
                if (!HashMap.ContainsKey(s))
                {
                    return false;
                }
            }

            return true;
        }

        public void ClearData()
        {
            HashMap.Clear();
        }
    }
}
