using System;
using System.Collections.Concurrent;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Enums;
using Mockus.WebApi.Abstractions.Data;
using Serilog;
using ILogger = Serilog.ILogger;


namespace Mockus.WebApi.Implementation
{
    public class ExecutionPolicyRepository : IExecutionPolicyRepository
    {
        private readonly ConcurrentDictionary<IRequestIdentifier, IExecutionPolicy> _executionPolicyStorage;
        private readonly ILogger _logger = Log.ForContext<ExecutionPolicyRepository>();
        public ExecutionPolicyRepository(IStorage storage)
        {
            _executionPolicyStorage = storage.ExecutionPolicies;
        }

        public void Add(IRequestIdentifier identifier, IExecutionPolicy policy)
        {
            if (_executionPolicyStorage.ContainsKey(identifier))
            {
                throw new InvalidOperationException($"Policy for request identifier {identifier} already defined");
            }
            _executionPolicyStorage.AddOrUpdate(identifier, policy, (_, _) => policy);
        }

        public IExecutionPolicy Get(IRequestIdentifier identifier)
        {
            return _executionPolicyStorage.TryGetValue(identifier, out var value) ? value : default;
        }

        public void SetStatus(IRequestIdentifier identifier, PolicyStatus status)
        {
            if (_executionPolicyStorage.TryGetValue(identifier, out var value))
            {
                _logger.Warning($"Policy for request {identifier} not found");
            }

            value.Status = status;
        }

        public void Remove(IRequestIdentifier identifier)
        {
            _executionPolicyStorage.TryRemove(identifier, out var _);
        }

        public void Clear()
        {
            _executionPolicyStorage.Clear();
        }
    }
}
