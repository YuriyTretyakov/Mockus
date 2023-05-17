using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mockus.Client.Policies;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.ExecutionPolicy;
using Mockus.Contracts.Request;
using Mockus.Contracts.Response;
using Mockus.Contracts.Enums;
using Mockus.Contracts.Exceptions;
using Serilog;
using Serilog.Core;

namespace Mockus.Client
{
    public class MockusClient
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private IJsonSerializer _serializer;

        public string MessageText => LastProcessedMessage?.Content.ReadAsStringAsync().Result ?? string.Empty;
        private HttpResponseMessage LastProcessedMessage { get; set; }

        public bool IsSuccess => LastProcessedMessage != null && LastProcessedMessage.IsSuccessStatusCode;


        public MockusClient(string baseUrl, ILogger logger) : this(baseUrl)
        {
            _logger = logger;
        }

        public MockusClient(string baseUrl)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromMinutes(3)
            };
            _logger = Logger.None;

            _serializer = new DefaultSerializer();
        }

        public void SetJsonSerializer(IJsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public async Task<MockedDataResponse> GetMockedData(RequestIdentifier requestIdentifier)
        {
            var currentSnapshot = await _client.PostAsync("/api/v1/Manage/GetMockedData",
                ToStringContent(requestIdentifier));

            var mockedData = _serializer.Deserialize<MockedDataResponse>(await currentSnapshot.Content.ReadAsStringAsync());
            return mockedData;
        }

        public async Task SetMockDataAsync(SetMockDataRequest mockDataRequest)
        {
            mockDataRequest.Operation = MockOperation.Add;
            mockDataRequest.MockResponse.DataVersion = 1;
            var setDataResponse =
                await _client.PostAsync("/api/v1/Manage/SetMockData", ToStringContent(mockDataRequest));
            LastProcessedMessage = setDataResponse;
            await ProcessSetMockDataResponseAsync(setDataResponse, mockDataRequest);
        }


        public async Task<SetMockDataRequest> SetMockDataAsync(
            Func<MockRequestOptions, MockRequestOptions> requestUpdateFunc,
            Func<MockedResponse, MockedResponse> responseUpdateFunc
            )
        {
            var retryPolicy = PollyPolicies.GetRetryToMatchDataVersionPolicy<SetMockDataRequest>(1000);

            return await retryPolicy.ExecuteAsync(async () =>
            {
                var instance = requestUpdateFunc.Invoke(new MockRequestOptions());

                
                var mockedSnapshot = await GetMockedData(instance.RequestIdentifier);
                mockedSnapshot.MockRequestOptions = requestUpdateFunc.Invoke(mockedSnapshot.MockRequestOptions);
                mockedSnapshot.MockedResponse = responseUpdateFunc.Invoke(mockedSnapshot.MockedResponse);
                mockedSnapshot.MockedResponse.DataVersion += 1;

                var setMockDataRequest=

                new SetMockDataRequest
                {
                    Operation = MockOperation.Update,
                    MockResponse = mockedSnapshot.MockedResponse,
                    MockRequest = mockedSnapshot.MockRequestOptions
                };
                
                var setDataResponse =
                    await _client.PostAsync("/api/v1/Manage/SetMockData", ToStringContent(setMockDataRequest));
                LastProcessedMessage = setDataResponse;
                await ProcessSetMockDataResponseAsync(setDataResponse, setMockDataRequest);
                return setMockDataRequest;
            });
        }

        private async Task ProcessSetMockDataResponseAsync(HttpResponseMessage setMockDataResponse, SetMockDataRequest setMockDataRequest)
        {
            if (!setMockDataResponse.IsSuccessStatusCode)
            {
                var content = await setMockDataResponse.Content.ReadAsStringAsync();

                if (setMockDataResponse.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    var errorMessage = _serializer.Deserialize<ErrorMessage>(content);

                    if (errorMessage.Error.Contains("Dataversion missmatch"))
                    {
                        _logger.Warning(@"Dataversion missmatch detected. Description: {@errorMessage}",
                            errorMessage);
                        throw new VersionMismatchException(errorMessage.Error);
                    }
                }
                _logger.Error($"Error when executing SetMockData. Data: {content}");
            }
            _logger.Information($"Successfully executed SetMockData. Content: {setMockDataRequest.MockResponse.Body}\n" +
                                $"Path: {setMockDataRequest.MockRequest.RequestIdentifier.Path}\n" +
                                $"Method: {setMockDataRequest.MockRequest.RequestIdentifier.Method}\n" +
                                $"Status code: {setMockDataRequest.MockResponse.ResponseOptions.StatusCode}");
        }

        private TData GetMessageAs<TData>()
        {
            return !string.IsNullOrEmpty(MessageText)
                ? _serializer.Deserialize<TData>(MessageText)
                : default(TData);
        }

        private StringContent ToStringContent(object obj)
        {
            return new (

                _serializer.Serialize(obj),
                Encoding.UTF8,
                "application/json");
        }

        public async Task<IList<CollectedRequest>> GetAllCapturedRequests()
        {
            LastProcessedMessage = await _client.GetAsync("/api/v1/Manage/GetIncomingRequests");
            var result = GetMessageAs<IList<CollectedRequest>>();

            Logging(nameof(GetAllCapturedRequests), result: result);

            return result;
        }

        public async Task<IList<ParsedCollectedRequest<TBodyType>>> GetCapturedRequests<TBodyType>()
        {
            var allRequests = await GetAllCapturedRequests();
            return ToParsedCollectedRequest<TBodyType>(allRequests);
        }

        public async Task<IList<ParsedCollectedRequest<TBodyType>>> GetCapturedRequests<TBodyType>(string partOfBody)
        {
            var allRequests = await GetCapturedRequests(partOfBody);

            return ToParsedCollectedRequest<TBodyType>(allRequests);
        }

        public async Task<IList<ParsedCollectedRequest<TBodyType>>> GetCapturedRequestsAsync<TBodyType>(RequestIdentifier identifier)
        {
            var allRequests = await GetCapturedRequests(identifier);

            return ToParsedCollectedRequest<TBodyType>(allRequests);
        }

        public async Task<IList<ParsedCollectedRequest<TBodyType>>> GetCapturedRequests<TBodyType>(RequestBodyPattern identifier)
        {
            var allRequests = await GetCapturedRequests(identifier);

            return ToParsedCollectedRequest<TBodyType>(allRequests);
        }

        private IList<ParsedCollectedRequest<TBodyType>> ToParsedCollectedRequest<TBodyType>(IList<CollectedRequest> collectedRequests)
        {
            return collectedRequests.Select(r =>
            {
                var result = ParsedCollectedRequest<TBodyType>.TryParse(r, _serializer, out var parsedRequest);
                return result ? parsedRequest : default;
            }).Where(r=>r!=null).ToList();
        }

        public async Task<IList<CollectedRequest>> GetCapturedRequests(string body)
        {
            LastProcessedMessage = await _client.GetAsync($"/api/v1/Manage/GetIncomingRequests/{body}");
            var result = GetMessageAs<IList<CollectedRequest>>();

            Logging(nameof(GetCapturedRequests), result, body);

            return result;
        }

        public async Task<IList<CollectedRequest>> GetCapturedRequests(RequestBodyPattern identifier)
        {
            LastProcessedMessage = await _client.PostAsync($"/api/v1/Manage/GetIncomingRequestsByPatternBody", ToStringContent(identifier));
            var result = GetMessageAs<IList<CollectedRequest>>();

            Logging(nameof(GetCapturedRequests), result, identifier);

            return result;
        }

        public async Task<IList<CollectedRequest>> GetCapturedRequests(RequestIdentifier identifier)
        {
            LastProcessedMessage = await _client.PostAsync($"/api/v1/Manage/GetIncomingRequests",ToStringContent(identifier));
            var result = GetMessageAs<IList<CollectedRequest>>();

            Logging(nameof(GetCapturedRequests), result, identifier);

            return result;
        }

        public async Task<ExecutionPolicyModel> SetExecutionPolicyAsync(ExecutionPolicyModel policy)
        {
            LastProcessedMessage = await _client.PostAsync("/api/v1/Manage/SetExecutionPolicy", ToStringContent(policy));

            Logging(nameof(SetExecutionPolicyAsync), data: policy);

            return policy;
        }

        public async Task SetPolicyStatus(RequestIdentifier identifier, PolicyStatus status)
        {
            LastProcessedMessage = await _client.PostAsync($"/api/v1/Manage/SetExecutionPolicy/{status}", ToStringContent(identifier));

            Logging(nameof(SetPolicyStatus), data: identifier);
        }

        public async Task ClearAllPolicies()
        {
            LastProcessedMessage = await _client.GetAsync("/api/v1/Manage/ClearAllExecutionPolicy");

            Logging(nameof(ClearAllPolicies));
        }

        public async Task ClearCapturedRequestsAsync()
        {
            LastProcessedMessage = await _client.GetAsync("/api/v1/Manage/ClearCollectedRequests");
        }

        public async Task ClearMockedDataAsync()
        {
            LastProcessedMessage = await _client.GetAsync("/api/v1/Manage/ClearMockData");
        }

        public async Task ClearAll()
        {
            LastProcessedMessage = await _client.GetAsync("/api/v1/Manage/ClearAll");
        }

        public async Task RemoveMockedRequests(IRequestIdentifier requestIdentifier)
        {
            LastProcessedMessage = await _client.PostAsync(
                "/api/v1/Manage/RemoveMockedRequests",
                ToStringContent(requestIdentifier));

            Logging(nameof(RemoveMockedRequests), data: requestIdentifier);
        }

        public async Task RemoveMockedRequestsByPattern(IRequestPatternIdentifier requestPattern)
        {
            LastProcessedMessage = await _client.PostAsync(
                "/api/v1/Manage/RemoveMockedRequestsByPattern",
                ToStringContent(requestPattern));

            Logging(nameof(RemoveMockedRequestsByPattern), data: requestPattern);
        }

        public async Task RemoveMockedRequests(MockRequestOptions mockRequest)
        {
            LastProcessedMessage = await _client.PostAsync(
                "/api/v1/Manage/RemoveMockByMockRequestOptions",
                ToStringContent(mockRequest));

            Logging(nameof(RemoveMockedRequests), data: mockRequest);
        }

        public async Task<IEnumerable<MockedDataResponse>> GetMockedData()
        {
            var currentSnapshot = await _client.GetAsync("/api/v1/Manage/GetMockedData");

            var mockedData = _serializer.Deserialize<IEnumerable<MockedDataResponse>>(await currentSnapshot.Content.ReadAsStringAsync());
            return mockedData;
        }

        private void Logging(string methodName, IList<CollectedRequest> result = null, object data = null)
        {
            if (LastProcessedMessage.IsSuccessStatusCode)
            {
                _logger.Information("Successfully executed {@methodName} Payload: {@identifier}. Data: {@data}",
                    methodName,
                    result,
                    data);
            }
            else
            {
                _logger.Error("Error when executing {@methodName}. Data: {@data} Error: {@error} ",
                    methodName,
                    data,
                    MessageText);
            }
        }
    }
}