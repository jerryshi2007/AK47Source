using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Builder;

namespace WorkflowDesigner.PlanScheduleDialog
{
	public partial class TaskAchivedDetail : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void dataSourceMain_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			WhereSqlClauseBuilder where = new WhereSqlClauseBuilder();
			where.AppendItem("TASK_GUID", Request.QueryString["id"]);

			dataSourceMain.Condition = where;
		}
	}
}