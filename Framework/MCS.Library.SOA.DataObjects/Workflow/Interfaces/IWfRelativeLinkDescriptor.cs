using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 相关链接的接口定义
	/// </summary>
	public interface IWfRelativeLinkDescriptor : IWfKeyedDescriptor
	{
		/// <summary>
		/// 相关链接的Url
		/// </summary>
		string Url
		{
			get;
		}

		/// <summary>
		/// 链接的类别
		/// </summary>
		string Category
		{
			get;
		}
	}
}
