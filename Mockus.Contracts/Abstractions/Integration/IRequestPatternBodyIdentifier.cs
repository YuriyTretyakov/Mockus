using Mockus.Contracts.Enums;

namespace Mockus.Contracts.Abstractions.Integration
{
    public interface IRequestBodyPattern
    {
        public string Value { get; set; }
        public StringCompareOptions CompareType { get; set; }
    }
}