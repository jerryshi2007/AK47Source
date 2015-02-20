using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
namespace MCS.Library.SOA.Contracts.DataObjects
{
  
    public interface IClientUser : IClientOguObject
    {
        //
        // Summary:
        //     用户的邮件地址
        string Email { get; }

        //
        // Summary:
        //     是否是兼职的用户信息
        bool IsSideline { get; }
        
        //
        // Summary:
        //     登录名称
        string LogOnName { get; }

        //
        // Summary:
        //     用户的职位
        string Occupation { get; }

        //
        // Summary:
        //     人员的级别
        ClientUserRankType Rank { get; }
    }
}
