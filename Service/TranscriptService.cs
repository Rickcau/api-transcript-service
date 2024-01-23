using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace api_transcript_service.Service
{
    public class Transcript
    {
        // A property that stores the file name
        public string FileName { get; set; }

        // A property that stores the summary of the conversation
        public string Summary { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // private string _fileName;
        private Stream _fileStream;

        // A constructor that takes a file name as a parameter
        public Transcript(ref Stream fileStream, string fileName)
        {
            // Assign the file name to the property
            FileName = fileName;
            _fileStream = fileStream;
        }

        public void GetSessionIdAndEmail(string fileName)
        {
            string[] parts = fileName.Split('#');
            string guid = parts[1];
            string email = Regex.Match(parts[2], @"^[^#]+").Value;
            SessionId = guid;
            Email = email;
        }

        public bool ReformattedTranscript()
        {
            // Create a stream reader object to read the stream
            StreamReader reader = new StreamReader(_fileStream);

            // Initialize the summary as an empty string
            Summary = "";

            // Initialize the previous speaker name as an empty string
            string prevSpeaker = "";
            string speaker = "";
            string message = "";

            // Initialize a counter to keep track of the line number
            int counter = 0;
            // Loop until the end of the stream
            // Loop until the end of the stream
            while (!reader.EndOfStream)
            {
                // Read a line from the stream
                counter++;
                string line = reader.ReadLine() ?? "";

                // Check if the line contains a time stamp
                if (Regex.IsMatch(line, @"\d+:\d+:\d+\.\d+ --> \d+:\d+:\d+\.\d+"))
                {
                    // Skip the line
                    continue;
                }
                // Remove the unnecessary whitespace
                line = Regex.Replace(line, @"\s+", " ");
                // if counter is 2 we have speaker name
                if (counter == 2)
                {
                    speaker = line;
                    if (speaker != prevSpeaker)
                    {
                        // Add a colon and the speaker name to the summary
                        Summary += ": " + speaker;
                    }
                    // Update the previous speaker name
                    prevSpeaker = speaker;
                    continue;

                }
                // if counter is 3 we have a message
                if (counter == 3)
                {
                    message = line;
                    // Add a colon and the message to the summary
                    Summary += ": " + message;
                    counter = 0;
                }
                // Update the previous speaker name
                prevSpeaker = speaker;
            }
            reader.Close();
            return true;
        }
    }
}
