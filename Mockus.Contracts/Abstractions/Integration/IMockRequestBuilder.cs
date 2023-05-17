using System.Collections.Generic;
using System.Net.Http;
using Mockus.Contracts.Abstractions.Options;
using Mockus.Contracts;
using Mockus.Contracts.Options;
using Mockus.Contracts.Request;
using Mockus.Contracts.Enums;

namespace Mockus.Contracts.Abstractions.Integration
{
    public interface IMockRequestBuilder
    {
        IMockRequestBuilder WithPathAndMethod(string path, HttpMethod method);
        IMockRequestBuilder WithPathAndMethod(RequestIdentifier requestIdentifier);
        IMockRequestBuilder WithBody(IOption option);
        IMockRequestBuilder WithHeaders(string key, string value);
        IMockRequestBuilder WithHeaders(string key, StringOption value);
        IMockRequestBuilder WithHeaders(MappedOption<StringOption> option);
        IMockRequestBuilder WithCookies(string key, string value);
        IMockRequestBuilder WithCookies(string key, StringOption value);
        IMockRequestBuilder WithCookies(MappedOption<StringOption> option);
        IMockRequestBuilder WithQueryParams(string key, string value);
        IMockRequestBuilder WithQueryParams(string key, SequenceOption value);
        IMockRequestBuilder WithQueryParams(string key,  IEnumerable<string> collection);
        MockRequestOptions Create();
        IMockRequestBuilder AddRequestIdentification(IdentificationSources queryParams, string lang, string locale);
    }
}