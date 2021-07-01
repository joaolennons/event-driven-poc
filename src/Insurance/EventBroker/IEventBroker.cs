using System.Threading;
using System.Threading.Tasks;

namespace Insurance.EventBroker
{
    public interface IEventBroker
    {
        Task Publish(object message, CancellationToken cancellationToken = default);
    }
}