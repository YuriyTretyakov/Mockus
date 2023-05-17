using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.ExecutionPolicy;
using Mockus.Contracts.Request;
using Mockus.Contracts.Enums;
using Serilog.Core;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Mockus.Client.Settings
{

    public static class MocusSettings
    {
        private static readonly ConcurrentBag<IRequestIdentifier> _batchSettings = new();
        private static string _url;
        private static ILogger _logger;

        public static void AddExecutionPolicy(RequestIdentifier identifier, IExecutionPolicy policy)
        {
            var setPolicyModel = new ExecutionPolicyModel
            {
                Identifier = identifier,
                ExecutionPolicy = policy
            };
            var client = CreateClient();

            Task.WaitAll(client.SetExecutionPolicyAsync(setPolicyModel));

            if (!client.IsSuccess)
                throw new ApplicationException($"Unable to Add execution policy. Error: {client.MessageText}");
        }

        public static void SetPolicyStatus(RequestIdentifier identifier, PolicyStatus status)
        {
            var client = CreateClient();

            Task.WaitAll(client.SetPolicyStatus(identifier, status));

            if (!client.IsSuccess)
                throw new ApplicationException($"Unable to Add execution policy. Error: {client.MessageText}");
        }

        public static void ClearExecutionPolicies()
        {
            var client = CreateClient();
            Task.WaitAll(client.ClearAllPolicies());
            if (!client.IsSuccess)
                throw new ApplicationException($"Unable to Add execution policy. Error: {client.MessageText}");
        }

        public static void AddMockClientConfig(string url, ILogger logger)
        {
            _url = url;
            _logger = logger;
        }
        private static MockusClient CreateClient()
        {
            return new MockusClient(_url, _logger);
        }
    }
}
