using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Collections;
using System.Drawing.Design;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI.Design.WebControls;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.GangedDataBindingControl.GangedDataBindingControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.Images.error.gif", "image/gif")]
namespace MCS.Web.WebControls
{
    [ParseChildren(true)]
    [DefaultProperty("ItemBindings")]
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [ClientScriptResource("MCS.Web.WebControls.GangedDataBindingControl", "MCS.Web.WebControls.GangedDataBindingControl.GangedDataBindingControl.js")]
    [ToolboxData("<{0}:GangedDataBindingControl runat=server></{0}:GangedDataBindingControl>")]
    public class GangedDataBindingControl : ScriptControlBase
    {
        private GangedDataBindingItemCollection itemBindings = new GangedDataBindingItemCollection();

        public GangedDataBindingControl()
            : base(true, HtmlTextWriterTag.Div)
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void Render(HtmlTextWriter output)
        {
            if (this.DesignMode)
            {
                output.Write(string.Format("<div>Data Binding begin</div>"));
                foreach (GangedDataBindingItem item in ItemBindings)
                {
                    output.Write(string.Format("<div>control:{0} </div>",
                        HttpUtility.HtmlEncode(item.ControlClientID)));
                }
                output.Write(string.Format("<div>Data Binding end</div>"));
            }
            else
            {
                base.Render(output);
            }
        }

        #region Properties

        [ScriptControlProperty]
        [ClientPropertyName("readOnly")]
        [DefaultValue(false)]
        public new bool ReadOnly
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "OverrideReadOnly", false);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "OverrideReadOnly", value);
            }
        }

        /// <summary>
        /// 绑定项设置
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty), Description("绑定项设置")]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [DefaultValue((string)null)]
        [Browsable(false)]
        [ScriptControlProperty]
        [ClientPropertyName("itemBindings")]
        public GangedDataBindingItemCollection ItemBindings
        {
            get
            {
                return itemBindings;
            }
        }

        #region Client Events

        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("createDefaultViewData")]
        [Bindable(true), Category("ClientEventsHandler"), Description("客户端创建默认ViewData时的事件")]
        public string OnCreateDefaultViewData
        {
            get
            {
                return GetPropertyValue("OnCreateDefaultViewData", string.Empty);
            }
            set
            {
                SetPropertyValue("OnCreateDefaultViewData", value);
            }
        }

        #endregion Client Events

        #endregion
    }
}
