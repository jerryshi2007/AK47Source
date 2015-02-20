using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.Configuration;
using MCS.Library.Accredit.AppAdmin;
using MCS.Library.Accredit.WebBase;
using MCS.Applications.AccreditAdmin.Properties;

namespace MCS.Applications.AccreditAdmin.classLib
{
	/// <summary>
	/// ������Ա����ϵͳ�е�Ȩ���������
	/// </summary>
	internal class OGUUserPermission
	{
		/// <summary>
		/// ���캯��
		/// </summary>
		public OGUUserPermission()
		{

		}

		/// <summary>
		/// ��ȡ��ǰ��¼�û��ڵ�ǰ��������Ա����ϵͳ���е�ȫ��Ȩ��
		/// </summary>
		/// <returns></returns>
		public static string GetOGUPemission()
		{
			string strResult = "setNoPermission";
			bool IsCustomsAuthentication = AccreditSection.GetConfig().AccreditSettings.CustomsAuthentication;
			if (IsCustomsAuthentication)
			{
				DataSet ds = SecurityCheck.GetUserPermissions(GlobalInfo.UserLogOnInfo.UserLogOnName, 
					AccreditResource.AppCodeName,
					UserValueType.LogonName, 
					RightMaskType.App, 
					DelegationMaskType.All);

				strResult = string.Empty;

				foreach (DataRow row in ds.Tables[0].Rows)
				{
					if (strResult.Length > 0)
						strResult += ",";

					strResult += OGUCommonDefine.DBValueToString(row["CODE_NAME"]);
				}
			}
			return strResult;
		}
	}
}
