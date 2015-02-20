using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;
using MCS.Library.Data.Builder;
using System.Transactions;
using MCS.Library.Data;
using System.Data;

namespace MCS.Library.SOA.DataObjects
{
	public class ExtUserMappingAdapter : UpdatableAndLoadableAdapterBase<ExtUserMapping, ExtUserMappingCollection>
	{
		public static readonly ExtUserMappingAdapter Instance = new ExtUserMappingAdapter();

		private ExtUserMappingAdapter()
		{
		}

		/// <summary>
		/// 调用本方法，更改或添加外网用户与内网用户的映射关系;
		/// </summary>
		/// <param name="extUser"></param>
		/// <param name="mappingUserId"></param>
		public void SetMappingUser(string extUserId, string mappingUserId)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				string sql = string.Format("DELETE EXT_INT_USERMAPPING WHERE Ext_UserID = {0}",
				 TSqlBuilder.Instance.CheckQuotationMark(extUserId, true));

				DbHelper.RunSql(sql, GetConnectionName());

				string insertSql = string.Format("INSERT EXT_INT_USERMAPPING(EXT_USERID, USERID) values({0}, {1})",
					TSqlBuilder.Instance.CheckQuotationMark(extUserId, true), TSqlBuilder.Instance.CheckQuotationMark(mappingUserId, true));

				DbHelper.RunSql(insertSql, GetConnectionName());

				scope.Complete();
			}
		}

		/// <summary>
		/// 调用本方法，删除外网用户与内网用户的映射关系
		/// </summary>
		/// <param name="group"></param>
		/// <param name="userIDs"></param>
		public void DeleteMappingUser(string extUserId)
		{
			string sql = string.Format("DELETE EXT_INT_USERMAPPING WHERE Ext_UserID = {0}",
				TSqlBuilder.Instance.CheckQuotationMark(extUserId, true));

			DbHelper.RunSql(sql, GetConnectionName());
		}

		/// <summary>
		/// 调用本方法，获取有Mapping关系的内部用户;
		/// </summary>
		/// <returns></returns>
		public DataTable GetMappingRelationUsers()
		{
			string sql = @"SELECT EXT_INT_USERMAPPING.EXT_USERID,EXT_INT_USERMAPPING.USERID, USERS.LOGON_NAME,USERS.LAST_NAME,USERS.FIRST_NAME 
                        FROM USERS,EXT_INT_USERMAPPING 
                        WHERE USERS.GUID = EXT_INT_USERMAPPING.USERID;";

			DataSet ds = DbHelper.RunSqlReturnDS(sql, GetConnectionName());
			if (ds != null && ds.Tables.Count == 1)
			{
				return ds.Tables[0];
			}
			return null;
		}

		protected override string GetConnectionName()
		{
			return ConnectionDefine.DefaultAccreditInfoConnectionName;
		}
	}
}
