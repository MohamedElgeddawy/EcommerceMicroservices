using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceProvider AddSharedServices<TContext>
            (this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
        {
            //Add Generic Database context
            services.AddDbContext<TContext>(options => options.UseSqlServer
            (config.GetConnectionString("eCommerceConnection"), sqlserverOption =>
                sqlserverOption.EnableRetryOnFailure()));

            //configure serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestem:yyyy-MM-dd HH:mm:ss.ff zzz} [{level:u3}] {message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // add JWT authentication Scheme
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);

            return services.BuildServiceProvider();
        }

        public static IApplicationBuilder UseShardPolicies(this IApplicationBuilder app)
        {
            //use global Exceptioin
            app.UseMiddleware<GlobalException>();

            // Register middleware to b;ock all outsiders API calls
            // app.UseMiddleware<ListenToOnlyApiGateway>();

            return app;
        }
    }
}
