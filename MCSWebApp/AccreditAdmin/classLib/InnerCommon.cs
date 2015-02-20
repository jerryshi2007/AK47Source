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
		/// 自动完成SQL的执行过程
		/// </summary>
		/// <param name="strSql">要求执行的数据查询SQL</param>
		/// <returns>本次操作所影响的数据条数</returns>
		internal static int ExecuteNonQuery(string strSql)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteNonQueryWithoutTransaction--strSql");
			using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
#if DEBUG
				Debug.WriteLine(strSql);
#endif
				ExceptionHelper.TrueThrow(string.IsNullOrEmpty(strSql), "数据处理语句SQL不能为空串！");

				return database.ExecuteNonQuery(CommandType.Text, strSql);
			}
		}
		/// <summary>
		/// 自动完成SQL的执行过程
		/// </summary>
		/// <param name="strSql">要求执行的数据查询SQL</param>
		/// <param name="strTables">要求配置的数据表名称</param>
		/// <returns>本次查询的数据结果集</returns>
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
		/// 自动完成SQL的执行过程
		/// </summary>
		/// <param name="strSql">要求执行的数据查询SQL</param>
		/// <returns>本次查询的结果对象</returns>
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
		/// 把数据集合DataSet中的所有数据转换成XML文档对象返回，解决字段中的NULL值在XML文档中不显示问题
		/// </summary>
		/// <param name="dataSet">要被转换的标准DataSet数据</param>
		/// <returns>符合指定XML格式的XML文档对象</returns>
		/// <remarks>
		/// 把数据集合DataSet中的所有数据转换成XML文档对象返回，解决字段中的NULL值在XML文档中不显示问题。
		/// 最后返回的XML文档对象中包括了所有DataSet中的所有数据，如果该DataSet中包含有多个DataTable，
		/// 返回的结果中会把所有这些数据都包括在内。（即允许多个DataTable在其中）
		/// </remarks>
		/// <example>
		/// 返回值：
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
		///		&lt;table1&gt;	////多个DataTable情况
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
		/// 把数据集合DataSet中的所有数据转换成XML文档对象返回，解决字段中的NULL值在XML文档中不显示问题
		/// </summary>
		/// <param name="dataSet">要被转换的标准DataSet数据</param>
		/// <param name="bCDataSection">返回时每个字段的值是否由CData括起来</param>
		/// <returns>符合指定XML格式的XML文档对象</returns>
		/// <remarks>
		/// 把数据集合DataSet中的所有数据转换成XML文档对象返回，解决字段中的NULL值在XML文档中不显示问题。
		/// 最后返回的XML文档对象中包括了所有DataSet中的所有数据，如果该DataSet中包含有多个DataTable，
		/// 返回的结果中会把所有这些数据都包括在内。（即允许多个DataTable在其中）
		/// </remarks>
		/// <example>
		/// 返回值：
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
		/// 把table中各个对象的字段设置送入XML对应的属性数据中
		/// </summary>
		/// <param name="table">包含所有数据对象的数据表</param>
		/// <param name="strNameCol">对象的类型对应字段名成（对应节点的指定的对应名称的字段）</param>
		/// <returns>把table中各个对象的字段设置送入XML对应的属性数据中</returns>
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
		/// 把xml节点上的数据值转换到新XML属性上
		/// </summary>
		/// <param name="xmlSource">带有节点数据的xml对象</param>
		/// <returns>转化为属性值的XML数据对象</returns>
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
		/// 获得xmlDoc指定的Xml文档对象中的NameSpaceManager对象
		/// </summary>
		/// <param name="xmlDoc">被处理的XML文档对象（一般是XSD文件内容）</param>
		/// <returns>此XML的NameSpceManage对象</returns>
		/// <remarks>
		/// 获得xmlDoc指定的Xml文档对象中的NameSpaceManager对象，本函数一般用于对于一些特殊形式的
		/// XML文档对象作处理。例如
		/// <code>
		/// &lt;DOC:INFO&gt;
		/// &lt;GUI:NODE name="hello"&gt;特殊化&lt;/GUI:NODE&gt;
		/// &lt;/DOC:INFO&gt;
		/// </code>
		/// 一类的XML文档对象。
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
		/// 从指定的XML文档对象（xmlDoc）中获得一个strColumnName指定的节点
		/// </summary>
		/// <param name="xmlDoc">指定的XML文档对象（这里一般是数据字典）</param>
		/// <param name="strColumnName">指定的节点名称（这里常用作为数据字典中的列名称）</param>
		/// <returns>XML文档对象中的指定节点</returns>
		/// <remarks>
		/// 从指定的XML文档对象（xmlDoc）中获得一个strColumnName指定的节点。这里主要用于对数据字典XSD文档的处理，
		/// 获得数据字典中的由strColumnName指定的数据节点（一般是数据字典中的列节点）
		/// </remarks>
		public static XmlNode GetXSDColumnNode(XmlDocument xmlDoc, string strColumnName)
		{
			return xmlDoc.SelectSingleNode(
				".//xs:sequence/xs:element[@name = \"" + strColumnName + "\"]",
				GetNSMgr(xmlDoc));
		}

		/// <summary>
		/// 在XML文档对象xmlDoc中获取在strColumnName指定节点下strAttr指定的属性值
		/// </summary>
		/// <param name="xmlDoc">包含数据的XML文档对象（数据字典的对应XML文档对象）</param>
		/// <param name="strColumnName">XML文档对象中strColumnName指定的节点名称（数据字典中的列名称）</param>
		/// <param name="strAttr">节点属性名称</param>
		/// <returns>XML文档对象指定节点中指定属性名对应的属性值</returns>
		/// <remarks>
		/// 在XML文档对象xmlDoc中获取在strColumnName指定节点下strAttr指定的属性值，这里主要应用于数据字典文件中
		/// 的节点对象的属性获得。
		/// </remarks>
		private static string GetXSDColumnAttrDirect(XmlDocument xmlDoc, string strColumnName, string strAttr)
		{
			XmlNode xmlnodeColumn = GetXSDColumnNode(xmlDoc, strColumnName);

			ExceptionHelper.TrueThrow(xmlnodeColumn == null, "在数据字典中未找到列" + strColumnName);

			return GetXSDColumnAttr(xmlnodeColumn, strAttr);
		}

		/// <summary>
		/// 获得指定列节点（数据表的列）nodeColumn的strAttr指定属性值
		/// </summary>
		/// <param name="nodeColumn">包含数据的列节点</param>
		/// <param name="strAttr">要获得数据的属性名称</param>
		/// <returns>列节点（数据表的列）的指定属性值</returns>
		/// <remarks>
		/// 获得指定列节点（数据表的列）nodeColumn中strAttr指定属性值，采用string的方式返回结果
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
		/// 主函数：根据数据的xml文档对象和对应数据库的数据字典生成标准的INSERT SQL 串
		/// </summary>
		/// <param name="xmlTableRowValue">符合插入XML标准格式的XML文档</param>
		/// <param name="xsdColumnDT">数据字典</param>
		/// <returns>标准的INSERT SQL串</returns>
		/// <remarks>
		/// 根据数据的xml文档对象和对应数据库的数据字典生成标准的INSERT SQL 串。如果组合成标准的SQL Insert语句不成功
		/// 将会产生异常报告。（这里我们的XML文档对象中可以形成多个insert数据库命令语句）
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

			ExceptionHelper.TrueThrow((strBInsertSqlStr.Length == 0), "送入的xml文档对象不符合规范，没有生成标准的SQL语句！");

			return strBInsertSqlStr.ToString();
		}
		/// <summary>
		/// 根据xml节点对象xmlTableNode（包含有插入的数据值）生成标准的INSERT SQL串（可以是多个用"；"分
		/// 隔的Insert SQL语句）
		/// </summary>
		/// <param name="xmlTableNode">符合插入XML标准格式的XML文档的节点</param>
		/// <param name="xsdColumnDT">数据字典</param>
		/// <returns>返回标准的INSERT SQL串（可以是多个用"；"分隔的Insert SQL语句）</returns>
		/// <remarks>
		/// 根据xml节点对象xmlTableNode（包含有插入的数据值）生成标准的INSERT SQL串（可以是多个用"；"分
		/// 隔的Insert SQL语句）。
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
		/// 根据xmlTableRowValue中的数据和数据字典生成标准UPDATE SQL数据库命令字符串
		/// </summary>
		/// <param name="xmlTableRowValue">符合Update XML标准格式的XML文档对象</param>
		/// <param name="xsdColumnDT">数据字典（XSD文件对应的XML文档对象）</param>
		/// <returns>标准UPDATA SQL串</returns>
		/// <remarks>
		/// 根据xmlTableRowValue中的数据和数据字典生成标准UPDATE SQL数据库命令字符串（数据字典采用XSD文件对应的xml文档对象数据）。
		/// 该字符串可以包含有多个Update语句的操作。
		/// </remarks>
		/// <example>
		/// <code> 
		///	&lt;Update&gt;
		///		&lt;INFO_FILE&gt;
		///			&lt;SET&gt;
		///				&lt;TITLE&gt;萨达萨&lt;/TITLE&gt;
		///				&lt;CONTENT&gt;萨法萨菲污染&lt;/CONTENT&gt;
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
		/// 结果：	Update INFO_FILE set TITLE='萨达萨', CONTENT='萨法萨菲污染', SEND_DATE='2003-1-3' WHERE GUID='100001';
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
		/// 把包含有一个或多个的INSERT数据的XML节点对象xmlTableNode中的数据形成对应的数据库命令SQL语句
		/// </summary>
		/// <param name="strBInsertSqlStr">用于保存数据的StringBuilder对象</param>
		/// <param name="xmlTableNode">数据库数据对应的xml节点</param>
		/// <param name="xsdColumnDT">数据字典（对应于数据库的XSD文件的XML文档对象）</param>
		/// <remarks>
		/// 数据字典xsdColumnDT包含了数据库表中的结构，各具该数据字典把xml节点对象中的数据形成一条或多条（由xmlTableNode
		/// 的节点数决定）的SQL命令的INSERT语句。产生的SQL命令字符串数据保存在strBInsertSqlStr中。
		/// </remarks>
		private static void AppendStrBInsert(StringBuilder strBInsertSqlStr, 
			XmlNode xmlTableNode, 
			XmlDocument xsdColumnDT)
		{
			string strTableName = xmlTableNode.Name;

			int nRow = 0;

			//开始循环，生成InsertSqlStr串，并把它们放在strBInsertSqlStr内存空间中
			while (xmlTableNode != null)
			{
				//调用函数，生成一个INSERT的标准SQL串
				GetOneInsertSqlStr(nRow, xmlTableNode.SelectSingleNode("SET"), strBInsertSqlStr, xsdColumnDT, strTableName);

				nRow++;
				xmlTableNode = xmlTableNode.NextSibling;
			}
		}
		/// <summary>
		/// 得到一个INSERT子句，并且存放在StringBuilder中
		/// </summary>
		/// <param name="nRow">第n个单独的SQL语句</param>
		/// <param name="xmlRowNode">包含有INSERT数据的xml数据节点</param>
		/// <param name="strBInsertSqlStr">用于存放InsertSqlStr的内存空间</param>
		/// <param name="xsdColumnDT">数据字典（对应于数据库的XSD文件的XML文档对象）</param>
		/// <param name="strTableName">要插入数据的数据表名称</param>
		/// <remarks>
		/// 根据参数中包含有插入数据值的集合节点xmlRowNode来说，配合数据字典对象形成插入到数据表strTableName指定的数据表中的SQL语句，
		/// 该语句暂存储在内存strBInsertSqlStr中，同时为了多个SQL的批量形成可以传入参数nRow指定要形成SQL的第几个SQL命令。因为多个SQL
		/// 之间需要有一定的分隔符，这里默认以SQL SERVER的“；”。
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

			//******开始生成一个INSERT子串******

			foreach (XmlNode xmlColumnNode in xmlRowNode.ChildNodes)
			{
				string strColumnName = xmlColumnNode.Name;
				string strColumnValue = xmlColumnNode.InnerText;

				if (strColumnValue != string.Empty)
				{
					//调用生成Column子串的方法
					CombineColumns(nColumns, strBColumnsSql, strColumnName);

					//调用GetColumnType函数获得子段类型
					string strColumnType = GetColumnType(xmlColumnNode, xsdColumnDT);

					//调用生成Values子串的方法
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

			//******一个INSERT子串生成完毕******
		}
		/// <summary>
		/// 用于生成Insert的标准SQL语句的Column子串
		/// </summary>
		/// <param name="nColumn">标志为第n列的数据位置</param>
		/// <param name="strBColumnsSql">用于存放生成的Column子串的空间</param>
		/// <param name="strColumnName">SQL中指定的字段名称</param>
		/// <remarks>
		/// 用于生成Insert的标准SQL语句的Column子串。该子串中的数据用于对应数据库中DataTable的字段名称，根据该数据表的
		/// 字段名称以便于数据的插入操作的实现。其中strBColumnsSql是包含了该子串数据的字符串
		/// </remarks>
		private static void CombineColumns(int nColumn, StringBuilder strBColumnsSql, string strColumnName)
		{
			if (nColumn > 0)
				strBColumnsSql.Append(", ");

			strBColumnsSql.Append(strColumnName);
		}
		/// <summary>
		/// 此函数用于生成Insert的标准SQL语句的Values子串的方法
		/// </summary>
		/// <param name="nColumn">第n个字段</param>
		/// <param name="strBValueSql">用于存放Value子串的内存空间</param>
		/// <param name="strColumnValue">对应字段的值</param>
		/// <param name="strColumnType">对应字段类型</param>
		/// <remarks>
		/// 根据数据的类型指定值strColumnType，把数据字段值strColumnValue添加到数据子串中的第nColumn列中以形成INERT语句中
		/// 的VALUES子句即数据值的综合。
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
		/// 根据xmlTableNode中的数据和数据字典的定义获得一个数据库SQL命令Update串
		/// </summary>
		/// <param name="xmlTableNode">数据表节点</param>
		/// <param name="xsdColumnDT">数据字典（对应数据XSD文件的XML文档对象）</param>
		/// <returns>返回一个UPDATE的标准SQL子串</returns>
		/// <remarks>
		/// 根据xmlTableNode中的数据和数据字典的定义获得一个数据库SQL命令Update串
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;TableSet type="update"&gt;
		///		&lt;Update&gt;
		///			&lt;INFO_FILE&gt;
		///				&lt;SET&gt;
		///					&lt;TITLE&gt;萨达萨&lt;/TITLE&gt;
		///					&lt;CONTENT&gt;萨法萨菲污染&lt;/CONTENT&gt;
		///					&lt;SEND_DATE&gt;2003-1-3&lt;/SEND_DATE&gt;
		///				&lt;/SET&gt;
		///				&lt;WHERE&gt;
		///					&lt;GUID operator="="&gt;100001&lt;/GUID&gt;
		///				&lt;/WHERE&gt;
		///			&lt;/INFO_FILE&gt;
		///		&lt;/Update&gt;
		///	&lt;/TableSet&gt;
		/// </code>
		/// 获得数据：“UPDATE INFO_FILE SET TITLE='萨达萨', CONTENT='萨法萨菲污染', SEND_DATE='2003-1-3' WHERE GUID='100001'”
		/// </example>
		private static string GetOneUpdateStr(XmlNode xmlTableNode, XmlDocument xsdColumnDT)
		{
			StringBuilder strBSet = new StringBuilder(1024);
			StringBuilder strBWhere = new StringBuilder(1024);

			string strTableName = xmlTableNode.Name;

			//生成 SET 子串
			GetSetStr(strBSet, xmlTableNode, xsdColumnDT);
			//生成Where子串
			GetWhereStr(strBWhere, xmlTableNode, xsdColumnDT, 0);

			string strSQL = string.Empty;

			//合成一个标准的UPDATE串
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
		/// 根据xml节点对象xmlTableNode配合数据字典xsdColumnDT生成 Update SQL 语句的 SET 子句的方法
		/// </summary>
		/// <param name="strBSet">存放SET子句的StringBuilder</param>
		/// <param name="xmlTableNode">数据表节点</param>
		/// <param name="xsdColumnDT">数据表对应的数据字典</param>
		/// <remarks>
		/// 根据xml节点对象xmlTableNode配合数据字典xsdColumnDT生成 Update SQL 语句的 SET 子句的方法。生成语句存放在
		/// strBSet指定的内存空间中。
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;TableSet type="update"&gt;
		///		&lt;Update&gt;
		///			&lt;INFO_FILE&gt;
		///				&lt;SET&gt;
		///					&lt;TITLE&gt;萨达萨&lt;/TITLE&gt;
		///					&lt;CONTENT&gt;萨法萨菲污染&lt;/CONTENT&gt;
		///					&lt;SEND_DATE&gt;2003-1-3&lt;/SEND_DATE&gt;
		///				&lt;/SET&gt;
		///				&lt;WHERE&gt;
		///					&lt;GUID operator="="&gt;100001&lt;/GUID&gt;
		///				&lt;/WHERE&gt;
		///			&lt;/INFO_FILE&gt;
		///		&lt;/Update&gt;
		///	&lt;/TableSet&gt;
		/// </code>
		/// 的数据："TITLE='萨达萨', CONTENT='萨法萨菲污染', SEND_DATE='2003-1-3'"
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
		/// 格式化数据列的值，对该数据的不同类型（日期类型与一般字符串类型有不同的处理）
		/// </summary>
		/// <param name="strColumnValue">列值（对应数据库中的字段的值）</param>
		/// <param name="strColumnType">对应字段的数据类型</param>
		/// <returns>返回数据格式化后两端都有单引号的字符串</returns>
		/// <remarks>
		/// 按一定的格式格式化数据strColumnValue指定的值，该数据的类型由strColumnType确定。如果strColumnType指定的类型是
		/// "xsd:dateTime"，数据将会格式化如下：YYYY-MM-DD hh:mm:ss格式化为'YYYYMMDD hh:mm:ss'；如果strColumnType指定的类
		/// 型不是"xsd:string"，数据就仅仅需要把其中的'转化为''，同时把数据的头尾各增加一个'。这样就可以为数据的入库做好
		/// 数据的准备了。
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
		/// 从数据字典中获得字段（列）的type（字段类型）值
		/// </summary>
		/// <param name="xmlColumnNode">XML文档中的一个列节点</param>
		/// <param name="xsdColumnDT">数据字典对应的xml文档对象</param>
		/// <returns>返回type（对应字段的类型）的值</returns>
		/// <remarks>
		/// 该函数首先判断节点xmlColumnNode中是否有type的属性，如果有则返回该属性值，否则就返回从数据字典xsdColumnDT
		/// 中对应该节点的数据类型值。最后函数将会返回对应节点数据的数据类型值。
		/// </remarks>
		/// <example>
		/// string、int、dateTime等数据
		/// </example>
		private static string GetColumnType(XmlNode xmlColumnNode, XmlDocument xsdColumnDT)
		{
			string strColumnType = ((XmlElement)xmlColumnNode).GetAttribute("type");

			if (strColumnType == string.Empty)
				strColumnType = GetXSDColumnAttrDirect(xsdColumnDT, xmlColumnNode.Name, "type");

			return strColumnType;
		}
		/// <summary>
		/// 把参数strValue中的'-'去除，即把格式为"YYYY-MM-DD hh:mm:ss"表示的DateTime格式化成"YYYYMMDD hh:mm:ss"表示
		/// </summary>
		/// <param name="strValue">表示日期的字符串，格式为YYYY-MM-DD hh:mm:ss</param>
		/// <returns>返回YYYYMMDD hh:mm:ss</returns>
		/// <remarks>
		/// 把参数strValue中的'-'去除，即把格式为"YYYY-MM-DD hh:mm:ss"表示的DateTime格式化成"YYYYMMDD hh:mm:ss"表示。
		/// 如果strValue中不包含有时间的表示值，就把数据格式“YYYY-MM-DD”转换成“YYYYMMDD”
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
		/// 去掉表示日期或时间值的字符串strDate中的'-'，形成一个由6个字符组成的日期或时间的表示字符串
		/// </summary>
		/// <param name="strDate">带有'-'表示的日期或时间字符串</param>
		/// <returns>不带'-'表示的6位表示时间或日期的字符串（这种字符串在SQL Server中可被人作为DateTime表示）</returns>
		/// <remarks>
		/// 去掉表示日期或时间值的字符串strDate中的'-'，形成一个由6个字符组成的日期或时间的表示字符串。便于程序上的
		/// 直观表示
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
		/// 根据参数给定xml节点对象xmlTableNode对应的数据生成SQL语句Where子句
		/// </summary>
		/// <param name="strBWhere">存放WHERE子句的StringBuilder</param>
		/// <param name="xmlTableNode">包含WHERE数据的表节点</param>
		/// <param name="xsdColumnDT">数据字典（XSD文件对应的xml文档对象）</param>
		/// <param name="nTable">第n个表节点</param>
		/// <remarks>
		/// 根据参数给定xml节点对象xmlTableNode对应的数据生成SQL语句Where子句
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
		/// 结果：(TITLE like '%yyyy%' AND TYPE='INFO' AND TYPEID=5698) OR (TITLE like 'tian' AND TYPEID=7856)
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

						//调用生成Where的Column子串的过程
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
		/// 根据给定的数据字段名和数据字段值生成where语句中的一个条件判断子句
		/// </summary>
		/// <param name="nColumn">第nColumn列</param>
		/// <param name="strBWhere">存放Where子句的Column子串</param>
		/// <param name="strColumnName">数据列名称</param>
		/// <param name="strColumnValue">数据列值</param>
		/// <param name="strColumnType">数据列的类型</param>
		/// <param name="strTableName">数据表名称</param>
		/// <param name="strOperator">数据操作符</param>
		/// <remarks>
		/// 根据给定的数据字段名和数据字段值生成where语句中的一个条件判断子句。
		/// </remarks>
		/// <example>
		/// 获得： Title like '%titleName%'
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
