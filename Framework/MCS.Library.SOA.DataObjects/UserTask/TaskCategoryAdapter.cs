#region 作者版本
// -------------------------------------------------
// Assembly	：	HB.DataObjects
// FileName	：	TaskCategoryAdapter.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    李苗	    20070725		创建
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
		#region 公有属性

		private static TaskCategoryAdapter instance = new TaskCategoryAdapter();

		public static TaskCategoryAdapter Instance
		{
			get
			{
				return instance;
			}
		}

		#endregion

		#region 构造函数

		internal protected TaskCategoryAdapter()
		{

		}

		#endregion

		#region 和自定义分类相关的
		/// <summary>
		/// 根据用户获取Category集合
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
		/// 根据用户ID获取category
		/// </summary>
		/// <param name="userID"></param>
		/// <returns></returns>
		public TaskCategoryCollection GetCategoriesByUserID(string userID)
		{
			int count = 0;
			return GetCategoriesByUserID(userID, ref count);
		}
		/// <summary>
		/// 根据SQL获取category
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
		/// 根据CategoryID获取Category集合
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
		/// 添加待办箱分类
		/// </summary>
		/// <param name="taskCategory">处理夹对象</param>
		public void InsertCategory(TaskCategory taskCategory)
		{
			string strSql = ORMapping.GetInsertSql(taskCategory, TSqlBuilder.Instance);

			DbHelper.RunSqlWithTransaction(strSql);
		}

		/// <summary>
		/// 更新待办箱分类
		/// </summary>
		/// <param name="taskCategory">处理夹对象</param>
		public void UpdateCategory(TaskCategory taskCategory)
		{
			string strSql = ORMapping.GetUpdateSql(taskCategory, TSqlBuilder.Instance);

			DbHelper.RunSqlWithTransaction(strSql);
		}

		/// <summary>
		/// 删除待办箱分类
		/// </summary>
		/// <param name="categoryGuid">处理夹ID</param>
		public void DeleteCategory(string CategoryID)
		{
			string strSql = "UPDATE USER_TASK SET CATEGORY_GUID = NULL WHERE CATEGORY_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(CategoryID, true) + ";";

			strSql += " DELETE WF.USER_TASK_CATEGORY WHERE  CATEGORY_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(CategoryID, true);

			DbHelper.RunSqlWithTransaction(strSql);
		}

		/// <summary>
		/// 获取可用的排序号
		/// </summary>
		/// <param name="userID">用户ID</param>
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
				//当排序号以达到最大值999时，仍返回最大值
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
