using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter.DataSources
{
	public class AppDataSource : System.Web.UI.DataSourceControl
	{
		public static readonly string[] ViewNames = new string[] { "Apps" };

		protected override System.Web.UI.DataSourceView GetView(string viewName)
		{
			switch (viewName)
			{
				case "Apps":
					return new AppView(this);
				default:
					throw new ArgumentOutOfRangeException("viewName");
			}
		}

		protected override System.Collections.ICollection GetViewNames()
		{
			return ViewNames;
		}

		private class AppView : System.Web.UI.DataSourceView
		{
			public AppView(System.Web.UI.IDataSource owner)
				: base(owner, "Apps")
			{
			}

			protected override System.Collections.IEnumerable ExecuteSelect(System.Web.UI.DataSourceSelectArguments arguments)
			{
				var allApps = PC.Adapters.SchemaObjectAdapter.Instance.LoadBySchemaType(PC.SchemaInfo.FilterByCategory("Applications").ToSchemaNames(), DateTime.MinValue);
				foreach (var app in allApps)
				{
					if (app.Status == SchemaObjectStatus.Normal)
					{
						yield return app.ToSimpleObject();
					}
				}
			}
		}
	}
}