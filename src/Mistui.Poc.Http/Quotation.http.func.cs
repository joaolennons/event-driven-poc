using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Insurance.Car;
using System;
using Insurance.Repositories;
using Insurance.EventBroker;
using System.Linq;
using Mitsui.Poc.Http.InputDto;

namespace Mitsui.Poc.Http
{
    public class QuotationHttpFuncs
    {
        private readonly IAggregateRootRepository _repository;
        private readonly IEventBroker _eventBroker;

        public QuotationHttpFuncs(IAggregateRootRepository repository, IEventBroker eventBroker)
        {
            _repository = repository;
            _eventBroker = eventBroker;
        }

        [FunctionName(nameof(CreateDraft))]
        public async Task<IActionResult> CreateDraft(
            [HttpTrigger(AuthorizationLevel.Function, HttpMethod.post, Route = "quotations")] HttpRequest req,
            [Queue(Resources.EventSourcing.Broker.Topics.QuotationUpdated, Connection = Resources.EventSourcing.Broker.ConnectionStringKey)] IAsyncCollector<string> topic,
            ILogger log)
        {
            try
            {
                var dto = await req.GetRequestBody<QuotationInput>();

                bool hasBeenCreated = false; 

                var quotation = new CarQuotation(dto.Identity, dto.ZipCode, dto.MainDriver, dto.Chassis, dto.LicensePlate, dto.Questions);

                if (quotation.IsValid())
                {
                    hasBeenCreated = await _repository.SaveEntityAsync(quotation);

                    if (hasBeenCreated)
                        await _eventBroker.Publish(quotation.Changes.Last(), topic);
                }

                if (hasBeenCreated)
                    return new CreatedResult("PostQuotation", quotation);
                
                return new BadRequestObjectResult("Quotation is invalid.");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName(nameof(PatchQuotation))]
        public async Task<IActionResult> PatchQuotation(
            [HttpTrigger(AuthorizationLevel.Function, HttpMethod.patch, Route = "quotations")] HttpRequest req,
            [Queue(Resources.EventSourcing.Broker.Topics.QuotationUpdated, Connection = Resources.EventSourcing.Broker.ConnectionStringKey)] IAsyncCollector<string> topic,
            ILogger log)
        {
            try
            {
                var dto = await req.GetRequestBody<QuotationInput>();

                var quotation = await _repository.LoadEntityAsync<CarQuotation>(dto.QuotationId);

                quotation.UpdateDraft(dto.Identity, dto.ZipCode, dto.MainDriver, dto.Chassis, dto.LicensePlate, dto.Questions);
                   
                if (quotation.IsValid())
                {
                    var result = await _repository.SaveEntityAsync(quotation);
                    if (result)
                    {
                        await _eventBroker.Publish(quotation.Changes.Last(), topic);
                    }
                }
                    
                return new OkObjectResult(quotation);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName(nameof(RequestQuotationCalc))]
        public async Task<IActionResult> RequestQuotationCalc(
            [HttpTrigger(AuthorizationLevel.Function, HttpMethod.post, Route = "quotations/{id}/request-calculation")] HttpRequest req,
            [Queue(Resources.EventSourcing.Broker.Topics.QuotationPlanCalculationRequested, Connection = Resources.EventSourcing.Broker.ConnectionStringKey)] IAsyncCollector<string> topic,
            ILogger log)
        {
            try
            {
                var dto = await req.GetRequestBody<QuotationInput>();

                var quotation = await _repository.LoadEntityAsync<CarQuotation>(dto.QuotationId);

                quotation.Calculate();

                if (quotation.IsValid())
                {
                    await _repository.SaveEntityAsync(quotation);
                    await _eventBroker.Publish(quotation.Changes.Last(), topic);
                }

                return new OkObjectResult(quotation);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName(nameof(RequestQuotationEmission))]
        public async Task<IActionResult> RequestQuotationEmission(
            [HttpTrigger(AuthorizationLevel.Function, HttpMethod.post, Route = "quotations/{id}/request-emission")] HttpRequest req,
            [Queue(Resources.EventSourcing.Broker.Topics.QuotationEmissionHasBeeenRequested, Connection = Resources.EventSourcing.Broker.ConnectionStringKey)] IAsyncCollector<string> topic,
            ILogger log)
        {
            try
            {
                var dto = await req.GetRequestBody<QuotationInput>();

                var quotation = await _repository.LoadEntityAsync<CarQuotation>(dto.QuotationId);

                if (quotation.RequestEmission())
                {
                    await _repository.SaveEntityAsync(quotation);
                    await _eventBroker.Publish(quotation.Changes.Last(), topic); ;
                }

                return new OkObjectResult(quotation);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
