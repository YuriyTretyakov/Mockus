using System.Threading.Tasks;
using Mockus.WebApi.Abstractions.Data;
using Microsoft.AspNetCore.Http;

namespace Mockus.WebApi.Middleware
{
    public class MockManagerEntryMiddleware : IMiddleware
    {
        private readonly IMockManager _mockManager;

        public MockManagerEntryMiddleware(IMockManager mockManager)
        {
            _mockManager = mockManager;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path.Value;

            if (path?.ToLower().Contains("manage") == true)
            {
                await next.Invoke(context);
            }

            else
            {
                await _mockManager.UpdateContextAsync(context);
            }
        }
    }
}