#region
// -------------------------------------------------
// Assembly	：	HB.DataObjects
// FileName	：	OguOrganization.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    李苗	    20070628		创建
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class OguOrganization : OguBase, IOrganization, IVirtualOrganization
	{
		private string customsCode = null;
		private DepartmentTypeDefine departmentType = DepartmentTypeDefine.Unspecified;
		private DepartmentClassType departmentClass = DepartmentClassType.Unspecified;

		[NonSerialized]
		private DepartmentRankType? rank = null;

		private bool excludeVirtualDepartment = false;

		#region IOrganization 成员

		/// <summary>
		/// 海关代码
		/// </summary>
		public string CustomsCode
		{
			get
			{
				if (this.customsCode == null && Ogu != null)
					this.customsCode = BaseOrganization.CustomsCode;

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
		public DepartmentTypeDefine DepartmentType
		{
			get
			{
				if (this.departmentType == DepartmentTypeDefine.Unspecified && Ogu != null)
					this.departmentType = BaseOrganization.DepartmentType;

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
		public DepartmentClassType DepartmentClass
		{
			get
			{
				if (this.departmentClass == DepartmentClassType.Unspecified && Ogu != null)
					this.departmentClass = BaseOrganization.DepartmentClass;

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
		public DepartmentRankType Rank
		{
			get
			{
				if (this.rank == null && Ogu != null)
					this.rank = BaseOrganization.Rank;

				return (DepartmentRankType)this.rank;
			}
			set
			{
				this.rank = value;
			}
		}

		/// <summary>
		/// 机构类型
		/// </summary>
		public override SchemaType ObjectType
		{
			get
			{
				return SchemaType.Organizations;
			}
		}

		/// <summary>
		/// 判断当前部门是不是顶级部门
		/// </summary>
		[NoMapping]
		public bool IsTopOU
		{
			get
			{
				return BaseOrganization.IsTopOU;
			}
		}

		/// <summary>
		/// 得到当前机构的子部门
		/// </summary>
		[NoMapping]
		public OguObjectCollection<IOguObject> Children
		{
			get
			{
				return BaseOrganization.Children;
			}
		}

		#region 基础类型
		private IOrganization BaseOrganization
		{
			get
			{
				return (IOrganization)Ogu;
			}
		}
		#endregion 基础类型

		#endregion

		OguObjectCollection<T> IOrganization.GetAllChildren<T>(bool includeSideLine)
		{
			return BaseOrganization.GetAllChildren<T>(includeSideLine);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="matchString"></param>
		/// <param name="includeSideLine"></param>
		/// <param name="level"></param>
		/// <param name="returnCount"></param>
		/// <returns></returns>
		public OguObjectCollection<T> QueryChildren<T>(string matchString, bool includeSideLine, SearchLevel level, int returnCount) where T : IOguObject
		{
			return BaseOrganization.QueryChildren<T>(matchString, includeSideLine, level, returnCount);
		}

		public OguOrganization(string id)
			: base(id, SchemaType.Organizations)
		{

		}

		public OguOrganization(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{

		}

		public OguOrganization(IOrganization organization)
			: base(organization, SchemaType.Organizations)
		{
		}

		public OguOrganization()
			: base(SchemaType.Organizations)
		{
		}

		/// <summary>
		/// 子部门是否排除虚拟部门
		/// </summary>
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
	}
}
