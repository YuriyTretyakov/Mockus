using System.Net.Http;

namespace Mockus.Contracts.Abstractions.Integration
{
    public interface IRequestIdentifier
    {
        public string Path { get; set; }
        public string Method { get; set; }
    }
}
