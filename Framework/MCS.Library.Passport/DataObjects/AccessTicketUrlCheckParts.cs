using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 票据检查Url时
	/// </summary>
	[Flags]
	public enum AccessTicketUrlCheckParts
	{
		/// <summary>
		/// 空
		/// </summary>
		None = 0,

		/// <summary>
		/// 协议、Host和端口
		/// </summary>
		SchemeHostAndPort = 1,

		/// <summary>
		/// 路径和参数
		/// </summary>
		PathAndParameters = 2,

		/// <summary>
		/// 全部
		/// </summary>
		All = SchemeHostAndPort | PathAndParameters
	}
}
