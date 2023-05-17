using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mockus.Contracts.Abstractions.Options;
using Mockus.Contracts.Request;
using Mockus.Contracts.Response;

namespace Mockus.WebApi.Contracts
{
    public class MockDataStorage
    {
        public ConcurrentBag<KeyValuePair<MockRequestOptions, MockedResponse>> Container { private set; get; }

        public MockDataStorage()
        {
            Container = new();
        }

        public IEnumerable<KeyValuePair<MockRequestOptions, MockedResponse>> GetMockedDataByReqId(RequestIdentifier identifier)
        {
            return Container
                .Where(s => s.Key
                    .RequestIdentifier.Equals(identifier)).ToList();
        }

        public void Clear()
        {
            Container.Clear();
        }

        public void RemoveMockedRequests(RequestIdentifier identifier)
        {
            lock (identifier)
            {
                var matchedItems = Container
                    .Where(s => s.Key.RequestIdentifier.Equals(identifier));

                Container = new ConcurrentBag<KeyValuePair<MockRequestOptions, MockedResponse>>
                    (Container.Except(matchedItems));
            }
        }

        public void RemoveMockedRequestsByPattern(RequestPatternIdentifier requestPattern)
        {
            lock (requestPattern)
            {
                var matchedItems = Container
                    .Where(s => requestPattern.IsMatch(s.Key.RequestIdentifier));
                
                Container = new ConcurrentBag<KeyValuePair<MockRequestOptions, MockedResponse>>
                    (Container.Except(matchedItems));
            }
        }

        public void RemoveMockByMockRequestOptions(MockRequestOptions mockRequest)
        {
            lock (mockRequest)
            {
                var matchedItems = Container
                    .Where(s => mockRequest.Equals(s.Key));
                
                Container = new ConcurrentBag<KeyValuePair<MockRequestOptions, MockedResponse>>
                    (Container.Except(matchedItems));
            }
        }

        public IEnumerable<MockedDataResponse> GetAll()
        {
            return Container
                .Select(kv => new MockedDataResponse(kv.Key, kv.Value));
        }

        public void Add(SetMockDataRequest setMockDataRequest)
        {
            Container.Add(new(
                setMockDataRequest.MockRequest,
                setMockDataRequest.MockResponse));
        }

        public void UpdateBatchedData(SetMockDataRequest mockData)
        {
            var batchedReqResponseContainer = new KeyValuePair<MockRequestOptions, MockedResponse>(
                mockData.MockRequest,
                mockData.MockResponse);

            var dataToRemove = GetMockedDataByReqId(mockData.MockRequest.RequestIdentifier);
            var elementsToKeep = Container
                .Except(dataToRemove)
                .ToArray();

            Container.Clear();
            Container.Add(batchedReqResponseContainer);
            elementsToKeep.ToList().ForEach(e=>Container.Add(e));
        }

        public MockDataStorage(IEnumerable<KeyValuePair<MockRequestOptions, MockedResponse>> keyValues)
        {
            Container = new ConcurrentBag<KeyValuePair<MockRequestOptions, MockedResponse>>
                (keyValues);
        }

        public MockDataStorage Filter(Func<KeyValuePair<MockRequestOptions, MockedResponse>, bool> func)
        {
            var filteredDict = Container
                .Where(func);

            return ToMockStorage(filteredDict);
        }

        public MockDataStorage Filter(
            IEnumerable<KeyValuePair<MockRequestOptions, MockedResponse>> lookupCollection,
            Func<KeyValuePair<MockRequestOptions, MockedResponse>, bool> func)
        {
            var filteredDict = lookupCollection
                .Where(func);

            return ToMockStorage(filteredDict);
        }

        public MockDataStorage FilterByPathAndMethod(string path, string method)
        {
            return Filter(x => x.Key.RequestIdentifier.EqualsByPathAndMethod(new RequestIdentifier(path, method)));
        }

        public MockDataStorage FilterByHeaders(IDictionary<string, string> appHeaders)
        {
            var mockedHeadersExist = Container
                .Where(kv => kv.Key.Headers != null)
                .ToArray();

            var filteredMocks = Filter(
                mockedHeadersExist,
                x => SafeMatch(x.Key.Headers, appHeaders));

            return !mockedHeadersExist.Any() ? this : filteredMocks;
        }

        public MockDataStorage FilterByCookies(IDictionary<string, string> appCookies)
        {
            var mocksWithCookies = Container
                .Where(kv => kv.Key.Cookies != null);

            return !mocksWithCookies.Any() ? this : Filter(x => SafeMatch(x.Key.Headers, appCookies));
        }

        public MockDataStorage FilterByBody(string body)
        {
            var mocksWithBody = Container.Where(s => s.Key.Body != null);
            return !mocksWithBody.Any() ? this : Filter(x => SafeMatch(x.Key.Body, body));
        }

        public MockDataStorage FilterByQueryParams(IDictionary<string, string> queryParams)
        {
            var mocksWithQueryParams = Container.Where(s => s.Key.QueryParams != null);
            return !mocksWithQueryParams.Any() ? this : Filter(x => SafeMatch(x.Key.QueryParams, queryParams));
        }

        private MockDataStorage ToMockStorage(IEnumerable<KeyValuePair<MockRequestOptions, MockedResponse>> keyValue)
        {
            return new MockDataStorage(keyValue);
        }

        private bool SafeMatch(IOption option, string other)
        {
            return option == null || option.IsMatch(other);
        }

        private bool SafeMatch(IOption option, IDictionary<string, string> other)
        {
            return option == null || option.IsMatch(other);
        }
    }
}