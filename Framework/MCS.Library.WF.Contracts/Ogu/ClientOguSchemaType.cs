using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Ogu
{
    [Flags]
    public enum ClientOguSchemaType
    {
        // Summary:
        //     未指定
        Unspecified = 0,
        //
        // Summary:
        //     组织机构
        Organizations = 1,
        //
        // Summary:
        //     用户
        Users = 2,
        //
        // Summary:
        //     组
        Groups = 4,

        //
        // Summary:
        //     所有条件
        All = 65535,
    }
}
