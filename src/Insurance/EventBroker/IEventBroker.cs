using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;

namespace Insurance.EventBroker
{
    public interface IEventBroker
    {
        Task Publish(object @message, IAsyncCollector<string> topic, CancellationToken cancellationToken = default);
    }
}