using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Web.WebControls;
using MCS.Web.Library;

namespace MCS.Library.SOA.Web.WebControls.Test.SubmitButton
{
	public partial class SubmitButtonWithProgress : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void submitWithProcess_Click(object sender, EventArgs e)
		{
			try
			{
				ProcessProgress.Current.RegisterResponser(SubmitButtonProgressResponser.Instance);

				ProcessProgress.Current.Response();

				for (int i = 0; i < 100; i++)
				{
					ProcessProgress.Current.Increment();
					ProcessProgress.Current.StatusText = string.Format("执行到第{0}步", i + 1);
					ProcessProgress.Current.Response();

					//if (i > 30)
					//    throw new ApplicationException("这是模拟出来的错误");

					Thread.Sleep(100);
				}
			}
			catch (System.Exception ex)
			{
				WebUtility.ResponseShowClientErrorScriptBlock(ex);
			}
			finally
			{
				this.Response.Write(MCS.Web.WebControls.SubmitButton.GetResetAllParentButtonsScript(true));
				this.Response.End();
			}
		}
	}
}