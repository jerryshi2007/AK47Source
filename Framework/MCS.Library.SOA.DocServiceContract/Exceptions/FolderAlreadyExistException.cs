using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCS.Library.SOA.DocServiceContract.Exceptions
{
    /// <summary>
    /// 文件夹已存在
    /// </summary>
    public class FolderAlreadyExistException:Exception
    {
        public FolderAlreadyExistException(string message)
            :base(message)
        { }
    }
}