using System;
using Mockus.Contracts.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mockus.Client
{
    public class ParsedCollectedRequest<TBodyType> : CollectedRequest
    {
        public TBodyType ParsedBody { private set; get; }
    

        public static bool TryParse(CollectedRequest request, IJsonSerializer serializer, out ParsedCollectedRequest<TBodyType> collectedRequest)
        {
            collectedRequest = new ParsedCollectedRequest<TBodyType>
            {
                Path = request.Path,
                QueryParams = request.QueryParams,
                Headers = request.Headers,
                Cookies = request.Cookies,
                Time = request.Time,
                Method = request.Method,
                Body = request.Body
            };

            collectedRequest.ParsedBody = serializer.Deserialize<TBodyType>(request.Body);
            return serializer.IsSuccess;
        }
    }
}


