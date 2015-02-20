using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.PermissionManager
{
    public enum UserValueType
    {
        // Summary:
        //     登录名
        LogonName = 1,

        //
        // Summary:
        //     人员全路径
        AllPath = 2,

        //
        // Summary:
        //     人员编号
        PersonID = 3,

        //
        // Summary:
        //     IC卡号
        ICCode = 4,

        //
        // Summary:
        //     人员Guid值
        Guid = 8,

        //
        // Summary:
        //     根据唯一索引查询(为配合南京海关统一平台切换，新增加字段ID[自增唯一字段])
        Identity = 16,
    }

}
