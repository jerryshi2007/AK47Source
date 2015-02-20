using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using MCS.Library.Core;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	public class ImportExecutor
	{
		private HttpPostedFile file = null;
		private MCS.Web.WebControls.UploadProgressResult result = null;
		private SCObjectSet objectSet = null;
		private System.Text.StringBuilder log = null;
		private UploadProgressStatus status = null;
		private int actionIndex = 0;
		private List<IImportAction> actions = new List<IImportAction>();

		public ImportExecutor(HttpPostedFile file, MCS.Web.WebControls.UploadProgressResult result)
		{
			this.file = file;
			this.result = result;
			this.log = new System.Text.StringBuilder(4096);
			this.status = new UploadProgressStatus();
		}

		public IImportAction this[int index]
		{
			get
			{
				return this.actions[index];
			}

			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				this.actions[index] = value;
			}
		}

		public void AddAction(ImportAction action)
		{
			if (this.actions == null)
				throw new ArgumentNullException("action");

			this.actions.Add(action);
		}

		public void Execute()
		{
			this.result.CloseWindow = false;
			this.result.DataChanged = true;

			ImportContext context = new ImportContext(this);

			if (this.EnsureFileLoaded())
			{
				this.actionIndex = 0;
				foreach (var action in this.actions)
				{
					try
					{
						action.DoImport(this.objectSet, context);
					}
					catch (Exception ex)
					{
						this.log.AppendFormat("发生错误:{0} 已终止了操作,堆栈:\r\n{1}\r\n", ex.Message, ex.StackTrace);
						break;
					}

					this.actionIndex++;
				}
			}

			this.log.AppendLine("导入操作结束");
			this.status.CurrentStep = this.status.MaxStep;
			this.status.StatusText = "结束";
			this.status.Response();
			this.result.ProcessLog = this.log.ToString();
		}

		public void Clear()
		{
			this.log.Clear();
		}

		private bool EnsureFileLoaded()
		{
			bool result = false;
			if (this.objectSet == null)
			{
				SCObjectSet aObjectSet = new SCObjectSet();

				try
				{
					using (System.IO.StreamReader reader = new StreamReader(this.file.InputStream))
					{
						aObjectSet.Load(reader);
					}

					this.objectSet = aObjectSet;
					result = true;
				}
				catch (Exception ex)
				{
					this.status.StatusText = "上传的文件格式错误无法解析:" + ex.Message;
					this.status.Response();
					this.log.AppendFormat("无法读取上传的文件：" + ex.Message);
				}

				this.log.AppendLine("已读取上传的文件，正在查找对象");
				this.status.MinStep = 0;
				this.status.CurrentStep = 0;
				this.status.MaxStep = 100;
				this.status.StatusText = "开始导入";
				this.status.Response();
			}

			return result;
		}

		public class ImportContext : IImportContext
		{
			private ImportExecutor executor = null;

			internal ImportContext(ImportExecutor executor)
			{
				this.executor = executor;
			}

			public void SetStatus(int currentStep, int maxStep, string message)
			{
				this.executor.status.MaxStep = 100;
				var curStep = (int)((100.0 * this.executor.actionIndex / this.executor.actions.Count) + ((100.0 / this.executor.actions.Count) * currentStep / maxStep));
				this.executor.status.CurrentStep = curStep < 0 ? 0 : (curStep > 100 ? 100 : curStep);
				this.executor.status.StatusText = message;
				this.executor.status.Response();
			}

			public void AppendLog(string message)
			{
				this.executor.log.AppendLine(message);
			}

			public void AppendLogFormat(string format, object arg)
			{
				this.executor.log.AppendFormat(format, arg);
			}

			public void AppendLogFormat(string format, object arg, object arg1)
			{
				this.executor.log.AppendFormat(format, arg, arg1);
			}

			public void AppendLogFormat(string format, object arg, object arg1, object arg2)
			{
				this.executor.log.AppendFormat(format, arg, arg1, arg2);
			}

			public void AppendLogFormat(string format, params object[] args)
			{
				this.executor.log.AppendFormat(format, args);
			}

			public void SetStatusAndLog(int currentStep, int maxStep, string message)
			{
				this.SetStatus(currentStep, maxStep, message);
				this.AppendLog(message);
			}

			public void SetSubStatusAndLog(int currentStep, int maxStep, string message)
			{
				this.AppendLog(message);
				HttpResponse response = HttpContext.Current.Response;
				ProcessProgress progress = ProcessProgress.Current;
				progress.CurrentStep = currentStep;
				progress.MaxStep = maxStep;
				progress.StatusText = message;

				response.Write(string.Format("<script type=\"text/javascript\">top.document.getElementById(\"processInfo\").value=\"{0}\";\ntop.document.getElementById(\"subProcessInfoChanged\").click();</script>",
				WebUtility.CheckScriptString(JSONSerializerExecute.Serialize(progress), false)));
				response.Flush();
			}


			public int ErrorCount { get; set; }
		}
	}

	public abstract class ImportAction : IImportAction
	{
		private ImportExecutor executor;

		internal ImportExecutor Executor
		{
			get { return this.executor; }
			set { this.executor = value; }
		}

		public abstract void DoImport(SCObjectSet objectSet, IImportContext context);
	}
}