using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.DataSources
{
	[DataObject]
	public class AURoleDataSource : AUMemberViewDataSourceBase
	{
		protected override string SnapshotTableName
		{
			get { return "SC.AURoleSnapshot"; }
		}

		protected override string CurrentSnapshotTableName
		{
			get { return "SC.AURoleSnapshot_Current"; }
		}

		protected override void BuildWhere(Data.Builder.WhereSqlClauseBuilder where)
		{
			base.BuildWhere(where);
			where.AppendCondition("M.ContainerID", "");
			where.AppendCondition("M.MemberSchemaType", AUCommon.SchemaAdminUnitRole);
		}
	}
}
