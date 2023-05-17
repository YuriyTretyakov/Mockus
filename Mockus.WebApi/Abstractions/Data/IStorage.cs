using System.Collections.Concurrent;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts;
using Mockus.Contracts.Request;
using Mockus.WebApi.Contracts;

namespace Mockus.WebApi.Abstractions.Data
{
    public interface IStorage
    {
        MockDataStorage MockDataStorage { get; }
        
        ReadOnlyStorage ReadOnlyStorage { get; }
        
        ConcurrentBag<CollectedRequest> CollectedRequests { get; }

        ConcurrentDictionary<IRequestIdentifier, IExecutionPolicy> ExecutionPolicies { get; }
    }
}