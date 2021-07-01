using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Insurance.Query
{
    public static class TransactionProjectionHandler
    {
        [FunctionName("TransactionProjectionHandler")]
        public static async Task Run([QueueTrigger("QuotationUpdated", Connection = "AzureWebJobsStorage")] string quotationMessage,
                [CosmosDB(databaseName: Resources.ReadDatabase.Name,
                collectionName: Resources.ReadDatabase.Collection,
                ConnectionStringSetting = Resources.ReadDatabase.ConnectionStringKey)] DocumentClient quotationCollection)
        {
            var quotation = JsonConvert.DeserializeObject<dynamic>(quotationMessage);

            var transaction = new TransactionProjection
            {
                Car = "Yaris",
                InsuredEmail = "e-mail",
                InsuredName = quotation.Identity,
                LicensePlate = quotation.LicensePlate,
                QuotationIdentifier = quotation.QuotationId,
                Plan = quotation.Value > 0 ? "Prêmio top das galáxias" : string.Empty,
                Status = quotation.Status ?? "Rascunho",
                Value = quotation.Value
            };

            await quotationCollection.UpsertDocumentAsync(Resources.ReadDatabase.CollectionUri, transaction);
        }
    }
}
