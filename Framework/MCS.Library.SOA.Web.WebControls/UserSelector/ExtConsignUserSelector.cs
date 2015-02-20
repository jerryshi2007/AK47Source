using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.Library.Script;
using System.Web.UI.HtmlControls;
using MCS.Library.OGUPermission;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Globalization;
using MCS.Library.SOA.DataObjects.Workflow;

[assembly: WebResource("MCS.Web.WebControls.UserSelector.ExtConsignUserSelector.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.ExtConsignUserSelector", "MCS.Web.WebControls.UserSelector.ExtConsignUserSelector.js")]
	[DialogContent("MCS.Web.WebControls.UserSelector.ExtConsignUserSelectorTemplate.htm", "MCS.Library.SOA.Web.WebControls")]
	[ToolboxData("<{0}:ExtConsignUserSelector runat=server></{0}:ExtConsignUserSelector>")]
	public class ExtConsignUserSelector : DialogControlBase<ExtConsignUserSelectorParams>
	{
		private ExtOuUserInputControl userInput = new ExtOuUserInputControl() { InvokeWithoutViewState = true };
		private RadioButtonList consignType = new RadioButtonList();
		private RadioButtonList sequenceType = new RadioButtonList();

		public ExtConsignUserSelector()
		{
			JSONSerializerExecute.RegisterConverter(typeof(DeluxeTreeNodeConverter));
			JSONSerializerExecute.RegisterConverter(typeof(DeluxeTreeNodeListConverter));
			JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));
		}

		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			EnsureChildControls();
			base.OnPagePreLoad(sender, e);
		}

		#region Properties
		/// <summary>
		/// 机构人员
		/// </summary>
		public IOrganization Root
		{
			get
			{
				return this.ControlParams.Root;
			}
			set
			{
				this.ControlParams.Root = value;
			}
		}

		/// <summary>
		/// 能够列出哪些对象（机构、人员、组）
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("listMask"), DefaultValue(UserControlObjectMask.Organization | UserControlObjectMask.User | UserControlObjectMask.Sideline)]
		public UserControlObjectMask ListMask
		{
			get
			{
				return ControlParams.ListMask;
			}
			set
			{
				ControlParams.ListMask = value;
			}
		}

		/// <summary>
		/// 能够选择哪些对象（机构、人员、组）
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("selectMask"), DefaultValue(UserControlObjectMask.User)]
		public UserControlObjectMask SelectMask
		{
			get
			{
				return ControlParams.SelectMask;
			}
			set
			{
				ControlParams.SelectMask = value;
			}
		}

		/// <summary>
		/// 是否多选
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("multiSelect")]
		[DefaultValue(false)]
		public bool MultiSelect
		{
			get
			{
				return ControlParams.MultiSelect;
			}
			set
			{
				ControlParams.MultiSelect = value;
			}
		}

		/// <summary>
		/// 是否是会签控件
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("isConsign")]
		[DefaultValue(false)]
		public bool IsConsign
		{
			get
			{
				return ControlParams.IsConsign;
			}
			set
			{
				ControlParams.IsConsign = value;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("userInputClientID")]
		private string UserInputClientID
		{
			get
			{
				return userInput.ClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("consignTypeSelectClientID")]
		private string ConsignTypeSelectClientID
		{
			get
			{
				return consignType.ClientID;
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("sequenceTypeSelectClientID")]
		private string SequenceTypeSelectClientID
		{
			get
			{
				return sequenceType.ClientID;
			}
		}
		#endregion Properties

		#region Protected
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (this.Page.IsCallback)
				EnsureChildControls();
		}

		protected override void InitDialogContent(Control container)
		{
			base.InitDialogContent(container);

			this.ID = "userSelectorDialog";

			HtmlForm form = (HtmlForm)WebControlUtility.FindParentControl(this, typeof(HtmlForm), true);

			if (form != null)
			{
				form.Style["width"] = "100%";
				form.Style["height"] = "100%";
			}

			this.Width = Unit.Percentage(100);
			this.Height = Unit.Percentage(100);

			InitUserInputControl(WebControlUtility.FindControlByHtmlIDProperty(container, "userInputContainer", true));

			if (this.IsConsign)
			{
				InitConsignTypeSelector(WebControlUtility.FindControlByHtmlIDProperty(container, "consignTypeContainer", true));
				InitSequenceTypeSelector(WebControlUtility.FindControlByHtmlIDProperty(container, "sequenceTypeContainer", true));
			}
		}

		protected override string GetDialogFeature()
		{
			WindowFeature feature = new WindowFeature();

			feature.Width = 640;
			feature.Height = 480;
			feature.Resizable = true;
			feature.ShowStatusBar = false;
			feature.ShowScrollBars = false;
			feature.Center = true;

			return feature.ToDialogFeatureClientString();
		}

		protected override void InitConfirmButton(HtmlInputButton confirmButton)
		{
			base.InitConfirmButton(confirmButton);

			confirmButton.Attributes["onclick"] = "onConfirmButtonClick();";
		}
		#endregion

		#region Private
		private void InitUserInputControl(Control container)
		{
			if (container != null)
			{
				userInput.Width = Unit.Percentage(98);
				userInput.Height = Unit.Percentage(100);
				userInput.ID = "userInput";
				userInput.SelectMask = this.SelectMask;
				userInput.ListMask = this.ListMask;
				userInput.MultiSelect = this.MultiSelect;

				if (this.Root != null)
					userInput.RootPath = this.Root.FullPath;

				container.Controls.Add(userInput);
			}
		}

		private void InitConsignTypeSelector(Control container)
		{
			if (container != null)
			{
				Label caption = new Label();
				caption.Text = Translator.Translate(Define.DefaultCulture, "处理类型：");
				container.Controls.Add(caption);

				consignType.ID = "consignType";
				consignType.Items.Add(new ListItem(Translator.Translate(Define.DefaultCulture, "所有人通过"), ((int)WfBranchProcessBlockingType.WaitAllBranchProcessesComplete).ToString()));
				consignType.Items.Add(new ListItem(Translator.Translate(Define.DefaultCulture, "任意人通过"), ((int)WfBranchProcessBlockingType.WaitAnyoneBranchProcessComplete).ToString()));
				consignType.RepeatLayout = RepeatLayout.Flow;
				consignType.RepeatDirection = RepeatDirection.Horizontal;
				consignType.SelectedIndex = 0;

				container.Controls.Add(consignType);
			}
		}

		private void InitSequenceTypeSelector(Control container)
		{
			if (container != null)
			{
				Label caption = new Label();
				caption.Text = Translator.Translate(Define.DefaultCulture, "执行顺序：");
				container.Controls.Add(caption);

				sequenceType.ID = "sequenceType";
				sequenceType.Items.Add(new ListItem(Translator.Translate(Define.DefaultCulture, "并行"), ((int)WfBranchProcessExecuteSequence.Parallel).ToString()));
				sequenceType.Items.Add(new ListItem(Translator.Translate(Define.DefaultCulture, "串行"), ((int)WfBranchProcessExecuteSequence.Serial).ToString()));
				sequenceType.RepeatLayout = RepeatLayout.Flow;
				sequenceType.RepeatDirection = RepeatDirection.Horizontal;
				sequenceType.SelectedIndex = 0;

				container.Controls.Add(sequenceType);
			}
		}
		#endregion
	}
}
