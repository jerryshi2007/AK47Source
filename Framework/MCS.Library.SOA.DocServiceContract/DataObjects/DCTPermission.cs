using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 权限定义
    /// </summary>
    public enum DCTPermission
    {
        NoPermissions=0,
        ViewFileOrFolder = 1,
        AddFileOrFolder=2,
        UpdateFileOrFolder=3,
        DeleteFileOrFolder=4,
    }
}