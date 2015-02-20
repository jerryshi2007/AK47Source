using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示管理范围物的基类
	/// </summary>
	[Serializable]
	public class AUAdminScopeItem : SchemaObjectBase
	{
		protected AUAdminScopeItem()
		{
		}

		protected AUAdminScopeItem(string schemaType)
			: base(schemaType)
		{
		}

		/// <summary>
		/// 管理范围的名称（必选属性）
		/// </summary>
		[NoMapping]
		public string AUScopeItemName
		{
			get
			{
				return this.Properties.GetValue("AUScopeItemName", string.Empty);
			}
			set
			{
				this.Properties.SetValue("AUScopeItemName", value);
			}
		}
	}
}
