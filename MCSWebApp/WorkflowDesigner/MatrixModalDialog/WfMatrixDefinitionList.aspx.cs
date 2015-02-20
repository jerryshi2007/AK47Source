using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;

namespace WorkflowDesigner.MatrixModalDialog
{
	public partial class WfMatrixDefinitionList : System.Web.UI.Page
	{
		private bool CanSelect
		{
			get
			{
				return Request.QueryString.GetValue("canSelect", false);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {

                //ExecuteQuery();
            }
        }

        private void ExecuteQuery()
        {
            LastQueryRowCount = -1;
            this.MatrixDefDeluxeGrid.SelectedKeys.Clear();
            this.MatrixDefDeluxeGrid.PageIndex = 0;
        }


		protected override void OnPreRender(EventArgs e)
		{
			btnOK.Visible = CanSelect;

			base.OnPreRender(e);
		}

		protected void objectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
		{
			e.InputParameters["totalCount"] = LastQueryRowCount;
		}

		protected void objectDataSource_Selected(object sender, ObjectDataSourceStatusEventArgs e)
		{
			LastQueryRowCount = (int)e.OutputParameters["totalCount"];
		}
		private int LastQueryRowCount
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "LastQueryRowCount", -1);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "LastQueryRowCount", value);
			}
		}

		protected void MatrixDefDeluxeGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				var matrixKey = e.Row.Cells[1].Text;
				string htmlAnchor = "<a href='#' onclick=\"modifyMatrixDefine('{0}');\">{1}</a>";

				e.Row.Cells[1].Text = string.Format(htmlAnchor,
					HttpUtility.JavaScriptStringEncode(matrixKey, false),
					HttpUtility.HtmlEncode(matrixKey));
			}
		}
		protected void btnConfirm_Click(object sender, EventArgs e)
		{
			var keys = MatrixDefDeluxeGrid.SelectedKeys;
			List<string> validKeys = new List<string>();
			List<string> invalidKeys = new List<string>();
			try 
			{	        
				foreach (var item in keys)
				{
					WfMatrix matrix = null;
					try
					{
						matrix = WfMatrixAdapter.Instance.Load(item);
					}
					catch (Exception) { }
					if(matrix == null)
						validKeys.Add(item);
					else
						invalidKeys.Add(item);
				}
				WfMatrixDefinitionAdapter.Instance.Delete(validKeys.ToArray());
                LastQueryRowCount = -1;
				if (invalidKeys.Count != 0)
				{
					string strInvalidKeys = string.Join(",", invalidKeys.ToArray());
					string strProcessMapMatrix = string.Empty;
					Page.ClientScript.RegisterStartupScript(this.GetType(), "undeleteMatrix",
					string.Format("alert('Key为[{0}]的矩阵定义无法删除，因为已经有流程模板在使用此矩阵。');", strInvalidKeys),
					true);
				}
				else
				{
					Page.ClientScript.RegisterStartupScript(this.GetType(), "deleteMatrix",
					string.Format("alert('删除成功!');"),
					true);

				}

			}
			catch (Exception ex) 
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");

			}

		}

        protected void hiddenServerBtn_Click(object sender, EventArgs e)
        {
            LastQueryRowCount = -1;
        }

	}
}