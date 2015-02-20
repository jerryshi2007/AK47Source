using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using System.ComponentModel;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using MCS.Web.Library.MVC;

[assembly: WebResource("MCS.Web.WebControls.Workflow.PrintControl.PrintControl.js", "application/x-javascript")]


namespace MCS.Web.WebControls
{
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.PrintControl", "MCS.Web.WebControls.Workflow.PrintControl.PrintControl.js")]

    [ToolboxData("<{0}:PrintControl runat=server></{0}:PrintControl>")]
    public class PrintControl : ScriptControlBase//WebControl, INamingContainer
    {
        public PrintControl()
            : base(HtmlTextWriterTag.Div)
        { }
        private HiddenButtonWrapper buttonWrapper = new HiddenButtonWrapper();

        #region Properties
        /// <summary>
        /// 目标控件的ID。目标控件通常是一个Button(客户端和服务器端)，由目标控件来触发流程操作。
        /// </summary>
        //[DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(DialogStartUpControlConverter))]
        [Category("Appearance")]
        [Browsable(false)]
        [ScriptControlProperty()]
        [ClientPropertyName("targetControlID")]
        public string TargetControlID
        {
            get
            {
                return (buttonWrapper.TargetControl as WebControl).ClientID;
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

        [Browsable(false)]
        private bool NeedToRender
        {
            get
            {
				bool result = this.Visible;

				if (result)
					if (WfClientContext.Current.OriginalActivity != null)
						result = WfClientContext.Current.OriginalActivity.ApprovalRootActivity.Descriptor.Properties.GetValue("AllowPrint", true);

                return result;
            }
        }

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("onPrint")]
        public string OnPrint
        {
            get
            {
                return GetPropertyValue("OnPrint", string.Empty);
            }
            set
            {
                SetPropertyValue("OnPrint", value);
            }
        }

        #endregion Properties

        #region Protected
        protected override void OnPreRender(EventArgs e)
        {
            InitTargetControl(this.buttonWrapper.TargetControl);

            base.OnPreRender(e);
            
        }
        #endregion Protected

        #region Private
        private void InitTargetControl(IAttributeAccessor target)
        {
            if (NeedToRender == false)
            {
                target.SetAttribute("disabled", "true");
                target.SetAttribute("class", "disable");
                target.SetAttribute("style", "display:none");
            }
            else
            {
                target.SetAttribute("href", "#");
                target.SetAttribute("class", "enable");
            }
        }

        private void RegisterScripts()
        {
            string clientScript = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
                "MCS.Web.WebControls.Workflow.PrintControl.PrintControlClient.js");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CommandPrint", clientScript, true);
        }

        private void RegisterWebBrowserObject()
        {
            string webBrowserTxt = ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
                "MCS.Web.WebControls.Workflow.PrintControl.PrintControl.txt");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "StringWebBrowser", webBrowserTxt, false);
        }
        #endregion Private
    }
}
