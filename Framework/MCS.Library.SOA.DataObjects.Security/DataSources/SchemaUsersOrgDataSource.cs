using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	/// <summary>
	/// 用户所属组织的查询列表的DataSource
	/// </summary>
	public class SchemaUsersOrgDataSource : ChildSchemaDataSource
	{
		protected override string SnapshotTableName
		{
			get { return TimePointContext.Current.UseCurrentTime ? "SC.SchemaOrganizationSnapshot_Current" : "SC.SchemaOrganizationSnapshot"; }
		}

		protected override string[] ParentSchemaTypes
		{
			get { return SchemaInfo.FilterByCategory("Organizations").ToSchemaNames(); }
		}

		protected override string[] ChildSchemaTypes
		{
			get { return SchemaInfo.FilterByCategory("Users").ToSchemaNames(); }
		}

		protected override string DefaultOrderBy
		{
			get { return "R.VersionStartTime DESC"; }
		}
	}
}
