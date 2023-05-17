using System.Collections.Generic;
using Mockus.Contracts.Request;

namespace Mockus.WebApi.Abstractions.Data
{
    public interface ICollectedRequestRepository
    {
        void Add(CollectedRequest request);
        void Clean();
        IEnumerable<CollectedRequest> GetAll();
        IEnumerable<CollectedRequest> GetSpecific(object spec);
        IEnumerable<CollectedRequest> GetWithBody(string text);
        IEnumerable<CollectedRequest> Get(RequestIdentifier identifier);
        IEnumerable<CollectedRequest> GetByPatternBody(RequestBodyPattern identifier);
    }
}