using System;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Transactions;
using System.Diagnostics;
using System.Text;

using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Accredit.OguAdmin;
using MCS.Applications.AccreditAdmin.Properties;

namespace MCS.Applications.AccreditAdmin.classLib
{
	internal class InnerCommon
	{
		#region Database
		/// <summary>
		/// �Զ����SQL��ִ�й���
		/// </summary>
		/// <param name="strSql">Ҫ��ִ�е����ݲ�ѯSQL</param>
		/// <returns>���β�����Ӱ�����������</returns>
		internal static int ExecuteNonQuery(string strSql)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteNonQueryWithoutTransaction--strSql");
			using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
#if DEBUG
				Debug.WriteLine(strSql);
#endif
				ExceptionHelper.TrueThrow(string.IsNullOrEmpty(strSql), "���ݴ������SQL����Ϊ�մ���");

				return database.ExecuteNonQuery(CommandType.Text, strSql);
			}
		}
		/// <summary>
		/// �Զ����SQL��ִ�й���
		/// </summary>
		/// <param name="strSql">Ҫ��ִ�е����ݲ�ѯSQL</param>
		/// <param name="strTables">Ҫ�����õ����ݱ�����</param>
		/// <returns>���β�ѯ�����ݽ����</returns>
		internal static DataSet ExecuteDataset(string strSql, params string[] strTables)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteDatasetWithoutTransaction--strSql");
#if DEBUG
			Debug.WriteLine(strSql);
#endif
			using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				return database.ExecuteDataSet(CommandType.Text, strSql, strTables);
			}
		}
		/// <summary>
		/// �Զ����SQL��ִ�й���
		/// </summary>
		/// <param name="strSql">Ҫ��ִ�е����ݲ�ѯSQL</param>
		/// <returns>���β�ѯ�Ľ������</returns>
		internal static object ExecuteScalar(string strSql)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteScalar--strSql");
#if DEBUG
			Debug.WriteLine(strSql);
#endif
			using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				return database.ExecuteScalar(CommandType.Text, strSql);
			}
		}
		#endregion

		#region Xml
		/// <summary>
		/// �����ݼ���DataSet�е���������ת����XML�ĵ����󷵻أ�����ֶ��е�NULLֵ��XML�ĵ��в���ʾ����
		/// </summary>
		/// <param name="dataSet">Ҫ��ת���ı�׼DataSet����</param>
		/// <returns>����ָ��XML��ʽ��XML�ĵ�����</returns>
		/// <remarks>
		/// �����ݼ���DataSet�е���������ת����XML�ĵ����󷵻أ�����ֶ��е�NULLֵ��XML�ĵ��в���ʾ���⡣
		/// ��󷵻ص�XML�ĵ������а���������DataSet�е��������ݣ������DataSet�а����ж��DataTable��
		/// ���صĽ���л��������Щ���ݶ��������ڡ�����������DataTable�����У�
		/// </remarks>
		/// <example>
		/// ����ֵ��
		/// <code>
		/// &lt;DataSet&gt;
		///		&lt;table&gt;
		///			&lt;colName&gt;columnValue1&lt;/colName&gt;
		///			........
		///		&lt;/table&gt;
		///		&lt;table&gt;
		///			&lt;colName&gt;columnValue2&lt;/colName&gt;
		///			........
		///		&lt;/table&gt;
		///		.........
		///		&lt;table1&gt;	////���DataTable���
		///			&lt;colNameA&gt;colValue&lt;/colNameA&gt;
		///			........
		///		&lt;/table1&gt;
		///		..........
		/// &lt;/DataSet&gt;
		/// </code>
		/// </example>
		public static XmlDocument GetXmlDoc(DataSet dataSet)
		{
			return GetXmlDoc(dataSet, false);
		}
		/// <summary>
		/// �����ݼ���DataSet�е���������ת����XML�ĵ����󷵻أ�����ֶ��е�NULLֵ��XML�ĵ��в���ʾ����
		/// </summary>
		/// <param name="dataSet">Ҫ��ת���ı�׼DataSet����</param>
		/// <param name="bCDataSection">����ʱÿ���ֶε�ֵ�Ƿ���CData������</param>
		/// <returns>����ָ��XML��ʽ��XML�ĵ�����</returns>
		/// <remarks>
		/// �����ݼ���DataSet�е���������ת����XML�ĵ����󷵻أ�����ֶ��е�NULLֵ��XML�ĵ��в���ʾ���⡣
		/// ��󷵻ص�XML�ĵ������а���������DataSet�е��������ݣ������DataSet�а����ж��DataTable��
		/// ���صĽ���л��������Щ���ݶ��������ڡ�����������DataTable�����У�
		/// </remarks>
		/// <example>
		/// ����ֵ��
		/// <code>
		///  <![CDATA[
		///  <DataSet>
		///		<Table1>
		///			<Field1>Value</Field1>
		///			<Field2>Value</Field2>
		///		</Table1>
		///		<Table2>
		///			<Field1>Value</Field1>
		///			<Field2>Value</Field2>
		///		</Table2>
		///  </DataSet>
		///  ]]>
		/// </code>
		/// </example>
		public static XmlDocument GetXmlDoc(DataSet dataSet, bool bCDataSection)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<DataSet/>");

			XmlElement root = xmlDoc.DocumentElement;

			foreach (DataTable dataTable in dataSet.Tables)
			{
				foreach (DataRow dataRow in dataTable.Rows)
				{
					XmlNode xmlTableNode = XmlHelper.AppendNode(root, dataTable.TableName);

					foreach (DataColumn dataColumn in dataTable.Columns)
					{
						string strColumnValue = OGUCommonDefine.DBValueToString(dataRow[dataColumn]);

						if (bCDataSection)
							XmlHelper.AppendCDataNode<string>(xmlTableNode, dataColumn.ColumnName, strColumnValue);
						else
							XmlHelper.AppendNode<string>(xmlTableNode, dataColumn.ColumnName, strColumnValue);
					}
				}
			}

			return xmlDoc;
		}
		/// <summary>
		/// ��table�и���������ֶ���������XML��Ӧ������������
		/// </summary>
		/// <param name="table">�����������ݶ�������ݱ�</param>
		/// <param name="strNameCol">��������Ͷ�Ӧ�ֶ����ɣ���Ӧ�ڵ��ָ���Ķ�Ӧ���Ƶ��ֶΣ�</param>
		/// <returns>��table�и���������ֶ���������XML��Ӧ������������</returns>
		public static XmlDocument GetXmlDocAttr(DataTable table, string strNameCol)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<DataTable />");
			XmlNode root = xmlDoc.DocumentElement;

			foreach (DataRow row in table.Rows)
			{
				XmlElement rowElem = (XmlElement)XmlHelper.AppendNode(root, OGUCommonDefine.DBValueToString(row[strNameCol]));
				foreach (DataColumn col in table.Columns)
					rowElem.SetAttribute(col.ColumnName, OGUCommonDefine.DBValueToString(row[col.ColumnName]));
			}

			return xmlDoc;
		}
		/// <summary>
		/// ��xml�ڵ��ϵ�����ֵת������XML������
		/// </summary>
		/// <param name="xmlSource">���нڵ����ݵ�xml����</param>
		/// <returns>ת��Ϊ����ֵ��XML���ݶ���</returns>
		public static XmlDocument XmlNodeSetToAttribute(XmlDocument xmlSource)
		{
			XmlDocument xmlResult = new XmlDocument();
			XmlNode nodeSet = xmlSource.DocumentElement.SelectSingleNode(".//SET");
			if (nodeSet != null)
			{
				xmlResult.LoadXml("<" + xmlSource.DocumentElement.LocalName + "/>");
				XmlElement root = xmlResult.DocumentElement;
				root = (XmlElement)XmlHelper.AppendNode(root, nodeSet.ParentNode.LocalName);

				foreach (XmlNode node in nodeSet.ChildNodes)
					root.SetAttribute(node.LocalName, node.InnerText);
			}
			return xmlResult;
		}
		#endregion

		#region Xsd
		/// <summary>
		/// ���xmlDocָ����Xml�ĵ������е�NameSpaceManager����
		/// </summary>
		/// <param name="xmlDoc">�������XML�ĵ�����һ����XSD�ļ����ݣ�</param>
		/// <returns>��XML��NameSpceManage����</returns>
		/// <remarks>
		/// ���xmlDocָ����Xml�ĵ������е�NameSpaceManager���󣬱�����һ�����ڶ���һЩ������ʽ��
		/// XML�ĵ���������������
		/// <code>
		/// &lt;DOC:INFO&gt;
		/// &lt;GUI:NODE name="hello"&gt;���⻯&lt;/GUI:NODE&gt;
		/// &lt;/DOC:INFO&gt;
		/// </code>
		/// һ���XML�ĵ�����
		/// <seealso cref="System.Xml.XmlNamespaceManager"/>
		/// </remarks>
		private static XmlNamespaceManager GetNSMgr(XmlDocument xmlDoc)
		{
			XmlNamespaceManager nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);

			if (nsMgr.HasNamespace("xs") == false)
				nsMgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");

			if (nsMgr.HasNamespace("msdata") == false)
				nsMgr.AddNamespace("msdata", "urn:schemas-microsoft-com:xml-msdata");

			return nsMgr;
		}

		/// <summary>
		/// ��ָ����XML�ĵ�����xmlDoc���л��һ��strColumnNameָ���Ľڵ�
		/// </summary>
		/// <param name="xmlDoc">ָ����XML�ĵ���������һ���������ֵ䣩</param>
		/// <param name="strColumnName">ָ���Ľڵ����ƣ����ﳣ����Ϊ�����ֵ��е������ƣ�</param>
		/// <returns>XML�ĵ������е�ָ���ڵ�</returns>
		/// <remarks>
		/// ��ָ����XML�ĵ�����xmlDoc���л��һ��strColumnNameָ���Ľڵ㡣������Ҫ���ڶ������ֵ�XSD�ĵ��Ĵ���
		/// ��������ֵ��е���strColumnNameָ�������ݽڵ㣨һ���������ֵ��е��нڵ㣩
		/// </remarks>
		public static XmlNode GetXSDColumnNode(XmlDocument xmlDoc, string strColumnName)
		{
			return xmlDoc.SelectSingleNode(
				".//xs:sequence/xs:element[@name = \"" + strColumnName + "\"]",
				GetNSMgr(xmlDoc));
		}

		/// <summary>
		/// ��XML�ĵ�����xmlDoc�л�ȡ��strColumnNameָ���ڵ���strAttrָ��������ֵ
		/// </summary>
		/// <param name="xmlDoc">�������ݵ�XML�ĵ����������ֵ�Ķ�ӦXML�ĵ�����</param>
		/// <param name="strColumnName">XML�ĵ�������strColumnNameָ���Ľڵ����ƣ������ֵ��е������ƣ�</param>
		/// <param name="strAttr">�ڵ���������</param>
		/// <returns>XML�ĵ�����ָ���ڵ���ָ����������Ӧ������ֵ</returns>
		/// <remarks>
		/// ��XML�ĵ�����xmlDoc�л�ȡ��strColumnNameָ���ڵ���strAttrָ��������ֵ��������ҪӦ���������ֵ��ļ���
		/// �Ľڵ��������Ի�á�
		/// </remarks>
		private static string GetXSDColumnAttrDirect(XmlDocument xmlDoc, string strColumnName, string strAttr)
		{
			XmlNode xmlnodeColumn = GetXSDColumnNode(xmlDoc, strColumnName);

			ExceptionHelper.TrueThrow(xmlnodeColumn == null, "�������ֵ���δ�ҵ���" + strColumnName);

			return GetXSDColumnAttr(xmlnodeColumn, strAttr);
		}

		/// <summary>
		/// ���ָ���нڵ㣨���ݱ���У�nodeColumn��strAttrָ������ֵ
		/// </summary>
		/// <param name="nodeColumn">�������ݵ��нڵ�</param>
		/// <param name="strAttr">Ҫ������ݵ���������</param>
		/// <returns>�нڵ㣨���ݱ���У���ָ������ֵ</returns>
		/// <remarks>
		/// ���ָ���нڵ㣨���ݱ���У�nodeColumn��strAttrָ������ֵ������string�ķ�ʽ���ؽ��
		/// </remarks>
		private static string GetXSDColumnAttr(XmlNode nodeColumn, string strAttr)
		{
			XmlAttribute xmlAttr = nodeColumn.Attributes[strAttr];

			if (xmlAttr == null)
				return string.Empty;
			else
				return xmlAttr.Value;
		}

		#endregion

		#region SqlStr
		/// <summary>
		/// ���������������ݵ�xml�ĵ�����Ͷ�Ӧ���ݿ�������ֵ����ɱ�׼��INSERT SQL ��
		/// </summary>
		/// <param name="xmlTableRowValue">���ϲ���XML��׼��ʽ��XML�ĵ�</param>
		/// <param name="xsdColumnDT">�����ֵ�</param>
		/// <returns>��׼��INSERT SQL��</returns>
		/// <remarks>
		/// �������ݵ�xml�ĵ�����Ͷ�Ӧ���ݿ�������ֵ����ɱ�׼��INSERT SQL ���������ϳɱ�׼��SQL Insert��䲻�ɹ�
		/// ��������쳣���档���������ǵ�XML�ĵ������п����γɶ��insert���ݿ�������䣩
		/// </remarks>
		/// <example>
		///	&lt;INSERT&gt;
		///		&lt;INFO_FILE&gt;
		///			&lt;SET&gt;
		///				&lt;TITLE&gt;title&lt;/TITLE&gt;
		///				&lt;CONTENT&gt;content&lt;/CONTENT&gt;
		///				&lt;SEND_DATE&gt;2003-1-3&lt;/SEND_DATE&gt;
		///			&lt;/SET&gt;
		///		&lt;/INFO_FILE&gt;
		///		&lt;INFO_FILE&gt;
		///			&lt;SET&gt;
		///				&lt;TITLE&gt;title1&lt;/TITLE&gt;
		///				&lt;CONTENT&gt;title2&lt;/CONTENT&gt;
		///				&lt;SEND_DATE&gt;2003-1-2&lt;/SEND_DATE&gt;
		///			&lt;/SET&gt;
		///		&lt;/INFO_FILE&gt;
		///	&lt;/INSERT&gt;
		/// </example>
		public static string GetInsertSqlStr(XmlDocument xmlTableRowValue, XmlDocument xsdColumnDT)
		{
			StringBuilder strBInsertSqlStr = new StringBuilder(1024);

			XmlNode rootNode = xmlTableRowValue.DocumentElement;

			XmlNode xmlTableNode = rootNode.FirstChild;

			int nTable = 0;

			while (xmlTableNode != null)
			{
				string strTableName = xmlTableNode.Name;

				GetOneInsertSqlStr(nTable, xmlTableNode.SelectSingleNode("SET"), strBInsertSqlStr, xsdColumnDT, strTableName);

				nTable++;
				xmlTableNode = xmlTableNode.NextSibling;
				/*
				if (nTable > 0)
					strBInsertSqlStr.Append(" ; ");
				
				AppendStrBInsert(strBInsertSqlStr, xmlTableNode, xsdColumnDT);
				
				nTable++;
				xmlTableNode = xmlTableNode.NextSibling ;
				*/
			}

			ExceptionHelper.TrueThrow((strBInsertSqlStr.Length == 0), "�����xml�ĵ����󲻷��Ϲ淶��û�����ɱ�׼��SQL��䣡");

			return strBInsertSqlStr.ToString();
		}
		/// <summary>
		/// ����xml�ڵ����xmlTableNode�������в��������ֵ�����ɱ�׼��INSERT SQL���������Ƕ����"��"��
		/// ����Insert SQL��䣩
		/// </summary>
		/// <param name="xmlTableNode">���ϲ���XML��׼��ʽ��XML�ĵ��Ľڵ�</param>
		/// <param name="xsdColumnDT">�����ֵ�</param>
		/// <returns>���ر�׼��INSERT SQL���������Ƕ����"��"�ָ���Insert SQL��䣩</returns>
		/// <remarks>
		/// ����xml�ڵ����xmlTableNode�������в��������ֵ�����ɱ�׼��INSERT SQL���������Ƕ����"��"��
		/// ����Insert SQL��䣩��
		/// </remarks>
		/// <example>
		///	&lt;Insert&gt;
		///		&lt;INFO_FILE&gt;
		///			&lt;SET&gt;
		///				&lt;TITLE&gt;title&lt;/TITLE&gt;
		///				&lt;CONTENT&gt;content&lt;/CONTENT&gt;
		///				&lt;SEND_DATE&gt;2003-1-3&lt;/SEND_DATE&gt;
		///			&lt;/SET&gt;
		///		&lt;/INFO_FILE&gt;
		///		&lt;INFO_FILE&gt;
		///			&lt;SET&gt;
		///				&lt;TITLE&gt;title1&lt;/TITLE&gt;
		///				&lt;CONTENT&gt;title2&lt;/CONTENT&gt;
		///				&lt;SEND_DATE&gt;2003-1-2&lt;/SEND_DATE&gt;
		///			&lt;/SET&gt;
		///		&lt;/INFO_FILE&gt;
		///	&lt;/Insert&gt;
		/// </example>
		public static string GetInsertSqlStr(XmlNode xmlTableNode, XmlDocument xsdColumnDT)
		{
			StringBuilder strB = new StringBuilder(1024);

			AppendStrBInsert(strB, xmlTableNode, xsdColumnDT);

			return strB.ToString();
		}
		/// <summary>
		/// ����xmlTableRowValue�е����ݺ������ֵ����ɱ�׼UPDATE SQL���ݿ������ַ���
		/// </summary>
		/// <param name="xmlTableRowValue">����Update XML��׼��ʽ��XML�ĵ�����</param>
		/// <param name="xsdColumnDT">�����ֵ䣨XSD�ļ���Ӧ��XML�ĵ�����</param>
		/// <returns>��׼UPDATA SQL��</returns>
		/// <remarks>
		/// ����xmlTableRowValue�е����ݺ������ֵ����ɱ�׼UPDATE SQL���ݿ������ַ����������ֵ����XSD�ļ���Ӧ��xml�ĵ��������ݣ���
		/// ���ַ������԰����ж��Update���Ĳ�����
		/// </remarks>
		/// <example>
		/// <code> 
		///	&lt;Update&gt;
		///		&lt;INFO_FILE&gt;
		///			&lt;SET&gt;
		///				&lt;TITLE&gt;������&lt;/TITLE&gt;
		///				&lt;CONTENT&gt;����������Ⱦ&lt;/CONTENT&gt;
		///				&lt;SEND_DATE type="datetime" &gt;2003-1-3&lt;/SEND_DATE&gt;
		///			&lt;/SET&gt;
		///			&lt;WHERE&gt;
		///				&lt;GUID operator="="&gt;100001&lt;/GUID&gt;
		///			&lt;/WHERE&gt;
		///		&lt;/INFO_FILE&gt;
		///		&lt;INFO_FILE&gt;
		///			&lt;SET&gt;
		///				&lt;TITLE&gt;yyy&lt;/TITLE&gt;
		///				&lt;CONTENT&gt;yyyyyyy&lt;/CONTENT&gt;
		///				&lt;SEND_DATE type="datetime" &gt;2002-1-3&lt;/SEND_DATE&gt;
		///			&lt;/SET&gt;
		///			&lt;WHERE&gt;
		///				&lt;GUID operator="="&gt;100002&lt;/GUID&gt;
		///			&lt;/WHERE&gt;
		///		&lt;/INFO_FILE&gt;
		///	&lt;/Update&gt;
		/// </code>
		/// �����	Update INFO_FILE set TITLE='������', CONTENT='����������Ⱦ', SEND_DATE='2003-1-3' WHERE GUID='100001';
		///			Update INFO_FILE set TITLE='yyy', CONTENT='yyyyyyy', SEND_DATE='2002-1-3' WHERE GUID='100002';
		/// </example>
		public static string GetUpdateSqlStr(XmlDocument xmlTableRowValue, XmlDocument xsdColumnDT)
		{
			XmlNode rootNode = xmlTableRowValue.DocumentElement;
			StringBuilder strBReturn = new StringBuilder();
			XmlNode xmlTableNode = rootNode.FirstChild;

			while (xmlTableNode != null)
			{
				string strOneSQL = GetOneUpdateStr(xmlTableNode, xsdColumnDT);

				if (strOneSQL != string.Empty)
				{
					if (strBReturn.Length > 0)
						strBReturn.Append(";");

					strBReturn.Append(strOneSQL);
				}

				xmlTableNode = xmlTableNode.NextSibling;
			}

			return strBReturn.ToString();
		}
		/// <summary>
		/// �Ѱ�����һ��������INSERT���ݵ�XML�ڵ����xmlTableNode�е������γɶ�Ӧ�����ݿ�����SQL���
		/// </summary>
		/// <param name="strBInsertSqlStr">���ڱ������ݵ�StringBuilder����</param>
		/// <param name="xmlTableNode">���ݿ����ݶ�Ӧ��xml�ڵ�</param>
		/// <param name="xsdColumnDT">�����ֵ䣨��Ӧ�����ݿ��XSD�ļ���XML�ĵ�����</param>
		/// <remarks>
		/// �����ֵ�xsdColumnDT���������ݿ���еĽṹ�����߸������ֵ��xml�ڵ�����е������γ�һ�����������xmlTableNode
		/// �Ľڵ�����������SQL�����INSERT��䡣������SQL�����ַ������ݱ�����strBInsertSqlStr�С�
		/// </remarks>
		private static void AppendStrBInsert(StringBuilder strBInsertSqlStr, 
			XmlNode xmlTableNode, 
			XmlDocument xsdColumnDT)
		{
			string strTableName = xmlTableNode.Name;

			int nRow = 0;

			//��ʼѭ��������InsertSqlStr�����������Ƿ���strBInsertSqlStr�ڴ�ռ���
			while (xmlTableNode != null)
			{
				//���ú���������һ��INSERT�ı�׼SQL��
				GetOneInsertSqlStr(nRow, xmlTableNode.SelectSingleNode("SET"), strBInsertSqlStr, xsdColumnDT, strTableName);

				nRow++;
				xmlTableNode = xmlTableNode.NextSibling;
			}
		}
		/// <summary>
		/// �õ�һ��INSERT�Ӿ䣬���Ҵ����StringBuilder��
		/// </summary>
		/// <param name="nRow">��n��������SQL���</param>
		/// <param name="xmlRowNode">������INSERT���ݵ�xml���ݽڵ�</param>
		/// <param name="strBInsertSqlStr">���ڴ��InsertSqlStr���ڴ�ռ�</param>
		/// <param name="xsdColumnDT">�����ֵ䣨��Ӧ�����ݿ��XSD�ļ���XML�ĵ�����</param>
		/// <param name="strTableName">Ҫ�������ݵ����ݱ�����</param>
		/// <remarks>
		/// ���ݲ����а����в�������ֵ�ļ��Ͻڵ�xmlRowNode��˵����������ֵ�����γɲ��뵽���ݱ�strTableNameָ�������ݱ��е�SQL��䣬
		/// ������ݴ洢���ڴ�strBInsertSqlStr�У�ͬʱΪ�˶��SQL�������γɿ��Դ������nRowָ��Ҫ�γ�SQL�ĵڼ���SQL�����Ϊ���SQL
		/// ֮����Ҫ��һ���ķָ���������Ĭ����SQL SERVER�ġ�������
		/// </remarks>
		private static void GetOneInsertSqlStr(int nRow, 
			XmlNode xmlRowNode, 
			StringBuilder strBInsertSqlStr,
			XmlDocument xsdColumnDT, 
			string strTableName)
		{
			int nColumns = 0;
			StringBuilder strBColumnsSql = new StringBuilder(1024);
			StringBuilder strBValuesSql = new StringBuilder(1024);

			//******��ʼ����һ��INSERT�Ӵ�******

			foreach (XmlNode xmlColumnNode in xmlRowNode.ChildNodes)
			{
				string strColumnName = xmlColumnNode.Name;
				string strColumnValue = xmlColumnNode.InnerText;

				if (strColumnValue != string.Empty)
				{
					//��������Column�Ӵ��ķ���
					CombineColumns(nColumns, strBColumnsSql, strColumnName);

					//����GetColumnType��������Ӷ�����
					string strColumnType = GetColumnType(xmlColumnNode, xsdColumnDT);

					//��������Values�Ӵ��ķ���
					CombineValues(nColumns, strBValuesSql, strColumnValue, strColumnType);

					nColumns++;
				}
			}

			string strTemp = string.Empty;
			strTemp = "INSERT INTO " + strTableName + " (" + strBColumnsSql.ToString() + ")"
				+ " VALUES (" + strBValuesSql.ToString() + ")";

			if (nRow > 0)
				strBInsertSqlStr.Append(" ; ");

			strBInsertSqlStr.Append(strTemp);

			//******һ��INSERT�Ӵ��������******
		}
		/// <summary>
		/// ��������Insert�ı�׼SQL����Column�Ӵ�
		/// </summary>
		/// <param name="nColumn">��־Ϊ��n�е�����λ��</param>
		/// <param name="strBColumnsSql">���ڴ�����ɵ�Column�Ӵ��Ŀռ�</param>
		/// <param name="strColumnName">SQL��ָ�����ֶ�����</param>
		/// <remarks>
		/// ��������Insert�ı�׼SQL����Column�Ӵ������Ӵ��е��������ڶ�Ӧ���ݿ���DataTable���ֶ����ƣ����ݸ����ݱ��
		/// �ֶ������Ա������ݵĲ��������ʵ�֡�����strBColumnsSql�ǰ����˸��Ӵ����ݵ��ַ���
		/// </remarks>
		private static void CombineColumns(int nColumn, StringBuilder strBColumnsSql, string strColumnName)
		{
			if (nColumn > 0)
				strBColumnsSql.Append(", ");

			strBColumnsSql.Append(strColumnName);
		}
		/// <summary>
		/// �˺�����������Insert�ı�׼SQL����Values�Ӵ��ķ���
		/// </summary>
		/// <param name="nColumn">��n���ֶ�</param>
		/// <param name="strBValueSql">���ڴ��Value�Ӵ����ڴ�ռ�</param>
		/// <param name="strColumnValue">��Ӧ�ֶε�ֵ</param>
		/// <param name="strColumnType">��Ӧ�ֶ�����</param>
		/// <remarks>
		/// �������ݵ�����ָ��ֵstrColumnType���������ֶ�ֵstrColumnValue��ӵ������Ӵ��еĵ�nColumn�������γ�INERT�����
		/// ��VALUES�Ӿ伴����ֵ���ۺϡ�
		/// </remarks>
		private static void CombineValues(int nColumn, StringBuilder strBValueSql, string strColumnValue, string strColumnType)
		{
			string strResult = string.Empty;

			strResult = GetColumnValueStr(strColumnValue, strColumnType);

			if (nColumn > 0)
				strBValueSql.Append(", ");

			strBValueSql.Append(strResult);

		}	
		/// <summary>
		/// ����xmlTableNode�е����ݺ������ֵ�Ķ�����һ�����ݿ�SQL����Update��
		/// </summary>
		/// <param name="xmlTableNode">���ݱ�ڵ�</param>
		/// <param name="xsdColumnDT">�����ֵ䣨��Ӧ����XSD�ļ���XML�ĵ�����</param>
		/// <returns>����һ��UPDATE�ı�׼SQL�Ӵ�</returns>
		/// <remarks>
		/// ����xmlTableNode�е����ݺ������ֵ�Ķ�����һ�����ݿ�SQL����Update��
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;TableSet type="update"&gt;
		///		&lt;Update&gt;
		///			&lt;INFO_FILE&gt;
		///				&lt;SET&gt;
		///					&lt;TITLE&gt;������&lt;/TITLE&gt;
		///					&lt;CONTENT&gt;����������Ⱦ&lt;/CONTENT&gt;
		///					&lt;SEND_DATE&gt;2003-1-3&lt;/SEND_DATE&gt;
		///				&lt;/SET&gt;
		///				&lt;WHERE&gt;
		///					&lt;GUID operator="="&gt;100001&lt;/GUID&gt;
		///				&lt;/WHERE&gt;
		///			&lt;/INFO_FILE&gt;
		///		&lt;/Update&gt;
		///	&lt;/TableSet&gt;
		/// </code>
		/// ������ݣ���UPDATE INFO_FILE SET TITLE='������', CONTENT='����������Ⱦ', SEND_DATE='2003-1-3' WHERE GUID='100001'��
		/// </example>
		private static string GetOneUpdateStr(XmlNode xmlTableNode, XmlDocument xsdColumnDT)
		{
			StringBuilder strBSet = new StringBuilder(1024);
			StringBuilder strBWhere = new StringBuilder(1024);

			string strTableName = xmlTableNode.Name;

			//���� SET �Ӵ�
			GetSetStr(strBSet, xmlTableNode, xsdColumnDT);
			//����Where�Ӵ�
			GetWhereStr(strBWhere, xmlTableNode, xsdColumnDT, 0);

			string strSQL = string.Empty;

			//�ϳ�һ����׼��UPDATE��
			if (strBSet.Length > 0)
			{
				strSQL = string.Format("UPDATE {0} SET {1}", strTableName, strBSet.ToString());

				string strWhere = string.Empty;

				if (strBWhere.Length > 0)
					strSQL += " WHERE " + strBWhere.ToString();
			}

			return strSQL;
		}
		/// <summary>
		/// ����xml�ڵ����xmlTableNode��������ֵ�xsdColumnDT���� Update SQL ���� SET �Ӿ�ķ���
		/// </summary>
		/// <param name="strBSet">���SET�Ӿ��StringBuilder</param>
		/// <param name="xmlTableNode">���ݱ�ڵ�</param>
		/// <param name="xsdColumnDT">���ݱ��Ӧ�������ֵ�</param>
		/// <remarks>
		/// ����xml�ڵ����xmlTableNode��������ֵ�xsdColumnDT���� Update SQL ���� SET �Ӿ�ķ����������������
		/// strBSetָ�����ڴ�ռ��С�
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;TableSet type="update"&gt;
		///		&lt;Update&gt;
		///			&lt;INFO_FILE&gt;
		///				&lt;SET&gt;
		///					&lt;TITLE&gt;������&lt;/TITLE&gt;
		///					&lt;CONTENT&gt;����������Ⱦ&lt;/CONTENT&gt;
		///					&lt;SEND_DATE&gt;2003-1-3&lt;/SEND_DATE&gt;
		///				&lt;/SET&gt;
		///				&lt;WHERE&gt;
		///					&lt;GUID operator="="&gt;100001&lt;/GUID&gt;
		///				&lt;/WHERE&gt;
		///			&lt;/INFO_FILE&gt;
		///		&lt;/Update&gt;
		///	&lt;/TableSet&gt;
		/// </code>
		/// �����ݣ�"TITLE='������', CONTENT='����������Ⱦ', SEND_DATE='2003-1-3'"
		/// </example>
		private static void GetSetStr(StringBuilder strBSet, XmlNode xmlTableNode, XmlDocument xsdColumnDT)
		{
			XmlNode xmlSetNode = xmlTableNode.SelectSingleNode("SET");

			XmlNode xmlColumnNode = xmlSetNode.FirstChild;

			bool bFirstColumn = true;

			while (xmlColumnNode != null)
			{
				string strColumnValue = xmlColumnNode.InnerText;

				string strColumnName = xmlColumnNode.Name;

				string strColumnType = GetColumnType(xmlColumnNode, xsdColumnDT);

				if (bFirstColumn == false)
					strBSet.Append(", ");
				else
					bFirstColumn = false;

				if (strColumnValue == string.Empty)
					strBSet.Append(strColumnName + " = null");
				else
				{
					string strGetResult = GetColumnValueStr(strColumnValue, strColumnType);
					strBSet.Append(strColumnName + " = " + strGetResult);
				}

				xmlColumnNode = xmlColumnNode.NextSibling;
			}
		}
		/// <summary>
		/// ��ʽ�������е�ֵ���Ը����ݵĲ�ͬ���ͣ�����������һ���ַ��������в�ͬ�Ĵ���
		/// </summary>
		/// <param name="strColumnValue">��ֵ����Ӧ���ݿ��е��ֶε�ֵ��</param>
		/// <param name="strColumnType">��Ӧ�ֶε���������</param>
		/// <returns>�������ݸ�ʽ�������˶��е����ŵ��ַ���</returns>
		/// <remarks>
		/// ��һ���ĸ�ʽ��ʽ������strColumnValueָ����ֵ�������ݵ�������strColumnTypeȷ�������strColumnTypeָ����������
		/// "xsd:dateTime"�����ݽ����ʽ�����£�YYYY-MM-DD hh:mm:ss��ʽ��Ϊ'YYYYMMDD hh:mm:ss'�����strColumnTypeָ������
		/// �Ͳ���"xsd:string"�����ݾͽ�����Ҫ�����е�'ת��Ϊ''��ͬʱ�����ݵ�ͷβ������һ��'�������Ϳ���Ϊ���ݵ��������
		/// ���ݵ�׼���ˡ�
		/// </remarks>
		private static string GetColumnValueStr(string strColumnValue, string strColumnType)
		{
			string strValue = strColumnValue;
			switch (strColumnType)
			{
				case "xs:dateTime":
					{
						/*string[] arrayDateAndTime = new String[2];
						arrayDateAndTime = strValue.Split(' ');
						string strDate = arrayDateAndTime[0];
						string strTime = string.Empty ;

						if (arrayDateAndTime.Length > 1)
							strTime = arrayDateAndTime[1];

						string[] arrayDate = new string[3];
						arrayDate = strDate.Split('-'); 
				
						if(arrayDate[1].Length == 1) 
							arrayDate[1] = "0" + arrayDate[1];

						if(arrayDate[2].Length == 1) 
							arrayDate[2] = "0" + arrayDate[2];

						strValue = arrayDate[0] + arrayDate[1] + arrayDate[2];

						if(strTime != string.Empty)
							strValue = strValue + " " + strTime;

						strValue = AddQuotationMark(strValue);*/
						strValue = GetStandFormatDateTimeStr(strColumnValue);
						break;
					}
				case "xs:string":
					{
						strValue = TSqlBuilder.Instance.CheckQuotationMark(strValue, true);
						break;
					}
			}

			return strValue;
		}
		/// <summary>
		/// �������ֵ��л���ֶΣ��У���type���ֶ����ͣ�ֵ
		/// </summary>
		/// <param name="xmlColumnNode">XML�ĵ��е�һ���нڵ�</param>
		/// <param name="xsdColumnDT">�����ֵ��Ӧ��xml�ĵ�����</param>
		/// <returns>����type����Ӧ�ֶε����ͣ���ֵ</returns>
		/// <remarks>
		/// �ú��������жϽڵ�xmlColumnNode���Ƿ���type�����ԣ�������򷵻ظ�����ֵ������ͷ��ش������ֵ�xsdColumnDT
		/// �ж�Ӧ�ýڵ����������ֵ����������᷵�ض�Ӧ�ڵ����ݵ���������ֵ��
		/// </remarks>
		/// <example>
		/// string��int��dateTime������
		/// </example>
		private static string GetColumnType(XmlNode xmlColumnNode, XmlDocument xsdColumnDT)
		{
			string strColumnType = ((XmlElement)xmlColumnNode).GetAttribute("type");

			if (strColumnType == string.Empty)
				strColumnType = GetXSDColumnAttrDirect(xsdColumnDT, xmlColumnNode.Name, "type");

			return strColumnType;
		}
		/// <summary>
		/// �Ѳ���strValue�е�'-'ȥ�������Ѹ�ʽΪ"YYYY-MM-DD hh:mm:ss"��ʾ��DateTime��ʽ����"YYYYMMDD hh:mm:ss"��ʾ
		/// </summary>
		/// <param name="strValue">��ʾ���ڵ��ַ�������ʽΪYYYY-MM-DD hh:mm:ss</param>
		/// <returns>����YYYYMMDD hh:mm:ss</returns>
		/// <remarks>
		/// �Ѳ���strValue�е�'-'ȥ�������Ѹ�ʽΪ"YYYY-MM-DD hh:mm:ss"��ʾ��DateTime��ʽ����"YYYYMMDD hh:mm:ss"��ʾ��
		/// ���strValue�в�������ʱ��ı�ʾֵ���Ͱ����ݸ�ʽ��YYYY-MM-DD��ת���ɡ�YYYYMMDD��
		/// </remarks>
		private static string GetStandFormatDateTimeStr(string strValue)
		{
			string[] arrayDateAndTime = strValue.Split(' ');

			string strDate = string.Empty;
			string strTime = string.Empty;

			if (arrayDateAndTime.Length > 1)
			{
				strDate = arrayDateAndTime[0];
				strTime = arrayDateAndTime[1];

				strDate = GetStandFormatDateStr(strDate);
			}
			else
				strDate = GetStandFormatDateStr(strValue);

			return TSqlBuilder.Instance.CheckQuotationMark(strDate + " " + strTime, true);
		}
		/// <summary>
		/// ȥ����ʾ���ڻ�ʱ��ֵ���ַ���strDate�е�'-'���γ�һ����6���ַ���ɵ����ڻ�ʱ��ı�ʾ�ַ���
		/// </summary>
		/// <param name="strDate">����'-'��ʾ�����ڻ�ʱ���ַ���</param>
		/// <returns>����'-'��ʾ��6λ��ʾʱ������ڵ��ַ����������ַ�����SQL Server�пɱ�����ΪDateTime��ʾ��</returns>
		/// <remarks>
		/// ȥ����ʾ���ڻ�ʱ��ֵ���ַ���strDate�е�'-'���γ�һ����6���ַ���ɵ����ڻ�ʱ��ı�ʾ�ַ��������ڳ����ϵ�
		/// ֱ�۱�ʾ
		/// </remarks>
		private static string GetStandFormatDateStr(string strDate)
		{
			string[] arrayDate = strDate.Split('-');

			string strReturn = arrayDate[0];

			for (int i = 1; i < 3; i++)
			{
				if (arrayDate[i].Length < 2)
					strReturn += "0" + arrayDate[i];
				else
					strReturn += arrayDate[i];
			}

			return strReturn;
		}
		/// <summary>
		/// ���ݲ�������xml�ڵ����xmlTableNode��Ӧ����������SQL���Where�Ӿ�
		/// </summary>
		/// <param name="strBWhere">���WHERE�Ӿ��StringBuilder</param>
		/// <param name="xmlTableNode">����WHERE���ݵı�ڵ�</param>
		/// <param name="xsdColumnDT">�����ֵ䣨XSD�ļ���Ӧ��xml�ĵ�����</param>
		/// <param name="nTable">��n����ڵ�</param>
		/// <remarks>
		/// ���ݲ�������xml�ڵ����xmlTableNode��Ӧ����������SQL���Where�Ӿ�
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;INFO_FILE&gt;
		///		&lt;DisplayNode&gt;
		///			&lt;SORTID/&gt;
		///			&lt;TITLE&gt;TITLENAME&lt;/TITLE&gt;
		///		&lt;/DisplayNode&gt;
		///		&lt;WHERE&gt;
		///			&lt;CONDITIONS&gt;
		///				&lt;TITLE op="like"&gt;%yyyy%&lt;/GUID&gt;
		///				&lt;TYPE op="="&gt;INFO&lt;/TYPE&gt;
		///				&lt;TYPEID&gt;5698&lt;/TYPEID&gt;
		///			&lt;/CONDITIONS&gt;
		///			&lt;CONDITIONs&gt;
		///				&lt;TITLE op="like"&gt;%tian%&lt;/TITLE&gt;
		///				&lt;TYPEID&gt;7856&lt;/TYPEID&gt;
		///			&lt;/CONDITIONs&gt;
		///		&lt;/WHERE&gt;
		/// &lt;/INFO_FILE&gt;
		/// </code>
		/// �����(TITLE like '%yyyy%' AND TYPE='INFO' AND TYPEID=5698) OR (TITLE like 'tian' AND TYPEID=7856)
		/// </example>
		private static void GetWhereStr(StringBuilder strBWhere, 
			XmlNode xmlTableNode,
			XmlDocument xsdColumnDT, 
			int nTable)
		{
			int nLen = strBWhere.Length;
			string strTableName = xmlTableNode.Name;

			foreach (XmlNode xmlWhereNode in xmlTableNode.SelectNodes("WHERE"))
			{
				XmlNode xmlFirstColumnNode = xmlWhereNode.FirstChild;

				if (xmlFirstColumnNode != null)
				{
					//if (nTable > 0 && strBWhere.Length > 0 )
					if (strBWhere.Length > nLen)
						strBWhere.Append(" OR ");

					int nColumn = 0;
					foreach (XmlNode xmlColumnNode in xmlWhereNode.ChildNodes)
					{
						string strColumnName = xmlColumnNode.Name;
						string strColumnValue = xmlColumnNode.InnerText;
						string strColumnType = GetColumnType(xmlColumnNode, xsdColumnDT);

						string strOperator = "=";
						XmlAttribute xmlAttr = xmlColumnNode.Attributes["operator"];

						if (xmlAttr != null)
							strOperator = xmlAttr.Value;

						//��������Where��Column�Ӵ��Ĺ���
						GetWhereColumnStr(nColumn, strBWhere, strColumnName, strColumnValue, strColumnType, strTableName, strOperator);

						nColumn++;
					}

				}
			}

			if (strBWhere.Length > nLen)
			{
				strBWhere.Insert(nLen, "(");
				strBWhere.Append(")");
			}
		}
		/// <summary>
		/// ���ݸ����������ֶ����������ֶ�ֵ����where����е�һ�������ж��Ӿ�
		/// </summary>
		/// <param name="nColumn">��nColumn��</param>
		/// <param name="strBWhere">���Where�Ӿ��Column�Ӵ�</param>
		/// <param name="strColumnName">����������</param>
		/// <param name="strColumnValue">������ֵ</param>
		/// <param name="strColumnType">�����е�����</param>
		/// <param name="strTableName">���ݱ�����</param>
		/// <param name="strOperator">���ݲ�����</param>
		/// <remarks>
		/// ���ݸ����������ֶ����������ֶ�ֵ����where����е�һ�������ж��Ӿ䡣
		/// </remarks>
		/// <example>
		/// ��ã� Title like '%titleName%'
		/// </example>
		private static void GetWhereColumnStr(int nColumn, 
			StringBuilder strBWhere, 
			string strColumnName,
			string strColumnValue, 
			string strColumnType, 
			string strTableName, 
			string strOperator)
		{
			string strGetResult = strColumnValue;
			string strTableColumn = strTableName + "." + strColumnName;

			if (strOperator.ToUpper() == "IS")
				strGetResult = "NULL";
			else
				strGetResult = GetColumnValueStr(strColumnValue, strColumnType);

			if (nColumn > 0)
				strBWhere.Append(" AND ");

			strBWhere.Append(strTableColumn);
			strBWhere.Append(" ");
			strBWhere.Append(strOperator);
			strBWhere.Append(" ");
			strBWhere.Append(strGetResult);
		}
		#endregion
	}
}
