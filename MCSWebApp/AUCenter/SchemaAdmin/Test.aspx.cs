using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.OGUPermission;

namespace AUCenter.SchemaAdmin
{
	public partial class Test : System.Web.UI.Page
	{
		protected void RandomGen1(object sender, EventArgs e)
		{
			var demoObj = (AU.AUAdminScopeItem)SchemaExtensions.CreateObject("AdminScope001");
			demoObj.ID = UuidHelper.NewUuidString();
			demoObj.AUScopeItemName = "Demo" + demoObj.ID.ToString();
			AU.AUCommon.DoDbAction(() =>
			{
				PC.Adapters.SchemaObjectAdapter.Instance.Update(demoObj);
			});
		}

		protected void ClearData(object sender, EventArgs e)
		{
			AU.Adapters.AUSnapshotHelper.ClearUp();
		}

		protected void MatrixTest(object sender, EventArgs e)
		{
			SOARolePropertiesQueryParam qp = new SOARolePropertiesQueryParam();
			qp.QueryName = "AdminUnit";
			qp.QueryValue = "";
			OguDataCollection<IUser> users = GetUsersFromSOARole(new SOARolePropertiesQueryParamCollection() { qp });
			users.ForEach(p =>
			{
				Response.Output.WriteLine("<br/>" + p.LogOnName);
			});
		}

		public OguDataCollection<IUser> GetUsersFromSOARole(SOARolePropertiesQueryParamCollection qps)
		{
			SOARolePropertyRowCollection rows = null;
			OguDataCollection<IUser> users = new OguDataCollection<IUser>();
			var schemaRoleID = txtSchemaRoleID.Text;
			var roleRows = AU.Adapters.AUMatrixHelper.LoadSchemaRolePropertyRows(schemaRoleID);

			if (qps != null)
			{
				rows = roleRows.Query(qps);
			}

			foreach (SOARolePropertyRowUsers rowUsers in rows.GenerateRowsUsers())
			{
				foreach (IUser user in rowUsers.Users)
				{
					if (users.Contains(user) == false)
					{
						users.Add(user);
					}
				}
			}
			return users;
		}
	}
}