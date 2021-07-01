using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Insurance.EventBroker
{
    public class EventBroker : IEventBroker
    {
        private readonly IAsyncCollector<string> _topic;

        public EventBroker(IAsyncCollector<string> topic)
        {
            _topic = topic;
        }

        public async Task Publish(object @message, CancellationToken cancellationToken = default)
            => await _topic.AddAsync(JsonConvert.SerializeObject(message), cancellationToken);
    }
}
