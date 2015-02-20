using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;

namespace MCS.Library.Accredit.SoapControl
{
	/// <summary>
	/// ʵ��SoapLog�ı�ǩ����
	/// </summary>
	public class SoapTraceExtensionAttribute : SoapExtensionAttribute
	{
		/// <summary>
		/// Soap Extension ����
		/// </summary>
		public override Type ExtensionType
		{
			get { return typeof(SoapTraceExtension); }
		}

		private int priority;
		/// <summary>
		/// �������ȼ�
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
		///// �ļ�����
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
