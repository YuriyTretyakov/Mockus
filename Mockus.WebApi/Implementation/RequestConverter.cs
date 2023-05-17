using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts;
using Mockus.Contracts.Request;
using Mockus.WebApi.Contracts;
using Microsoft.AspNetCore.Http;

namespace Mockus.WebApi.Implementation
{
    public class RequestConverter
    {
        private readonly HttpRequest _appRequest;

        public RequestConverter(HttpRequest appRequest)
        {
            _appRequest = appRequest;
        }

        public static CollectedRequest ToCollectedRequest(IRequest request)
        {
            return new()
            {
                Body = request.Body,
                Path = request.Path,
                Method = request.Method,
                Headers = request.Headers,
                Cookies = request.Cookies,
                QueryParams = request.QueryParams,
                Time = DateTime.Now
            };
        }

        public async Task<IRequest> ConvertToRequestDtoAsync()
        {
            return new RequestDto
            {
                Path = _appRequest.Path,
                Method = _appRequest.Method,
                Body = await GetBodyString(_appRequest),
                Headers = _appRequest
                    .Headers
                    .ToDictionary(
                        hk => hk.Key,
                        hv => hv.Value.First()),
                Cookies = _appRequest
                    .Cookies
                    .ToDictionary(
                        ck => ck.Key,
                        cv => cv.Value),
                QueryParams = GetQueryParams(_appRequest.QueryString)
            };
        }

        public IRequestIdentifier ToRequestIdentifier()
        {
            return new RequestIdentifier(_appRequest.Path, _appRequest.Method);
        }

        private async Task<string> GetBodyString(HttpRequest httpRequest)
        {
            string bodyStr;
            try
            {
                httpRequest.EnableBuffering();
                using var reader = new StreamReader(httpRequest.Body, Encoding.UTF8, true, 1024, true);
                bodyStr = await reader.ReadToEndAsync();
            }
            finally
            {
                httpRequest.Body.Position = 0;
            }

            return bodyStr;
        }

        private Dictionary<string, string> GetQueryParams(QueryString query)
        {
            if (!query.HasValue)
                return null;

            var collection = System.Web.HttpUtility.ParseQueryString(query.ToUriComponent());
            return collection.Keys.Cast<string>().ToDictionary(k => k, v => collection[v]);
        }
    }
}