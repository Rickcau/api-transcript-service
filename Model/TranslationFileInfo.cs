using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace api_transcript_service.Model
{
   public class TranslationFileInfoRequest
    {
        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;
        
        [JsonPropertyName("containerName")]
        public string ContainerName { get; set; } = string.Empty;

        [JsonPropertyName("targetLanguage")]
        public string TargetLanguage { get; set; } = string.Empty;
    }

}
