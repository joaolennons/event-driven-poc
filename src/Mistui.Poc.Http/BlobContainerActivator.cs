using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Mitsui.Poc.Http
{
    public static class BlobContainerActivator
    {
        [FunctionName("BlobContainerActivator")]
        public static async Task Run([BlobTrigger("sql-blobcontainer/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, ILogger log)
        {
            var streamReader = new StreamReader(myBlob);
            var streamContent = await streamReader.ReadToEndAsync();

            log.LogInformation(streamContent);
            throw new Exception();
        }
    }
}