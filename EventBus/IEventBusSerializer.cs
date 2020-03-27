using System;

namespace EventBus
{
    public interface IEventBusSerializer
    {
        byte[] Serialize(object item);

        object Deserialize(string message, Type type);
    }
}
