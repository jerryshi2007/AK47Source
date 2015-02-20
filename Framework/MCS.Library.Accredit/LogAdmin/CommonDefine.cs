using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Transactions;
using System.Diagnostics;

using MCS.Library.Data;
using MCS.Library.Core;
using MCS.Library.Accredit.Properties;

namespace MCS.Library.Accredit.LogAdmin
{
	internal class CommonDefine
	{
		/// <summary>
		/// �Զ����SQL��ִ�й���
		/// </summary>
		/// <param name="strSql">Ҫ��ִ�е����ݲ�ѯSQL</param>
		/// <returns>���β�����Ӱ�����������</returns>
		internal static int ExecuteNonQuery(string strSql)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteNonQueryWithoutTransaction--strSql");
			using (DbContext context = DbContext.GetContext(CommonResource.LogConnAlias))
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
			using (DbContext context = DbContext.GetContext(CommonResource.LogConnAlias))
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
			using (DbContext context = DbContext.GetContext(CommonResource.LogConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				return database.ExecuteScalar(CommandType.Text, strSql);
			}
		}
	}
}
