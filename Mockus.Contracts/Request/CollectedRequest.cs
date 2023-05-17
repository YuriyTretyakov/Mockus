using System;
using System.Collections.Generic;
using Mockus.Contracts.Abstractions.Integration;

namespace Mockus.Contracts.Request
{
    public class CollectedRequest : IRequest
    {
        public string Path { get; set; }
        public IDictionary<string, string> QueryParams { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public IDictionary<string, string> Cookies { get; set; }
        public DateTime Time { get; set; }
        public string Method { get; set; }
        public string Body { get; set; }

        public override string ToString()
        {
            return $"Path: {Path} Method: {Method} Body:{Body}";
        }
    }
}