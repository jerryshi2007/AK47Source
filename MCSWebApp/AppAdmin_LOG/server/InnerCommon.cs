using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using System.Xml;

using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Accredit.OguAdmin;

using MCS.Applications.AppAdmin_LOG.Properties;

namespace MCS.Applications.AppAdmin_LOG.server
{
	public class InnerCommon
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
			using (DbContext context = DbContext.GetContext(LogResource.ConnAlias))
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
			using (DbContext context = DbContext.GetContext(LogResource.ConnAlias))
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
			using (DbContext context = DbContext.GetContext(LogResource.ConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				return database.ExecuteScalar(CommandType.Text, strSql);
			}
		}
		#endregion


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
	}
}
