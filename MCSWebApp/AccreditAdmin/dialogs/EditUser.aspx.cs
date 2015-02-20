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

using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.Configuration;
using MCS.Library.Accredit.AppAdmin;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Applications.AccreditAdmin.classLib;
using MCS.Applications.AccreditAdmin.Properties;
#endregion

namespace MCS.Applications.AccreditAdmin.dialogs
{
	/// <summary>
	/// EditUser ��ժҪ˵����
	/// </summary>
	public partial class EditUser : WebUserBaseClass
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
					InitPageObject();
					switch (strOPType)
					{
						case "Update": UpdateObjects(strParentGuid);
							break;
						case "Insert": InsertObjects(strParentGuid);
							break;
						case "AddSideline": AddSidelineObjects(strParentGuid);
							break;
						default:
							ExceptionHelper.TrueThrow(true, "�Բ���ϵͳ�������ݡ�opType������ȷ��");
							break;
					}
					CheckPermission(strOPType);
				}
			}
		}

		private void AddSidelineObjects(string strOrgGuid)
		{
			string strObjGuid = (string)GetRequestData("objGuid", string.Empty);
			DataSet ds, orgDs;
			DataRow row;
			ds = OGUReader.GetObjectsDetail("USERS",
				strObjGuid,
				SearchObjectColumn.SEARCH_USER_GUID,
				strOrgGuid,
				SearchObjectColumn.SEARCH_GUID);
			ExceptionHelper.TrueThrow((ds == null) || (ds.Tables.Count == 0) || (ds.Tables[0].Rows.Count == 0),
				"�Բ���ϵͳ��û���ҵ�ָ���Ķ���");
			row = ds.Tables[0].Rows[0];

			string strSParentGuid = (string)GetRequestData("SParentGuid", string.Empty);
			orgDs = OGUReader.GetObjectsDetail("ORGANIZATIONS",
				strSParentGuid,
				SearchObjectColumn.SEARCH_GUID,
				string.Empty,
				SearchObjectColumn.SEARCH_NULL);

			parentAllPathName.Value = OGUCommonDefine.DBValueToString(orgDs.Tables[0].Rows[0]["ALL_PATH_NAME"]);

			row["ALL_PATH_NAME"] = parentAllPathName.Value + "\\" + OGUCommonDefine.DBValueToString(row["OBJ_NAME"]);
			row["SIDELINE"] = 1;
			row["CREATE_TIME"] = row["END_TIME"] = row["START_TIME"] = row["RANK_NAME"] = DBNull.Value;

			userData.Value = InnerCommon.GetXmlDoc(ds).OuterXml;
		}

		private void UpdateObjects(string strOrgGuid)
		{
			string strObjGuid = (string)GetRequestData("objGuid", string.Empty);

			DataSet ds = OGUReader.GetObjectsDetail("USERS",
				strObjGuid,
				SearchObjectColumn.SEARCH_USER_GUID,
				strOrgGuid,
				SearchObjectColumn.SEARCH_GUID);

			ExceptionHelper.TrueThrow((ds == null) || (ds.Tables.Count == 0) || (ds.Tables[0].Rows.Count == 0),
				"�Բ���ϵͳ��û���ҵ�ָ���Ķ���");

			userData.Value = InnerCommon.GetXmlDoc(ds).OuterXml;
			string strAllPathName = OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]);

			if (strAllPathName.LastIndexOf("\\") >= 0)
				parentAllPathName.Value = strAllPathName.Substring(0, strAllPathName.LastIndexOf("\\"));
		}

		private void InsertObjects(string strOrgGuid)
		{
			DataSet ds = OGUReader.GetObjectsDetail("ORGANIZATIONS",
				strOrgGuid,
				SearchObjectColumn.SEARCH_GUID,
				string.Empty,
				SearchObjectColumn.SEARCH_NULL);

			parentAllPathName.Value = OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]);
		}

		private void InitPageObject()
		{
			DataSet ds = OGUReader.GetRankDefine(2, 1);

			RANK_CODE.DataSource = new DataView(ds.Tables[0]);
			RANK_CODE.DataTextField = "NAME";
			RANK_CODE.DataValueField = "CODE_NAME";
			RANK_CODE.DataBind();
			RANK_CODE.Items.Insert(0, new ListItem("--", string.Empty));
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
							AccreditResource.Func_ModifyUser,
							UserValueType.LogonName,
							DelegationMaskType.All);
						if (bPermission)
						{
							DataSet ds = SecurityCheck.GetUserFunctionsScopes(LogOnUserInfo.UserLogOnName,
								AccreditResource.AppCodeName,
								AccreditResource.Func_ModifyUser,
								UserValueType.LogonName,
								DelegationMaskType.All,
								ScopeMaskType.All);
							string strObjGuid = (string)GetRequestData("objGuid", string.Empty);
							bPermission = IsObjectIsIncludeInObjects("USERS", strObjGuid, SearchObjectColumn.SEARCH_GUID, ds);
						}
						break;
					case "AddSideline":
					case "Insert":
						bPermission = SecurityCheck.DoesUserHasPermissions(LogOnUserInfo.UserLogOnName,
							AccreditResource.AppCodeName,
							AccreditResource.Func_CreateUser,
							UserValueType.LogonName,
							DelegationMaskType.All);
						ExceptionHelper.FalseThrow(bPermission, "�Բ�����û��Ȩ�޴����µġ��û�����");
						if (bPermission)
						{
							DataSet ds = SecurityCheck.GetUserFunctionsScopes(LogOnUserInfo.UserLogOnName,
								AccreditResource.AppCodeName,
								AccreditResource.Func_CreateUser,
								UserValueType.LogonName,
								DelegationMaskType.All,
								ScopeMaskType.All);
							ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS",
								parentAllPathName.Value,
								SearchObjectColumn.SEARCH_ALL_PATH_NAME,
								ds),
								"�Բ�����û���ڵ�ǰ�����д������û�����Ȩ�ޣ�");
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
						if (strValue.Length <= strObjAllPathName.Length && strValue.Length >= 0 && strObjAllPathName.Substring(0, strValue.Length) == strValue)
							return true;
					}
				}
			}

			return false;
		}


		#region Web ������������ɵĴ���
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: �õ����� ASP.NET Web ���������������ġ�
			//
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
