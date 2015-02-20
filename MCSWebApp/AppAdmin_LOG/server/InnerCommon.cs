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
		/// �Զ����SQL��ִ�й���
		/// </summary>
		/// <param name="strSql">Ҫ��ִ�е����ݲ�ѯSQL</param>
		/// <returns>���β�����Ӱ�����������</returns>
		internal static int ExecuteNonQuery(string strSql)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteNonQueryWithoutTransaction--strSql");
			using (DbContext context = DbContext.GetContext(LogResource.ConnAlias))
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
			using (DbContext context = DbContext.GetContext(LogResource.ConnAlias))
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
			using (DbContext context = DbContext.GetContext(LogResource.ConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				return database.ExecuteScalar(CommandType.Text, strSql);
			}
		}
		#endregion


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
	}
}
