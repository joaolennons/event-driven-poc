using System;

namespace Insurance
{
    public interface IEntity
    {
        Guid Id { get; }
        DateTime CreatedAt { get; }
    }
}
