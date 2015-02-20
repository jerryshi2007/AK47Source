using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// XElement序列化的属性，作用在Field上
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public class XElementFieldSerializeAttribute : Attribute
	{
		private bool _IgnoreDeserializeError = false;
		private string _AlternateFieldName = null;

		/// <summary>
		/// 序列化时用的属性名称
		/// </summary>
		public string AlternateFieldName
		{
			get { return this._AlternateFieldName; }
			set { this._AlternateFieldName = value; }
		}

		/// <summary>
		/// 反序列化时，是否忽略错误（不停止反序列化）
		/// </summary>
		public bool IgnoreDeserializeError
		{
			get { return this._IgnoreDeserializeError; }
			set { this._IgnoreDeserializeError = value; }
		}
	}
}
