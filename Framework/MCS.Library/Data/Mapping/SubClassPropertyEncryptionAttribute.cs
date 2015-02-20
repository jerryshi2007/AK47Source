using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// 属性为子对象时的加密属性定义
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class SubClassPropertyEncryptionAttribute : PropertyEncryptionAttribute
	{
		private string subPropertyName = string.Empty;

		/// <summary>
        /// 构造方法
        /// </summary>
		protected SubClassPropertyEncryptionAttribute()
        {
        }

		/// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="subPropertyName">子对象的属性名称</param>
		/// <param name="encName">加密器的名称</param>
		public SubClassPropertyEncryptionAttribute(string subPropertyName, string encName)
            : base(encName)
        {
            this.subPropertyName = subPropertyName;
        }

		/// <summary>
		/// 源对象的属性名称
		/// </summary>
		public string SubPropertyName
		{
			get
			{
				return this.subPropertyName;
			}
			set
			{
				this.subPropertyName = value;
			}
		}
	}
}
