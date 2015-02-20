using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.TextBox;
using CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Proxies;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using MCS.Library.WF.Contracts.Workflow.Runtime;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.Comments
{

	public class WFComments<TModel> : WFControlBase
	{
		private int m_Lines = 1;
		private bool m_Enabled = true;
		private const string DefaultCommentsControlID = "wfComments";

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="vc"></param>
		/// <param name="vdd"></param>
		public WFComments(ViewContext vc, ViewDataDictionary vdd)
			: base(vc, vdd)
		{

		}
		/// <summary>
		/// 文本控件
		/// </summary>
		internal TextBox<TModel, string> InnerTextBox { get; set; }

		///// <summary>
		///// 行数
		///// </summary>
		public int Lines
		{
			get
			{
				return m_Lines;
			}
			set
			{
				this.m_Lines = value;
			}
		}

		/// <summary>
		/// 列数
		/// </summary>
		public int Collumns { get; set; }

		/// <summary>
		/// 是否可用
		/// </summary>
		public bool Enabled
		{
			get
			{
				return m_Enabled;
			}
			set
			{
				m_Enabled = value;
			}
		}

		/// <summary>
		/// 意见输入控件容器ID
		/// </summary>
		public string OpinionContainerId
		{
			get;
			set;
		}

		protected override WFParameterWithResponseBase PrepareParameters()
		{
			WFCommentsParameter param = new WFCommentsParameter();

			WfClientOpinion clientOpinion = GetCurrentOpinion();

			if (clientOpinion != null && string.IsNullOrEmpty(clientOpinion.ID) == false)
			{
				param.ClientOpinionId = clientOpinion.ID;
			}

			return param;
		}

		protected override void InitWidgetAttributes(WidgetBase widget)
		{
			widget.HtmlAttributes.Remove("TextBoxData");
			widget.HtmlAttributes.Remove("tbIdentifier");
		}

		public override void WriteHtml(System.IO.StringWriter stringWriter)
		{
			WFUIRuntimeContext runtime = this.ViewContext.HttpContext.Request.GetWFContext();

			if (runtime.Process == null || (runtime.Process != null && runtime.Process.AuthorizationInfo.InMoveToMode))
			{
				//意见控件Id赋值
				if (string.IsNullOrEmpty(this.Id))
				{
					this.Id = DefaultCommentsControlID;
				}

				if (string.IsNullOrEmpty(this.Name))
				{
					this.Name = DefaultCommentsControlID;
				}

                InnerTextBox.Id = !string.IsNullOrEmpty(this.Id) ? this.Id : this.Name;
				InnerTextBox.Name = this.Name;

				//取得意见
				WfClientOpinion clientOpinion = GetCurrentOpinion();
				if (clientOpinion != null)
				{
                    if (clientOpinion.Content == "{{**string.Empty**}}")
                    {
                        clientOpinion.Content = "";
                    }
					InnerTextBox.Text = clientOpinion.Content;
				}
				InnerTextBox.Enabled = this.Enabled;
				InnerTextBox.IsBound = false;
				InnerTextBox.Collumns = this.Collumns;
				InnerTextBox.Lines = this.Lines;

				this.Widget = this.InnerTextBox;
				HtmlAttributes.Clear();
            
                string strScript = @"<script language=""javascript"" type=""text/javascript"">
                     $(function () {{
                        $('#{0}').keyup(function(){{
                            $.fn.HSR.Controls.WFComments.InputLimit('{1}');
                        }});
                    }})
                    </script>";
                stringWriter.Write(string.Format(strScript, InnerTextBox.Id, InnerTextBox.Id));

				base.WriteHtml(stringWriter);
			}
			else
			{
				if (!string.IsNullOrEmpty(this.OpinionContainerId))
				{
					string strScript = @"<script language=""javascript"" type=""text/javascript"">
                     $(function () {$(""#" + this.OpinionContainerId + @""").hide(); })</script>";
					stringWriter.Write(strScript);
					base.WriteHtml(stringWriter);
				}
			}
		}


		private WfClientOpinion GetCurrentOpinion()
		{
			WFUIRuntimeContext runtime = this.ViewContext.HttpContext.Request.GetWFContext();
			WfClientOpinion clientOpinion = null;

			if (runtime != null && runtime.Process != null)
				clientOpinion = runtime.Process.CurrentOpinion;

			return clientOpinion;
		}
	}
}
