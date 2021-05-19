using Elfo.Wardein.Abstractions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Elfo.Wardein.Core.ConfigurationManagers;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core;
using Elfo.Wardein.Abstractions;

namespace Elfo.Wardein.Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                const string docVersion = "v1";
                c.SwaggerDoc(docVersion, new OpenApiInfo
                {
                    Title = "Wardein API",
                    Version = docVersion
                });

                c.CustomSchemaIds(s => s.FullName);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", builder =>
                {
                    builder
                    .WithOrigins("http://localhost:5000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithExposedHeaders("Content-Disposition");
                });
            });

            services.AddSingleton<IAmWardeinConfigurationManager>(sp =>
            {
                switch (Enum.Parse(typeof(ConnectionType), Configuration["StorageConnectionType"]))
                {
                    case ConnectionType.FileSystem:
                        return new WardeinConfigurationManagerFromJSON(Configuration["StorageConnectionString"]);
                    case ConnectionType.Oracle:
                        return new OracleWardeinConfigurationManager(sp.GetService<IOracleHelper>(), HostHelper.GetName());
                    default:
                        throw new NotImplementedException();

                }
            });
            services.AddTransient<IOracleHelper, OracleHelper>();
            services.AddSingleton<OracleConnectionConfiguration>(sp =>
             {                 
                 var builder = new OracleConnectionConfiguration.Builder(Configuration["StorageConnectionString"]);
                     builder
                         .WithClientId(Configuration["OracleAdditionalParams:ClientId"])
                         .WithClientInfo(Configuration["OracleAdditionalParams:ClientInfo"])
                         .WithModuleName(Configuration["OracleAdditionalParams:ModuleName"])
                         .WithDateLanguage(Configuration["OracleAdditionalParams:DateLanguage"]);
                 return builder.Build();
             });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowSpecificOrigins");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Wardein API";
                c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("v1/swagger.json", "Specification 1");
            });

        }
    }
}
