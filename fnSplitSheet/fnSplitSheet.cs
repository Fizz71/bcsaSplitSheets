using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace fnSplitSheet
{
    public class fnSplitSheet
    {
        private readonly ILogger _logger;

        public fnSplitSheet(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<fnSplitSheet>();
        }

        [Function("fnSplitSheet")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions - Split Sheet!");

            return response;
        }
    }
}
