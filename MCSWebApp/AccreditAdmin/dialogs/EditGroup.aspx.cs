#region using

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.Configuration;
using MCS.Library.Accredit.AppAdmin;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Applications.AccreditAdmin.Properties;
using MCS.Applications.AccreditAdmin.classLib;

#endregion

namespace MCS.Applications.AccreditAdmin.dialogs
{
	/// <summary>
	/// EditGroup ��ժҪ˵����
	/// </summary>
	public partial class EditGroup : WebUserBaseClass
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetNoStore();

			string strParentGuid = (string)GetRequestData("parentGuid", string.Empty);
			ExceptionHelper.TrueThrow(strParentGuid == string.Empty, "�Բ���ϵͳ��������ȱ�١�parentGuid����");
			string strOPType = (string)GetRequestData("opType", string.Empty);
			ExceptionHelper.TrueThrow(strOPType == string.Empty, "�Բ���ϵͳ��������ȱ�١�opType����");

			if (false == IsPostBack)
			{
				using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
				{
					switch (strOPType)
					{
						case "Update":
							string strObjGuid = (string)GetRequestData("objGuid", string.Empty);
							ExceptionHelper.TrueThrow(strObjGuid == string.Empty, "�Բ���ϵͳ��������ȱ�١�objGuid����");
							DataSet ds = OGUReader.GetObjectsDetail("GROUPS", strObjGuid,
								SearchObjectColumn.SEARCH_GUID, strParentGuid, SearchObjectColumn.SEARCH_GUID);
							ExceptionHelper.TrueThrow((ds == null) || (ds.Tables.Count == 0) || (ds.Tables[0].Rows.Count == 0),
								"�Բ���ϵͳ��û���ҵ�ָ���Ķ���");

							GroupData.Value = InnerCommon.GetXmlDoc(ds).OuterXml;
							string strAllPathName = OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]);
							if (strAllPathName.LastIndexOf("\\") >= 0)
								parentAllPathName.Value = strAllPathName.Substring(0, strAllPathName.LastIndexOf("\\"));
							break;
						case "Insert":
							string strSql = "SELECT ALL_PATH_NAME FROM ORGANIZATIONS WHERE GUID = " +
								TSqlBuilder.Instance.CheckQuotationMark(strParentGuid, true);
							parentAllPathName.Value = InnerCommon.ExecuteScalar(strSql).ToString();
							break;
						default: ExceptionHelper.TrueThrow(true, "�Բ���ϵͳ�������ݡ�opType������ȷ��");
							break;
					}
				}
				CheckPermission(strOPType);
			}
		}

		private void CheckPermission(string strOPType)
		{
			bool bPermission = true;

			bool isCustomsAuthentication = AccreditSection.GetConfig().AccreditSettings.CustomsAuthentication;
			if (isCustomsAuthentication)
			{
				switch (strOPType)
				{
					case "Update":
						bPermission = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName,
							AccreditResource.AppCodeName,
							AccreditResource.Func_ModifyGroup,
							UserValueType.LogonName,
							DelegationMaskType.All);
						if (bPermission)
						{
							DataSet ds = SecurityCheck.GetUserFunctionsScopes(LogOnUserInfo.UserLogOnName,
								AccreditResource.AppCodeName,
								AccreditResource.Func_ModifyGroup,
								UserValueType.LogonName,
								DelegationMaskType.All,
								ScopeMaskType.All);
							string strObjGuid = (string)GetRequestData("objGuid", string.Empty);
							bPermission = IsObjectIsIncludeInObjects("GROUPS", strObjGuid, SearchObjectColumn.SEARCH_GUID, ds);
						}
						break;
					case "Insert":
						bPermission = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName,
							AccreditResource.AppCodeName,
							AccreditResource.Func_CreateGroup,
							UserValueType.LogonName,
							DelegationMaskType.All);
						ExceptionHelper.FalseThrow(bPermission, "�Բ�����û��Ȩ�޴����µ���Ա�飡");
						if (bPermission)
						{
							DataSet ds = SecurityCheck.GetUserFunctionsScopes(LogOnUserInfo.UserLogOnName,
								AccreditResource.AppCodeName,
								AccreditResource.Func_CreateGroup,
								UserValueType.LogonName,
								DelegationMaskType.All,
								ScopeMaskType.All);
							ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS",
								parentAllPathName.Value,
								SearchObjectColumn.SEARCH_ALL_PATH_NAME,
								ds), "�Բ�����û���ڵ�ǰ�����д�������Ա�顱��Ȩ�ޣ�");
						}
						break;
				}
			}

			opPermission.Value = bPermission.ToString().ToLower();
		}

		private bool IsObjectIsIncludeInObjects(string strObjType, string strObjValue, SearchObjectColumn soc, DataSet objsDs)
		{
			DataSet oDs = OGUReader.GetObjectsDetail(strObjType, strObjValue, soc, string.Empty, SearchObjectColumn.SEARCH_NULL);
			foreach (DataRow oRow in oDs.Tables[0].Rows)
			{
				string strObjAllPathName = OGUCommonDefine.DBValueToString(oRow["ALL_PATH_NAME"]);
				foreach (DataRow sRow in objsDs.Tables[0].Rows)
				{
					string[] strArry = OGUCommonDefine.DBValueToString(sRow["DESCRIPTION"]).Split(',', ';', ' ');
					foreach (string strValue in strArry)
					{
						if (strValue.Length <= strObjAllPathName.Length
							&& strValue.Length >= 0
							&& strObjAllPathName.Substring(0, strValue.Length) == strValue)
							return true;
					}
				}
			}

			return false;
		}


		#region Web ������������ɵĴ���
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

	}
}
