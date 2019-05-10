﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Common;
using API_SERVER.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API_SERVER
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
            services.AddMvc().AddMvcOptions(options =>
            {
                // This adds both Input and Output formatters based on DataContractSerializer
                //options.AddXmlDataContractSerializerFormatter();

                // To add XmlSerializer based Input and Output formatters.
                //options.InputFormatters.Add(new XmlSerializerInputFormatter());
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            });
         
            services.AddCors(options => 
                             options.AddPolicy("AllowSameDomain",builder => 
                                               builder.AllowAnyOrigin()
                                               .AllowAnyHeader()
                                               .AllowAnyMethod()
                                               .WithExposedHeaders(new string[]{"code", "msg"})
                                               .AllowCredentials()));

            Common.Global.StartUp();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseCors("AllowSameDomain");
            app.UseStaticFiles("/"+ Global.ROUTE_PX);
            app.Map("/zf", SocketController.Map);
        }
    }
}
