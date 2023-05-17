using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Mockus.WebApi.Abstractions.Data
{
    public interface IMockManager
    {
        Task UpdateContextAsync(HttpContext context);
    }
}