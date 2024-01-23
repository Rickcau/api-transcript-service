using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using api_transcript_service.Util;
using api_transcript_service.Service;
using System.Reflection.Metadata;
using System.Text;

namespace api_transcript_service.Functions
{
    public class ParseTranscriptFunction
    {
        private readonly ILogger<ParseTranscriptFunction> _logger;
        private readonly Kernel _kernel;
        private readonly AIHelper _aiHelper;
        private static BlobHelper? _blobHelper;
        private static string _transcriptSummary = "";
        private static string _blobConnection = Helper.GetEnvironmentVariable("BlobConnection");

        public ParseTranscriptFunction(ILogger<ParseTranscriptFunction> logger, Kernel kernel)
        {
            _logger = logger;
            _kernel = kernel;
            _aiHelper = new AIHelper(_kernel);
            _blobHelper = new BlobHelper()
            {
                Container = "output",
                ConnectionString = _blobConnection
            };
        }

        [Function(nameof(ParseTranscriptFunction))]
        public async Task Run([BlobTrigger("input/{name}", Connection = "BlobConnection")] Stream stream, string name)
        {
            var transcriptService = new Transcript(stream, name);
            // Read the file and condense the chat session
            transcriptService.ReformattedTranscript();
            // We need to pull the SessionId and Email from inbound file name for use later
            transcriptService.GetSessionIdAndEmail(name);
            // Write the reformatted session to blob, just because we can
            _transcriptSummary = transcriptService.Summary;

            string response_summary = "";
            if (_blobHelper != null && _aiHelper != null)
            {
                await using var streamSummary = new MemoryStream(Encoding.UTF8.GetBytes(transcriptService.Summary));
                await _blobHelper.WriteToBlobAsync(streamSummary, $"ReformattedTranscript#{transcriptService.SessionId}#{transcriptService.Email}#.txt");

                response_summary = await _aiHelper.GetSummaryAsync(_transcriptSummary);
                // Now we can write the Summary of the Condensed Transcript to a Blob Container 
                _blobHelper.Container = "summary";  // set the container to be the summary container
                await using var streamsummary = new MemoryStream(Encoding.UTF8.GetBytes(response_summary));
                await _blobHelper.WriteToBlobAsync(streamsummary, $"Summary#{transcriptService.SessionId}#{transcriptService.Email}#.txt");
            }

            // _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
