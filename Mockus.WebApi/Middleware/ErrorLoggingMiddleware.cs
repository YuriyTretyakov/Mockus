using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Mockus.Contracts.Response;
using Mockus.Contracts.Exceptions;
using Mockus.WebApi.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace Mockus.WebApi.Middleware
{
    public class ErrorLoggingMiddleware : IMiddleware
    {
        private readonly ILogger _logger = Log.ForContext<ErrorLoggingMiddleware>();

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
#if DEBUG
                context.Request.EnableBuffering();
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    var bodyStr = await reader.ReadToEndAsync();

                    if (!string.IsNullOrEmpty(bodyStr))
                    {
                        _logger.Debug(
                            "Request received :: Path {request} Method {@meth} Body {@bodyStr}",
                            context.Request.Path,
                            context.Request.Method,
                            bodyStr);
                        context.Request.Body.Position = 0;
                    }
                }
#endif

                    _logger.Information(
                    "Request received :: Path {path} Method {@meth}",
                    context.Request.Path,
                    context.Request.Method);


                await next.Invoke(context);
                _logger.Information("Response code returned {@ResponseCode}", context.Response.StatusCode);
            }

            catch (InvalidModelException ex)
            {
                var message = ErrorMessage.Convert(ex);
                await AddErrorIntoContext(context.Response, HttpStatusCode.BadRequest, message);
            }

            catch (VersionMismatchException ex)
            {
                var message = ErrorMessage.Convert(ex);
                await AddErrorIntoContext(context.Response, HttpStatusCode.BadRequest, message);
            }

            catch (Exception ex)
            {
                var message = ErrorMessage.Convert(ex);
                await AddErrorIntoContext(context.Response, HttpStatusCode.InternalServerError, message);
            }
        }

        private async Task AddErrorIntoContext(HttpResponse response, HttpStatusCode code, ErrorMessage message)
        {
            if (response.HasStarted)
            {
                _logger.Error("The response has already started, the api exception middleware will not be executed");
            }

            _logger.Error("Exception was caught by middleware: {@ex}", message);


            response.StatusCode = (int) code;
            response.ContentType = "application/json";
            await response.WriteAsync(JsonConvert.SerializeObject(message, Formatting.Indented));
        }
    }
}