using Mockus.Contracts.Response;
using Mockus.Contracts.Enums;

namespace Mockus.Contracts.Abstractions.Integration
{
    public interface IExecutionPolicy
    {
        PolicyStatus Status { get;set; }
        MockedResponse Run();

    }
}
