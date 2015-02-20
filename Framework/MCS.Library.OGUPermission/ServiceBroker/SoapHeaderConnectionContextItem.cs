using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// Soap Header中的上下文字典项
	/// </summary>
	public class SoapHeaderContextItem
	{
		/// <summary>
		/// 
		/// </summary>
		public SoapHeaderContextItem()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public SoapHeaderContextItem(string key, object value)
		{
			this.Key = key;
			this.Value = value;
		}

		/// <summary>
		/// 上下文的Key
		/// </summary>
		public string Key
		{
			get;
			set;
		}

		/// <summary>
		/// 上下文的值
		/// </summary>
		public object Value
		{
			get;
			set;
		}
	}
}
