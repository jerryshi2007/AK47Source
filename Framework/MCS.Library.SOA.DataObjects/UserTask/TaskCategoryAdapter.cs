#region ���߰汾
// -------------------------------------------------
// Assembly	��	HB.DataObjects
// FileName	��	TaskCategoryAdapter.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070725		����
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Data;
using System.Transactions;
using System.Collections.Generic;
using System.Xml;

using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects
{
	public class TaskCategoryAdapter
	{
		#region ��������

		private static TaskCategoryAdapter instance = new TaskCategoryAdapter();

		public static TaskCategoryAdapter Instance
		{
			get
			{
				return instance;
			}
		}

		#endregion

		#region ���캯��

		internal protected TaskCategoryAdapter()
		{

		}

		#endregion

		#region ���Զ��������ص�
		/// <summary>
		/// �����û���ȡCategory����
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public TaskCategoryCollection GetCategoriesByUserID(string userID, ref int totalCount)
		{
			TaskCategoryCollection tc = GetCategoriesByWhereCondition("", "INNER_SORT_ID", new List<string>());
			totalCount = tc.Count;
			return tc;
		}
		/// <summary>
		/// �����û�ID��ȡcategory
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public TaskCategoryCollection GetCategoriesByUserID(string userID)
		{
			int count = 0;
			return GetCategoriesByUserID(userID, ref count);
		}
		/// <summary>
		/// ����SQL��ȡcategory
		/// </summary>
		/// <param name="where"></param>
		/// <param name="orderby"></param>
		/// <param name="selecter"></param>
		/// <returns></returns>
		public TaskCategoryCollection GetCategoriesByWhereCondition(string where, string orderby, List<string> selecter)
		{
			string sql = string.Format("SELECT");

			if (selecter != null && selecter.Count > 0)
			{
				foreach (string sel in selecter)
				{
					sql += TSqlBuilder.Instance.CheckQuotationMark(sel, true);
				}
			}
			else
				sql += " * ";

			sql += string.Format("FROM WF.USER_TASK_CATEGORY WHERE USER_ID={0}", TSqlBuilder.Instance.CheckQuotationMark(DeluxeIdentity.CurrentUser.ID, true));

			if (where != string.Empty&&where!=null)
				sql += string.Format("AND ({0})", where);
			if (orderby != string.Empty&&orderby != null)
				sql += string.Format("ORDER BY {0}", orderby);

			DataView dv = DbHelper.RunSqlReturnDS(sql).Tables[0].DefaultView;
			TaskCategoryCollection tcc = new TaskCategoryCollection();
			tcc.LoadFromDataView(dv);

			return tcc;
		}

		/// <summary>
		/// ����CategoryID��ȡCategory����
		/// </summary>
		/// <param name="categoryID"></param>
		/// <returns></returns>
		public TaskCategoryCollection GetCategoriesByCategoryIDs(params string[] categoryID)
		{
			string strSQL = string.Empty;
			TaskCategoryCollection taskCategories = new TaskCategoryCollection();

			if (categoryID.Length > 0)
			{
				InSqlClauseBuilder inSQL = new InSqlClauseBuilder();
				inSQL.AppendItem(categoryID);

				strSQL = string.Format("SELECT * FROM WF.USER_TASK_CATEGORY WHERE CATEGORY_GUID IN ({0}) ORDER BY INNER_SORT_ID", inSQL.ToSqlString(TSqlBuilder.Instance));

				DataView dv = DbHelper.RunSqlReturnDS(strSQL).Tables[0].DefaultView;
				taskCategories.LoadFromDataView(dv);
			}

			return taskCategories;
		}

		/// <summary>
		/// ��Ӵ��������
		/// </summary>
		/// <param name="taskCategory">����ж���</param>
		public void InsertCategory(TaskCategory taskCategory)
		{
			string strSql = ORMapping.GetInsertSql(taskCategory, TSqlBuilder.Instance);

			DbHelper.RunSqlWithTransaction(strSql);
		}

		/// <summary>
		/// ���´��������
		/// </summary>
		/// <param name="taskCategory">����ж���</param>
		public void UpdateCategory(TaskCategory taskCategory)
		{
			string strSql = ORMapping.GetUpdateSql(taskCategory, TSqlBuilder.Instance);

			DbHelper.RunSqlWithTransaction(strSql);
		}

		/// <summary>
		/// ɾ�����������
		/// </summary>
		/// <param name="categoryGuid">�����ID</param>
		public void DeleteCategory(string CategoryID)
		{
			string strSql = "UPDATE USER_TASK SET CATEGORY_GUID = NULL WHERE CATEGORY_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(CategoryID, true) + ";";

			strSql += " DELETE WF.USER_TASK_CATEGORY WHERE  CATEGORY_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(CategoryID, true);

			DbHelper.RunSqlWithTransaction(strSql);
		}

		/// <summary>
		/// ��ȡ���õ������
		/// </summary>
		/// <param name="userID">�û�ID</param>
		/// <returns></returns>
		public int GetMaxSort(string userID)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(null == userID, "userID");

			int result = 0;
			string strSQL = string.Format("SELECT MAX(INNER_SORT_ID) AS NUM FROM WF.USER_TASK_CATEGORY WHERE USER_ID = {0}", TSqlBuilder.Instance.CheckQuotationMark(userID, true));

			object num = DbHelper.RunSqlReturnScalar(strSQL);
			if (num.ToString() != string.Empty)
			{
				result = (int)num;
				//��������Դﵽ���ֵ999ʱ���Է������ֵ
				if (result < 999)
				{
					result++;
				}
			}

			return result;
		}
		#endregion
	}
}
