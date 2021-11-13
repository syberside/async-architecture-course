using Newtonsoft.Json;

namespace aTES.SchemaRegistry
{
    public class Serializer
    {
        public string Serialize<T>(T message) where T : IMessage
        {
            return JsonConvert.SerializeObject(message);
        }

        public T Deserialize<T>(string message) where T : IMessage
        {
            return JsonConvert.DeserializeObject<T>(message);
        }
    }
}
