namespace Orders.Domain;

// ReSharper disable once UnusedTypeParameter
public interface IRepository<T> where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}
