using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	public class SchemaGroupConstMembersDataSource : ConstBelongingSchemaDataSource
	{
		protected override void OnAfterQuery(System.Data.DataView result)
		{
			base.OnAfterQuery(result);
			DataSourceUtil.FillUserDefaultParent(result, "ID", "ParentID", this.GetConnectionName());
		}

		protected override string SnapshotTableName
		{
			get { return TimePointContext.Current.UseCurrentTime ? "SC.SchemaUserSnapshot_Current" : "SC.SchemaUserSnapshot"; }
		}

		protected override string[] ContainerSchemaTypes
		{
			get { return SchemaInfo.FilterByCategory("Groups").ToSchemaNames(); }
		}

		protected override string[] MemberSchemaTypes
		{
			get { return SchemaInfo.FilterByCategory("Users").ToSchemaNames(); }
		}
	}
}
