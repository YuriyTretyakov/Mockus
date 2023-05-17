using System.Collections.Generic;
using System.Linq;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Response;
using Mockus.WebApi.Contracts;

namespace Mockus.WebApi.Implementation
{
    public class ResponseFinder
    {
        private readonly MockDataStorage _mockDataStorage;
        private readonly ReadOnlyStorage _readOnlyStorage;

        public ResponseFinder(
            MockDataStorage mockDataStorage,
            ReadOnlyStorage readOnlyStorage)
        {
            _mockDataStorage = mockDataStorage;
            _readOnlyStorage = readOnlyStorage;
        }

        public MockedResponse FindResponse(IRequest appRequest)
        {
            var responses = FindResponses(appRequest)
                .ToList();
            
            if (!responses.Any())
            {
                responses = _readOnlyStorage
                    .FilterByPathAndMethod(appRequest.Path, appRequest.Method)
                    .Container
                    .Select(kv => kv.Value)
                    .ToList();
            }

            return responses.Any()
                ? responses?.Aggregate((it1, it2) => it1.DataVersion > it2.DataVersion ? it1 : it2)
                : null;
        }

        public IEnumerable<MockedResponse> FindResponses(IRequest appRequest)
        {
            var filteredMocks =
                _mockDataStorage.FilterByPathAndMethod(appRequest.Path, appRequest.Method)
                    .FilterByBody(appRequest.Body)
                    .FilterByHeaders(appRequest.Headers)
                    .FilterByCookies(appRequest.Cookies)
                    .FilterByQueryParams(appRequest.QueryParams);

            return filteredMocks.Container.Select(kv => kv.Value).ToList();
        }
    }
}