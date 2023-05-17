using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mockus.Client
{
    internal class DefaultSerializer : IJsonSerializer
    {
        private JsonSerializerSettings _settings;
        
        public bool IsSuccess { private set; get; }

        public DefaultSerializer()
        {
            _settings = new JsonSerializerSettings();
            _settings.ContractResolver = new DefaultContractResolver();
            _settings.TypeNameHandling = TypeNameHandling.Auto;
            _settings.SerializationBinder = new DefaultSerializationBinder();
            _settings.Error = (sender, errorArgs) =>
            {
                IsSuccess = false;
                errorArgs.ErrorContext.Handled = true;
            };
        }

        public void SetSerializerSettings(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public string Serialize(object obj)
        {
           IsSuccess=true;
           return JsonConvert.SerializeObject(obj, _settings);
        }

        public TType Deserialize<TType>(string jsonString)
        {
            IsSuccess = true;
            return JsonConvert.DeserializeObject<TType>(jsonString, _settings);
        }
    }
}
