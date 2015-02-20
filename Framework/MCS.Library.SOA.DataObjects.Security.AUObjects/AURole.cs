using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示一个管理单元角色
	/// </summary>
	public class AURole : SchemaObjectBase
	{
		public AURole()
			: base(AUCommon.SchemaAdminUnitRole)
		{
		}

		public AURole(string schemaType)
			: base(schemaType)
		{
		}

		/// <summary>
		/// 引用的管理架构角色的ID
		/// </summary>
		[NoMapping]
		public string SchemaRoleID
		{
			get { return (string)this.Properties.GetValue("SchemaRoleID", string.Empty); }
			set { this.Properties.SetValue("SchemaRoleID", value); }
		}
	}
}
