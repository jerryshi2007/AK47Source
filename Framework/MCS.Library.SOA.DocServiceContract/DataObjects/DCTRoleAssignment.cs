using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 角色与授权实体的关系
    /// </summary>
    [DataContract]
	[Serializable]
    public class DCTRoleAssignment
    {
        BaseCollection<DCTRoleDefinition> roleDefinitions;
        /// <summary>
        /// 角色定义集合
        /// </summary>
        [DataMember]
        public BaseCollection<DCTRoleDefinition> RoleDefinitions
        {
            get { return roleDefinitions; }
            set { roleDefinitions = value; }
        }

        DCTPrincipal member;
        /// <summary>
        /// 授权实体
        /// </summary>
        [DataMember]
        public DCTPrincipal Member
        {
            get { return member; }
            set { member = value; }
        }
    }
}