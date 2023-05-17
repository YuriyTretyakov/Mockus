using System.Collections.Concurrent;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Request;
using Mockus.WebApi.Abstractions.Data;
using Mockus.WebApi.Contracts;
using Mockus.WebApi.Options;

namespace Mockus.WebApi.Data
{
    public class Storage : IStorage
    {
        public MockDataStorage MockDataStorage { get; set; }
        public ReadOnlyStorage ReadOnlyStorage { get; set; }
        public ConcurrentBag<CollectedRequest> CollectedRequests { get; set; }
        public ConcurrentDictionary<IRequestIdentifier, IExecutionPolicy> ExecutionPolicies { get; }

        public Storage()
        {
            MockDataStorage = new MockDataStorage();
            CollectedRequests = new ConcurrentBag<CollectedRequest>();
            ExecutionPolicies = new ConcurrentDictionary<IRequestIdentifier, IExecutionPolicy>();
            ReadOnlyStorage = new ReadOnlyStorage();
        }
    }
}