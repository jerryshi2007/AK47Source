using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects.Archive
{
	/// <summary>
	/// 归档执行器的虚基类
	/// </summary>
	public abstract class ArchiveExecutorBase : IArchiveExecutor
	{
		#region IArchiveExecutor Members

		public void Archive(ArchiveBasicInfo info)
		{
			LoadOriginalData(info);

			using (DbConnectionMappingContext context =
				DbConnectionMappingContext.CreateMapping(ConnectionDefine.DBConnectionName, "Archive"))
			{
				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					SaveArchiveData(info);
					scope.Complete();
				}
			}

			if (ArchiveSettings.GetConfig().DeleteOriginalData)
				InnerDelete(info);
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="info"></param>
		public void Delete(ArchiveBasicInfo info)
		{
			LoadOriginalData(info);

			InnerDelete(info);
		}
		#endregion

		#region 可重载的方法
		protected virtual void LoadOriginalData(ArchiveBasicInfo info)
		{
			WorkflowArchiveOperation.Instance.LoadOriginalData(info);
		}

		protected virtual void SaveArchiveData(ArchiveBasicInfo info)
		{
			WorkflowArchiveOperation.Instance.SaveArchiveData(info);
		}

		protected virtual void DeleteOriginalData(ArchiveBasicInfo info)
		{
			WorkflowArchiveOperation.Instance.DeleteOriginalData(info);
		}
		#endregion 可重载的方法

		#region Private
		private void InnerDelete(ArchiveBasicInfo info)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				DeleteOriginalData(info);
				scope.Complete();
			}
		}
		#endregion
	}
}
