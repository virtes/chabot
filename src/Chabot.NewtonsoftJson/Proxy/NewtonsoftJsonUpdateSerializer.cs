using System.Text;
using Chabot.Proxy;
using Newtonsoft.Json;

namespace Chabot.NewtonsoftJson.Proxy;

internal class NewtonsoftJsonUpdateSerializer<TUpdate> : IUpdateSerializer<TUpdate, byte[]>
{
    public byte[] Serialize(TUpdate update)
    {
        var jsonString = JsonConvert.SerializeObject(update);
        return Encoding.UTF8.GetBytes(jsonString);
    }

    public TUpdate Deserialize(byte[] serializedUpdate)
    {
        return JsonConvert.DeserializeObject<TUpdate>(Encoding.UTF8.GetString(serializedUpdate))
            ?? throw new NullReferenceException("Deserialized value is null");
    }
}