using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.Client;

namespace Insurance.Query.Endpoints
{
    public static class QuotationsHttp
    {
        [FunctionName("transactions")]
        public static async Task<IActionResult> GetTransactions(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "quotations")] HttpRequest req,
           [CosmosDB(databaseName: Resources.ReadDatabase.Name,
                collectionName: Resources.ReadDatabase.Collection,
                ConnectionStringSetting = Resources.ReadDatabase.ConnectionStringKey)] DocumentClient quotationCollection)
        {
           
            return new OkObjectResult(await quotationCollection.CreateDocumentQuery<TransactionProjection>(Resources.ReadDatabase.CollectionUri).ToListAsync());
        }
    }
}
