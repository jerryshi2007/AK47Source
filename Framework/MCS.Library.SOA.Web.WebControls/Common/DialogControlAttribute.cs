using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// �Ի���ؼ�ģ�����������
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class DialogTemplateAttribute : Attribute
	{
		private string resourcePath = string.Empty;
		private string assemblyName = string.Empty;

		/// <summary>
		/// ���췽��
		/// </summary>
		public DialogTemplateAttribute()
		{
		}

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="fullResourceName"></param>
		public DialogTemplateAttribute(string fullResourceName)
		{
			this.resourcePath = fullResourceName;
		}

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="fullResourceName"></param>
		/// <param name="fullAssemblyName"></param>
		public DialogTemplateAttribute(string fullResourceName, string fullAssemblyName)
		{
			this.resourcePath = fullResourceName;
			this.assemblyName = fullAssemblyName;
		}

		/// <summary>
		/// ��Դ��·��
		/// </summary>
		public string ResourcePath
		{
			get
			{
				return this.resourcePath;
			}
			set
			{
				this.resourcePath = value;
			}
		}

		/// <summary>
		/// Assembly������
		/// </summary>
		public string AssemblyName
		{
			get { return this.assemblyName; }
			set { this.assemblyName = value; }
		}
	}

	/// <summary>
	/// �Ի���ؼ����ݵ���������
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class DialogContentAttribute : Attribute
	{
		private string resourcePath = string.Empty;
		private string assemblyName = string.Empty;
		private bool enableViewState = false;

		/// <summary>
		/// ���췽��
		/// </summary>
		public DialogContentAttribute()
		{
		}

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="fullResourceName"></param>
		public DialogContentAttribute(string fullResourceName)
		{
			this.resourcePath = fullResourceName;
		}
		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="fullResourceName"></param>
		/// <param name="enableVs"></param>
		public DialogContentAttribute(string fullResourceName, bool enableVs)
		{
			this.resourcePath = fullResourceName;
			this.enableViewState = enableVs;
		}

		/// <summary>
		/// ���췽��
		/// </summary>
		/// <param name="fullResourceName"></param>
		/// <param name="fullAssemblyName"></param>
		public DialogContentAttribute(string fullResourceName, string fullAssemblyName)
		{
			this.resourcePath = fullResourceName;
			this.assemblyName = fullAssemblyName;
		}

		public DialogContentAttribute(string fullResourceName, string fullAssemblyName, bool enableVs)
		{
			this.resourcePath = fullResourceName;
			this.assemblyName = fullAssemblyName;
			this.enableViewState = enableVs;
		}

		/// <summary>
		/// ��Դ��·��
		/// </summary>
		public string ResourcePath
		{
			get
			{
				return this.resourcePath;
			}
			set
			{
				this.resourcePath = value;
			}
		}

		/// <summary>
		/// Assembly������
		/// </summary>
		public string AssemblyName
		{
			get { return this.assemblyName; }
			set { this.assemblyName = value; }
		}

		/// <summary>
		/// Ĭ���Ƿ�����ViewState
		/// </summary>
		public bool EnableViewState
		{
			get { return this.enableViewState; }
			set { this.enableViewState = value; }
		}
	}
}
