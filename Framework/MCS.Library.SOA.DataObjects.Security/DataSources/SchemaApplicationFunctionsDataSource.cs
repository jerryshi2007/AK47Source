using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.DataSources
{
	/// <summary>
	/// 表示提供应用程序功能的数据源
	/// </summary>
	public class SchemaApplicationFunctionsDataSource : ConstBelongingSchemaDataSource
	{
		protected override string SnapshotTableName
		{
			get { return TimePointContext.Current.UseCurrentTime ? "SC.SchemaPermissionSnapshot_Current" : "SC.SchemaPermissionSnapshot"; }
		}

		protected override string[] ContainerSchemaTypes
		{
			get { return SchemaInfo.FilterByCategory("Applications").ToSchemaNames(); }
		}

		protected override string[] MemberSchemaTypes
		{
			get { return SchemaInfo.FilterByCategory("Permissions").ToSchemaNames(); }
		}
	}
}
