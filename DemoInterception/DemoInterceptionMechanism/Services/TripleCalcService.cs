using System;
using System.Collections.Generic;
using System.Text;

namespace DemoInterceptionMechanism
{
    public class TripleCalcService : ICalcService
    {
        public int Calc(int data)
        {
            int returnVal = data * 3;
            Console.WriteLine("Triple {0} is {1}", data, returnVal);
            return returnVal;
        }
    }
}
