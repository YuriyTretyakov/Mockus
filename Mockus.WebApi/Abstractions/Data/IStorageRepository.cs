using System.Collections.Generic;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Request;
using Mockus.Contracts.Response;


namespace Mockus.WebApi.Abstractions.Data
{
    public interface IStorageRepository
    {
        void Add(SetMockDataRequest setMockDataRequest);
        MockedResponse GetResponse(IRequest appRequest);
        IEnumerable<MockedDataResponse> GetAll();
        void Clean();
        IEnumerable<MockedDataResponse> GetMockedData(RequestIdentifier identifier);
        void RemoveMockedRequests(RequestIdentifier requestIdentifier);
        public void RemoveMockedRequestsByPattern(RequestPatternIdentifier requestPattern);
        public void RemoveMockByMockRequestOptions(MockRequestOptions mockRequest);
    }
}