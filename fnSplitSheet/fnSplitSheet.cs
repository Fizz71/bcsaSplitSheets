using System.IO.Compression;
using System.Net;
using HttpMultipartParser;
using Microsoft.AspNetCore.Http;
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
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP fnSplitSheet Started.");

            var parsedFormBody = await MultipartFormDataParser.ParseAsync(req.Body);
            var inFile = parsedFormBody.Files[0];
            if (!inFile.FileName.ToLower().EndsWith(".zip"))
            {
                var noGood = req.CreateResponse(HttpStatusCode.BadRequest);
                noGood.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                noGood.WriteString($"Invalid File Extension - {inFile.FileName}");
                return noGood;
            }

            var outFile = Path.Combine(Path.GetTempPath(), inFile.FileName);
            using (var fileStream = new FileStream(outFile, FileMode.Create))
                await inFile.Data.CopyToAsync(fileStream);

            using (FileStream zipToOpen = new FileStream(outFile, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.Name.EndsWith("HY3"))
                        {
                            using (var stream = entry.Open())
                            using (var reader = new StreamReader(stream))
                            {
                                var foundHY3 = req.CreateResponse(HttpStatusCode.BadRequest);
                                foundHY3.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                                foundHY3.WriteString($"Found HY3 File - {inFile.FileName}");
                                return foundHY3;
                            }
                        }
                    }
                }
            }

            if (System.IO.File.Exists(outFile)) System.IO.File.Delete(outFile);

            var noHY3 = req.CreateResponse(HttpStatusCode.BadRequest);
            noHY3.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            noHY3.WriteString($"No HY3 File Found in Zip File - {inFile.FileName}");
            return noHY3;

            //var formdata = await req.

            //Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "uploads"));
            //var file = Path.Combine(Path.GetTempPath(), "uploads", Upload.FileName);
            //using (var fileStream = new FileStream(file, FileMode.Create))
            //{
            //    //todo: Validate file size and extention.
            //    await Upload.CopyToAsync(fileStream);
            //}

            //using (FileStream zipToOpen = new FileStream(file, FileMode.Open))
            //{
            //    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
            //    {
            //        foreach (var entry in archive.Entries)
            //        {
            //            if (entry.Name.EndsWith("HY3"))
            //            {
            //                using (var stream = entry.Open())
            //                using (var reader = new StreamReader(stream))
            //                {
            //                    Console.WriteLine(_hy3.ParseHY3File(reader));
            //                }
            //            }
            //        }
            //    }
            //}

            //if (System.IO.File.Exists(file)) System.IO.File.Delete(file);




            //var response = req.CreateResponse(HttpStatusCode.OK);
            //response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            //response.WriteString("Welcome to Azure Functions - Split Sheet - Round 2!");

            //return response;
        }
    }
}
