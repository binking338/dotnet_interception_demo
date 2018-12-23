using System;
using System.Collections.Generic;
using System.Text;

namespace DemoInterceptionMechanism
{
    /// <summary>
    /// 拦截器接口
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// 方法调用前执行
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="data"></param>
        void Before(string methodName, int data);

        /// <summary>
        /// 方法调用后执行
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="data"></param>
        /// <param name="returnValue"></param>
        void After(string methodName, int data, int returnValue);
    }
}
