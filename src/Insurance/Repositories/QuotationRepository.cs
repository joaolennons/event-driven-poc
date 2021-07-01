using System;
using System.Linq;
using System.Threading.Tasks;

namespace Insurance.Repositories
{
    public class QuotationRepository
    {
        private readonly IEventStore _eventStore; 

        public QuotationRepository()
        {
            _eventStore = new EventStore();
        }

        public async Task<bool> SaveQuotationAsync(_Quotation quotation)
        {
            if (quotation.Changes.Any())
                return await _eventStore.AppendToStreamAsync(quotation.Id.ToString(), quotation.Version, quotation.Changes);

            return false; 
        }

        public async Task<T> LoadQuotationAsync<T>(Guid id) where T : _Quotation, new()
        {
            var stream = await _eventStore.LoadStreamAsync(id);
            var entity = new T();
            entity.Rehydrate(stream.Events);
            return entity;
        }
    }
}
