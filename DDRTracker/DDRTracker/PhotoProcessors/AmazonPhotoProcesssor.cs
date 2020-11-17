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
    /// Note: Change my regexes to be better and change the match group.
    /// Note: Consider making this a singleton because I only want one instance connecting at a time.
    /// </summary>
    public class AmazonPhotoProcesssor : IPhotoProcessor
    {
        readonly AmazonRekognitionClient arClient;
        public IDictionary<string, string> HashMap { get; }

        readonly float confidenceThreshold; // Consider doing a getter and setter for this if the user ever wants to based on their own pref.

        /// <summary>
        /// Constructor for AmazonPhotoProcessor. Intializes values to be setup when using this class.
        /// </summary>
        public AmazonPhotoProcesssor()
        {
            // Initialize the Amazon Cognito credentials provider
            CognitoAWSCredentials credentials = new CognitoAWSCredentials(
                Constants.AmazonCredentials, // Identity pool ID
                RegionEndpoint.USEast2 // Region
            );

            arClient = new AmazonRekognitionClient(credentials, RegionEndpoint.USEast2);

            HashMap = new Dictionary<string, string>();

            confidenceThreshold = (float)0.75;
        }

        /// <summary>
        /// Reads a picture and returns the data obtained from it into a dictionary with key-value pairs.
        /// </summary>
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
        /// Checks if the fields that need to be filled in as given by the user matches with what the processor grabs. This is a pre-optimization to ensuring that all fields are filled in. Consider removing it.
        /// </summary>
        /// <param name="targetFields">user provides the key fields that need to be filled</param>
        /// <returns></returns>
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
