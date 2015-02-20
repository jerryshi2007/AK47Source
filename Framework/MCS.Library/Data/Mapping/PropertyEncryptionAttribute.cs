using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// 需要加密的属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class PropertyEncryptionAttribute : Attribute
	{
		private string _EncryptorName = null;

		/// <summary>
		/// 构造方法
		/// </summary>
		public PropertyEncryptionAttribute()
		{
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="encName"></param>
		public PropertyEncryptionAttribute(string encName)
		{
			this._EncryptorName = encName;
		}

		/// <summary>
		/// 加密器的名称，如果没有则是用默认的
		/// </summary>
		public string EncryptorName
		{
			get { return this._EncryptorName; }
			set { this._EncryptorName = value; }
		}
	}
}
