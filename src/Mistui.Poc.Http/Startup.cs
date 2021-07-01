using Insurance;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Mitsui.Poc.Http.Startup))]

namespace Mitsui.Poc.Http
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDomainDependencies();
        }
    }
}