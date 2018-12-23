using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using System;

namespace DemoAutofac
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // Logger function registration
            builder.RegisterType<Services.ConsoleLogger>().As<Interfaces.ILogger>();

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
            
            // Resolve services
            var doubleService = container.Resolve<Services.DoubleService>();
            var tripleService = container.Resolve<Interfaces.ICalcService>();

            // Call calc via class interception
            doubleService.Calc(1);
            Console.WriteLine();
            // Call calc via interface interception
            tripleService.Calc(2);
            Console.ReadLine();
        }
    }
}
