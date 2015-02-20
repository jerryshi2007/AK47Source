using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;
using MCS.Library.Core;

namespace MCS.Library.SOA.Web.WebControls.Test.PostProgressControl
{
	public partial class SimplePostProgressControlTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void uploadProgress_DoPostedData(object sender, PostProgressDoPostedDataEventArgs eventArgs)
		{
			ProcessProgress.Current.Initialize(0, eventArgs.Steps.Count, 0);
			ProcessProgress.Current.Response();

			//接收客户端传递的附加数据
			ProcessProgress.Current.Response(eventArgs.ClientExtraPostedData);
			Thread.Sleep(1000);	//假装等待

			while (ProcessProgress.Current.CurrentStep < ProcessProgress.Current.MaxStep)
			{
				ProcessProgress.Current.Increment(string.Format("已经处理完成第\"{0}\"步", ProcessProgress.Current.CurrentStep));
				ProcessProgress.Current.Response();

				ProcessProgress.Current.Output.WriteLine("Processed = \"{0}\"", (ProcessProgress.Current.CurrentStep));

				Thread.Sleep(1000);	//假装等待
			}

			ProcessProgress.Current.Response("处理完成");

			eventArgs.Result.DataChanged = true;
			eventArgs.Result.CloseWindow = false;
			eventArgs.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}
	}
}