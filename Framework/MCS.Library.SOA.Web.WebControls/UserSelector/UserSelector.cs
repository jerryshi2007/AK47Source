using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using MCS.Web.Library.Script;
using MCS.Web.Library;

using MCS.Library.SOA.DataObjects;
using MCS.Library.OGUPermission;

[assembly: WebResource("MCS.Web.WebControls.UserSelector.userSelectorl.js", "application/x-javascript")]
//[assembly: WebResource("MCS.Web.WebControls.UserSelector.userSelectorTemplate.htm", "text/html")]

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 人员选择控件
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.UserSelector", "MCS.Web.WebControls.UserSelector.userSelectorl.js")]
    [DialogContent("MCS.Web.WebControls.UserSelector.userSelectorTemplate.htm", "MCS.Library.SOA.Web.WebControls")]
    [ToolboxData("<{0}:UserSelector runat=server></{0}:UserSelector>")]
    public class UserSelector : DialogControlBase<UserSelectorParams>
    {
        private OuUserInputControl userInput = new OuUserInputControl() { InvokeWithoutViewState = true };
        private HtmlGenericControl userInputTip = new HtmlGenericControl("div");

        /// <summary>
        /// 构造方法
        /// </summary>
        public UserSelector()
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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Page.IsCallback)
                EnsureChildControls();
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

        [DefaultValue(false)]
        public bool ShowOpinionInput
        {
            get
            {
                return ControlParams.ShowOpinionInput;
            }
            set
            {
                ControlParams.ShowOpinionInput = value;
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

        #endregion Properties

        #region Protected
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

            InitUserInputTipControl(WebControlUtility.FindControlByHtmlIDProperty(container, "userInputContainer", true));

            InitOpinionInoutControl();
        }

        protected override string GetDialogFeature()
        {
            WindowFeature feature = new WindowFeature();

            feature.Width = 420;

            if (ShowOpinionInput)
                feature.Height = 400;
            else
                feature.Height = 200;

            feature.Resizable = false;
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

		protected override void InitCancelButton(HtmlInputButton cancelButton)
		{
			base.InitCancelButton(cancelButton);

			cancelButton.Attributes["onclick"] = "onCancelButtonClick();";
		}
        #endregion

        #region Private
        private void InitOpinionInoutControl()
        {
            HtmlTableCell userInputCell = (HtmlTableCell)WebControlUtility.FindControlByHtmlIDProperty(this, "userInputCell", true);

            if (userInputCell != null)
            {
                if (ShowOpinionInput)
                {
                    userInputCell.Style["height"] = "64px";
                }
            }

            HtmlGenericControl opinionBody = (HtmlGenericControl)WebControlUtility.FindControlByHtmlIDProperty(this, "opinionBody", true);

            if (opinionBody != null)
            {
                if (ShowOpinionInput)
                    opinionBody.Style["display"] = "inline";
            }
        }

        private void InitUserInputControl(Control container)
        {
            if (container != null)
            {
                userInput.ID = "userInput";
                userInput.SelectMask = this.SelectMask;
                userInput.ListMask = this.ListMask;
                userInput.MultiSelect = this.MultiSelect;

                if (this.Root != null)
                    userInput.RootPath = this.Root.FullPath;

                userInput.Width = Unit.Percentage(80);

                container.Controls.Add(userInput);
            }
        }

        private void InitUserInputTipControl(Control container)
        {
            if (container != null)
            {
                userInputTip.Attributes["category"] = Define.DefaultCulture;
                userInputTip.Style["margin-top"] = "8px";
                userInputTip.InnerText = this.MultiSelect ? "可以选择多个用户" : "仅能选择单个用户";
                container.Controls.Add(userInputTip);
            }
        }
        #endregion
    }
}
