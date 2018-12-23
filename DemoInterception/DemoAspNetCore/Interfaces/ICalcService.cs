using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoAspNetCore.Interfaces
{
    /// <summary>
    /// 计算服务接口
    /// </summary>
    public interface ICalcService
    {
        int Calc(int data);
    }
}
