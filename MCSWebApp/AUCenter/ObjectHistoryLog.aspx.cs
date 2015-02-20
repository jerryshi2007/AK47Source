using System;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace AUCenter
{
	public partial class ObjectHistoryLog : System.Web.UI.Page
	{
		protected string id = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.id = this.Request.QueryString["id"];
			var encodeId = Server.UrlEncode(this.id);
			this.lnkList.NavigateUrl = "~/lists/ObjectHistoryLog.aspx?id=" + encodeId;

			//if (Page.IsPostBack == false)
			//    this.gridMain.PageSize = ProfileUtil.PageSize;
		}

		protected void dataSourceMain_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			this.schemaTypeLabel.InnerText = e.OutputParameters["schemaType"] as string;
		}

		protected void HandleRowBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				((Label)e.Row.FindControl("lblStatus")).Text = this.GetStateDescription(((PC.HistoryEntry)e.Row.DataItem).Status);
			}
		}

		private string GetStateDescription(SchemaObjectStatus status)
		{
			switch (status)
			{
				case SchemaObjectStatus.Deleted:
					return "已删除";
				case SchemaObjectStatus.DeletedByContainer:
					return "被容器删除";
				case SchemaObjectStatus.Normal:
					return "正常";
				case SchemaObjectStatus.Unspecified:
					return "未指定";
				default:
					return "(未知)";
			}
		}
	}
}