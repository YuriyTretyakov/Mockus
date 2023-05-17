using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Extensions;
using Newtonsoft.Json;

namespace Mockus.Contracts.Response
{
    public class MockedResponse : IResponse, IHasResponseOptions, IMockResponseBuilder, IHasDataVersion
    {
        public string Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> Cookies { get; set; }
        public string ContentType { get; set; }
        public ResponseOptions ResponseOptions { get; set; }

        [Required] 
        public int DataVersion { get; set; }


        public MockedResponse()
        {
            ContentType = "application/json";
            ResponseOptions = new ResponseOptions();
        }

        public IMockResponseBuilder WithBody(string value)
        {
            Body = value;
            return this;
        }

        public IMockResponseBuilder WithJsonBody(object value)
        {
            Body = JsonConvert.SerializeObject(value);
            return this;
        }

        public IMockResponseBuilder WithHeaders(string key, string value)
        {
            Headers = CreateOrUpdate(Headers, key, value);
            return this;
        }

        public IMockResponseBuilder WithCookies(string key, string value)
        {
            Cookies = CreateOrUpdate(Cookies, key, value);
            return this;
        }

        public IMockResponseBuilder WithStatusCode(HttpStatusCode statusCode)
        {
            ResponseOptions.StatusCode = (int) statusCode;
            return this;
        }

        public IMockResponseBuilder WithDelay(int seconds)
        {
            ResponseOptions.DelaySeconds = seconds;
            return this;
        }

        public MockedResponse Create()
        {
            return this;
        }

        private Dictionary<string, string> CreateOrUpdate(Dictionary<string, string> option, string key, string value)
        {
            if (option == null)
            {
                option = new Dictionary<string, string>
                {
                    {key, value}
                };
            }
            else
            {
                option.Add(key, value);
            }

            return option;
        }

        public async Task Execute()
        {
            if (ResponseOptions.DelaySeconds.HasValue)
                await Task.Delay(ResponseOptions.DelaySeconds.Value * 1000);
        }

        public override bool Equals(object? obj)
        {
            var other = obj as MockedResponse;

            if (other == null)
                return false;

            var equals =  other.Headers.CompareContent(Headers) &&
                other.Body?.Equals(Body) is true or null &&
                other.Cookies.CompareContent(Cookies) &&
                other.DataVersion.Equals(DataVersion)&&
                other.ContentType.Equals(ContentType)&&
                other.ResponseOptions.Equals(ResponseOptions);

            return equals;
        }
    }
}