using System;
using System.Collections.Generic;
using System.Text;

namespace DemoAspNetCore.Interfaces
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILogger
    {
        void Log(string content);
    }
}
