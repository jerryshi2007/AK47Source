using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using MCS.Library.Core;

namespace MCS.Web.Library.Script
{
	/// <summary>
	/// 静态回调方法加载控件后的事件
	/// </summary>
	/// <param name="targetControl"></param>
	public delegate void StaticCallBackProxyControlLoadedEventHandler(Control targetControl);

	/// <summary>
	/// 实现静态回调的代理对象，从ScriptControlBase基类派生
	/// </summary>
	public class StaticCallBackProxy : Control, ICallbackEventHandler
	{
		internal const string STATIC_CALLBACKPROXY_CONTROL_ID = "__staticCallbackProxyControl";

		private string _callbackArgument = string.Empty;

		/// <summary>
		/// 目标控件被加载
		/// </summary>
		public event StaticCallBackProxyControlLoadedEventHandler TargetControlLoaded;

		#region constructor
		/// <summary>
		/// 构造函数
		/// </summary>
		/// 
		public StaticCallBackProxy()
			: base()
		{
		}

		#endregion

		/// <summary>
		/// 静态实例
		/// </summary>
		public static StaticCallBackProxy Instance
		{
			get
			{
				StaticCallBackProxy staticCP = HttpContext.Current.Items["StaticCallbackProxy"] as StaticCallBackProxy;

				if (staticCP == null)
				{
					staticCP = new StaticCallBackProxy();
					staticCP.ID = STATIC_CALLBACKPROXY_CONTROL_ID;
					staticCP.EnableViewState = false;
					staticCP.Visible = false;

					HttpContext.Current.Items["StaticCallbackProxy"] = staticCP;
				}

				return staticCP;
			}
		}

		private static bool IsRegistered
		{
			get
			{
				bool registered = false;

				if (HttpContext.Current.Items["IsStaticCallbackProxyRegistered"] != null)
					registered = (bool)HttpContext.Current.Items["IsStaticCallbackProxyRegistered"];

				return registered;
			}
			set
			{
				HttpContext.Current.Items["IsStaticCallbackProxyRegistered"] = value;
			}
		}

		internal static void RegisterStaticCallbackProxyControl(Page page)
		{
			if (StaticCallBackProxy.IsRegistered == false)
			{
				if (page.IsCallback)
				{
					page.Controls.Add(StaticCallBackProxy.Instance);
					StaticCallBackProxy.IsRegistered = true;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventArgument"></param>
		void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
		{
			this._callbackArgument = eventArgument;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string ICallbackEventHandler.GetCallbackResult()
		{
			string argument = this._callbackArgument;
			this._callbackArgument = null;

			Dictionary<string, object> callInfo = JSONSerializerExecute.DeserializeObject(argument, typeof(Dictionary<string, object>)) as Dictionary<string, object>;

			string serverControlType = (string)callInfo["serverControlType"];
			string originalControlID = (string)callInfo["originalControlID"];

			Page page = WebUtility.GetCurrentPage();
			Control control = null;
			
			if (originalControlID.IsNotEmpty())
				control = page.FindControl(originalControlID);

			if (control == null)
			{
				control = (Control)TypeCreator.CreateInstance(serverControlType);
				control.ID = originalControlID;
				page.Controls.Add(control);

				if (TargetControlLoaded != null)
					TargetControlLoaded(control);
			}

			return ScriptObjectBuilder.ExecuteCallbackMethod(control, callInfo);
		}
	}
}
