using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;

namespace WorkflowDesigner.ModalDialog
{
    public partial class ActivityKeyEditor : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void confirmButton_TextChanged(object sender, EventArgs e)
		{
			try
			{
                //WfProcessDescriptorManager.ExsitsProcessKey(processKey.Value).TrueThrow("Key为{0}的流程定义已经存在", processKey.Value);
                //foreach (var item in activities)
                //{
                //    ListItem listItem = new ListItem(item.Name, item.Key);
                //    dropdownActivities.Items.Add(listItem);
                //}
                //Response.Write(string.Format("<script type='text/javascript'>top.returnValue = '{0}'; top.close();</script>",
                //    WebUtility.CheckScriptString(processKey.Value)));

                //Response.End();
			}
			catch (System.Exception ex)
			{
				WebUtility.ShowClientError(ex.Message, ex.StackTrace, "错误");
			}
		}
	}
}