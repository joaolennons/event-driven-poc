using EventStore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Insurance.Repositories
{
    public class AggregateRootRepository : IAggregateRootRepository
    {
        private readonly IEventStore _eventStore;

        public AggregateRootRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<bool> SaveEntityAsync(_AggregateRoot entity)
        {
            if (entity.Changes.Any())
                return await _eventStore.AppendToStreamAsync(entity.Id.ToString(), entity.Version, entity.Changes);

            return false;
        }

        public async Task<T> LoadEntityAsync<T>(Guid id) where T : _AggregateRoot, new()
        {
            var stream = await _eventStore.LoadStreamAsync(id);
            var entity = new T();
            entity.Rehydrate(stream.Events);
            return entity;
        }
    }
}
