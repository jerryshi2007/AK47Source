using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Library.Services;

namespace MCS.OA.CommonPages.ThreadStatus
{
	public partial class ServiceThreadStatus : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			if (!IsPostBack)
			{
				BindServers(ddlServers);
				BindData();
			}
		}

		protected void queryBtn_Click(object sender, EventArgs e)
		{
			BindData();
		}

		private void BindServers(DropDownList ddlServers)
		{
			ServiceStatusSettings service = ServiceStatusSettings.GetConfig();
			ddlServers.DataSource = service.Servers;
			ddlServers.DataTextField = "name";
			ddlServers.DataValueField = "description";
			ddlServers.DataBind();
			ddlServers.SelectedIndex = 0;
		}

		private void BindData()
		{
			ServiceStatusConfigElementCollection Servers = ServiceStatusSettings.GetConfig().Servers;
			string uri = "";
			this.serviceStatusGrid.DataSource = new ServiceThreadCollection();

			try
			{
				uri = ddlServers.SelectedValue;
				IStatusService serviceStatus =
					(IStatusService)Activator.GetObject(typeof(IStatusService), uri);
				ServiceThreadCollection threads = serviceStatus.GetThreadStatus();

				this.serviceStatusGrid.DataSource = threads;

				if (this.errorMessage.Visible == true)
					this.errorMessage.Visible = false;
			}
			catch (System.Net.WebException ex)
			{
				this.errorMessage.Visible = true;
				this.errorMessage.Text = string.Format("连接状态服务器\"{0}\"失败，地址不正确或者服务没有启动。--{1}<br>{2}",
					HttpUtility.HtmlEncode(uri),
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
					HttpUtility.HtmlEncode(ex.Message));
			}
			catch (Exception ex)
			{
				this.errorMessage.Visible = true;
				this.errorMessage.Text = HttpUtility.HtmlEncode(ex.Message);
			}
			finally
			{
				this.serviceStatusGrid.DataBind();
			}
		}

		protected void serviceStatusGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				IServiceThread thread = (IServiceThread)e.Row.DataItem;

				TableCell nameCell = e.Row.Cells[FindColumnIndexByField("Params", this.serviceStatusGrid.Columns)];
				nameCell.Text = thread.Params.Name;

				TableCell lastPollTimeCell = e.Row.Cells[FindColumnIndexByField("LastPollTime", this.serviceStatusGrid.Columns)];
				lastPollTimeCell.Text = thread.LastPollTime == DateTime.MinValue ? string.Empty : thread.LastPollTime.ToString("yyyy-MM-dd HH:mm:ss");

				TableCell lastMsgCell = e.Row.Cells[FindColumnIndexByField("LastMessage", this.serviceStatusGrid.Columns)];
				InnerSetCell(thread.Params.Name, thread.LastMessage, lastMsgCell);

				TableCell lastExceptionCell = e.Row.Cells[FindColumnIndexByField("LastExceptionMessage", this.serviceStatusGrid.Columns)];
				InnerSetCell(thread.Params.Name, thread.LastExceptionMessage, lastExceptionCell);
			}
		}

		private void InnerSetCell(string threadName, string msg, TableCell cell)
		{
			cell.Text = string.Empty;

			HtmlGenericControl div = new HtmlGenericControl("div");
			cell.Controls.Add(div);

			HtmlAnchor anchor = new HtmlAnchor();
			anchor.HRef = "#";
			anchor.Attributes["threadName"] = threadName;
			anchor.Attributes["onclick"] = "onDetailMessageClick()";

			HtmlGenericControl nobr = new HtmlGenericControl("nobr");
			nobr.InnerText = msg;
			anchor.Controls.Add(nobr);

			div.Controls.Add(anchor);
			div.Style["width"] = "200px";
			div.Style["height"] = "16px";
			div.Style["text-overflow"] = "ellipsis";
			div.Style["overflow"] = "hidden";
		}

		private int FindColumnIndexByField(string field, DataControlFieldCollection columns)
		{
			int result = -1;

			for (int i = 0; i < columns.Count; i++)
			{
				DataControlField col = columns[i];

				if (col is BoundField)
				{
					if (string.Compare(field, ((BoundField)col).DataField, true) == 0)
					{
						result = i;
						break;
					}
				}
			}

			ExceptionHelper.FalseThrow(result != -1, string.Format("不能在Grid中找到字段为{0}的BoundField", field));

			return result;
		}
	}
}