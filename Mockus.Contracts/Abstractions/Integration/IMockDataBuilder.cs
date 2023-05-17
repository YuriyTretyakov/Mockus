using Mockus.Contracts;
using Mockus.Contracts.Request;

namespace Mockus.Contracts.Abstractions.Integration
{
    public interface IMockDataBuilder
    {
        IMockDataBuilder WhenRequest(IMockRequestBuilder request);
        IMockDataBuilder ThenRespond(IMockResponseBuilder response);
        SetMockDataRequest Create();
    }
}