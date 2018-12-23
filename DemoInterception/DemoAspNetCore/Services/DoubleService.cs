using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoAspNetCore.Services
{
    /// <summary>
    /// 双倍计算服务
    /// </summary>
    [Intercept(typeof(Interceptors.CallLogger))] // Associate (option 1)
    //[Intercept("log-calls")]// Associate (option 1)
    public class DoubleService : DemoAspNetCore.Interfaces.ICalcService
    {
        public virtual int Calc(int data)
        {
            int returnVal = data * 2;
            Console.WriteLine("Double {0} is {1}", data, returnVal);
            return returnVal;
        }
    }
}
