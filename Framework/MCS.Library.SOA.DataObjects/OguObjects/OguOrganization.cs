#region
// -------------------------------------------------
// Assembly	��	HB.DataObjects
// FileName	��	OguOrganization.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070628		����
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

		#region IOrganization ��Ա

		/// <summary>
		/// ���ش���
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
		/// ���ŵ�һЩ�������ԣ�1���������2һ�㲿�š�4�칫�ң�������8�ۺϴ���
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
		/// ���ŵķ��࣬��:32�������ء�64��פ����...
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
		/// ���ŵļ���
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
		/// ��������
		/// </summary>
		public override SchemaType ObjectType
		{
			get
			{
				return SchemaType.Organizations;
			}
		}

		/// <summary>
		/// �жϵ�ǰ�����ǲ��Ƕ�������
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
		/// �õ���ǰ�������Ӳ���
		/// </summary>
		[NoMapping]
		public OguObjectCollection<IOguObject> Children
		{
			get
			{
				return BaseOrganization.Children;
			}
		}

		#region ��������
		private IOrganization BaseOrganization
		{
			get
			{
				return (IOrganization)Ogu;
			}
		}
		#endregion ��������

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
		/// �Ӳ����Ƿ��ų����ⲿ��
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
