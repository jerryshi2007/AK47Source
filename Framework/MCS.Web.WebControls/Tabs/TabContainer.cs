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
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Web.Script.Serialization;
using System.Diagnostics;

using MCS.Web.Library;
using MCS.Web.Library.Script;

#region [ Resources ]

[assembly: WebResource("MCS.Web.WebControls.Tabs.Tabs.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.Tabs.Tabs.css", "text/css", PerformSubstitution=true)]
[assembly: WebResource("MCS.Web.WebControls.Tabs.tab-line.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Tabs.tab.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Tabs.tab-left.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Tabs.tab-right.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Tabs.tab-hover.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Tabs.tab-hover-left.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Tabs.tab-hover-right.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Tabs.tab-active.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Tabs.tab-active-left.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Tabs.tab-active-right.gif", "image/gif")]

#endregion

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 
    /// </summary>
    [Designer("MCS.Web.WebControls.TabContainerDesigner, DeluxeWorks.Web.WebControls")]
    [ParseChildren(typeof(TabPanel))]
    [RequiredScript(typeof(ControlBaseScript))]
    [ClientCssResource("MCS.Web.WebControls.Tabs.Tabs.css")]
    [ClientScriptResource("MCS.Web.WebControls.TabContainer", "MCS.Web.WebControls.Tabs.Tabs.js")]
    [System.Drawing.ToolboxBitmap(typeof(TabContainer), "Tabs.Tabs.ico")]
    public class TabContainer : ScriptControlBase, IPostBackEventHandler
    {
        #region [ Static Fields ]

        private static readonly object EventActiveTabChanged = new object();

        #endregion

        #region [ Fields ]

        private int _activeTabIndex = -1;
        private int _cachedActiveTabIndex = -1;
        private bool _initialized;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new TabContainer
        /// </summary>
        public TabContainer()
            : base(true, HtmlTextWriterTag.Div)
        {
        }

        #endregion

        #region [ Events ]

        /// <summary>
        /// ActiveTabChanged
        /// </summary>
        [Category("Behavior")]
        public event EventHandler ActiveTabChanged
        {
            add { Events.AddHandler(EventActiveTabChanged, value); }
            remove { Events.RemoveHandler(EventActiveTabChanged, value); }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// ActiveTabIndex
        /// </summary>
        [DefaultValue(-1)]
        [Category("Behavior")]
        [ScriptControlProperty]
        [ClientPropertyName("activeTabIndex")]
        public virtual int ActiveTabIndex
        {
            get
            {
                if (_cachedActiveTabIndex > -1)
                {
                    return _cachedActiveTabIndex;
                }
                if (Tabs.Count == 0)
                {
                    return -1;
                }
                return _activeTabIndex;
            }
            set
            {
                if (value < -1) throw new ArgumentOutOfRangeException("value");
                if (Tabs.Count == 0 && !_initialized)
                {
                    _cachedActiveTabIndex = value;
                }
                else
                {
                    if (value >= Tabs.Count)
                    {
                        throw new ArgumentOutOfRangeException("value");
                    }
                    if (ActiveTabIndex != value)
                    {
                        if (ActiveTabIndex != -1 && ActiveTabIndex < Tabs.Count)
                        {
                            Tabs[ActiveTabIndex].Active = false;
                        }
                        _activeTabIndex = value;
                        _cachedActiveTabIndex = -1;
                        if (ActiveTabIndex != -1 && ActiveTabIndex < Tabs.Count)
                        {
                            Tabs[ActiveTabIndex].Active = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tabs
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabPanelCollection Tabs
        {
            get { return (TabPanelCollection)Controls; }
        }

        /// <summary>
        /// ActiveTab
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabPanel ActiveTab
        {
            get
            {
                int i = ActiveTabIndex;
                if (i < 0 || i >= Tabs.Count)
                {              
                    return null;
                }
                EnsureActiveTab();
                return Tabs[i];
            }
            set
            {
                int i = Tabs.IndexOf(value);
                if (i < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                ActiveTabIndex = i;
            }
        }

        /// <summary>
        /// AutoPostBack
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        [ScriptControlProperty]
        [ClientPropertyName("autoPostBack")]
        public bool AutoPostBack
        {
            get
            {
                return this.GetPropertyValue<bool>("AutoPostBack", false); ;
            }
            set
            {
                this.SetPropertyValue<bool>("AutoPostBack", value);
            }
        }

        /// <summary>
        /// Height
        /// </summary>
        [DefaultValue(typeof(Unit), "")]
        [Category("Appearance")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "Assembly is not localized")]
        public override Unit Height
        {
            get { return base.Height; }
            set
            {
                if (!value.IsEmpty && value.Type != UnitType.Pixel)
                {
                    throw new ArgumentOutOfRangeException("value", "Height must be set in pixels only, or Empty.");
                }
                base.Height = value;
            }
        }

        /// <summary>
        /// Width
        /// </summary>
        [DefaultValue(typeof(Unit), "")]
        [Category("Appearance")]
        public override Unit Width
        {
            get { return base.Width; }
            set { base.Width = value; }
        }

        /// <summary>
        /// CssClass
        /// </summary>
        [DefaultValue("ajax__tab_xp")]
        [Category("Appearance")]
        public override string CssClass
        {
            get { return base.CssClass; }
            set { base.CssClass = value; }
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
        /// OnClientActiveTabChanged
        /// </summary>
        [DefaultValue("")]
        [Category("Behavior")]
        [ScriptControlEvent]
        [ClientPropertyName("activeTabChanged")]
        public string OnClientActiveTabChanged
        {
            get { return this.GetPropertyValue<string>("OnClientActiveTabChanged", string.Empty); }
            set { this.SetPropertyValue<string>("OnClientActiveTabChanged", value); }
        }

        /// <summary>
        /// To enable AutoPostBack, we need to call an ASP.NET script method with the UniqueId
        /// on the client side.  To do this, we just use this property as the one to serialize and
        /// alias it's name.
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("autoPostBackId")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member", Justification="Following ASP.NET naming conventions...")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value", Justification="Required for serialization")]
        public new string UniqueID
        {
            get
            {
                return base.UniqueID;
            }
            set
            {
                // need to add a setter for serialization to work properly.
            }
        }

        #endregion

        #region [ Methods ]
        /// <summary>
        /// OnInit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.RegisterRequiresControlState(this);

            _initialized = true;
            if (_cachedActiveTabIndex > -1)
            {
                ActiveTabIndex = _cachedActiveTabIndex;
            }
            else if (Tabs.Count > 0)
            {
                ActiveTabIndex = 0;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (this.AutoPostBack)
            {
                EnsureActiveTab();
            }
            base.OnLoad(e);
        }
    
        /// <summary>
        /// OnActiveTabChanged
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnActiveTabChanged(EventArgs e)
        {
            EventHandler eh = Events[EventActiveTabChanged] as EventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <summary>
        /// AddParsedSubObject
        /// </summary>
        /// <param name="obj"></param>
        protected override void AddParsedSubObject(object obj)
        {
            TabPanel objTabPanel = obj as TabPanel;
            if (null != objTabPanel)
            {
                Controls.Add(objTabPanel);
            }
            else if (!(obj is LiteralControl))
            {
                throw new HttpException(string.Format(CultureInfo.CurrentCulture, "TabContainer cannot have children of type '{0}'.", obj.GetType()));
            }
        }

        /// <summary>
        /// AddedControl
        /// </summary>
        /// <param name="control"></param>
        /// <param name="index"></param>
        protected override void AddedControl(Control control, int index)
        {
            ((TabPanel)control).SetOwner(this);
            base.AddedControl(control, index);
        }

        /// <summary>
        /// RemovedControl
        /// </summary>
        /// <param name="control"></param>
        protected override void RemovedControl(Control control)
        {
            TabPanel controlTabPanel = control as TabPanel;
            if (control != null && controlTabPanel.Active && ActiveTabIndex < Tabs.Count)
            {
                EnsureActiveTab();
            }
            controlTabPanel.SetOwner(null);
            base.RemovedControl(control);
        }

        /// <summary>
        /// CreateControlCollection
        /// </summary>
        /// <returns></returns>
        protected override ControlCollection CreateControlCollection()
        {
            return new TabPanelCollection(this);
        }

        /// <summary>
        /// CreateControlStyle
        /// </summary>
        /// <returns></returns>
        protected override Style CreateControlStyle()
        {
            TabContainerStyle style = new TabContainerStyle(ViewState);
            style.CssClass = "ajax__tab_xp";
            return style;
        }

        /// <summary>
        /// LoadClientState
        /// </summary>
        /// <param name="clientState"></param>
        protected override void LoadClientState(string clientState)
        {
            Dictionary<string, object> state = (Dictionary<string, object>)new JavaScriptSerializer().DeserializeObject(clientState);
            if (state != null)
            {
                ActiveTabIndex = (int)state["ActiveTabIndex"];
                object[] tabState = (object[])state["TabState"];
                for (int i = 0; i < tabState.Length; i++)
                {
                    Tabs[i].Enabled = (bool)tabState[i];
                }
            }
        }

        /// <summary>
        /// SaveClientState
        /// </summary>
        /// <returns></returns>
        protected override string SaveClientState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            state["ActiveTabIndex"] = ActiveTabIndex;

            List<object> tabState = new List<object>();
            foreach (TabPanel panel in Tabs)
            {
                tabState.Add(panel.Enabled);
            }
            state["TabState"] = tabState;
            return new JavaScriptSerializer().Serialize(state);
        }

        /// <summary>
        /// LoadControlState
        /// </summary>
        /// <param name="savedState"></param>
        protected override void LoadControlState(object savedState)
        {
            Pair p = (Pair)savedState;
            if (p != null)
            {
                base.LoadControlState(p.First);
                ActiveTabIndex = (int)p.Second;
            }
            else
            {
                base.LoadControlState(null);
            }
        }

        /// <summary>
        /// SaveControlState
        /// </summary>
        /// <returns></returns>
        protected override object SaveControlState()
        {
            Pair p = new Pair();
            p.First = base.SaveControlState();
            p.Second = ActiveTabIndex;
            if (p.First == null && p.Second == null)
            {
                return null;
            }
            else
            {
                return p;
            }
        }

        /// <summary>
        /// AddAttributesToRender
        /// </summary>
        /// <param name="writer"></param>
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            Style.Remove(HtmlTextWriterStyle.Visibility);
            if (!ControlStyleCreated)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "ajax__tab_xp");
            }
            base.AddAttributesToRender(writer);
            //writer.AddStyleAttribute(HtmlTextWriterStyle.Visibility, "hidden");
        }

        /// <summary>
        /// RenderContents
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            Page.VerifyRenderingInServerForm(this);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_header");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                RenderHeader(writer);
            }
            writer.RenderEndTag();

            if (!Height.IsEmpty)
                writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_body");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Overflow, "auto");

            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                RenderChildren(writer);
            }
            writer.RenderEndTag();
        }

        /// <summary>
        /// RenderHeader
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void RenderHeader(HtmlTextWriter writer)
        {
            foreach (TabPanel panel in Tabs)
            {
                panel.RenderHeader(writer);
            }
        }

        /// <summary>
        /// LoadPostData
        /// </summary>
        /// <param name="postDataKey"></param>
        /// <param name="postCollection"></param>
        /// <returns></returns>
        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            int tabIndex = ActiveTabIndex;
            bool result = base.LoadPostData(postDataKey, postCollection);
            if (tabIndex != ActiveTabIndex)
            {
                return true;
            }
            return result;
        }

        /// <summary>
        /// RaisePostDataChangedEvent
        /// </summary>
        protected override void RaisePostDataChangedEvent()
        {
            OnActiveTabChanged(EventArgs.Empty);
        }

        private void EnsureActiveTab()
        {

            if (_activeTabIndex < 0 || _activeTabIndex >= Tabs.Count)
            {
                _activeTabIndex = 0;
            }

            for (int i = 0; i < Tabs.Count; i++)
            {
                if (i == ActiveTabIndex)
                {
                    Tabs[i].Active = true;
                }
                else
                {
                    Tabs[i].Active = false;
                }
            }
        }

        #endregion       

        #region [ TabContainerStyle ]

        private sealed class TabContainerStyle : Style
        {
            public TabContainerStyle(StateBag state)
                : base(state)
            {
            }

            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                base.FillStyleAttributes(attributes, urlResolver);

                attributes.Remove(HtmlTextWriterStyle.Height);
                attributes.Remove(HtmlTextWriterStyle.BackgroundColor);
                attributes.Remove(HtmlTextWriterStyle.BackgroundImage);
            }
        }

        #endregion

        #region IPostBackEventHandler Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification="Called by ASP.NET infrastructure")]
        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.StartsWith("activeTabChanged"))
            {
                // change the active tab.
                //
                int parseIndex = eventArgument.IndexOf(":");
                Debug.Assert(parseIndex != -1, "Expected new active tab index!");
                if (parseIndex != -1 && Int32.TryParse(eventArgument.Substring(parseIndex + 1), out parseIndex))
                {
                    ActiveTabIndex = parseIndex;
                    OnActiveTabChanged(EventArgs.Empty);
                }
            }
        }

        #endregion
    }
}
