#region using 

using System;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.OguAdmin.Interfaces;
using MCS.Library.Core;

#endregion

namespace MCS.Applications.AccreditAdmin.exports
{
	/// <summary>
	/// OGUTree ��ժҪ˵����
	/// </summary>
	public partial class OGUTree : WebBaseClass
	{
			//
//		protected System.Web.UI.HtmlControls.HtmlInputHidden LShowSideline;		// �����쵼��ְ����ʾ
		// չʾ����Ҫ��ʹ�õ���ɫ
			//
		//
		//
				//
		/*****************************************************************************************************************/
		// �Ƿ�չ�ְ�ť
		// �Ƿ������ѡ
		// Ҫ���г��Ķ������ͣ���������Ա�顢��Ա��
	// Ҫ���г��Ķ��������ɾ�����ݣ�����ʹ�á�ֱ���߼�ɾ��������߼�ɾ����
	// �г������еĿ�ѡ���󣨻�������Ա�顢��Ա��
			// Ҫ��չ�ֵĻ�����ʹ�õġ�����������Ĭ��Ϊ�գ�
		// ��������Ҫ���Զ�չ���Ķ���������Զ�չ����
			// Ҫ�󷵻ص���������
	// ����Organization�ļ������
	// ����User�ļ������
			// Ҫ������������������Ϣ�е�ָ�������ϵ�������Ϣ������
		// ѡ���Ƿ�Ҫ���¼����
	// �Ƿ�����ѡ��������ṹ�ĸ��ڵ�
		// �Ƿ�չ�ֻ������еĻ���վ
		protected System.Web.UI.HtmlControls.HtmlInputHidden LListType;
		protected System.Web.UI.HtmlControls.HtmlInputHidden LListDelete;
		protected System.Web.UI.HtmlControls.HtmlInputHidden LSelectType;
	// Ĭ��ѡ���Ķ��󣨲��ñ�Ǽ�¼��
			// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
			// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
//		protected System.Web.UI.HtmlControls.HtmlInputHidden LShowMyOrg;		// �Ƿ�չ�ֵ�ǰ������Ա���ڵĻ���(cgac\yuanyong [20041101])

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetNoStore();

			LMaxLevel.Value = GetParamData("maxLevel", (int)999999999).ToString();							// ���չ�����
			LBackColor.Value = GetParamData("backColor", string.Empty).ToString();							// չʾ����Ҫ��ʹ�õ���ɫ
//			LShowSideline.Value = GetParamData("showSideline", "false").ToString();							// �����쵼��ְ����ʾ
			LTarget.Value = GetParamData("target", string.Empty).ToString();								//
			string strTopControl = GetParamData("topControl", string.Empty).ToString();						//
			string strPostURL = GetParamData("postURL", string.Empty).ToString();							//
			TBSubChildData.Text = GetParamData("firstChildren", string.Empty).ToString();					// �����ҵ��ϵͳ�ж��ڽ�ɫ�µ����ݶ���ѡ��
			/*****************************************************************************************************************/
			LShowButtons.Value = GetParamData("showButtons", 0).ToString();									// �Ƿ�չ�ְ�ť
			LMultiSelect.Value = GetParamData("multiSelect", 0).ToString();									// �Ƿ������ѡ
			LListObjType.Value = GetParamData("listObjType", (int)ListObjectType.ALL_TYPE).ToString();		// Ҫ���г��Ķ������ͣ���������Ա�顢��Ա��
			LListObjDelete.Value = GetParamData("listObjDelete", (int)ListObjectDelete.COMMON).ToString();	// Ҫ���г��Ķ��������ɾ�����ݣ�����ʹ�á�ֱ���߼�ɾ��������߼�ɾ����
			LSelectObjType.Value = GetParamData("selectObjType", (int)ListObjectType.ALL_TYPE).ToString();	// �г������еĿ�ѡ���󣨻�������Ա�顢��Ա��
			LRootOrg.Value = GetParamData("rootOrg", string.Empty).ToString();								// Ҫ��չ�ֵĻ�����ʹ�õġ�����������Ĭ��Ϊ�գ�
			LAutoExpand.Value = GetParamData("autoExpand", string.Empty).ToString();						// ��������Ҫ���Զ�չ���Ķ���������Զ�չ����
			LExtAttr.Value = GetParamData("extAttr", string.Empty).ToString();								// Ҫ�󷵻ص���������
			LOrgAccessLevel.Value = GetParamData("orgAccessLevel", string.Empty).ToString();				// ����Organization�ļ������
			LUserAccessLevel.Value = GetParamData("userAccessLevel", string.Empty).ToString();				// ����User�ļ������
			LHideType.Value = GetParamData("hideType", string.Empty).ToString();							// Ҫ������������������Ϣ�е�ָ�������ϵ�������Ϣ������
			LSelectSort.Value = GetParamData("selectSort", 0).ToString();									// ѡ���Ƿ�Ҫ���¼����
			LCanSelectRoot.Value = GetParamData("canSelectRoot", "false").ToString();						// �Ƿ�����ѡ��������ṹ�ĸ��ڵ�
			LNodesSelected.Value = GetParamData("nodesSelected", string.Empty).ToString();					// Ĭ��ѡ���Ķ��󣨲��ñ�Ǽ�¼��
			LShowTrash.Value = GetParamData("showTrash", 0).ToString();										// �Ƿ�չ�ֻ������еĻ���վ
			LOrgClass.Value = GetParamData("orgClass", string.Empty).ToString();
			LOrgType.Value = GetParamData("orgType", string.Empty).ToString();

			if (strTopControl != string.Empty)
			{
				LSumitType.Value = "topControl";
				LSumitData.Value = strTopControl;
			}
			else
			{
				if (strPostURL != null)
				{
					LSumitType.Value = "post";
					LSumitData.Value = strPostURL;
				}
			}

			SetShowMyOrgValue();
		}

		private void SetShowMyOrgValue()
		{
//			LShowMyOrg.Value = GetRequestData("ShowMyOrg", 0).ToString();									// �Ƿ�չ�ֵ�ǰ������Ա���ڵĻ���(cgac\yuanyong [20041101])
			if (GetParamData("ShowMyOrg", 0).ToString() == "1")
			{
				string strAutoExpand = LAutoExpand.Value.Trim();

				ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
				if (lou != null)
				{
					string strTemp = lou.OuUsers[0].AllPathName;
					string strParentAllPathName = strTemp.Substring(0, strTemp.LastIndexOf("\\"));

					XmlDocument xmlDoc = new XmlDocument();

					if (strAutoExpand != string.Empty)
						xmlDoc.LoadXml(strAutoExpand);
					else
						xmlDoc.LoadXml("<root />");

					XmlElement objXml = (XmlElement)XmlHelper.AppendNode(xmlDoc.DocumentElement, "object");
					objXml.SetAttribute("ALL_PATH_NAME", strParentAllPathName);
					LAutoExpand.Value = xmlDoc.OuterXml;
				}
			}
		}

		private object GetParamData(string strObjName, object oDefaultValue)
		{
			object oResult = null;

			if (Request.Form.Count <= 0)
				oResult = GetRequestData(strObjName, oDefaultValue);
			else
				oResult = GetFormData("hid" + strObjName, oDefaultValue);

			return oResult;
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
