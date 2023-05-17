using System;
using System.Text.RegularExpressions;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Enums;

namespace Mockus.Contracts.Request
{
    public class RequestBodyPattern : IRequestBodyPattern
    {
        public string Value { get; set; }
        public StringCompareOptions CompareType { get; set; }

        public RequestBodyPattern()
        {
        }

        public RequestBodyPattern(string value)
        {
            Value = value;
            CompareType = StringCompareOptions.Equals;
        }

        public RequestBodyPattern(string value, StringCompareOptions compare) : this(value)
        {
            CompareType = compare;
        }

        private bool Compare(string other)
        {
            return CompareType switch
            {
                StringCompareOptions.Equals => other.Equals(Value),
                StringCompareOptions.EndsWith => other.EndsWith(Value),
                StringCompareOptions.StartsWith => other.StartsWith(Value),
                StringCompareOptions.Pattern => new Regex(Value, RegexOptions.Multiline|RegexOptions.Singleline).IsMatch(other),
                StringCompareOptions.Contains => other.Contains(Value),
                _ => throw new NotImplementedException($"{nameof(CompareType)} is not implemented: {CompareType}"),
            };
        }

        public bool IsMatch(string body)
        {
            return Compare(body);
        }
    }
}