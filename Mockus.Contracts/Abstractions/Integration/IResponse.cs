using System.Collections.Generic;

namespace Mockus.Contracts.Abstractions.Integration
{
    public interface IResponse
    {
        string Body { get; set; }
        Dictionary<string, string> Headers { get; set; }
        Dictionary<string, string> Cookies { get; set; }

        string ContentType { get; set; }
    }
}