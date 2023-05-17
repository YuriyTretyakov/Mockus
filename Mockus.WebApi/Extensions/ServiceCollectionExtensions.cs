using Mockus.WebApi.Options;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Mockus.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, string swaggerTitle)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = swaggerTitle, Version = "1.0"});
                c.CustomSchemaIds(type => type.ToString());
            });
            return services;
        }

        public static IServiceCollection WithCors(this IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("allow_all_policy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
            return services;
        }

        public static IServiceCollection WithMvc(this IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.RespectBrowserAcceptHeader = true;
                    options.OutputFormatters.RemoveType<StringOutputFormatter>();
                    options.OutputFormatters.Add(new StringOutputFormatter());
                })
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    opt.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                    opt.SerializerSettings.SerializationBinder = new DefaultSerializationBinder();
                });

            return services;
        }
        
        public static void AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
        {
        }
    }
}