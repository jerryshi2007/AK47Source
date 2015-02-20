using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Web.Responsive.Library.MVC
{
	/// <summary>
	/// ��ʶ�����еķ����ǿ���������
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true)]
	public sealed class ControllerMethodAttribute : Attribute
	{
		private bool isDefault = true;

		/// <summary>
		/// ���췽��
		/// </summary>
		public ControllerMethodAttribute()
		{
		}

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="defaultMethod"></param>
		public ControllerMethodAttribute(bool defaultMethod)
		{
			this.isDefault = defaultMethod;
		}

		/// <summary>
		/// ���췽�����ö��Ż�ֺŷָ��Ĳ��������б�һ����������Ĳ�������ǿ�ƺ��Դ˷���
		/// </summary>
		/// <param name="forceIgnoreParameters"></param>
		public ControllerMethodAttribute(string forceIgnoreParameters)
		{
			this.ForceIgnoreParameters = forceIgnoreParameters;
		}

		/// <summary>
		/// �Ƿ���ȱʡ�ķ���
		/// </summary>
		public bool Default
		{
			get
			{
				return this.isDefault;
			}
			set
			{
				this.isDefault = value;
			}
		}

		/// <summary>
		/// �ö��Ż�ֺŷָ��Ĳ��������б�һ����������Ĳ�������ǿ�ƺ��Դ˷���
		/// </summary>
		public string ForceIgnoreParameters
		{
			get;
			set;
		}
	}
}
