using System;
using System.Collections.Generic;
using System.Text;

namespace DemoInterceptionMechanism.CodeByHand
{
    /// <summary>
    /// Demo for interface interceptor
    /// </summary>
    public class TripleCalcServiceProxy : ICalcService
    {
        private CallLogger interceptor;
        private TripleCalcService target;

        public TripleCalcServiceProxy()
        {
            interceptor = new CallLogger();
            target = new TripleCalcService();
        }

        public int Calc(int data)
        {
            int returnVal;

            interceptor.Before("Calc", data);

            returnVal = target.Calc(data);

            interceptor.After("Calc", data, returnVal);

            return returnVal;
        }
    }
}
