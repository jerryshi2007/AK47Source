using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using System.Web;
using System.Collections.Specialized;
using MCS.Library.Core;
using MCS.Library.Passport;
using MCS.Web.Library.MVC;

[assembly: WebResource("MCS.Web.WebControls.Workflow.LockContextControl.LockContextControl.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 3)]
	//[RequiredScript(typeof(PopUpMessageControl), 5)]
	[ClientScriptResource("MCS.Web.WebControls.LockContextControl",
	  "MCS.Web.WebControls.Workflow.LockContextControl.LockContextControl.js")]
	public class LockContextControl : ScriptControlBase
	{
		private CustomValidator submitValidator = new CustomValidator();

		public LockContextControl()
			: base(HtmlTextWriterTag.Div)
		{
			JSONSerializerExecute.RegisterConverter(typeof(LockConverter));
		}

		#region Properties
		/// <summary>
		/// 是否有效
		/// </summary>
		[DefaultValue(true), Category("Appearance"), ScriptControlProperty, ClientPropertyName("enabled"), Description("是否有效"), Browsable(false)]
		public override bool Enabled
		{
			get
			{
				return LockConfigSetting.GetConfig().Enabled;
			}
		}

		[ScriptControlProperty, ClientPropertyName("callBackUrl"), Description("回调的url"), Browsable(false)]
		private string CallBackUrl
		{
			get
			{
				string result = string.Empty;

				if (HttpContext.Current != null)
				{
					string url = WebUtility.GetRequestExecutionUrl(string.Empty);

					NameValueCollection originalParams = UriHelper.GetUriParamsCollection(url);

					originalParams.Add("_op", "lockCallBack");
					originalParams.Remove(PassportManager.TicketParamName);

					result = UriHelper.CombineUrlParams(url, originalParams);
				}

				return result;
			}
		}

		/// <summary>
		/// 检查锁的时间间隔,默认为1分钟
		/// </summary>
		[ScriptControlProperty, ClientPropertyName("checkInterval"), Description("检查锁的时间间隔,默认为1分钟"), Browsable(false)]
		private TimeSpan CheckInterval
		{
			get
			{
				return GetPropertyValue<TimeSpan>("CheckInterval", LockConfigSetting.GetConfig().DefaultCheckIntervalTime);
			}
		}

		[ScriptControlProperty, ClientPropertyName("locks"), Description("锁对象集合"), Browsable(false)]
		private Lock[] Locks
		{
			get
			{
				Lock[] result = null;

				if (DesignMode == false)
					//result = LockContext.Current.Locks.ToArray();
					result = WfClientContext.Current.Locks.ToArray();
				return result;
			}
		}
		#endregion
		protected override void OnInit(EventArgs e)
		{
			if (WebUtility.GetRequestQueryString("_op", string.Empty) == "lockCallBack")
			{
				if (string.Compare(Page.Request.HttpMethod, "POST", true) == 0)
				{
					string cmd = WebUtility.GetRequestQueryString("cmd", string.Empty);

					string requestData = string.Empty;
					string result = string.Empty;

					try
					{
						using (StreamReader sr = new StreamReader(Page.Request.InputStream))
						{
							requestData = sr.ReadToEnd();

							Lock[] locks = (Lock[])JSONSerializerExecute.DeserializeObject(requestData, typeof(Lock[]));
							List<Lock> addLocks = new List<Lock>();

							switch (cmd)
							{
								case "checkLock":
									Array.ForEach(locks, l => addLocks.Add(LockAdapter.SetLock(l).NewLock));
									break;
								case "unlock":
									LockAdapter.Unlock(locks);
									break;
								default:
									throw new ApplicationException(string.Format("Invalid command {0}", cmd));
							}

							result = JSONSerializerExecute.Serialize(addLocks.ToArray());
						}
					}
					catch (System.Exception ex)
					{
						var err = new { type = "$ErrorType", message = ex.Message };

						result = JSONSerializerExecute.Serialize(err);
					}
					finally
					{
						Page.Response.Write(result);
						Page.Response.End();
					}
				}
			}
			else
				base.OnInit(e);
		}

		protected override void CreateChildControls()
		{
			if (this.Page.Items.Contains("lockSubmitValidator") == false)
			{
				this.Controls.Add(this.submitValidator);
				this.Page.Items.Add("lockSubmitValidator", "exist");
			}

			base.CreateChildControls();
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.submitValidator.ClientValidationFunction = "$HBRootNS.LockContextControl.lockClientValidate";
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if (DesignMode)
				writer.Write("LockContextControl");
			else
				base.Render(writer);
		}
	}
}
