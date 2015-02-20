using System;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using MCS.Web.Library.Script;
using System.Web.UI.HtmlControls;
using MCS.Web.Library;
using System.Web.UI.WebControls;
using System.ComponentModel;

[assembly: WebResource("MCS.Web.WebControls.Opinion.OpinionInputDialog.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.OpinionInputDialog",
        "MCS.Web.WebControls.Opinion.OpinionInputDialog.js")]
    [DialogContent("MCS.Web.WebControls.Opinion.OpinionInputDialog.htm", "MCS.Library.SOA.Web.WebControls")]
	[ToolboxData("<{0}:OpinionInputDialog runat=server></{0}:OpinionInputDialog>")]
	public class OpinionInputDialog : DialogControlBase<OpinionInputDialogParams>
	{
		private OpinionReasonItemCollection reasons = new OpinionReasonItemCollection();

		[PersistenceMode(PersistenceMode.InnerProperty), Description("原因列表")]
		[MergableProperty(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue((string)null)]
		[Browsable(false)]
		public OpinionReasonItemCollection Reasons
		{
			get
			{
				return this.reasons;
			}
		}

		/// <summary>
		/// 是否允许空意见
		/// </summary>
		public bool AllowEmptyOpinion
		{
			get
			{
				return ControlParams.AllowEmptyOpinion;
			}
			set
			{
				ControlParams.AllowEmptyOpinion = value;
			}
		}

		/// <summary>
		/// 空意见的提示信息
		/// </summary>
		public string EmptyOpinionPrompt
		{
			get
			{
				return ControlParams.EmptyOpinionPrompt;
			}
			set
			{
				ControlParams.EmptyOpinionPrompt = value;
			}
		}

		/// <summary>
		/// 获取对话框外观属性
		/// </summary>
		/// <returns></returns>
		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 400;
			feature.Height = 300;
			feature.Resizable = false;
			feature.ShowStatusBar = false;
			feature.ShowScrollBars = false;
			feature.Center = true;

			return feature.ToDialogFeatureClientString();
		}

		/// <summary>
		/// 初始化确认按钮
		/// </summary>
		/// <param name="confirmButton"></param>
		protected override void InitConfirmButton(HtmlInputButton confirmButton)
		{
			confirmButton.Attributes["onclick"] = "onDialogConfirm();";
		}

		protected override void InitDialogContent(Control container)
		{
			base.InitDialogContent(container);

			this.ID = "OpinionInputDialog";

			Control opinionContainer = WebControlUtility.FindControlByHtmlIDProperty(container, "inputContainer", true);

			if (opinionContainer != null)
			{
				TextBox input = new TextBox();

				input.ID = "OpinionInput";
				input.TextMode = TextBoxMode.MultiLine;
				input.Width = Unit.Percentage(99);
				input.Height = Unit.Percentage(99);

				opinionContainer.Controls.Add(input);
			}

			HtmlInputHidden promptText = (HtmlInputHidden)WebControlUtility.FindControlByHtmlIDProperty(container, "promptText", true);

			if (promptText != null)
			{
				if (AllowEmptyOpinion)
					promptText.Value = string.Empty;
				else
					promptText.Value = EmptyOpinionPrompt;
			}		
		}

		protected override void OnPagePreRenderComplete(object sender, EventArgs e)
		{
			if (CurrentMode == ControlShowingMode.Dialog)
			{
                HtmlGenericControl reasonList = (HtmlGenericControl)WebControlUtility.FindControlByHtmlIDProperty(this, "reasonList", true);

                if (reasonList != null)
				{
					if (this.Reasons.Count > 0)
					{
                        reasonList.Style["display"] = "inline";

						HtmlSelect resonSelector = (HtmlSelect)WebControlUtility.FindControlByHtmlIDProperty(this, "resonSelector", true);

						if (resonSelector != null)
						{
							foreach (var reason in this.Reasons)
								resonSelector.Items.Add(new ListItem(reason.Description, reason.Key));
						}
					}	
				}
			}

			base.OnPagePreRenderComplete(sender, e);
		}
	}
}
