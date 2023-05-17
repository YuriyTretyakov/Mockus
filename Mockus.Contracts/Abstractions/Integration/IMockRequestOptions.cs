using Mockus.Contracts.Abstractions.Options;
using Mockus.Contracts.Options;
using Mockus.Contracts.Request;

namespace Mockus.Contracts.Abstractions.Integration
{
    public interface IMockRequestOptions
    {
        RequestIdentifier RequestIdentifier { get; set; }
        IOption Body { get; set; }
        MappedOption<StringOption> Headers { get; set; }
        MappedOption<StringOption> Cookies { get; set; }
        MappedOption<SequenceOption> QueryParams { get; set; }
        void Validate();
    }
}