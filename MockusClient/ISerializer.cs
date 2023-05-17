using System.Xml;
using Newtonsoft.Json;

namespace Mockus.Client
{
    public interface IJsonSerializer
    {
        void SetSerializerSettings(JsonSerializerSettings settings);
        string Serialize(object obj);
        TType Deserialize<TType>(string jsonString);

        bool IsSuccess { get;  }
    }
}
