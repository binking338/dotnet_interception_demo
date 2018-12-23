using System;
using System.Collections.Generic;
using System.Text;

namespace DemoInterceptionMechanism
{
    public class DoubleCalcService : ICalcService
    {
        public virtual int Calc(int data)
        {
            int returnVal = data * 2;
            Console.WriteLine("Double {0} is {1}", data, returnVal);
            return returnVal;
        }
    }
}
