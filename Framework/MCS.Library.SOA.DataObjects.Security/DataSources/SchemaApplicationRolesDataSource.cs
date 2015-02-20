using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	/// <summary>
	/// 提供应用程序角色的数据源
	/// </summary>
	public class SchemaApplicationRolesDataSource : ConstBelongingSchemaDataSource
	{
		protected override string SnapshotTableName
		{
			get { return TimePointContext.Current.UseCurrentTime ? "SC.SchemaRoleSnapshot_Current" : "SC.SchemaRoleSnapshot"; }
		}

		protected override string[] ContainerSchemaTypes
		{
			get { return SchemaInfo.FilterByCategory("Applications").ToSchemaNames(); }
		}

		protected override string[] MemberSchemaTypes
		{
			get { return SchemaInfo.FilterByCategory("Roles").ToSchemaNames(); }
		}
	}
}
