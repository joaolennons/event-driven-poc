using Insurance.Car;
using Insurance.Repositories;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Mitsui.Poc.Events;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Insurance.Query
{
    public class NeurotechIntegrationHandler
    {
        private readonly HttpClient _client;

        public NeurotechIntegrationHandler(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }


        [FunctionName("NeurotechIntegrationHandler")]
        public async Task Run([QueueTrigger("QuotationPlanCalculationRequested", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            log.LogInformation($"NeurotechIntegrationHandler function started: {message}.");

            var response = await _client.PostAsync("http://localhost:7073/api/Neurotech", new StringContent(message));

            var @event = JsonConvert.DeserializeObject<QuotationPlanCalculationHasBeenRequested>(message);
            var repository = new QuotationRepository();
            var quotation = await repository.LoadQuotationAsync<CarQuotation>(@event.QuotationId);
            
            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadAsAsync<decimal>();
                quotation.SuggestPlan(value);
            }
            else
            {
                quotation.ReportError("Neurotech", await response.Content.ReadAsStringAsync());
            }

            await repository.SaveQuotationAsync(quotation);

            log.LogInformation($"NeurotechIntegrationHandler function finished.");
        }
    }
}
