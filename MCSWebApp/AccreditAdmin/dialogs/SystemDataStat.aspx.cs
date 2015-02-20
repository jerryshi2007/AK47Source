#region using 

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Xml;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Core;
using MCS.Library.Accredit.OguAdmin;
using MCS.Applications.AccreditAdmin.Properties;
using System.Xml.XPath;
#endregion

namespace MCS.Applications.AccreditAdmin.dialogs
{
	/// <summary>
	/// SystemDataStat 的摘要说明。
	/// </summary>
	public partial class SystemDataStat : WebUserBaseClass
	{

		private int _ColumnCount = 0;
		private XmlElement _XmlParam = null;
		

		protected void Page_Load(object sender, System.EventArgs e)
		{
			XmlDocument xmlDoc = GetSearchData();
			XmlElement root = xmlDoc.DocumentElement;
			resultTitle.Text = root.GetAttribute("rootOrg");

			ShowResultToPage(root, bTable);
		}

		private XmlDocument GetSearchData()
		{
			string strAllColumns = (string)GetFormData("dataColumns", "ALL_PATH_NAME");
			GetXmlParam(strAllColumns);

			string strRootOrgGuid = (string)GetFormData("rootOrganizationGuid", string.Empty);
			int iDataType = (int)GetFormData("ExportOrganization", 0) + (int)GetFormData("ExportGroup", 0) + (int)GetFormData("ExportUser", 0);

			if ((int)GetFormData("ExportUser", 0) != 0)
				iDataType += 8;

			DataSet ds = OGUReader.GetOrganizationChildren(strRootOrgGuid, 
				SearchObjectColumn.SEARCH_GUID, 
				iDataType, 
				(int)ListObjectDelete.COMMON, 
				0, 
				string.Empty, 
				string.Empty, 
				strAllColumns);

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc = OGUReader.GetLevelSortXmlDocAttr(ds.Tables[0], 
				"GLOBAL_SORT", 
				"OBJECTCLASS", 
				AccreditResource.OriginalSortDefault.Length);
			XmlElement root = xmlDoc.DocumentElement;
			string strRootOrg = root.FirstChild.Attributes["ALL_PATH_NAME"].Value;
			if (root.FirstChild.Attributes["OBJECTCLASS"].Value != "ORGANIZATIONS")
				strRootOrg = OGUReader.GetObjectsDetail("ORGANIZATIONS", 
					strRootOrgGuid, 
					SearchObjectColumn.SEARCH_GUID).Tables[0].Rows[0]["ALL_PATH_NAME"].ToString();
			
			root.SetAttribute("rootOrg", strRootOrg.Replace("\\", "．"));
			
			xmlResultServer.DocumentContent = xmlDoc.DocumentElement.OuterXml;

			return xmlDoc;
		}

		private void GetXmlParam(string strAllColumns)
		{
			XmlDocument xmlResult = new XmlDocument();
			xmlResult.LoadXml("<Options/>");
			_XmlParam = xmlResult.DocumentElement;

			XmlDocument xmlSource = this.GetXMLDocument("ExportOrImport");
			XmlElement rootSource = xmlSource.DocumentElement;

			string[] strArr = strAllColumns.Split(',');
			foreach (string str in strArr)
			{
				XmlNode oNode = rootSource.SelectSingleNode("Option[Value=\"" + str + "\"]");
				if (oNode != null)
				{
					XmlNode nNode = XmlHelper.AppendNode(_XmlParam, "Option");
					XmlHelper.AppendNode(nNode, "Value", oNode.SelectSingleNode("Value").InnerText);
					XmlHelper.AppendNode(nNode, "Text", oNode.SelectSingleNode("Text").InnerText);
				}
			}

			_ColumnCount = _XmlParam.ChildNodes.Count;

			xmlResultServer.DocumentContent = xmlResult.DocumentElement.OuterXml;
		}

		private void ShowResultToPage(XmlElement root, HtmlTable oTable)
		{
			ShowTitleToPage(oTable);
			foreach (XmlElement eleRoot in root.ChildNodes)
			{
				string strObjClass = eleRoot.GetAttribute("OBJECTCLASS");

				switch (strObjClass)
				{
					case "ORGANIZATIONS":	ShowOrganizationToPage(eleRoot, oTable);
						break;
					case "USERS":			ShowUserToPage(eleRoot, oTable);		
						break;
					case "GROUPS":			ShowGroupToPage(eleRoot, oTable);
						break;
					default:				ExceptionHelper.TrueThrow(true, "数据类型“" + strObjClass + "”不符合规范！");
						break;
				}
			}
		}


		private void ShowOrganizationToPage(XmlElement root, HtmlTable oTable)
		{
			ShowContentToPage(root, oTable);

			if (root.ChildNodes.Count > 0)
			{
				HtmlTableRow oRow = new HtmlTableRow();
				oTable.Rows.Add(oRow);
				HtmlTableCell oCell = new HtmlTableCell();
				oRow.Cells.Add(oCell);
				oCell.InnerHtml = "&nbsp;";

				oCell = new HtmlTableCell();
				oRow.Cells.Add(oCell);
				oCell.ColSpan = 2 + _ColumnCount;
                
				HtmlTable newTable = new HtmlTable();
				newTable.CellPadding = 0;
				newTable.CellSpacing = 0;
				newTable.Border = 1;
				newTable.Width = "100%";				
				oCell.Controls.Add(newTable);

				ShowResultToPage(root, newTable);
			}
		}

		private void ShowGroupToPage(XmlElement root, HtmlTable oTable)
		{
			ShowContentToPage(root, oTable);
		}

		private void ShowUserToPage(XmlElement root, HtmlTable oTable)
		{
			ShowContentToPage(root, oTable);
		}


		private void ShowContentToPage(XmlElement root, HtmlTable oTable)
		{
			// 行准备
			HtmlTableRow oRow = new HtmlTableRow();
			oTable.Rows.Add(oRow);

			// 收缩预留
			HtmlTableCell dCell = new HtmlTableCell();
			oRow.Cells.Add(dCell);
			dCell.Align = "center";
			if (root.ChildNodes.Count > 0) //一定是机构
				dCell.InnerHtml = "<strong onclick=\"showChildren()\">-</strong>";
			else
				dCell.InnerHtml = "&nbsp;";

			// 图像预留
			HtmlTableCell imgCell = new HtmlTableCell();
			oRow.Cells.Add(imgCell);
			imgCell.Align = "center";
			string strObjClass = root.GetAttribute("OBJECTCLASS");
			imgCell.InnerHtml = "<IMG alt=\"．\" src=\"../images/" + strObjClass.Substring(0, strObjClass.Length - 1) + ".gif\">";

			// 填充所有数据
			foreach (XmlNode oNode in _XmlParam.ChildNodes)
			{
				HtmlTableCell oCell = new HtmlTableCell();
				oRow.Cells.Add(oCell);

				oCell.Align = "right";
				string strValue = root.GetAttribute(oNode.SelectSingleNode("Value").InnerText);
				if (strValue == string.Empty)
					strValue = "&nbsp;";

				oCell.InnerHtml = strValue;
			}

			// 统计数据
			HtmlTableCell sCell = new HtmlTableCell();
			oRow.Cells.Add(sCell);
			sCell.Align = "right";
			if (root.ChildNodes.Count > 0)
			{
				string strStat = string.Empty;
				int iStat = root.SelectNodes("ORGANIZATIONS").Count;
				if (iStat > 0)
					strStat += "直属机构<B>" + iStat.ToString() + "</B>个；";

				iStat = root.SelectNodes(".//ORGANIZATIONS").Count;
				if (iStat > 0)
					strStat += "子孙机构<B>" + iStat.ToString() + "</B>个；";
				iStat = root.SelectNodes(".//GROUPS").Count;
				if (iStat > 0)
					strStat += "人员组<B>" + iStat.ToString() + "</B>个；";
				iStat = root.SelectNodes(".//USERS").Count;
				if (iStat > 0)
					strStat += "个人账号<B>" + iStat.ToString() + "</B>个；";

				sCell.InnerHtml = strStat;
			}
			else
				sCell.InnerHtml = "&nbsp;";
		}

		private void ShowTitleToPage(HtmlTable oTable)
		{
			// 行准备
			HtmlTableRow oRow = new HtmlTableRow();
			oTable.Rows.Add(oRow);

			// 为收缩预留
			HtmlTableCell dCell = new HtmlTableCell();
			oRow.Cells.Add(dCell);
			dCell.Align = "center";
			dCell.Width = "8px";
			dCell.InnerHtml = "&nbsp;";

			// 为图像预留
			HtmlTableCell imgCell = new HtmlTableCell();
			oRow.Cells.Add(imgCell);
			imgCell.Align = "center";
			imgCell.Width = "16px";
			imgCell.InnerHtml = "&nbsp;";

			// 预备所有填充头
			foreach (XmlNode oNode in _XmlParam.ChildNodes)
			{
				HtmlTableCell oCell = new HtmlTableCell();
				oRow.Cells.Add(oCell);
				oCell.Align = "Center";
				oCell.InnerHtml = "<strong>" + oNode.SelectSingleNode("Text").InnerText + "</strong>";
			}

			// 预备数据统计
			HtmlTableCell sCell = new HtmlTableCell();
			oRow.Cells.Add(sCell);
			sCell.Align = "center";
			sCell.InnerHtml = "<strong>统计备注</strong>";
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
