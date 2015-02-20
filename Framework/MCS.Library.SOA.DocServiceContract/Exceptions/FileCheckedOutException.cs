using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCS.Library.SOA.DocServiceContract.Exceptions
{
    /// <summary>
    /// 文件无法编辑，因为被签出
    /// </summary>
    public class FileCheckedOutException:Exception
    {
    }
}