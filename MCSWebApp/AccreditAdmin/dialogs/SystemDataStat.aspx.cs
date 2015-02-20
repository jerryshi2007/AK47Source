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
	/// SystemDataStat ��ժҪ˵����
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
			
			root.SetAttribute("rootOrg", strRootOrg.Replace("\\", "��"));
			
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
					default:				ExceptionHelper.TrueThrow(true, "�������͡�" + strObjClass + "�������Ϲ淶��");
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
			// ��׼��
			HtmlTableRow oRow = new HtmlTableRow();
			oTable.Rows.Add(oRow);

			// ����Ԥ��
			HtmlTableCell dCell = new HtmlTableCell();
			oRow.Cells.Add(dCell);
			dCell.Align = "center";
			if (root.ChildNodes.Count > 0) //һ���ǻ���
				dCell.InnerHtml = "<strong onclick=\"showChildren()\">-</strong>";
			else
				dCell.InnerHtml = "&nbsp;";

			// ͼ��Ԥ��
			HtmlTableCell imgCell = new HtmlTableCell();
			oRow.Cells.Add(imgCell);
			imgCell.Align = "center";
			string strObjClass = root.GetAttribute("OBJECTCLASS");
			imgCell.InnerHtml = "<IMG alt=\"��\" src=\"../images/" + strObjClass.Substring(0, strObjClass.Length - 1) + ".gif\">";

			// �����������
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

			// ͳ������
			HtmlTableCell sCell = new HtmlTableCell();
			oRow.Cells.Add(sCell);
			sCell.Align = "right";
			if (root.ChildNodes.Count > 0)
			{
				string strStat = string.Empty;
				int iStat = root.SelectNodes("ORGANIZATIONS").Count;
				if (iStat > 0)
					strStat += "ֱ������<B>" + iStat.ToString() + "</B>����";

				iStat = root.SelectNodes(".//ORGANIZATIONS").Count;
				if (iStat > 0)
					strStat += "�������<B>" + iStat.ToString() + "</B>����";
				iStat = root.SelectNodes(".//GROUPS").Count;
				if (iStat > 0)
					strStat += "��Ա��<B>" + iStat.ToString() + "</B>����";
				iStat = root.SelectNodes(".//USERS").Count;
				if (iStat > 0)
					strStat += "�����˺�<B>" + iStat.ToString() + "</B>����";

				sCell.InnerHtml = strStat;
			}
			else
				sCell.InnerHtml = "&nbsp;";
		}

		private void ShowTitleToPage(HtmlTable oTable)
		{
			// ��׼��
			HtmlTableRow oRow = new HtmlTableRow();
			oTable.Rows.Add(oRow);

			// Ϊ����Ԥ��
			HtmlTableCell dCell = new HtmlTableCell();
			oRow.Cells.Add(dCell);
			dCell.Align = "center";
			dCell.Width = "8px";
			dCell.InnerHtml = "&nbsp;";

			// Ϊͼ��Ԥ��
			HtmlTableCell imgCell = new HtmlTableCell();
			oRow.Cells.Add(imgCell);
			imgCell.Align = "center";
			imgCell.Width = "16px";
			imgCell.InnerHtml = "&nbsp;";

			// Ԥ���������ͷ
			foreach (XmlNode oNode in _XmlParam.ChildNodes)
			{
				HtmlTableCell oCell = new HtmlTableCell();
				oRow.Cells.Add(oCell);
				oCell.Align = "Center";
				oCell.InnerHtml = "<strong>" + oNode.SelectSingleNode("Text").InnerText + "</strong>";
			}

			// Ԥ������ͳ��
			HtmlTableCell sCell = new HtmlTableCell();
			oRow.Cells.Add(sCell);
			sCell.Align = "center";
			sCell.InnerHtml = "<strong>ͳ�Ʊ�ע</strong>";
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
