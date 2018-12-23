using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DemoNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            // Logger function registration (by Microsoft.Extensions.DependencyInjection)
            services.AddSingleton<Interfaces.ILogger, Services.ConsoleLogger>();

            var serviceProvider = ConfigureServicesByAutofac(services);
            
            // Resolve services
            var doubleService = serviceProvider.GetService<Services.DoubleService>();
            var tripleService = serviceProvider.GetService<Interfaces.ICalcService>();

            // Call calc via class interception
            doubleService.Calc(1);
            Console.WriteLine();
            // Call calc via interface interception
            tripleService.Calc(2);
            Console.ReadLine();
        }

        /// <summary>
        /// Autofac
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        static IServiceProvider ConfigureServicesByAutofac(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);

            //// Logger function registration
            //builder.RegisterType<Services.ConsoleLogger>().As<Interfaces.ILogger>();

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
                //.InterceptedBy(typeof(Interceptors.CallLogger)) // Step 4： Associate (option 2)
                ;
            // Interface interception
            builder.RegisterType<Services.TripleService>().As<Interfaces.ICalcService>()
                .EnableInterfaceInterceptors()
                //.InterceptedBy(typeof(Interceptors.CallLogger)) // Step 4： Associate (option 2)
                ;

            // Build container
            var container = builder.Build();
            var serviceProvider = new AutofacServiceProvider(container);
            return serviceProvider;
        }
    }
}
