using System.Collections.Generic;
using System.Threading.Tasks;
using Mockus.Contracts.Response;
using Mockus.WebApi.Abstractions.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog;

namespace Mockus.WebApi.Implementation
{
    public class MockManager : IMockManager
    {
        private readonly IStorageRepository _storageRepository;
        private readonly ICollectedRequestRepository _collectedRequestRepository;
        private readonly IExecutionPolicyRepository _executionPolicyRepository;
        private readonly ILogger _logger = Log.ForContext<MockManager>();

        public MockManager(
            IStorageRepository storageRepository,
            ICollectedRequestRepository collectedRequestRepository,
            IExecutionPolicyRepository executionPolicyRepository)
        {
            _storageRepository = storageRepository;
            _collectedRequestRepository = collectedRequestRepository;
            _executionPolicyRepository = executionPolicyRepository;
        }

        public async Task<MockedResponse> GetMockedResponseAsync(HttpRequest request)
        {
            var requestConverter = new RequestConverter(request);
            var requestDto = await requestConverter.ConvertToRequestDtoAsync();
            _collectedRequestRepository.Add(RequestConverter.ToCollectedRequest(requestDto));

            var identifier = requestConverter.ToRequestIdentifier();
            var response = _executionPolicyRepository.Get(identifier)?.Run() ?? _storageRepository.GetResponse(requestDto);


            if (response == null)
            {
                _logger.Warning("GetMockedResponseAsync: Mocked response not found {@request}", requestDto);
            }

            return response;
        }

        public async Task UpdateContextAsync(HttpContext context)
        {
            var response = await GetMockedResponseAsync(context.Request);

            if (response == null)
            {
                context.Response.StatusCode = 404;
            }
            else
            {
                await response.Execute();
                context.Response.StatusCode = response.ResponseOptions.StatusCode;
                context.Response.ContentType = response.ContentType;
                AddHeaders(context.Response.Headers, response.Headers);
                AddCookies(context.Response.Cookies, response.Cookies);
                await SafeSetBody(context.Response,response?.Body);
            }
        }

        private void AddHeaders(IHeaderDictionary headers, IDictionary<string, string> dict)
        {
            if (dict == null)
            {
                return;
            }

            foreach (var kv in dict)
            {
                headers.Add(kv.Key, new StringValues(kv.Value));
            }
        }

        private void AddCookies(IResponseCookies cookies, IDictionary<string, string> dict)
        {
            if (dict == null)
            {
                return;
            }

            foreach (var kv in dict)
            {
                cookies.Append(kv.Key, new StringValues(kv.Value));
            }
        }

        private async Task SafeSetBody(HttpResponse response, string body)
        {
            if (!string.IsNullOrEmpty(body))
                await response.WriteAsync(body);
        }
    }
}