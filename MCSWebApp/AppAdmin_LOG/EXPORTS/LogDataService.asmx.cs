using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;

using MCS.Library.Accredit.LogAdmin;

namespace MCS.Applications.AppAdmin_LOG.exports
{
	/// <summary>
	/// LogDataService ��ժҪ˵����
	/// </summary>
	public class LogDataService : System.Web.Services.WebService
	{
		public LogDataService()
		{
			InitializeComponent();
		}

		#region �����������ɵĴ���
		
		private IContainer components = null;
				
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// ������������ʹ�õ���Դ��
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		[WebMethod]
		public void InsertUserLog(string userLogonName, string appName, string hostIP, string hostName, string url, string goalID, string goalName, string goalDisplayName, string opType, string explain, string originalData, bool bOpSucceed)
		{
			UserDataWrite.InsertUserLog(userLogonName, appName, hostIP, hostName, url, goalID, goalName, goalDisplayName, opType, explain, originalData, bOpSucceed);
		}

		public void InsertSysLog(string userLogonName, string winVer, string ieVer, string hostIP, string hostName, string killInfo, string status, string description)
		{
			SysDataWrite.InsertSysLog(userLogonName, winVer, ieVer, hostIP, hostName, killInfo, status, description);
		}

	}
}
