using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Enums;

namespace Mockus.WebApi.Abstractions.Data
{
    public interface IExecutionPolicyRepository
    {
        void Add(IRequestIdentifier identifier, IExecutionPolicy policy);
        IExecutionPolicy Get(IRequestIdentifier identifier);
        void SetStatus(IRequestIdentifier identifier, PolicyStatus status);
        void Remove(IRequestIdentifier identifier);
        void Clear();
    }
}