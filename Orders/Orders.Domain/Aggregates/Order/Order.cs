using System;
using JetBrains.Annotations;
using Orders.Domain.Events;

namespace Orders.Domain.Aggregates.Order
{
    public class Order : Entity, IAggregateRoot
    {
        // Implicitly used by EF Core
#pragma warning disable 414
#pragma warning disable IDE0052
        private DateTime _creationDateTime;
#pragma warning restore IDE0052
#pragma warning restore 414

        private int _orderStatus;

        private string _name;

        private Order() { }

        public static Order CreateNew([NotNull] string name)
        {
            var order = new Order { _creationDateTime = DateTime.UtcNow, _orderStatus = -1 };
            order.SetName(name);
            return order;
        }

        public void SetStatusTo(int newStatus)
        {
            if (newStatus <= _orderStatus)
            {
                return;
            }

            AddDomainEvent(new OrderStatusChangedDomainEvent(Id, _orderStatus, newStatus));

            _orderStatus = newStatus;
        }

        public void SetName(string newName)
        {
            ValidateName(newName);

            _name = newName;
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            _name = name;
        }
    }
}