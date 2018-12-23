using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DemoNetCore.Interceptors
{
    /// <summary>
    ///  Step 1：
    ///  创建拦截器：调用日志记录器
    /// </summary>
    public class CallLogger : IInterceptor
    {
        Interfaces.ILogger logger;

        public CallLogger(Interfaces.ILogger logger)
        {
            this.logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            logger.Log($"通过代理调用方法 {invocation.Method.Name} 开始: 参数为 {string.Join(", ", invocation.Arguments.Select(arg => arg?.ToString()))}");
            invocation.Proceed();
            logger.Log($"通过代理调用方法 {invocation.Method.Name} 完成: 结果是 {invocation.ReturnValue}.");
        }
    }
}
