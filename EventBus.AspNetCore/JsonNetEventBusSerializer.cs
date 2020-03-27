using Newtonsoft.Json;
using System;
using System.Text;

namespace EventBus.AspNetCore
{
    public class JsonNetEventBusSerializer : IEventBusSerializer
    {
        public object Deserialize(string message, Type type)
        {
            return JsonConvert.DeserializeObject(message, type);
        }

        public byte[] Serialize(object obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }
    }
}
