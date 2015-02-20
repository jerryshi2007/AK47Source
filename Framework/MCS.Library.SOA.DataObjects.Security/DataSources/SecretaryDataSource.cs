using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	/// <summary>
	/// 表示秘书的数据源
	/// </summary>
	public class SecretaryDataSource : ConstBelongingSchemaDataSource
	{
		bool queryBoss = false; // 内部使用，当为true时表示检索BOSS

		/// <summary>
		/// 获取或设置一个值，表示正在进行检索秘书还是上司
		/// </summary>
		public bool QueryBoss
		{
			get { return queryBoss; }
		}

		protected override string SnapshotTableName
		{
			get { return TimePointContext.Current.UseCurrentTime ? "SC.SchemaUserSnapshot_Current" : "SC.SchemaUserSnapshot"; }
		}

		protected override string[] ContainerSchemaTypes
		{
			get { return SchemaInfo.FilterByCategory("Users").ToSchemaNames(); }
		}

		protected override string[] MemberSchemaTypes
		{
			get { return SchemaInfo.FilterByCategory("Users").ToSchemaNames(); }
		}

		protected override string ParentIdField
		{
			get
			{
				if (this.queryBoss)
					return "R.MemberID";
				else
					return "R.ContainerID";
			}
		}

		protected override string FromClause
		{
			get
			{
				return this.queryBoss ?
					(this.SnapshotTableName + (TimePointContext.Current.UseCurrentTime ? " O INNER JOIN SC.SchemaMembersSnapshot_Current R ON O.ID = R.ContainerID" : " O INNER JOIN SC.SchemaMembersSnapshot R ON O.ID = R.ContainerID")) :
					(this.SnapshotTableName + (TimePointContext.Current.UseCurrentTime ? " O INNER JOIN SC.SchemaMembersSnapshot_Current R ON O.ID = R.MemberID" : " O INNER JOIN SC.SchemaMembersSnapshot R ON O.ID = R.MemberID"));
			}
		}

		public DataView Query(bool bossMode, string userId, string where, ref int totalCount, string orderBy, int startRowIndex, int maximumRows)
		{
			this.queryBoss = bossMode;
			return base.Query(userId, where, ref totalCount, orderBy, startRowIndex, maximumRows);
		}

		public int GetQueryCount(bool bossMode, string userId, string where, ref int totalCount)
		{
			return base.GetQueryCount(userId, where, ref totalCount);
		}
	}
}
