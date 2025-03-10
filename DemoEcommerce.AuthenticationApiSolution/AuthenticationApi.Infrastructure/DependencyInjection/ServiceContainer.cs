using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using eCommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;


namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // Add database connectivity
            // JWT Add Authentication scheme
            SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config["MySerilog:FileName"]!);

            // Create Dependency Injection 
            services.AddScoped<IUser, UserRepository>();

            return services;
        }

        public static IApplicationBuilder   UserInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register middlewarw sych as:
            // Global Exception : Handle external errors 
            //Liten to only API Gateway: Block all outsiders API calls

            SharedServiceContainer.UseShardPolicies(app);

            return app;
        }
    }
}
