using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace api_transcript_service.Util
{
    public class AIHelper
    {
        private Kernel _kernel;
        private string _promptSummarize = @"SUMMARIZE THE CONVERSATION IN 20 BULLET POINTS OR LESS

        SUMMARY MUDT BE:
        - WORKPLACE / FAMILY SAFE NO SEXISM, RACISM OR OTHER BIAS/BIGOTRY
        - G RATED

        {{$input}}";


        public AIHelper(Kernel kernel)
        {
            this._kernel = kernel;
        }

        public async Task<string> GetSummaryAsync(string transcript)
        {
            var summarizeFunction = _kernel.CreateFunctionFromPrompt(_promptSummarize, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 100 });

            var response = await _kernel.InvokeAsync(summarizeFunction, new() { ["input"] = transcript });

            return response.GetValue<string>() ?? "";
        }

    }
}
