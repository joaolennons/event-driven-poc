using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Mitsui.Poc.Events;
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
            var quotation = JsonConvert.DeserializeObject<CarQuotationDraftHasBeenUpdated>(quotationMessage);

            var transaction = new TransactionProjection
            {
                Car = "Yaris",
                InsuredEmail = "e-mail",
                InsuredName = quotation.Identity,
                LicensePlate = quotation.LicensePlate,
                QuotationIdentifier = quotation.QuotationId,
                Plan = "Prêmio top das galáxias",
                Status = "Rascunho",
                Value = string.Empty
            };

            await quotationCollection.UpsertDocumentAsync(Resources.ReadDatabase.CollectionUri, transaction);
        }
    }
}
