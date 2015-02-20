#region using 

using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;

using System.Web.UI;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Applications.AccreditAdmin.Properties;
using MCS.Applications.AccreditAdmin.classLib;
#endregion

namespace MCS.Applications.AccreditAdmin.Test
{
	/// <summary>
	/// （工具）提取数据库中的各个数据表结构的Schema
	/// </summary>
	public partial class GetOGUSchema : WebBaseClass
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			string strXSDPath = Server.MapPath(Request.ApplicationPath) + @"/xsd/";
			XmlDocument sqlXml = GetXMLDocument("GetDBSchema");

			using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
			{
				string strSqlTable = sqlXml.DocumentElement.SelectSingleNode("QueryTable").InnerText;
				string strSqlColumn = sqlXml.DocumentElement.SelectSingleNode("QueryColumns").InnerText;

				DataSet dsTables = InnerCommon.ExecuteDataset(strSqlTable);

				foreach (DataRow row in dsTables.Tables[0].Rows)//each table
				{
					if (row["opk_id"] is DBNull)
						continue;

					string strSql = "SELECT TOP 1 * FROM ["
						+ TSqlBuilder.Instance.CheckQuotationMark(OGUCommonDefine.DBValueToString(row["name"]), false) + "]";
					DataSet ds = InnerCommon.ExecuteDataset(strSql);

					XmlDocument xmlDoc = XmlHelper.CreateDomDocument(ds.GetXmlSchema());

					strSql = string.Format(strSqlColumn,
						TSqlBuilder.Instance.CheckQuotationMark(OGUCommonDefine.DBValueToString(row["opk_id"]), true),
						TSqlBuilder.Instance.CheckQuotationMark(OGUCommonDefine.DBValueToString(row["id"]), true));

					DataSet tableInfo = InnerCommon.ExecuteDataset(strSql);

					foreach (DataRow infoRow in tableInfo.Tables[0].Rows)
					{

						XmlElement elem = (XmlElement)InnerCommon.GetXSDColumnNode(xmlDoc, OGUCommonDefine.DBValueToString(infoRow["name"]));
						elem.SetAttribute("size", OGUCommonDefine.DBValueToString(infoRow["prec"]));
						elem.SetAttribute("caption", OGUCommonDefine.DBValueToString(infoRow["description"]));
						elem.SetAttribute("allowNull", OGUCommonDefine.DBValueToString(infoRow["isnullable"]) == "0" ? "false" : "true");
						elem.SetAttribute("isKey", infoRow["keyno"] is DBNull ? "false" : "true");
						elem.SetAttribute("imeMode", OGUCommonDefine.DBValueToString(infoRow["type"]).ToUpper() == "NVARCHAR" ? "active" : "inactive");
					}

					XmlElement root = xmlDoc.DocumentElement;
					while (root.FirstChild != null && root.GetAttribute("name") != "Table")
						root = (XmlElement)root.FirstChild;
					if (root.GetAttribute("name") == "Table")
						root.SetAttribute("name", OGUCommonDefine.DBValueToString(row["name"]));
					xmlDoc.Save(strXSDPath + OGUCommonDefine.DBValueToString(row["name"]) + ".xsd");
				}
			}
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
