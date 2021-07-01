using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Insurance.EventBroker
{
    internal class EventBroker : IEventBroker
    {
        public async Task Publish(object @message, IAsyncCollector<string> topic, CancellationToken cancellationToken = default)
            => await topic.AddAsync(JsonConvert.SerializeObject(message), cancellationToken);
    }
}
