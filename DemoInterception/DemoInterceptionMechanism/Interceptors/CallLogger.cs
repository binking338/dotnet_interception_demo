using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DemoInterceptionMechanism
{
    /// <summary>
    /// 拦截器实现：调用日志记录器
    /// </summary>
    public class CallLogger : IInterceptor
    {

        public void Before(string methodName, int data)
        {
            Console.WriteLine("通过代理调用方法 {0} 开始: 参数为 {1}", methodName, data);
        }

        public void After(string methodName, int data, int returnValue)
        {
            Console.WriteLine("通过代理调用方法 {0} 完成: 结果是 {0}.", returnValue);
        }
    }
}
