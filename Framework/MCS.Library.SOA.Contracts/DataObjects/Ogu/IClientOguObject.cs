using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects
{
    /// <summary>
    /// 客户端用户和机构对象的基类
    /// </summary>
    
   
    public interface IClientOguObject
    {
        // Summary:
        //     描述信息
        string Description { get; set; }

        //
        // Summary:
        //     对象的显示名称
        string DisplayName { get; set; }
        
        //
        // Summary:
        //     对象在系统中的全部路径
        string FullPath { get; set; }
        
        //
        // Summary:
        //     在整个机构中的排序号
        string GlobalSortID { get; set; }
        
        //
        // Summary:
        //     对象的ID
        string ID { get; set; }
        
        //
        // Summary:
        //     对象在组织机构树上的层次
        int Levels { get; set; }
        
        //
        // Summary:
        //     对象的名称
        string Name { get; set; }
        
        //
        // Summary:
        //     对象的类型
        ClientObjectSchemaType ObjectType { get; set; }
    }
}
