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

                //o evento de dom�nio t� aqui
                var quotation = new CarQuotation(dto.Identity, dto.ZipCode, dto.MainDriver, dto.Chassis, dto.LicensePlate, dto.Questions);

                if (quotation.IsValid())
                {
                    //o event sourcing t� aqui
                    hasBeenCreated = await _repository.SaveEntityAsync(quotation);

                    // o message broker t� aqui
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

                //aqui, o que vai ser carregado s�o todos os eventos que ocorreram nessa cota��o
                //a partir da leitura cronol�gica desses eventos, a entidade vai ser remontada no seu estado atual
                var quotation = await _repository.LoadEntityAsync<CarQuotation>(dto.QuotationId);

                //aqui est� o evento de dom�nio
                quotation.UpdateDraft(dto.Identity, dto.ZipCode, dto.MainDriver, dto.Chassis, dto.LicensePlate, dto.Questions);
                   
                if (quotation.IsValid())
                {
                    //aqui, o novo evento � empilhado no event sourcing
                    var result = await _repository.SaveEntityAsync(quotation);
                    if (result)
                    {
                        //aqui o evento � comunicado via message broker
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

                //aqui, o que vai ser carregado s�o todos os eventos que ocorreram nessa cota��o
                //a partir da leitura cronol�gica desses eventos, a entidade vai ser remontada no seu estado atual
                var quotation = await _repository.LoadEntityAsync<CarQuotation>(dto.QuotationId);

                //aqui est� o evento de dom�nio
                quotation.Calculate();

                if (quotation.IsValid())
                {
                    //aqui o evento de solicita��o de c�lculo � empilhado no event sourcing
                    await _repository.SaveEntityAsync(quotation);
                    //aqui o evento de solicita��o de c�lculo � comunicado via message broker
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
