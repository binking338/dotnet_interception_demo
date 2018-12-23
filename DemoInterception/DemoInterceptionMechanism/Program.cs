using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DemoInterceptionMechanism
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("不使用代理时的调用结果：");
            NoneProxy();

            Console.WriteLine();
            Console.ReadLine();
            Console.WriteLine("使用静态态代理时的调用结果：");
            StaticProxy();

            Console.WriteLine();
            Console.ReadLine();
            Console.WriteLine("使用动态代理时的调用结果：");
            DynamicProxy();
        }

        /// <summary>
        /// 示例不使用代理
        /// </summary>
        static void NoneProxy()
        {
            var classInstance = new DoubleCalcService();
            var interfaceImplInstance = new TripleCalcService();

            classInstance.Calc(1);
            interfaceImplInstance.Calc(1);
        }

        /// <summary>
        /// 示例静态代理
        /// </summary>
        static void StaticProxy()
        {
            var classInstanceProxy = new CodeByHand.DoubleCalcServiceProxy();
            var interfaceImplInstanceProxy = new CodeByHand.TripleCalcServiceProxy();

            classInstanceProxy.Calc(2);
            interfaceImplInstanceProxy.Calc(2);
        }

        /// <summary>
        /// 示例动态代理
        /// </summary>
        static void DynamicProxy()
        {
            var proxyFactory = new DynamicProxyFactory();

            //Class拦截代理
            var classProxyType = proxyFactory.CreateClassProxy<CallLogger>(typeof(DoubleCalcService),
                typeof(DoubleCalcService).GetMethod("Calc"));
            //Interface拦截代理
            var interfaceProxyType = proxyFactory.CreateInterfaceProxy<CallLogger>(typeof(ICalcService), typeof(TripleCalcService),
                typeof(ICalcService).GetMethod("Calc"));

            //Expression实例化代理类
            var classProxyInstance = Expression.Lambda<Func<DoubleCalcService>>(Expression.New(classProxyType)).Compile().Invoke();
            var interfaceProxyInstance = Expression.Lambda<Func<ICalcService>>(Expression.New(interfaceProxyType)).Compile().Invoke();

            //通过代理对象调用Double服务
            classProxyInstance.Calc(3);
            //通过代理对象调用Triple服务
            interfaceProxyInstance.Calc(3);
            Console.ReadLine();
        }
    }
}

