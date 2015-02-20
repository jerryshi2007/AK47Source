using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Transactions;
using System.Data;
using MCS.Library.Data;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 锁操作类
	/// </summary>
	public static class LockAdapter
	{
		public static SetLockResult SetLock(Lock lockEntity)
		{
			return SetLock(lockEntity, false);
		}

		public static SetLockResult SetLock(Lock lockInfo, bool forceLock)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(lockInfo == null, "lockInfo");

			using (TransactionScope ts = TransactionScopeFactory.Create(TransactionScopeOption.Required))
			{
				DataTable table = DbHelper.RunSPReturnDS("WF.SetLock",
											  lockInfo.LockID,
											  lockInfo.ResourceID,
											  lockInfo.PersonID,
											  lockInfo.EffectiveTime.TotalSeconds,
											  lockInfo.LockType,
											  forceLock ? "y" : "n").Tables[0];
				ts.Complete();

				return new SetLockResult(lockInfo.PersonID, table);
			}
		}

		public static CheckLockResult CheckLock(string lockID, string personID)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(lockID, "lockID");
			ExceptionHelper.CheckStringIsNullOrEmpty(personID, "personID");

			string sql = string.Format("SELECT *, GETDATE() AS [CURRENT_TIME] FROM WF.LOCK WHERE LOCK_ID = {0}",
				TSqlBuilder.Instance.CheckQuotationMark(lockID, true));

			DataTable table = DbHelper.RunSqlReturnDS(sql).Tables[0];

			return new CheckLockResult(personID, table);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lockInfo"></param>
		/// <returns></returns>
		public static CheckLockResult CheckLock(Lock lockInfo)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(lockInfo == null, "lockInfo");

			return CheckLock(lockInfo.LockID, lockInfo.PersonID);
		}

		/// <summary>
		/// 解锁
		/// </summary>
		/// <param name="lockID">锁ID</param>
		/// <param name="personID">用户ID</param>
		public static void Unlock(string lockID, string personID)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(lockID == null, "lockID");
			ExceptionHelper.TrueThrow<ArgumentNullException>(personID == null, "personID");

			Unlock(new Lock(lockID, personID));
		}

		/// <summary>
		/// 解锁
		/// </summary>
		/// <param name="lockEntity">锁实体</param>
		public static void Unlock(params Lock[] lockEntity)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(lockEntity != null, "lockEntity");

			StringBuilder strWhere = new StringBuilder(255);

			for (int i = 0; i < lockEntity.Length; i++)
			{
				if (strWhere.Length > 0)
					strWhere.Append(" OR ");

				WhereSqlClauseBuilder objWCB = new WhereSqlClauseBuilder();
				objWCB.AppendItem("LOCK_ID", lockEntity[i].LockID);
				objWCB.AppendItem("LOCK_PERSON_ID", lockEntity[i].PersonID);

				strWhere.AppendFormat("({0})", objWCB.ToSqlString(TSqlBuilder.Instance));
			}

			if (strWhere.Length > 0)
			{
				string sql = string.Format("DELETE FROM WF.LOCK WHERE {0}", strWhere.ToString());

				DbHelper.RunSqlWithTransaction(sql);
			}
		}

		public static void ForceUnlock(params Lock[] lockEntity)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(lockEntity != null, "lockEntity");

			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			for (int i = 0; i < lockEntity.Length; i++)
				builder.AppendItem(lockEntity[i].LockID);

			if (builder.Count > 0)
			{
				string sql = string.Format("DELETE FROM WF.LOCK WHERE LOCK_ID {0}", builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DbHelper.RunSqlWithTransaction(sql);
			}
		}

		/// <summary>
		/// 强制解锁
		/// </summary>
		/// <param name="lockIDs">锁ID</param>
		public static void ForceUnlock(params string[] lockIDs)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(lockIDs != null, "lockID");

			InSqlClauseBuilder builder = new InSqlClauseBuilder();

			for (int i = 0; i < lockIDs.Length; i++)
				builder.AppendItem(lockIDs[i]);

			if (builder.Count > 0)
			{
				string sql = string.Format("DELETE FROM WF.LOCK WHERE LOCK_ID {0}", builder.ToSqlStringWithInOperator(TSqlBuilder.Instance));

				DbHelper.RunSqlWithTransaction(sql);
			}
		}
	}
}
