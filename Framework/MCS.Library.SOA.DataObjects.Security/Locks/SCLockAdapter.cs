using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.Locks
{
	public class SCLockAdapter
	{
		public static readonly SCLockAdapter Instance = new SCLockAdapter();

		private SCLockAdapter()
		{
		}

		/// <summary>
		/// 加锁。返回新加的锁，或者原来锁的状态。成功与否，检查SCCheckLockResult的Available属性
		/// </summary>
		/// <param name="lockData"></param>
		/// <returns></returns>
		public SCCheckLockResult AddLock(SCLock lockData)
		{
			SCCheckLockResult result = null;
			lockData.LockTime = DateTime.MinValue;

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				//插入是否成功，判断锁是否已经存在
				SCLock lockInDB = Insert(lockData);

				if (lockInDB == null)
				{
					//更新是否成功，如果不成功，表示锁被占用。
					bool updated = false;

					lockInDB = Update(lockData, false, out updated);

					if (updated == false)
						result = BuildNotAvailableResult(lockInDB);
					else
						result = BuildAvailableResult(lockInDB, true);
				}
				else
					result = BuildAvailableResult(lockInDB, false);

				scope.Complete();
			}

			return result;
		}

		/// <summary>
		/// 延长锁的时间
		/// </summary>
		/// <param name="lockData"></param>
		/// <returns></returns>
		public SCCheckLockResult ExtendLockTime(SCLock lockData)
		{
			SCCheckLockResult result = null;
			lockData.LockTime = DateTime.MinValue;

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				bool updated = false;

				SCLock lockInDB = lockInDB = Update(lockData, true, out updated);

				if (updated == false)
					result = BuildNotAvailableResult(lockData);
				else
					result = BuildAvailableResult(lockInDB, true);

				scope.Complete();
			}

			return result;
		}

		public void DeleteLock(SCLock lockData)
		{
			lockData.NullCheck("lockData");

			DeleteLock(lockData.LockID);
		}

		public void DeleteLock(string lockID)
		{
			lockID.CheckStringIsNullOrEmpty("lockID");

			string sql = string.Format("DELETE {0} WHERE LockID = {1}",
				GetMappingInfo().TableName, TSqlBuilder.Instance.CheckUnicodeQuotationMark(lockID));

			DbHelper.RunSql(sql, GetConnectionName());
		}

		private static SCCheckLockResult BuildNotAvailableResult(SCLock lockData)
		{
			SCCheckLockResult result = new SCCheckLockResult();

			result.Lock = lockData;
			result.LockStatus = SCCheckLockStatus.Locked;

			return result;
		}

		private static SCCheckLockResult BuildAvailableResult(SCLock lockData, bool overrideLock)
		{
			SCCheckLockResult result = new SCCheckLockResult();

			result.Lock = lockData;

			if (overrideLock)
				result.LockStatus = SCCheckLockStatus.LockExpired;
			else
				result.LockStatus = SCCheckLockStatus.NotLocked;

			return result;
		}

		private static SCLock GetLockInfo(string lockID)
		{
			string sql = string.Format("SELECT * FROM {0} WHERE LockID = {1}",
				GetMappingInfo().TableName, TSqlBuilder.Instance.CheckUnicodeQuotationMark(lockID));

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			(table.Rows.Count > 0).FalseThrow("不能根据\"{0}\"找到对应的锁信息", lockID);

			SCLock result = new SCLock();

			ORMapping.DataRowToObject(table.Rows[0], GetMappingInfo(), result);

			return result;
		}

		private static SCLock Update(SCLock lockData, bool forceOverride, out bool updated)
		{
			DataSet ds = DbHelper.RunSqlReturnDS(PrepareUpdateData(lockData, forceOverride), GetConnectionName());

			(ds.Tables[1].Rows.Count > 0).FalseThrow("不能找到ID为{0}的锁信息", lockData.LockID);

			updated = (int)ds.Tables[0].Rows[0][0] > 0;

			SCLock result = new SCLock();

			ORMapping.DataRowToObject(ds.Tables[1].Rows[0], GetMappingInfo(), result);

			return result;
		}

		private static string PrepareUpdateData(SCLock lockData, bool forceOverride)
		{
			ORMappingItemCollection mappingInfo = GetMappingInfo();

			UpdateSqlClauseBuilder uBuilder = ORMapping.GetUpdateSqlClauseBuilder(lockData, mappingInfo);
			WhereSqlClauseBuilder wBuilder = ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(lockData, mappingInfo);

			if (forceOverride == false)
				wBuilder.AppendItem("LockTime", "DATEADD(SECOND, -EffectiveTime, GETDATE())", "<", true);

			StringBuilder sql = new StringBuilder();

			sql.AppendFormat("UPDATE {0} SET {1} WHERE {2}",
				mappingInfo.TableName,
				uBuilder.ToSqlString(TSqlBuilder.Instance),
				wBuilder.ToSqlString(TSqlBuilder.Instance));

			sql.Append(TSqlBuilder.Instance.DBStatementSeperator);

			sql.Append("SELECT @@ROWCOUNT");

			sql.Append(TSqlBuilder.Instance.DBStatementSeperator);

			sql.AppendFormat("SELECT * FROM {0} WHERE LockID = {1}",
				GetMappingInfo().TableName,
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(lockData.LockID));

			return sql.ToString();
		}

		private static SCLock Insert(SCLock lockData)
		{
			StringBuilder sql = new StringBuilder();

			sql.Append(ORMapping.GetInsertSql(lockData, TSqlBuilder.Instance));
			sql.Append(TSqlBuilder.Instance.DBStatementSeperator);
			sql.AppendFormat("SELECT * FROM {0} WHERE LockID = {1}",
				GetMappingInfo().TableName,
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(lockData.LockID));

			SCLock result = null;

			try
			{
				DataTable table = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0];

				result = new SCLock();

				ORMapping.DataRowToObject(table.Rows[0], GetMappingInfo(), result);
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				if (ex.Number != 2627)
					throw;
			}

			return result;
		}

		private static ORMappingItemCollection GetMappingInfo()
		{
			return ORMapping.GetMappingInfo<SCLock>();
		}

		private static string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
