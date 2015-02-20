using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCS.Library.SOA.DocServiceContract.Exceptions
{
    /// <summary>
    /// 角色已经存在
    /// </summary>
    public class RoleAlreadyExistException:Exception
    {
        public RoleAlreadyExistException(string message)
            : base(message)
        {
        }
        
    }
}