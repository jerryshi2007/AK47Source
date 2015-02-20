using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;

namespace MCS.Library.Accredit.SoapControl
{
	/// <summary>
	/// 实现SoapLog的标签定义
	/// </summary>
	public class SoapTraceExtensionAttribute : SoapExtensionAttribute
	{
		/// <summary>
		/// Soap Extension 类型
		/// </summary>
		public override Type ExtensionType
		{
			get { return typeof(SoapTraceExtension); }
		}

		private int priority;
		/// <summary>
		/// 处理优先级
		/// </summary>
		public override int Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				this.priority = value;
			}
		}
		//private string filename = "c:\\log.txt";
		///// <summary>
		///// 文件名称
		///// </summary>
		//public string Filename
		//{
		//    get
		//    {
		//        return filename;
		//    }
		//    set
		//    {
		//        filename = value;
		//    }
		//}
	}
}
