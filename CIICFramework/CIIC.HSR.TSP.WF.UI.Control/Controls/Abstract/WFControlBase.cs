using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Web.Library.Script;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CIIC.HSR.TSP.WebComponents.Extensions;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using CIIC.HSR.TSP.WF.Bizlet.Common;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract
{
	public abstract class WFControlBase : WidgetBase
	{
		private string defaultCommentsControlId = "wfComments";

		protected WFControlBase(ViewContext vc, ViewDataDictionary vdd)
			: base(vc, vdd)
		{
			InitPropertiesByAttributes();
		}
        
		internal protected virtual WidgetBase Widget
		{
			get;
			set;
		}

		public string ActionUrl
		{
			get;
			set;
		}

		/// <summary>
		/// 调用前客户端事件句柄
		/// </summary>
		public string BeforeClick
		{
			get;
			set;
		}

		/// <summary>
		/// 调用后客户端事件句柄
		/// </summary>
		public string AfterClick
		{
			get;
			set;
		}

		/// <summary>
		/// 按钮对应的行为编码
		/// </summary>
		public string ActionResult
		{
			get;
			set;
		}
		/// <summary>
		/// 审核意见控件ID
		/// </summary>
		public string CommentsControlId
		{
			get
			{
				return defaultCommentsControlId;
			}
			set
			{
				this.defaultCommentsControlId = value;
			}
		}

		protected virtual string ClientButtonClickScript
		{
			get;
			set;
		}

		/// <summary>
		/// 准备需要序列化到客户端的参数
		/// </summary>
		/// <returns></returns>
		protected virtual WFParameterWithResponseBase PrepareParameters()
		{
			return null;
		}

		/// <summary>
		/// 初始化Widget的额外属性。派生类无需执行基类的此方法
		/// </summary>
        /// <param name="widget"></param>
		protected virtual void InitWidgetAttributes(WidgetBase widget)
		{
		}

		protected virtual bool GetEnabled()
		{
			return true;
		}

		protected virtual bool GetVisible()
		{
			return true;
		}

		public override void WriteHtml(System.IO.StringWriter writer)
		{
			WFParameterWithResponseBase parameter = PrepareParameters();

			this.InitBasicParameterProperties(parameter);

			if (this.Widget != null)
			{
				this.Widget.Id = this.Id;
				this.Widget.Name = this.Name;

				InitWidgetAttributes(this.Widget);

				if (parameter != null)
				{
					WfClientJsonConverterHelper.Instance.RegisterConverters();

					string serializedParam = JSONSerializerExecute.Serialize(parameter);

					this.Widget.HtmlAttributes[Consts.WFParas] = serializedParam;
				}

				writer.Write(this.Widget.ToString());
				this.ViewContext.HttpContext.RegisterWebResource(writer, "WFControlsJS", this.GetType(), "CIIC.HSR.TSP.WF.UI.Control.JS.WFControl.js");
			}

			base.WriteHtml(writer);
		}

		public void InitBasicParameterProperties(WFParameterWithResponseBase parameter)
		{
			if (parameter != null)
			{
				parameter.ActionUrl = this.ActionUrl;
				parameter.RuntimeContext = null;
				parameter.BeforeClick = this.BeforeClick;
				parameter.AfterClick = this.AfterClick;
				parameter.CommentsControlId = CommentsControlId;
				WFUIRuntimeContext runtime = this.ViewContext.HttpContext.Request.GetWFContext();

				if (runtime != null && runtime.Process != null)
				{
					parameter.ProcessId = runtime.Process.ID;
					parameter.ResourceId = runtime.Process.ResourceID;
					parameter.ActivityId = runtime.ActivityID;
				}
			}
		}

		private void InitPropertiesByAttributes()
		{
			WFControlDescriptionAttribute attr = (WFControlDescriptionAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(WFControlDescriptionAttribute));

			if (attr != null)
			{
				if (string.IsNullOrEmpty(attr.DefaultActionUrl) == false)
				{
					Controller controller = (Controller)this.ViewContext.Controller;
					this.ActionUrl = controller.Url.Action(attr.DefaultActionUrl, "WFDefaultOperation");
				}

				this.ClientButtonClickScript = attr.ClientButtonClickScript;
			}
		}
	}
}
