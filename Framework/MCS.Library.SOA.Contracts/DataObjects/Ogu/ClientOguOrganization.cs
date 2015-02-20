using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects
{
    [DataContract(IsReference = true)]
    [KnownType(typeof(ClientOguOrganization))]
    public class ClientOguOrganization : ClientOguObjectBase, IClientOrganization
    {
        private string customsCode = null;
        private ClientDepartmentTypeDefine departmentType = ClientDepartmentTypeDefine.Unspecified;
        private ClientDepartmentClassType departmentClass = ClientDepartmentClassType.Unspecified;
        private ClientDepartmentRankType rank ;
        private ClientObjectSchemaType schemaType = ClientObjectSchemaType.Organizations;
		private bool excludeVirtualDepartment = false;
        //private ClientOguOrganization _Parent = null;
        //private ClientOguOrganization _topOU = null;
        private ClientOguObjectCollection<ClientOguOrganization> _Children = null;

        #region IClientOrganization 成员


        /// <summary>
		/// 海关代码
		/// </summary>
        [DataMember]
		public string CustomsCode
		{
			get
			{
				return this.customsCode;
			}
			set
			{
				this.customsCode = value;
			}
		}

		/// <summary>
		/// 部门的一些特殊属性（1虚拟机构、2一般部门、4办公室（厅）、8综合处）
		/// </summary>
        [DataMember]
		public ClientDepartmentTypeDefine DepartmentType
		{
			get
			{

				return this.departmentType;
			}
			set
			{
				this.departmentType = value;
			}
		}

		/// <summary>
		/// 部门的分类，如:32隶属海关、64派驻机构...
		/// </summary>
        [DataMember]
		public ClientDepartmentClassType DepartmentClass
		{
			get
			{
				return this.departmentClass;
			}
			set
			{
				this.departmentClass = value;
			}
		}

		/// <summary>
		/// 部门的级别
		/// </summary>
        [DataMember]
		public ClientDepartmentRankType Rank
		{
			get
			{
				return this.rank;
			}
			set
			{
				this.rank = value;
			}
		}

		/// <summary>
		/// 机构类型
		/// </summary>
        [DataMember]
        public new ClientObjectSchemaType ObjectType
		{
			get
			{
                return this.schemaType;
			}
            set
            {
                this.schemaType = value;
            }
		}

		/// <summary>
		/// 判断当前部门是不是顶级部门
		/// </summary>
        [DataMember]
        public bool IsTopOU
        {
            get;
            set;
        }

		/// <summary>
		/// 得到当前机构的子部门
		/// </summary>
        [DataMember]
        public ClientOguObjectCollection<ClientOguOrganization> Children
		{
			get
			{
                if (_Children == null)
                    _Children = new ClientOguObjectCollection<ClientOguOrganization>();
				return _Children;
			}
            set
            {
                _Children = value;
            }
		}


		#endregion

		/// <summary>
		/// 子部门是否排除虚拟部门
		/// </summary>
        [DataMember]
		public bool ExcludeVirtualDepartment
		{
			get
			{
				return this.excludeVirtualDepartment;
			}
			set
			{
				this.excludeVirtualDepartment = value;
			}
		}
        [DataMember]
        public string SortID { get; set; }
    }
}
