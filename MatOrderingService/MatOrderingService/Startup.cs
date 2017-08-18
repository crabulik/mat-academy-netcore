using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MatOrderingService.Services.Storage;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using MatOrderingService.Services.Auth;
using MatOrderingService.Services.Logger;
using MatOrderingService.Services.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MatOrderingService.Services.Storage.Impl;
using MatOrderingService.Services.Generator;
using MatOrderingService.Services.Generator.Impl;
using MatOrderingService.Filters;

namespace MatOrderingService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetValue<string>("Data:ConnectionString");

            services.AddDbContext<OrdersDbContext>
                (options => options.UseSqlServer(
                    connectionString)
                    .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.QueryClientEvaluationWarning)));

            services.AddCors();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(EntityNotFoundExceptionFilter));
            });

            services.AddAutoMapper();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Materialise Academy Orders API", Version = "v1" });
                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "MatOrderingService.xml");
                c.IncludeXmlComments(filePath);
                c.OperationFilter<SwaggerAuthorizationHeaderParameter>(Configuration.GetValue<string>("AuthOptions:AuthenticationScheme"));
            });

            services.Configure<MatOsAuthOptions>(Configuration.GetSection("AuthOptions"));

            services.AddAuthorization(auth =>
            {
                auth.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(Configuration.GetValue<string>("AuthOptions:AuthenticationScheme"))
                    .RequireAuthenticatedUser().Build();
            });

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<IGuidGenerator, GuidGenerator>();
            services.AddSingleton<IOrdersList, OrdersList>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors(options =>
                options.WithOrigins("*")
                    .WithHeaders("Content-Type", "x-xsrf-token", "authorization")
                    .WithMethods("GET", "POST", "PUT", "DELETE", "HEAD", "OPTIONS"));

            app.UseMiddleware<MatOsAuthMiddleware>();

            app.UseMiddleware<RequestLoggerMiddleware>();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Materialise Academy Orders API");
            });

            app.Map("/env", HandleEnvInfo);

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("Mat prefix: ");
                await next.Invoke();
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from MAT ordering service");
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync("How can you see me????");
            });
        }

        private static void HandleEnvInfo(IApplicationBuilder app)
        {
            app.Map("/info", Info);

            app.Run(async context =>
            {
                var srv = (IConfiguration)context.RequestServices.GetService(typeof(IConfiguration));
                await context.Response.WriteAsync(srv.GetValue<string>("EnvironmentInfo"));
            });
        }

        private static void Info(IApplicationBuilder app)
        {

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Deep Info");
            });
        }
    }
}
