using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using MCS.Web.Library.Script;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.MVC;
using MCS.Library.Globalization;
using System.Web.UI.WebControls;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 退回控件的工作模式
    /// </summary>
    [Flags]
    public enum WfRejectMode
    {
        /// <summary>
        /// 人工选择被退回的步骤
        /// </summary>
        SelectRejectStep = 1,

        //王黎注释
        ///// <summary>
        ///// 自动退回上一部
        ///// </summary>
        //RejectToPreviousStep = 2,

        /// <summary>
        /// 退回到拟稿人
        /// </summary>
        RejectToDrafter = 4,

        /// <summary>
        /// 类似于加签式的退回
        /// </summary>
        LikeAddApprover = 8,
    }

    /// <summary>
    /// 退件控件
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.WfReturnControl",
        "MCS.Web.WebControls.Workflow.Abstract.WfControls.js")]
    [ToolboxData("<{0}:WfReturnControl runat=server></{0}:WfReturnControl>")]
    public class WfReturnControl : ScriptControlBase, INamingContainer
    {
        private HiddenButtonWrapper buttonWrapper = new HiddenButtonWrapper();
        private RejectActivitySelector activitySelector = new RejectActivitySelector() { Category = Define.DefaultCulture };
        private OpinionReasonItemCollection reasons = new OpinionReasonItemCollection();

        public WfReturnControl()
            : base(true, HtmlTextWriterTag.Div)
        {
        }

        #region properties
        /// <summary>
        /// 目标控件的ID。目标控件通常是一个Button(客户端和服务器端)，由目标控件来触发流程操作。
        /// </summary>
        [DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(DialogStartUpControlConverter))]
        [Category("Appearance")]
        public string TargetControlID
        {
            get
            {
                return buttonWrapper.TargetControlID;
            }
            set
            {
                buttonWrapper.TargetControlID = value;
            }
        }

        /// <summary>
        /// 目标控件实例。通常由目标控件的ID来计算出实例
        /// </summary>
        [Browsable(false)]
        public IAttributeAccessor TargetControl
        {
            get
            {
                return buttonWrapper.TargetControl;
            }
            set
            {
                buttonWrapper.TargetControl = value;
            }
        }

        /// <summary>
        /// 是否允许在分支流程内退回
        /// </summary>
        [DefaultValue(false)]
        [Description("是否允许在分支流程内退回")]
        public bool AllowRejectInBranchProcess
        {
            get
            {
                return GetPropertyValue("AllowRejectInBranchProcess", false);
            }
            set
            {
                SetPropertyValue("AllowRejectInBranchProcess", value);
            }
        }

        [Browsable(false)]
        [ScriptControlProperty()]
        [ClientPropertyName("activityID")]
        private string ActivityID
        {
            get
            {
                string result = string.Empty;

                if (WfClientContext.Current.OriginalActivity != null)
                    result = WfClientContext.Current.OriginalActivity.ID;

                return result;
            }
        }

        /// <summary>
        /// 退回控件的工作模式
        /// </summary>
        [Browsable(true)]
        [ScriptControlProperty()]
        [ClientPropertyName("rejectMode")]
        [DefaultValue(WfRejectMode.SelectRejectStep)]
        public WfRejectMode RejectMode
        {
            get
            {
                return GetPropertyValue("RejectMode", WfRejectMode.SelectRejectStep);
            }
            set
            {
                SetPropertyValue("RejectMode", value);
            }
        }

        /// <summary>
        /// 退件时是否退到操作人
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        public bool ReturnToOperator
        {
            get
            {
                return GetPropertyValue("ReturnToOperator", false);
            }
            set
            {
                SetPropertyValue("ReturnToOperator", value);
            }
        }

        [DefaultValue(false)]
        public bool ShowOpinionInput
        {
            get
            {
                return GetPropertyValue("ShowOpinionInput", false);
            }
            set
            {
                SetPropertyValue("ShowOpinionInput", value);
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), Description("退件原因列表")]
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

        [Browsable(false)]
        [ScriptControlProperty()]
        [ClientPropertyName("activitySelectorClientID")]
        private string ActivitySelectorClientID
        {
            get
            {
                return this.activitySelector.ClientID;
            }
        }

        [Browsable(false)]
        [ScriptControlProperty()]
        [ClientPropertyName("moveToControlClientID")]
        private string MoveToControlClientID
        {
            get
            {
                string result = string.Empty;

                if (WfMoveToControl.Current != null)
                    result = WfMoveToControl.Current.ClientID;

                return result;
            }
        }

        [Browsable(false)]
        private bool NeedToRender
        {
            get
            {
                bool result = this.Visible && !this.ReadOnly;

                if (result)
                {
                    IWfActivity originalActivity = WfClientContext.Current.CurrentActivity;

                    if (originalActivity != null)
                    {
                        IWfActivity associateAct = null;
                        string associateActId = originalActivity.Descriptor.AssociatedActivityKey;

                        if (associateActId != null)
                        {
                            associateAct = originalActivity.Process.Activities.FindActivityByDescriptorKey(associateActId);
                        }

                        bool currentActCanReturn = WfClientContext.Current.CurrentActivity.Descriptor.Properties.GetValue("AllowReturn", true);

                        result = (originalActivity.Process.IsApprovalRootProcess && currentActCanReturn);

                        if (result)
                        {
                            result = WfClientContext.Current.InMoveToMode;
                        }
                    }
                }

                return result;
            }
        }
        #endregion properties

        #region protected
        protected override string SaveClientState()
        {
            string result = null;

            WfControlNextStep rejectStep = null;

            if (WfClientContext.Current.CurrentActivity != null)
            {
                IWfActivity rejectActivity = null;

                switch (RejectMode)
                {
                    case WfRejectMode.RejectToDrafter:
                        rejectActivity = WfClientContext.Current.CurrentActivity.Process.RootProcess.InitialActivity;
                        break;
                }

                if (rejectActivity != null)
                {
                    rejectStep = new WfControlNextStep(rejectActivity);
                    result = JSONSerializerExecute.Serialize(rejectStep);
                }
            }

            return result;
        }

        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            EnsureChildControls();

            base.OnPagePreLoad(sender, e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (DesignMode == false)
            {
                ExceptionHelper.FalseThrow(WfMoveToControl.Current != null,
                    "WfReturnControl控件必须和WfMoveToControl一起使用");

                this.activitySelector.DialogTitle = string.Format("请选择退件环节");
                this.activitySelector.ReturnToOperator = this.ReturnToOperator;
                this.activitySelector.ShowOpinionInput = this.ShowOpinionInput;
            }

            DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "确定要回退吗？");

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.DesignMode)
                writer.Write(string.Format("<img src=\"{0}\"/><span>{1}</span>",
                    ControlResources.ActivityLogoUrl,
                    HttpUtility.HtmlEncode("退件")));
            else
                base.Render(writer);
        }

        protected override void CreateChildControls()
        {
            if (DesignMode == false)
            {
                this.activitySelector.ID = "activitySelector";

                this.Reasons.ForEach(r => activitySelector.Reasons.Add(r));
                Controls.Add(this.activitySelector);

                InitHiddenButton();
            }

            base.CreateChildControls();
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);

            this.buttonWrapper = (HiddenButtonWrapper)ViewState["ButtonWrapper"];
        }

        protected override object SaveViewState()
        {
            ViewState["ButtonWrapper"] = this.buttonWrapper;
            return base.SaveViewState();
        }

        protected override void OnPagePreRenderComplete(object sender, EventArgs e)
        {
            base.OnPagePreRenderComplete(sender, e);

            if (RenderMode.OnlyRenderSelf == false)
                InitTargetControl(this.buttonWrapper.TargetControl);
        }
        #endregion protected

        #region private
        private void InitHiddenButton()
        {
            this.buttonWrapper.CreateHiddenButton("innerBtn", null);
            Controls.Add(this.buttonWrapper.HiddenButton);
        }

        private void InitTargetControl(IAttributeAccessor target)
        {
            if (target != null)
            {
                target.SetAttribute("onclick", string.Format("event.returnValue = false; if (!this.disabled) $find(\"{0}\")._doOperation();return false;", this.ClientID));
                target.SetAttribute("class", "enable");

                if (this.Visible)
                {
                    if (NeedToRender == false)
                    {
                        target.SetAttribute("disabled", "true");
                        target.SetAttribute("class", "disable");
                    }

                    if (target is IButtonControl)
                    {
                        IButtonControl button = (IButtonControl)target;

                        string buttonText = "退件";

                        if (WfClientContext.Current.CurrentActivity != null)
                            buttonText = WfClientContext.Current.CurrentActivity.Descriptor.Properties.GetValue("ReturnButtonName", "退件");

                        button.Text = button.Text.Replace("退件", HttpUtility.HtmlEncode(buttonText));
                    }
                }
                else
                {
                    ((Control)target).Visible = false;
                }
            }
        }
        #endregion private
    }
}
