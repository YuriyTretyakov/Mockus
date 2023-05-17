using System.Collections.Generic;

namespace Mockus.Contracts.Abstractions.Integration
{
    public interface IRequest
    {
        string Path { get; set; }
        string Method { get; set; }
        string Body { get; set; }
        IDictionary<string, string> Headers { get; set; }
        IDictionary<string, string> Cookies { get; set; }
        IDictionary<string, string> QueryParams { get; set; }
    }
}