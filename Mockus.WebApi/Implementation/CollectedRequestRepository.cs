using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mockus.Contracts.Request;
using Mockus.WebApi.Abstractions.Data;
using Serilog;
using Mockus.WebApi.Abstractions.Data;

namespace Mockus.WebApi.Implementation
{
    public class CollectedRequestRepository : ICollectedRequestRepository
    {
        private readonly ConcurrentBag<CollectedRequest> _collectedRequests;
        private readonly ILogger _logger = Log.ForContext<CollectedRequestRepository>();
        public CollectedRequestRepository(IStorage storage)
        {
            _collectedRequests = storage.CollectedRequests;
        }

        public void Add(CollectedRequest request)
        {
            _logger.Information($"Storing captured request {request}");
            _collectedRequests.Add(request);
        }

        public void Clean()
        {
            _collectedRequests.Clear();
        }

        public IEnumerable<CollectedRequest> GetAll()
        {
            return _collectedRequests;
        }

        public IEnumerable<CollectedRequest> GetSpecific(object spec)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CollectedRequest> GetWithBody(string text)
        {
            return _collectedRequests.Where(r => r.Body.Contains(text));
        }

        public IEnumerable<CollectedRequest> GetByPatternBody(RequestBodyPattern identifier)
        {
            return _collectedRequests.Where(r => identifier.IsMatch(r.Body));
        }

        public IEnumerable<CollectedRequest> Get(RequestIdentifier identifier)
        {
            return _collectedRequests
                .Where(
                    r => r.Path.Equals(identifier.Path)&&
                         r.Method.Equals(identifier.Method));
        }
    }
}