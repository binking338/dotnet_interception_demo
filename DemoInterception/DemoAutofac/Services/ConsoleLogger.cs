using System;
using System.Collections.Generic;
using System.Text;

namespace DemoAutofac.Services
{
    /// <summary>
    /// 控制台日志实现
    /// </summary>
    public class ConsoleLogger : Interfaces.ILogger
    {
        public void Log(string content)
        {
            Console.WriteLine(content);
        }
    }
}
