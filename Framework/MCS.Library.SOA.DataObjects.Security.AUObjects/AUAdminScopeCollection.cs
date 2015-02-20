using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示某一管理单元的管理范围的集合（不可包含重复的管理范围类别）
	/// </summary>
	public class AUAdminScopeCollection : SchemaObjectEditableKeyedCollectionBase<AUAdminScope, AUAdminScopeCollection>
	{
		public AUAdminScopeCollection()
		{
		}

		public AUAdminScopeCollection(IEnumerable<SchemaObjectBase> src)
		{
			foreach (AUAdminScope scope in src)
			{
				this.Add(scope);
			}
		}

		protected override AUAdminScopeCollection CreateFilterResultCollection()
		{
			return new AUAdminScopeCollection();
		}

		protected override string GetKeyForItem(AUAdminScope item)
		{
			return item.ID;
		}

		protected override void OnInsert(int index, object value)
		{
			AUAdminScope scope = (AUAdminScope)value;
			if (GetScope(scope.ScopeSchemaType) != null)
				throw new InvalidOperationException("插入管理范围类别时包含重复范围范围类别。");

			base.OnInsert(index, value);
		}

		/// <summary>
		/// 获取指定管理范围类别的<see cref="AUAdminScope"/>
		/// </summary>
		/// <param name="scopeSchemaType"></param>
		/// <returns>返回为<see langword="null"/>或<see cref="AUAdminScope"/>。</returns>
		public AUAdminScope GetScope(string scopeSchemaType)
		{
			AUAdminScope result = null;
			foreach (AUAdminScope scope in this)
			{
				if (scope.ScopeSchemaType == scopeSchemaType)
				{
					result = scope;
					break;
				}
			}

			return result;
		}
	}
}
