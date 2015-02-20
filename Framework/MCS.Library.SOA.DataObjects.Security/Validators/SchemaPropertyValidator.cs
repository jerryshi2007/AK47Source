using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects.Security.Validators
{
	/// <summary>
	/// 表示模式属性的值访问
	/// </summary>
	public class SchemaPropertyValueAccess : ValueAccess
	{
		private SchemaPropertyValue _PropertyValue = null;

		/// <summary>
		/// 使用指定的<see cref="SchemaPropertyValue"/>实例 初始化<see cref="SchemaPropertyValueAccess"/>的新实例。
		/// </summary>
		/// <param name="pv">一个<see cref="SchemaPropertyValue"/>对象</param>
		public SchemaPropertyValueAccess(SchemaPropertyValue pv)
		{
			pv.NullCheck("pv");

			this._PropertyValue = pv;
		}

		/// <summary>
		/// 获取对象的值
		/// </summary>
		/// <param name="target"><see cref="T:SchemaObjectBase"/>的派生类型的实例。</param>
		/// <returns>真实的值</returns>
		public override object GetValue(object target)
		{
			object result = null;

			if (target != null)
			{
				(target is SchemaObjectBase).FalseThrow("{0}不是SchemaObjectBase类型，不能进行属性值的获取", target.GetType());
				result = this._PropertyValue.GetRealValue();
			}

			return result;
		}
	}
}
