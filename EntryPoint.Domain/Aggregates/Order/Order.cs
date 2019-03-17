using EntryPoint.Domain.Events;
using JetBrains.Annotations;
using System;

namespace EntryPoint.Domain.Aggregates.Order
{
    public class Order : Entity, IAggregateRoot
    {
        private DateTime _creationDateTime;

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