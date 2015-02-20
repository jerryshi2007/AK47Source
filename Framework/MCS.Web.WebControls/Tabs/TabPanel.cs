// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Script;
using System.ComponentModel;
using System.Collections.Generic;

using MCS.Web.Library;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// TabPanel
    /// </summary>
    [ParseChildren(true)]
    [RequiredScript(typeof(ControlBaseScript))]
    [RequiredScript(typeof(TabContainer))]
    [ClientScriptResource("MCS.Web.WebControls.TabPanel", "MCS.Web.WebControls.Tabs.Tabs.js")]
    [ToolboxItem(false)]
    [Designer(typeof(TabPanelDesigner))]
    public class TabPanel : ScriptControlBase
    {
        #region [ Fields ]

        private bool _active;
        //private bool _autoPostBack;
        private ITemplate _contentTemplate;
        private ITemplate _headerTemplate;
        private TabContainer _owner;
        private Control _headerControl;
        private Control _contentControl;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// TabPanel
        /// </summary>
        public TabPanel()
            : base(false)
        {
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// HeaderText
        /// </summary>
        [DefaultValue("")]
        [Category("Appearance")]
        public string HeaderText
        {
            get { return (string)(ViewState["HeaderText"] ?? string.Empty); }
            set { ViewState["HeaderText"] = value; }
        }

        /// <summary>
        /// HeaderTemplate
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        [Browsable(false)]
        [MergableProperty(false)]
        public ITemplate HeaderTemplate
        {
            get { return _headerTemplate; }
            set { _headerTemplate = value; }
        }

        /// <summary>
        /// ContentTemplate
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]        
        [Browsable(false)]
        [MergableProperty(false)]
        public ITemplate ContentTemplate
        {
            get { return _contentTemplate; }
            set { _contentTemplate = value; }
        }

        /// <summary>
        /// Enabled
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        [ScriptControlProperty]
        [ClientPropertyName("enabled")]
        public override bool Enabled
        {
            get { return base.Enabled; }
            set { base.Enabled = value; }
        }

        /// <summary>
        /// ScrollBars
        /// </summary>
        [DefaultValue(ScrollBars.None)]
        [Category("Behavior")]
        [ScriptControlProperty]
        [ClientPropertyName("scrollBars")]
        public ScrollBars ScrollBars
        {
            get { return this.GetPropertyValue<ScrollBars>("ScrollBars", ScrollBars.None); }
            set { this.SetPropertyValue<ScrollBars>("ScrollBars", value); }
        }

        /// <summary>
        /// OnClientClick
        /// </summary>
        [DefaultValue("")]
        [Category("Behavior")]
        [ScriptControlEvent]
        [ClientPropertyName("click")]
        public string OnClientClick
        {
            get { return this.GetPropertyValue<string>("OnClientClick", string.Empty); }
            set { this.SetPropertyValue<string>("OnClientClick", value); }
        }

        //[DefaultValue("")]
        //[Category("Behavior")]
        //[ScriptControlProperty]
        //[ClientPropertyName("dynamicServicePath")]
        //[UrlProperty]
        //public string DynamicServicePath
        //{
        //    get { return (string)(ViewState["DynamicServicePath"] ?? string.Empty); }
        //    set { ViewState["DynamicServicePath"] = value; }
        //}

        //[DefaultValue("")]
        //[Category("Behavior")]
        //[ScriptControlProperty]
        //[ClientPropertyName("dynamicServiceMethod")]
        //public string DynamicServiceMethod
        //{
        //    get { return (string)(ViewState["DynamicServiceMethod"] ?? string.Empty); }
        //    set { ViewState["DynamicServiceMethod"] = value; }
        //}

        //[DefaultValue("")]
        //[Category("Behavior")]
        //[ScriptControlProperty]
        //[ClientPropertyName("dynamicContextKey")]
        //public string DynamicContextKey
        //{
        //    get { return (string)(ViewState["DynamicContextKey"] ?? string.Empty); }
        //    set { ViewState["DynamicContextKey"] = value; }
        //}

        //[DefaultValue("")]
        //[Category("Behavior")]
        //[ScriptControlEvent]
        //[ClientPropertyName("populating")]
        //public string OnClientPopulating
        //{
        //    get { return (string)(ViewState["OnClientPopulating"] ?? string.Empty); }
        //    set { ViewState["OnClientPopulating"] = value; }
        //}

        //[DefaultValue("")]
        //[Category("Behavior")]
        //[ScriptControlEvent]
        //[ClientPropertyName("populated")]
        //public string OnClientPopulated
        //{
        //    get { return (string)(ViewState["OnClientPopulated"] ?? string.Empty); }
        //    set { ViewState["OnClientPopulated"] = value; }
        //}

        internal bool Active
        {
            get { return _active; }
            set {
                _active = value;
                SetContentControlStatus();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="e"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification="Local c is handed off to Controls collection")]
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (_headerTemplate != null)
            {
                _headerControl = new Control();
                _headerTemplate.InstantiateIn(_headerControl);
                Controls.Add(_headerControl);
            }
            if (_contentTemplate != null)
            {
                _contentControl = new Control();
                _contentTemplate.InstantiateIn(_contentControl);
                Controls.Add(_contentControl);
            }
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);
            EnsureChildControls();
        }

        private void SetContentControlStatus()
        {
            if (this._contentControl != null)
            {
                if (!this.Active && this._owner.AutoPostBack)
                    this._contentControl.Visible = false;
                else
                    this._contentControl.Visible = true;
            }
        }
       
        /// <summary>
        /// RenderHeader
        /// </summary>
        /// <param name="writer"></param>
        protected internal virtual void RenderHeader(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "__tab_" + ClientID);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            if (_headerControl != null)
            {
                _headerControl.RenderControl(writer);
            }
            else
            {
                writer.Write(HeaderText);
            }
            writer.RenderEndTag();
        }
        
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (_headerControl != null)
            {
                _headerControl.Visible = false;
            }
            
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            if (!Active)
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                writer.AddStyleAttribute(HtmlTextWriterStyle.Visibility, "hidden");
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            RenderChildren(writer);
            writer.RenderEndTag();
            ScriptManager.RegisterScriptDescriptors(this);
        }

        /// <summary>
        /// DescribeComponent
        /// </summary>
        /// <param name="descriptor"></param>
        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            descriptor.AddElementProperty("headerTab", "__tab_" + ClientID);
            if (_owner != null)
            {
                descriptor.AddComponentProperty("owner", _owner.ClientID);
            }
        }

        internal void SetOwner(TabContainer owner)
        {
            _owner = owner; 
        }

        #endregion
    }
}
