using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Request;
using Mockus.Contracts.Response;
using Mockus.Contracts.Enums;
using Mockus.Contracts.Exceptions;
using Mockus.WebApi.Abstractions.Data;
using Mockus.WebApi.Contracts;

namespace Mockus.WebApi.Implementation
{
    public class StorageRepository : IStorageRepository
    {
        private readonly MockDataStorage _mockDataStorage;
        private readonly ReadOnlyStorage _readOnlyStorage;
        private static readonly object _locker = new();

        public StorageRepository(IStorage storage)
        {
            _mockDataStorage = storage.MockDataStorage;
            _readOnlyStorage = storage.ReadOnlyStorage;
        }

        public void Add(SetMockDataRequest setMockDataRequest)
        {
            if (setMockDataRequest.Operation == MockOperation.Add)
            {
                _mockDataStorage.Add(setMockDataRequest);
            }

            else if (setMockDataRequest.Operation == MockOperation.Update)
            {
                lock (_locker)
                {
                    var storedDataByReqId = _mockDataStorage
                        .GetMockedDataByReqId(setMockDataRequest.MockRequest.RequestIdentifier);

                    if (storedDataByReqId.Any())
                    {
                        var batchedReqResponseContainer = storedDataByReqId
                            .FirstOrDefault();

                        if (batchedReqResponseContainer.Value.DataVersion >=
                            setMockDataRequest.MockResponse.DataVersion)
                        {
                            throw new VersionMismatchException(
                                setMockDataRequest.MockResponse.DataVersion,
                                batchedReqResponseContainer.Value.DataVersion);
                        }

                        _mockDataStorage.UpdateBatchedData(setMockDataRequest);
                    }
                    else
                    {
                        _mockDataStorage.Add(setMockDataRequest);
                    }
                }
            }
        }

        public MockedResponse GetResponse(IRequest request)
        {
            var internalResponse = new ResponseFinder(_mockDataStorage, _readOnlyStorage).FindResponse(request);
            return internalResponse;
        }

        public IEnumerable<MockedDataResponse> GetAll()
        {
            return _mockDataStorage.GetAll();
        }

        //Used by controller
        public IEnumerable<MockedDataResponse> GetMockedData(RequestIdentifier identifier)
        {
            var mockedData = _mockDataStorage.GetMockedDataByReqId(identifier).ToList();
            return CreateOrGet(mockedData, identifier);
        }

        IEnumerable<MockedDataResponse> CreateOrGet(
            IEnumerable<KeyValuePair<MockRequestOptions, MockedResponse>> mockedData,
            RequestIdentifier identifier)
        {
            var resultList = new List<MockedDataResponse>();
            if (!mockedData.Any())
            {
                resultList.Add(new MockedDataResponse(new MockRequestOptions { RequestIdentifier = identifier },
                    new MockedResponse()));
            }
            else
            {
                resultList.AddRange(new List<MockedDataResponse>(mockedData
                    .Select(s => new MockedDataResponse(s.Key, s.Value))));
            }

            return resultList;
        }


        public void Clean()
        {
            _mockDataStorage.Clear();
        }

        public void RemoveMockedRequests(RequestIdentifier requestIdentifier)
        {
            _mockDataStorage.RemoveMockedRequests(requestIdentifier);
        }

        public void RemoveMockedRequestsByPattern(RequestPatternIdentifier requestPattern)
        {
            _mockDataStorage.RemoveMockedRequestsByPattern(requestPattern);
        }

        public void RemoveMockByMockRequestOptions(MockRequestOptions mockRequest)
        {
            _mockDataStorage.RemoveMockByMockRequestOptions(mockRequest);
        }
    }
}
