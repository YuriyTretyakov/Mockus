using Mockus.Contracts.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mockus.Client.Extensions
{
    public static class ResponseExtensions
    {
        public static TType GetBodyAs<TType>(this MockedResponse response) where TType : class
        {
            return JsonConvert.DeserializeObject<TType>(response.Body,
                new JsonSerializerSettings {Error = HandleDeserializationError});
        }

        public static TType TreatBodyAs<TType>(this MockedResponse response) where TType : class, new()
        {
            if (string.IsNullOrEmpty(response.Body))
            {
                response.Body = "";
                return new TType();
            }

            return response.GetBodyAs<TType>();
        }

        private static void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
        }
    }
}
