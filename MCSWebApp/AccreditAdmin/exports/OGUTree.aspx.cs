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
	/// OGUTree 的摘要说明。
	/// </summary>
	public partial class OGUTree : WebBaseClass
	{
			//
//		protected System.Web.UI.HtmlControls.HtmlInputHidden LShowSideline;		// 用于领导兼职的显示
		// 展示界面要求使用的颜色
			//
		//
		//
				//
		/*****************************************************************************************************************/
		// 是否展现按钮
		// 是否允许多选
		// 要求列出的对象类型（机构、人员组、人员）
	// 要求列出的对象包括被删除内容（正常使用、直接逻辑删除、间接逻辑删除）
	// 列出对象中的可选对象（机构、人员组、人员）
			// 要求展现的机构树使用的“根机构”（默认为空）
		// 机构树中要求自动展开的对象（树结点自动展开）
			// 要求返回的属性数据
	// 用于Organization的级别控制
	// 用于User的级别控制
			// 要求机构树中针对配置信息中的指定配置上的数据信息被屏蔽
		// 选择是否要求记录次序
	// 是否允许选择机构树结构的根节点
		// 是否展现机构树中的回收站
		protected System.Web.UI.HtmlControls.HtmlInputHidden LListType;
		protected System.Web.UI.HtmlControls.HtmlInputHidden LListDelete;
		protected System.Web.UI.HtmlControls.HtmlInputHidden LSelectType;
	// 默认选定的对象（采用标记记录）
			// 要求展现机构的类型(cgac\yuan_yong 20041030)
			// 要求展现机构的属性(cgac\yuan_yong 20041030)
//		protected System.Web.UI.HtmlControls.HtmlInputHidden LShowMyOrg;		// 是否展现当前操作人员所在的机构(cgac\yuanyong [20041101])

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetNoStore();

			LMaxLevel.Value = GetParamData("maxLevel", (int)999999999).ToString();							// 最大展开层次
			LBackColor.Value = GetParamData("backColor", string.Empty).ToString();							// 展示界面要求使用的颜色
//			LShowSideline.Value = GetParamData("showSideline", "false").ToString();							// 用于领导兼职的显示
			LTarget.Value = GetParamData("target", string.Empty).ToString();								//
			string strTopControl = GetParamData("topControl", string.Empty).ToString();						//
			string strPostURL = GetParamData("postURL", string.Empty).ToString();							//
			TBSubChildData.Text = GetParamData("firstChildren", string.Empty).ToString();					// 针对于业务系统中对于角色下的数据对象选择
			/*****************************************************************************************************************/
			LShowButtons.Value = GetParamData("showButtons", 0).ToString();									// 是否展现按钮
			LMultiSelect.Value = GetParamData("multiSelect", 0).ToString();									// 是否允许多选
			LListObjType.Value = GetParamData("listObjType", (int)ListObjectType.ALL_TYPE).ToString();		// 要求列出的对象类型（机构、人员组、人员）
			LListObjDelete.Value = GetParamData("listObjDelete", (int)ListObjectDelete.COMMON).ToString();	// 要求列出的对象包括被删除内容（正常使用、直接逻辑删除、间接逻辑删除）
			LSelectObjType.Value = GetParamData("selectObjType", (int)ListObjectType.ALL_TYPE).ToString();	// 列出对象中的可选对象（机构、人员组、人员）
			LRootOrg.Value = GetParamData("rootOrg", string.Empty).ToString();								// 要求展现的机构树使用的“根机构”（默认为空）
			LAutoExpand.Value = GetParamData("autoExpand", string.Empty).ToString();						// 机构树中要求自动展开的对象（树结点自动展开）
			LExtAttr.Value = GetParamData("extAttr", string.Empty).ToString();								// 要求返回的属性数据
			LOrgAccessLevel.Value = GetParamData("orgAccessLevel", string.Empty).ToString();				// 用于Organization的级别控制
			LUserAccessLevel.Value = GetParamData("userAccessLevel", string.Empty).ToString();				// 用于User的级别控制
			LHideType.Value = GetParamData("hideType", string.Empty).ToString();							// 要求机构树中针对配置信息中的指定配置上的数据信息被屏蔽
			LSelectSort.Value = GetParamData("selectSort", 0).ToString();									// 选择是否要求记录次序
			LCanSelectRoot.Value = GetParamData("canSelectRoot", "false").ToString();						// 是否允许选择机构树结构的根节点
			LNodesSelected.Value = GetParamData("nodesSelected", string.Empty).ToString();					// 默认选定的对象（采用标记记录）
			LShowTrash.Value = GetParamData("showTrash", 0).ToString();										// 是否展现机构树中的回收站
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
//			LShowMyOrg.Value = GetRequestData("ShowMyOrg", 0).ToString();									// 是否展现当前操作人员所在的机构(cgac\yuanyong [20041101])
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

		#region Web 窗体设计器生成的代码
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
