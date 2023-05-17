using System;
using System.Text.RegularExpressions;
using Mockus.Contracts.Abstractions.Options;
using Mockus.Contracts.Enums;

namespace Mockus.Contracts.Options
{
    public class StringOption : IOption, IHasValue<string>
    {
        public StringOption()
        {
        }

        public StringOption(string value)
        {
            Value = value;
            CompareType = StringCompareOptions.Equals;
        }

        public StringOption(string value, StringCompareOptions compare) : this(value)
        {
            CompareType = compare;
        }

        public string Value { get; set; }

        public bool IsMatch(string other)
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

        public  void Validate()
        {
            switch (CompareType)
            {
                case StringCompareOptions.Pattern:
                    Regex.Match("", Value);
                    break;
            }
        }

        public  bool IsMatch(object other)
        {
            var stringObj = other as string;
            return IsMatch(stringObj);
        }

        public StringCompareOptions CompareType { get; set; }
        

        public override bool Equals(object? obj)
        {
            var other = obj as StringOption;

            if (other == null)
                return false;

            return (CompareType.Equals(other.CompareType) & (Value.Equals(other.Value)));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + CompareType.GetHashCode();
                hash = hash * 23 + Value.GetHashCode();
                return hash;
            }
        }
    }
}