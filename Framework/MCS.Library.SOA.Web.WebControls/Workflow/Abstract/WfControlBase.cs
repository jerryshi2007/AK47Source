using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using MCS.Web.Library.Script;
using System.Web;
using MCS.Library.Core;
using System.Reflection;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.WebControls.Workflow.Abstract.wfOperationNotifier.htm", "text/html")]

namespace MCS.Web.WebControls
{
	[Flags]
	public enum WfControlAccessbility
	{
		None = 0,
		Visible = 1,
		ReadOnly = 2,
		Enabled = 4
	}

	/// <summary>
	/// 修改流程后的事件
	/// </summary>
	/// <param name="actDesp"></param>
	/// <param name="dataContext"></param>
	public delegate void ProcessChangedEventHandler(IWfProcess process);

	/// <summary>
	/// 得到控件的可访问性
	/// </summary>
	/// <param name="dataContext"></param>
	/// <returns></returns>
	public delegate WfControlAccessbility GetAccessbilityEventHandler(WfControlAccessbility defaultAccessbility);

	/// <summary>
	/// 当创建完执行器之后
	/// </summary>
	/// <param name="executor"></param>
	public delegate void AfterCreateExecutorHandler(WfExecutorBase executor);

	/// <summary>
	/// 工作流控件的基类
	/// </summary>
	public abstract class WfControlBase : ScriptControlBase, INamingContainer
	{
		/// <summary>
		/// 得到控件可访问性的事件
		/// </summary>
		public event GetAccessbilityEventHandler GetAccessbility;

		/// <summary>
		/// 创建完Executor之后的事件
		/// </summary>
		public event AfterCreateExecutorHandler AfterCreateExecutor;

		protected WfControlBase()
			: base(true, HtmlTextWriterTag.Div)
		{
		}

		/// <summary>
		/// 添加一个显示通知的控件，这样有流程控件的表单，就可以显示提醒的内容
		/// </summary>
		/// <param name="e"></param>
		protected override void OnInit(EventArgs e)
		{
			this.Controls.Add(new NotifyDialogControl());
			WebBrowserWrapper.Register(this);

			base.OnInit(e);
		}

		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			EnsureChildControls();
			base.OnPagePreLoad(sender, e);
		}

		/// <summary>
		/// 在表单上注册一个隐藏域，接受来自于其它窗口的通知文本，然后触发控件的客户端事件
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			Page.ClientScript.RegisterClientScriptBlock(typeof(WfControlBase), "WfOperationNotifier",
					ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
					"MCS.Web.WebControls.Workflow.Abstract.wfOperationNotifier.htm"));

			base.OnPreRender(e);
		}

		public static void ResponsePostBackPlaceHolder()
		{
			HttpContext.Current.Response.Write("<div id='WfControlBase'></div>");
		}

		/// <summary>
		/// 得到控件的可访问性
		/// </summary>
		/// <returns></returns>
		protected virtual WfControlAccessbility OnGetAccessbility(WfControlAccessbility defaultAccessbility)
		{
			WfControlAccessbility result = defaultAccessbility;

			if (GetAccessbility != null)
				result = GetAccessbility(result);

			return result;
		}

		protected virtual void OnAfterCreateExecutor(WfExecutorBase executor)
		{
			executor.NullCheck("executor");

			if (AfterCreateExecutor != null)
				AfterCreateExecutor(executor);

			ProcessProgress.Current.RegisterResponser(SubmitButtonProgressResponser.Instance);
		}

		/// <summary>
		/// 创建一个隐藏的服务器端SubmitButton。在客户端激活这个Button的Click会通过PostBack
		/// </summary>
		/// <param name="id">Button的ID</param>
		/// <param name="popupCaption">如果指定此参数，会显示量程计，并且显示此标题</param>
		/// <param name="relativeControlID">相关控件ID。Button Click后，会Disable掉相关控件</param>
		/// <param name="clickHandler">Button Click后的服务器端事件</param>
		/// <returns>SubmitButton的实例</returns>
		protected virtual SubmitButton CreateHiddenButton(string id, string popupCaption, string relativeControlID, EventHandler clickHandler)
		{
			SubmitButton btn = new SubmitButton();

			btn.ID = id;
			btn.Click += clickHandler;
			btn.Style["display"] = "none";
			btn.PopupCaption = popupCaption;
			btn.RelativeControlID = relativeControlID;
			btn.ProgressMode = SubmitButtonProgressMode.BySteps;
			btn.MinStep = 0;
			btn.MaxStep = 100;

			return btn;
		}
	}
}
