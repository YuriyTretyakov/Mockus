using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Enums;

namespace Mockus.Contracts.Request
{
    public class RequestPatternIdentifier : IRequestPatternIdentifier
    {
        public string Path { get; set; }
        public StringCompareOptions CompareType { get; set; }
        public string Method { get; set; }

        public RequestPatternIdentifier()
        {
        }

        public RequestPatternIdentifier(string path)
        {
            Path = path;
            CompareType = StringCompareOptions.Equals;
        }

        public RequestPatternIdentifier(string path, StringCompareOptions compare) : this(path)
        {
            CompareType = compare;
        }

        public RequestPatternIdentifier(string path, StringCompareOptions compare, HttpMethod method) : this(path,compare)
        {
            Method = method.ToString();
        }

        private bool Compare(string other)
        {
            return CompareType switch
            {
                StringCompareOptions.Equals => other.Equals(Path),
                StringCompareOptions.EndsWith => other.EndsWith(Path),
                StringCompareOptions.StartsWith => other.StartsWith(Path),
                StringCompareOptions.Pattern => new Regex(Path, RegexOptions.Multiline|RegexOptions.Singleline).IsMatch(other),
                StringCompareOptions.Contains => other.Contains(Path),
                _ => throw new NotImplementedException($"{nameof(CompareType)} is not implemented: {CompareType}"),
            };
        }

        public bool IsMatch(RequestIdentifier requestIdentifier)
        {
            return Compare(requestIdentifier.Path)
                   && Method?.Equals(requestIdentifier.Method) is true or null;
        }
    }
}