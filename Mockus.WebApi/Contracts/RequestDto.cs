using System.Collections.Generic;
using Mockus.Contracts.Abstractions.Integration;

namespace Mockus.WebApi.Contracts
{
    public class RequestDto : IRequest
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public string Body { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public IDictionary<string, string> Cookies { get; set; }
        public IDictionary<string, string> QueryParams { get; set; }
    }
}