using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace InterlockStubs
{
    public static class Interlocks
    {
        [FunctionName("Neurotech")]
        public static async Task<IActionResult> RunNeurotechStub(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            var value = new Random().Next(50000);
            var success = new Random().NextDouble() > 0.5;

            if (success)
                return new OkObjectResult(value);
            
            return new BadRequestObjectResult("NT is fucked.");
        }

        [FunctionName("Emit")]
        public static async Task<IActionResult> RunEmitStub(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            var success = new Random().NextDouble() > 0.5;

            if (success)
                return new OkObjectResult(success);

            return new BadRequestObjectResult("Interlock could not emit.");
        }
    }
}
