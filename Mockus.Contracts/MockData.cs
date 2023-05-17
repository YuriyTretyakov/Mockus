using System;
using System.ComponentModel.DataAnnotations;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Request;
using Mockus.Contracts.Response;

namespace Mockus.Contracts
{
    public class MockData : IMockDataBuilder
    {
        [Required] public MockRequestOptions MockRequestOptions { get; set; }
        [Required] public MockedResponse MockedResponse { get; set; }
        

        public IMockDataBuilder WhenRequest(MockRequestOptions request)
        {
            MockRequestOptions = request;
            return this;
        }

        public IMockDataBuilder WhenRequest(IMockRequestBuilder request)
        {
            MockRequestOptions = request.Create();
            return this;
        }

        public IMockDataBuilder ThenRespond(IMockResponseBuilder response)
        {
            MockedResponse = response.Create();
            return this;
        }

        public IMockDataBuilder ThenRespond(MockedResponse response)
        {
            MockedResponse = response;
            return this;
        }

        public SetMockDataRequest Create()
        {
            if (MockRequestOptions == null)
            {
                throw new InvalidOperationException($"{nameof(MockRequestOptions)} shouldnot be null");
            }

            if (MockedResponse == null)
            {
                throw new InvalidOperationException($"{nameof(MockedResponse)} shouldnot be null");
            }


            return new SetMockDataRequest
            {
                MockResponse = MockedResponse,
                MockRequest = MockRequestOptions
            };
        }
    }
}