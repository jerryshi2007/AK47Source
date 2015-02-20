using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
	public delegate void DoUploadProgressDelegate(HttpPostedFile file, UploadProgressResult result);
	public delegate void UploadProgressContentInitedEventHandler(object sender, UploadProgressContentEventArgs eventArgs);

	/// <summary>
	/// 初始化内容的事件
	/// </summary>
	public class UploadProgressContentEventArgs : System.EventArgs
	{
		internal UploadProgressContentEventArgs(Control container)
		{
			this.AfterFileSelectorContainer = container.FindControlByID("afterFileSelector", true);
		}

		public Control AfterFileSelectorContainer
		{
			get;
			private set;
		}
	}

	/// <summary>
	/// 上传处理的状态信息
	/// </summary>
	public class UploadProgressStatus
	{
		public UploadProgressStatus()
		{
		}

		public int MinStep
		{
			get
			{
				return ProcessProgress.Current.MinStep;
			}
			set
			{
				ProcessProgress.Current.MinStep = value;
			}
		}

		public int MaxStep
		{
			get
			{
				return ProcessProgress.Current.MaxStep;
			}
			set
			{
				ProcessProgress.Current.MaxStep = value;
			}
		}

		public int CurrentStep
		{
			get
			{
				return ProcessProgress.Current.CurrentStep;
			}
			set
			{
				ProcessProgress.Current.CurrentStep = value;
			}
		}

		public string StatusText
		{
			get
			{
				return ProcessProgress.Current.StatusText;
			}
			set
			{
				ProcessProgress.Current.StatusText = value;
			}
		}

		public string ProcessListItem
		{
			get;
			set;
		}

		public void Response()
		{
			ProcessProgress.Current.Response();
		}
	}

	public class UploadProgressResponser : IProcessProgressResponser
	{
		public static readonly UploadProgressResponser Instance = new UploadProgressResponser();

		private UploadProgressResponser()
		{
		}

		#region IProcessProgressResponser Members

		public void Register(ProcessProgress progress)
		{
			progress.MinStep = 1;
			progress.MaxStep = 100;
			progress.CurrentStep = 1;
			progress.StatusText = string.Empty;
		}

		public void Response(ProcessProgress progress)
		{
			HttpResponse response = HttpContext.Current.Response;

			response.Write(string.Format("<script type=\"text/javascript\">top.document.getElementById(\"processInfo\").value=\"{0}\";\ntop.document.getElementById(\"processInfoChanged\").click();</script>",
				WebUtility.CheckScriptString(JSONSerializerExecute.Serialize(progress), false)));

			response.Flush();
		}

		#endregion
	}

	public class UploadProgressResult
	{
		public UploadProgressResult()
		{
			DataChanged = false;
			CloseWindow = true;
			ProcessLog = string.Empty;
		}

		public bool DataChanged
		{
			get;
			set;
		}

		public bool CloseWindow
		{
			get;
			set;
		}

		public string ProcessLog
		{
			get;
			set;
		}

		public string Data
		{
			get;
			set;
		}

		public void Response()
		{
			HttpResponse response = HttpContext.Current.Response;

			response.Write(string.Format("<script type=\"text/javascript\">top.document.getElementById(\"processResult\").value=\"{0}\";\ntop.document.getElementById(\"processResultButton\").click();</script>",
				WebUtility.CheckScriptString(JSONSerializerExecute.Serialize(this), false)));
		}
	}
}
