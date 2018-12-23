using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;

namespace DemoAspNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // Logger function registration (by Microsoft.Extensions.DependencyInjection)
            services.AddSingleton<Interfaces.ILogger, Services.ConsoleLogger>();

            var builder = new ContainerBuilder();
            builder.Populate(services);

            // Step 2：
            // Named registration
            builder.RegisterType<Interceptors.CallLogger>()
                .Named<IInterceptor>("log-calls");
            // Typed registration
            builder.RegisterType<Interceptors.CallLogger>();

            //  Step 3：
            // Class interception
            builder.RegisterType<Services.DoubleService>()
                .EnableClassInterceptors()
                //.InterceptedBy(typeof(Interceptors.CallLogger));// Step 4： Associate (option 2)
                ;
            // Interface interception
            builder.RegisterType<Services.TripleService>().As<Interfaces.ICalcService>()
                .EnableInterfaceInterceptors()
                //.InterceptedBy(typeof(Interceptors.CallLogger)) // Step 4： Associate (option 2)
                ;
            this.ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
