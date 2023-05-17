using System.Threading.Tasks;
using Mockus.Contracts.Response;

namespace Mockus.Contracts.Abstractions.Integration
{
    public interface IHasResponseOptions
    {
        ResponseOptions ResponseOptions { get; set; }
        Task Execute();
    }
}