using EventStore;
using Insurance.EventBroker;
using Insurance.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Insurance
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainDependencies(this IServiceCollection services)
            => services.AddTransient<IEventStore, EventStore.EventStore>()
                .AddTransient<IEventBroker, EventBroker.EventBroker>()
                .AddTransient<IAggregateRootRepository, AggregateRootRepository>();

    }
}
