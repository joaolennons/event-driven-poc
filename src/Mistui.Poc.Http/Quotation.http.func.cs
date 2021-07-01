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
    public static class QuotationHttpFuncs
    {
        [FunctionName(nameof(CreateDraft))]
        public static async Task<IActionResult> CreateDraft(
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
                    var repository = new QuotationRepository();
                    hasBeenCreated = await repository.SaveQuotationAsync(quotation);

                    if (hasBeenCreated)
                        await new EventBroker(topic).Publish(quotation.Changes.Last());
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
        public static async Task<IActionResult> PatchQuotation(
            [HttpTrigger(AuthorizationLevel.Function, HttpMethod.patch, Route = "quotations")] HttpRequest req,
            [Queue(Resources.EventSourcing.Broker.Topics.QuotationUpdated, Connection = Resources.EventSourcing.Broker.ConnectionStringKey)] IAsyncCollector<string> topic,
            ILogger log)
        {
            try
            {
                var dto = await req.GetRequestBody<QuotationInput>();

                var repository = new QuotationRepository();
                var quotation = await repository.LoadQuotationAsync<CarQuotation>(dto.QuotationId);

                quotation.UpdateDraft(dto.Identity, dto.ZipCode, dto.MainDriver, dto.Chassis, dto.LicensePlate, dto.Questions);
                   
                if (quotation.IsValid())
                {
                    var result = await repository.SaveQuotationAsync(quotation);
                    if (result)
                    {
                        await new EventBroker(topic).Publish(quotation.Changes.Last());
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
        public static async Task<IActionResult> RequestQuotationCalc(
            [HttpTrigger(AuthorizationLevel.Function, HttpMethod.post, Route = "quotations/{id}/request-calculation")] HttpRequest req,
            [Queue(Resources.EventSourcing.Broker.Topics.QuotationPlanCalculationRequested, Connection = Resources.EventSourcing.Broker.ConnectionStringKey)] IAsyncCollector<string> topic,
            ILogger log)
        {
            try
            {
                var dto = await req.GetRequestBody<QuotationInput>();

                var repository = new QuotationRepository();
                var quotation = await repository.LoadQuotationAsync<CarQuotation>(dto.QuotationId);

                quotation.Calculate();

                if (quotation.IsValid())
                {
                    await repository.SaveQuotationAsync(quotation);
                    await new EventBroker(topic).Publish(quotation.Changes.Last());
                }

                return new OkObjectResult(quotation);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName(nameof(RequestQuotationEmission))]
        public static async Task<IActionResult> RequestQuotationEmission(
            [HttpTrigger(AuthorizationLevel.Function, HttpMethod.post, Route = "quotations/{id}/request-emission")] HttpRequest req,
            [Queue(Resources.EventSourcing.Broker.Topics.QuotationEmissionHasBeeenRequested, Connection = Resources.EventSourcing.Broker.ConnectionStringKey)] IAsyncCollector<string> topic,
            ILogger log)
        {
            try
            {
                var dto = await req.GetRequestBody<QuotationInput>();

                var repository = new QuotationRepository();
                var quotation = await repository.LoadQuotationAsync<CarQuotation>(dto.QuotationId);

                if (quotation.RequestEmission())
                {
                    await repository.SaveQuotationAsync(quotation);
                    await new EventBroker(topic).Publish(quotation.Changes.Last());
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
