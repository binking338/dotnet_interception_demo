using System;
using System.Collections.Generic;
using System.Text;

namespace DemoInterceptionMechanism.CodeByHand
{
    /// <summary>
    /// Demo for class interceptor
    /// </summary>
    public class DoubleCalcServiceProxy : DoubleCalcService
    {
        private CallLogger interceptor;

        public DoubleCalcServiceProxy()
            : base()
        {
            interceptor = new CallLogger();
        }

        public override int Calc(int data)
        {
            int returnVal;

            interceptor.Before("Calc", data);

            returnVal  = base.Calc(data);

            interceptor.After("Calc", data, returnVal);

            return returnVal;
        }
    }
}
