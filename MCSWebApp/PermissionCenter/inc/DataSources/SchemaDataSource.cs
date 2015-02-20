using System.Security.Permissions;
using System.Web;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Configuration;

namespace PermissionCenter.DataSources
{
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class SchemaDataSource : System.Web.UI.DataSourceControl
	{
		private const string VIEW_NAME_USERS = "users";
		private const string VIEW_NAME_ROOTS = "rootorgs";
		private const string VIEW_NAME_ALLTYPES = "allTypes";
		private static readonly string[] viewNames = new string[] { VIEW_NAME_USERS, VIEW_NAME_ROOTS, VIEW_NAME_ALLTYPES };
		private ViewUserSchemas users = null;
		private ViewRootOrgnizations roots = null;
		private ViewAllSchemas allTypes = null;

		public SchemaDataSource()
			: base()
		{
		}

		protected override System.Web.UI.DataSourceView GetView(string viewName)
		{
			switch (viewName)
			{
				case VIEW_NAME_USERS:
					if (this.users == null)
						this.users = new ViewUserSchemas(this);
					return this.users;
				case VIEW_NAME_ROOTS:
					if (this.roots == null)
						this.roots = new ViewRootOrgnizations(this);
					return this.roots;
				case VIEW_NAME_ALLTYPES:
					if (this.allTypes == null)
						this.allTypes = new ViewAllSchemas(this);
					return this.allTypes;
				default:
					return null;
			}
		}

		protected override System.Collections.ICollection GetViewNames()
		{
			return viewNames;
		}

		#region 嵌套类型
		/// <summary>
		/// 表示人员类型的查询视图
		/// </summary>
		public class ViewUserSchemas : System.Web.UI.DataSourceView
		{
			public ViewUserSchemas(System.Web.UI.IDataSource owner)
				: base(owner, SchemaDataSource.VIEW_NAME_USERS)
			{
			}

			protected override System.Collections.IEnumerable ExecuteSelect(System.Web.UI.DataSourceSelectArguments arguments)
			{
				return SchemaInfo.FilterByCategory("Users");
			}
		}

		/// <summary>
		/// 表示根机构的视图
		/// </summary>
		public class ViewRootOrgnizations : System.Web.UI.DataSourceView
		{
			public ViewRootOrgnizations(System.Web.UI.IDataSource owner)
				: base(owner, SchemaDataSource.VIEW_NAME_ROOTS)
			{
			}

			protected override System.Collections.IEnumerable ExecuteSelect(System.Web.UI.DataSourceSelectArguments arguments)
			{
				// SchemaOrgQueryDataSource src = new SchemaOrgQueryDataSource();

				// var view = src.GetChildrenView(SCOrganization.RootOrganizationID);
				SCObjectAndRelationCollection relations = SCSnapshotAdapter.Instance.QueryObjectAndRelationByParentIDs(SchemaInfo.FilterByCategory("Organizations").ToSchemaNames(), new string[] { SCOrganization.RootOrganizationID }, false, true, false, Util.GetTime());

				foreach (var item in relations)
				{
					if (Util.IsOrganization(item.SchemaType))
					{
						yield return new SCSimpleObject()
						{
							DisplayName = item.DisplayName,
							Name = item.Name,
							ID = item.ID,
							SchemaType = item.SchemaType,
							Status = item.Status,
						};
					}
				}
			}
		}

		/// <summary>
		/// 表示所有类型的视图
		/// </summary>
		public class ViewAllSchemas : System.Web.UI.DataSourceView
		{
			public ViewAllSchemas(System.Web.UI.IDataSource owner)
				: base(owner, SchemaDataSource.VIEW_NAME_ALLTYPES)
			{
			}

			protected override System.Collections.IEnumerable ExecuteSelect(System.Web.UI.DataSourceSelectArguments arguments)
			{
				return (new SchemaInfoCollection(ObjectSchemaSettings.GetConfig().Schemas)).FilterByNotRelation();
			}
		}
		#endregion
	}
}