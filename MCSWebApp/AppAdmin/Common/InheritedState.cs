using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace MCS.Applications.AppAdmin.Common
{
	/// <summary>
	/// Ӧ�õļ����̳ж��������ֵ
	/// </summary>
	[Flags]
	public enum InheritedState
	{
		/// <summary>
		///û�м̳� 
		/// </summary>
		NONE = 0,
		/// <summary>
		/// ����Χ�����룬1
		/// </summary>
		SCOPES = 1,
		/// <summary>
		/// ��ɫ�����룬2
		/// </summary>
		ROLES = 2,
		/// <summary>
		/// ���ܵ����룬4
		/// </summary>
		FUNCTIONS = 4,
		/// <summary>
		/// ��ɫ���ܹ�ϵ�����룬8
		/// </summary>
		ROLE_TO_FUNCTIONS = 8,
		/// <summary>
		/// ����Ȩ��������룬16
		/// </summary>
		OBJECT = 16
	}
}
