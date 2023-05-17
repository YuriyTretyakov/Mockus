using Mockus.WebApi.Abstractions.Data;
using Mockus.WebApi.Data;
using Mockus.WebApi.Extensions;
using Mockus.WebApi.Implementation;
using Mockus.WebApi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mockus.WebApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationOptions(_configuration);
            
            services.AddSingleton<IStorage, Storage>();
            services.AddSingleton<IMockManager, MockManager>();
            services.AddSingleton<ICollectedRequestRepository, CollectedRequestRepository>();
            services.AddSingleton<IStorageRepository, StorageRepository>();
            services.AddSingleton<IExecutionPolicyRepository, ExecutionPolicyRepository>();

            services.AddTransient<ErrorLoggingMiddleware>();
            services.AddTransient<MockManagerEntryMiddleware>();
            services.AddSwagger("Mockus Web Api");
            services.WithCors();
            services.WithMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MockusWebApi");
                c.RoutePrefix = "";
            });
            app.UseRouting();
            app.UseMiddleware<ErrorLoggingMiddleware>();
            app.UseMiddleware<MockManagerEntryMiddleware>();

            app.UseEndpoints(endpoints => 
            { 
                endpoints.MapControllers();
            });
        }
    }
}