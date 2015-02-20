using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;

namespace MCS.Library.SOA.DocServiceContract.Exceptions
{
    /// <summary>
    /// 错误处理器
    /// </summary>
    public class ErrorProcessor
    {
        public static void Process(Exception ex, int errorCode = 0)
        {
            switch (errorCode)
            {
                case -2130575293: throw new RoleAlreadyExistException("角色已存在.");
                case -2147024809: throw new TargetNotFoundException();
                case -2146232832: throw new TargetNotFoundException("角色未找到.");
                default: throw ex;
            }
        }
    }
}
