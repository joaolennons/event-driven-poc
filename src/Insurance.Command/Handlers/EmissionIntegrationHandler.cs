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
    public class EmissionIntegrationHandler
    {
        private readonly HttpClient _client;
        private readonly IAggregateRootRepository _repository;

        public EmissionIntegrationHandler(IHttpClientFactory httpClientFactory, IAggregateRootRepository repository)
        {
            _repository = repository;
            _client = httpClientFactory.CreateClient();
        }


        [FunctionName("EmissionIntegrationHandler")]
        public async Task Run([QueueTrigger("QuotationEmissionHasBeeenRequested", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {
            log.LogInformation($"EmissionIntegrationHandler function started: {message}.");

            var response = await _client.PostAsync("http://localhost:7073/api/Emit", new StringContent(message));

            var @event = JsonConvert.DeserializeObject<QuotationEmissionHasBeeenRequested>(message);
            var quotation = await _repository.LoadEntityAsync<CarQuotation>(@event.QuotationId);
            
            if (response.IsSuccessStatusCode)
            {
                quotation.Emit();
            }
            else
            {
                quotation.ReportError("Emiter", await response.Content.ReadAsStringAsync());
            }

            await _repository.SaveEntityAsync(quotation);

            log.LogInformation($"EmissionIntegrationHandler function finished.");
        }
    }
}
