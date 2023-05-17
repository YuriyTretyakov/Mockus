using Mockus.Contracts.Enums;

namespace Mockus.Contracts.Abstractions.Integration
{
    public interface IRequestPatternIdentifier
    {
        public string Path { get; set; }
        public StringCompareOptions CompareType { get; set; }
        public string Method { get; set; }
    }
}