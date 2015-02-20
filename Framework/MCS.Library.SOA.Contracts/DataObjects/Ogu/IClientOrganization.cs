using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
namespace MCS.Library.SOA.Contracts.DataObjects
{
     
     
    public interface IClientOrganization : IClientOguObject
    {
        // Summary:
        //     该部门的下一级子对象
        ClientOguObjectCollection<ClientOguOrganization> Children { get; set; }
        //
        // Summary:
        //     关区代码
        string CustomsCode { get; set; }
        //
        // Summary:
        //     部门的类别
        ClientDepartmentClassType DepartmentClass { get; set; }
        //
        // Summary:
        //     部门的类型
        ClientDepartmentTypeDefine DepartmentType { get; set; }
        //
        // Summary:
        //     该部门是否是顶级部门
        bool IsTopOU { get; set; }
        //
        // Summary:
        //     部门的级别
        ClientDepartmentRankType Rank { get; set; }
    }
}
