using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_transcript_service.Model
{
    public class HttpResponseDto
    {
        public bool WasRequestSuccessful { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string Result { get; set; } = string.Empty;
    }

}
