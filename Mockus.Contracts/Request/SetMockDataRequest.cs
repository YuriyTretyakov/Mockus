using Mockus.Contracts.Response;
using Mockus.Contracts.Enums;

namespace Mockus.Contracts.Request
{
    public class SetMockDataRequest
    {
        public MockRequestOptions MockRequest { get; set; }
        public MockedResponse MockResponse { get; set; }

        public MockOperation Operation { get; set; }
    }
}
