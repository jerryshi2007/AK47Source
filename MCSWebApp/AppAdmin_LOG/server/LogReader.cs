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
using System.Text;

using MCS.Library.Data;
using MCS.Library.Data.Builder;

namespace MCS.Applications.AppAdmin_LOG.server
{
	/// <summary>
	/// LogReader 的摘要说明。
	/// </summary>
	internal class LogReader
	{
		public const string LOG_ORIGINAL_SORT = "0000";
		public LogReader()
		{
		}

		/// <summary>
		/// 将返回数据集转为xml
		/// </summary>
		/// <param name="ds"></param>
		/// <returns></returns>
		public static XmlDocument GetLevelSortXmlDocAttr(DataTable table, string strSortCol, string strNameCol, int iSortLength)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<DataTable />");
			XmlNode root = xmlDoc.DocumentElement;
			string strParentSort = string.Empty;
			string strOrgGuid = string.Empty;

			foreach (DataRow row in table.Rows)
			{
				XmlElement rowElem = (XmlElement)xmlDoc.CreateNode(XmlNodeType.Element, DBValueToString(row[strNameCol]), string.Empty);
				string strSortValue = DBValueToString(row[strSortCol]);

				while (strParentSort != strSortValue.Substring(0, strSortValue.Length - iSortLength))
				{
					if (root == xmlDoc.DocumentElement)
						break;
					else
					{
						root = root.ParentNode;
						strParentSort = ((XmlElement)root).GetAttribute(strSortCol);
					}
				}
				root = root.AppendChild(rowElem);
				strParentSort = strSortValue;

				rowElem.SetAttribute("APP_GUID", strOrgGuid);

				foreach (DataColumn col in table.Columns)
				{
					if (DBValueToString(row[col.ColumnName]) == "0000")
					{
						foreach (DataColumn col2 in table.Columns)
						{
							if (col2.ColumnName == "GUID")
								strOrgGuid = DBValueToString(row[col2.ColumnName]);
						}
					}
					if (col.ColumnName == strNameCol)
						continue;
					rowElem.SetAttribute(col.ColumnName, DBValueToString(row[col.ColumnName]));
				}
			}

			return xmlDoc;
		}

		/// <summary>
		/// 将数据库字段值转换为字符串
		/// </summary>
		public static string DBValueToString(object objValue)
		{
			string strResult = string.Empty;

			if (objValue != null)
			{
				if (objValue is System.DateTime)
				{
					if ((DateTime)objValue != DateTime.MinValue && (System.DateTime)objValue != new DateTime(1900, 1, 1, 0, 0, 0, 0))
						strResult = string.Format("{0:yyyy-MM-dd HH:mm:ss}", objValue);
				}
				else
					strResult = objValue.ToString();
			}

			return strResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appTypeGuid"></param>
		/// <param name="strSort"></param>
		/// <returns></returns>
		public static string GetNewClassValue(string appTypeGuid, string strSort)
		{
			string strSql = "SELECT ISNULL(MAX(CLASS + 1), 1) FROM APP_OPERATION_TYPE WHERE APP_GUID = "
				+ TSqlBuilder.Instance.CheckQuotationMark(appTypeGuid, true);

			string strTemp = "0000" + InnerCommon.ExecuteScalar(strSql).ToString();
			return strSort + strTemp.Substring(strTemp.Length - 4, 4);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="appTypeGuid"></param>
		/// <returns></returns>
		//		private static string GetClassByGuid(string appTypeGuid)
		//		{
		//			string strClass = string.Empty;
		//			string strSql = "SELECT LEFT(CLASS, 8) FROM APP_OPERATION_TYPE  WHERE APP_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(appTypeGuid);
		//			
		//			DataAccess da = new DataAccess(HGLogDefine.C_CONN_STRING);
		//
		//			using (da.dBContextInfo)
		//			{
		//				da.dBContextInfo.OpenConnection();
		//
		//				object obj = da.SqlDBHelper.ExecuteScalar(da.dBContextInfo.Context, CommandType.Text, strSql);
		//
		//				if(false==(obj is DBNull))
		//					strClass = (string)obj;
		//			}
		//			
		//			return strClass;
		//		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string GetClassForApp()
		{
			string strSql = "SELECT ISNULL(MAX(CLASS + 1), 1) FROM APP_LOG_TYPE WHERE LEN(CLASS) = 8";

			string strTemp = "00000000" + InnerCommon.ExecuteScalar(strSql).ToString();
			return strTemp.Substring(strTemp.Length - 8, 8);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="displayName"></param>
		/// <param name="strTableName"></param>
		/// <returns></returns>
		public static string GetGuidByDisplayName(string displayName, string strTableName)
		{
			string strGuid = string.Empty;
			string strSql = "SELECT GUID FROM " + strTableName + " WHERE DISPLAYNAME = "
				+ TSqlBuilder.Instance.CheckQuotationMark(displayName, true);

			object obj = InnerCommon.ExecuteScalar(strSql);
			if (false == (obj is DBNull))
				strGuid = (string)obj;

			return strGuid;
		}
	}
}
