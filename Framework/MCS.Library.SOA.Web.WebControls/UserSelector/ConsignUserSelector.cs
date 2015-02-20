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
using MCS.Web.WebControls;
using MCS.Library.OGUPermission;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.WebControls.UserSelector.ConsignUserSelector.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 人员选择控件
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.ConsignUserSelector", "MCS.Web.WebControls.UserSelector.ConsignUserSelector.js")]
    [DialogContent("MCS.Web.WebControls.UserSelector.ConsignUserSelectorTemplate.htm", "MCS.Library.SOA.Web.WebControls")]
    [ToolboxData("<{0}:ConsignUserSelector runat=server></{0}:ConsignUserSelector>")]
    public class ConsignUserSelector : DialogControlBase<ConsignUserSelectorParams>
    {
        private OuUserInputControl userInput = new OuUserInputControl() { InvokeWithoutViewState = true };
        private HtmlGenericControl userInputTip = new HtmlGenericControl("div");
        private RadioButtonList consignType = new RadioButtonList();

        /// <summary>
        /// 构造方法
        /// </summary>
        public ConsignUserSelector()
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
        #endregion Properties

        #region Protected
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Page.IsCallback)
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

            InitUserInputTipControl(WebControlUtility.FindControlByHtmlIDProperty(container, "userInputContainer", true));

            if (this.IsConsign)
                InitConsignTypeSelector(WebControlUtility.FindControlByHtmlIDProperty(container, "consignTypeContainer", true));
        }

        protected override string GetDialogFeature()
        {
            WindowFeature feature = new WindowFeature();

            feature.Width = 420;
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
        #endregion

        #region Private
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

        private void InitConsignTypeSelector(Control container)
        {
            if (container != null)
            {
                Label caption = new Label();
                caption.Text = "请选择处理类型：";

                TableCell cell1 = new TableCell();
                cell1.Controls.Add(caption);

                consignType.ID = "consignType";
                consignType.Items.Add(new ListItem("会签 所有用户通过后，返回会签发起人", "0"));
                consignType.Items.Add(new ListItem("并签 某一用户通过后，返回并签发起人", "1"));
                consignType.RepeatLayout = RepeatLayout.Table;
                consignType.RepeatDirection = RepeatDirection.Vertical;
                consignType.SelectedIndex = 0;

                TableCell cell2 = new TableCell();
                cell2.Controls.Add(consignType);

                TableRow row = new TableRow();
                row.Cells.Add(cell1);
                row.Cells.Add(cell2);

                Table table = new Table();
                table.Attributes.Add("cellpadding", "0");
                table.Attributes.Add("cellspacing", "0");
                table.Style.Add(HtmlTextWriterStyle.BorderWidth, "0px");

                table.Rows.Add(row);

                container.Controls.Add(table);
                //container.Controls.Add(caption);
                //container.Controls.Add(consignType);
            }
        }

        private void InitUserInputTipControl(Control container)
        {
            if (container != null)
            {
                userInputTip.Style["margin-top"] = "8px";
                userInputTip.InnerText = this.MultiSelect ?
                    "可以选择多个用户" : "仅能选择单个用户";

                container.Controls.Add(userInputTip);
            }
        }
        #endregion
    }
}
