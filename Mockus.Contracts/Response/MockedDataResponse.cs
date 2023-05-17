using Mockus.Contracts.Request;

namespace Mockus.Contracts.Response
{
    public class MockedDataResponse
    {
        public MockRequestOptions MockRequestOptions { get; set; }
        public MockedResponse MockedResponse { get; set; }

        public MockedDataResponse(MockRequestOptions mockRequest, MockedResponse mockedResponse)
        {
            MockRequestOptions = mockRequest;
            MockedResponse = mockedResponse;
        }
    }
}
