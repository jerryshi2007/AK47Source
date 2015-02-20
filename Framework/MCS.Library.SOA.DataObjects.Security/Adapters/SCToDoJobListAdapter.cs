using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Tasks;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 写入后台任务的适配器
	/// </summary>
	public class SCToDoJobListAdapter
	{
		public static readonly SCToDoJobListAdapter Instance = new SCToDoJobListAdapter();

		private SCToDoJobListAdapter()
		{
		}

		public void Insert(SCToDoJob job)
		{
			job.NullCheck("job");

			string sql = ORMapping.GetInsertSql(job, TSqlBuilder.Instance);

			DbHelper.RunSql(sql, this.GetConnectionName());
		}

		/// <summary>
		/// 获取连接的名称
		/// </summary>
		/// <returns>表示连接名称的字符串</returns>
		protected string GetConnectionName()
		{
			return SCConnectionDefine.DBConnectionName;
		}
	}
}
