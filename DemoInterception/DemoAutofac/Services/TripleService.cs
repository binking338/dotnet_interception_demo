using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoAutofac.Services
{
    /// <summary>
    /// 三倍计算服务
    /// </summary>
    //[Intercept(typeof(Interceptors.CallLogger))] // Associate (option 1)
    [Intercept("log-calls")]// Associate (option 1)
    public class TripleService : Interfaces.ICalcService
    {
        public int Calc(int data)
        {
            int returnVal = data * 3;
            Console.WriteLine("Triple {0} is {1}", data, returnVal);
            return returnVal;
        }
    }
}
