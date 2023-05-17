using System.Net;
using Mockus.Contracts;
using Mockus.Contracts.Response;

namespace Mockus.Contracts.Abstractions.Integration
{
    public interface IMockResponseBuilder
    {
        IMockResponseBuilder WithBody(string value);
        IMockResponseBuilder WithJsonBody(object value);
        IMockResponseBuilder WithHeaders(string key, string value);
        IMockResponseBuilder WithCookies(string key, string value);
        IMockResponseBuilder WithStatusCode(HttpStatusCode code);
        IMockResponseBuilder WithDelay(int seconds);
        MockedResponse Create();
    }
}