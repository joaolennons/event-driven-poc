using System;
using System.Threading.Tasks;

namespace Insurance.Repositories
{
    public interface IAggregateRootRepository
    {
        Task<T> LoadEntityAsync<T>(Guid id) where T : _AggregateRoot, new();
        Task<bool> SaveEntityAsync(_AggregateRoot entity);
    }
}