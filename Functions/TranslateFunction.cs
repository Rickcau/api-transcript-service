using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using api_transcript_service.Functions;
using api_transcript_service.Util;
using api_transcript_service.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Json.Schema.Generation.Intents;

namespace api_transcript_service.Functions
{
    public class TranslateFunction
    {
        private readonly ILogger _logger;
        private readonly Kernel _kernel;
        private static BlobHelper? _blobHelper;
        private readonly AIHelper _aiHelper;
        private static string _blobConnection = Helper.GetEnvironmentVariable("BlobConnection");


        public TranslateFunction(ILoggerFactory loggerFactory, Kernel kernel)
        {
            _logger = loggerFactory.CreateLogger<TranslateFunction>();
            _kernel = kernel;
            _aiHelper = new AIHelper(_kernel);
            _blobHelper = new BlobHelper()
            {
                Container = "translation",
                ConnectionString = _blobConnection
            };
        }

        [Function("TranslateFunction")]
        public async Task<HttpResponseDto> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            var httpResponseDto = new HttpResponseDto
            {
                StatusCode = 400,
                StatusMessage = "Error with Service Call",
                Result = ""
            };
           _logger.LogInformation("C# HTTP trigger Translate function processed a request.");

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var translationRequest = JsonSerializer.Deserialize<TranslationFileInfoRequest>(requestBody); 
                if (!string.IsNullOrEmpty(translationRequest.FileName))
                {
                    _logger.LogInformation("Valid Translation request Model");
                    // now we can call the AIHelper to translate the content
                    _blobHelper.Container = translationRequest.ContainerName;  // This is the container we will read the file from
                    var contentToTranslate = await _blobHelper.GetContentFromBlob(translationRequest.FileName);
                    var translatedcontent = await _aiHelper.GetTranslationAsync(contentToTranslate, translationRequest.TargetLanguage);
                    httpResponseDto.Result = translatedcontent;
                }
                httpResponseDto.StatusMessage = "Success";
                httpResponseDto.StatusCode = 200;
                return httpResponseDto;  // Return the translated content to the caller.
            }
            catch (ArgumentException e) 
            {
                httpResponseDto.Result = e.Message;
                httpResponseDto.StatusMessage = "Bad Argument";
                httpResponseDto.StatusCode = 400;
            }
            catch (InvalidOperationException e)
            {
                httpResponseDto.Result = e.Message;
                httpResponseDto.StatusMessage = "Invalid Operation";
                httpResponseDto.StatusCode = 400;
            }
            catch (Exception e) 
            {
                httpResponseDto.Result = e.Message;
                httpResponseDto.StatusMessage = "Generic Exception";
                httpResponseDto.StatusCode = 400;
            }
            return httpResponseDto;
        }
    }
}

