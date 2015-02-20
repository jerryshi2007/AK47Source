using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using System.Web;
using System.Runtime;

namespace MCS.Web.Responsive.WebControls
{
    [Designer("MCS.Web.Responsive.WebControls.Design.GenericDesigner, MCS.Web.Responsive.WebControls")]
    [ToolboxData("<{0}:SiteMapPath runat=\"server\" />")]
    public class SiteMapPath : CompositeControl
    {
        // Fields
        private ITemplate _currentNodeTemplate;
        private static readonly object _eventItemCreated = new object();
        private static readonly object _eventItemDataBound = new object();
        private ITemplate _nodeTemplate;
        private SiteMapProvider _provider;
        private ITemplate _rootNodeTemplate;

        // Events
        [Category("Action"), Description("DataControls_OnItemCreated")]
        public event SiteMapNodeItemEventHandler ItemCreated
        {
            add
            {
                base.Events.AddHandler(_eventItemCreated, value);
            }
            remove
            {
                base.Events.RemoveHandler(_eventItemCreated, value);
            }
        }

        [Description("SiteMapPath_OnItemDataBound"), Category("Action")]
        public event SiteMapNodeItemEventHandler ItemDataBound
        {
            add
            {
                base.Events.AddHandler(_eventItemDataBound, value);
            }
            remove
            {
                base.Events.RemoveHandler(_eventItemDataBound, value);
            }
        }

        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            this.CreateControlHierarchy();
            base.ClearChildState();
        }

        protected virtual void CreateControlHierarchy()
        {
            if (this.Provider != null)
            {
                int index = 0;
                SiteMapNode currentNodeAndHintAncestorNodes = this.Provider.GetCurrentNodeAndHintAncestorNodes(-1);
                if (currentNodeAndHintAncestorNodes != null)
                {
                    SiteMapNode parentNode = currentNodeAndHintAncestorNodes.ParentNode;
                    if (parentNode != null)
                    {
                        this.CreateControlHierarchyRecursive(ref index, parentNode, this.ParentLevelsDisplayed);
                    }
                    this.CreateItem(index++, SiteMapNodeItemType.Current, currentNodeAndHintAncestorNodes);
                }
            }
        }

        private void CreateControlHierarchyRecursive(ref int index, SiteMapNode node, int parentLevels)
        {
            if (parentLevels != 0)
            {
                SiteMapNode parentNode = node.ParentNode;
                if (parentNode != null)
                {
                    this.CreateControlHierarchyRecursive(ref index, parentNode, parentLevels - 1);
                    this.CreateItem(index++, SiteMapNodeItemType.Parent, node);
                }
                else
                {
                    this.CreateItem(index++, SiteMapNodeItemType.Root, node);
                }
            }
        }

        protected ResSiteMapNodeItem CreateItem(int itemIndex, SiteMapNodeItemType itemType, SiteMapNode node)
        {
            ResSiteMapNodeItem item = new ResSiteMapNodeItem(itemIndex, itemType);
            int index = (this.PathDirection == PathDirection.CurrentToRoot) ? 0 : -1;
            SiteMapNodeItemEventArgs e = new SiteMapNodeItemEventArgs(item);
            item.SiteMapNode = node;
            this.InitializeItem(item);
            this.OnItemCreated(e);
            this.Controls.AddAt(index, item);
            item.DataBind();
            this.OnItemDataBound(e);
            item.SiteMapNode = null;
            item.EnableViewState = false;

            return item;
        }

        public override void DataBind()
        {
            this.OnDataBinding(EventArgs.Empty);
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            this.ControlStyle.CssClass += " breadcrumb";
            base.AddAttributesToRender(writer);
        }

        protected virtual void InitializeItem(ResSiteMapNodeItem item)
        {
            ITemplate nodeTemplate = null;
            SiteMapNodeItemType itemType = item.ItemType;
            SiteMapNode siteMapNode = item.SiteMapNode;
            switch (itemType)
            {
                case SiteMapNodeItemType.Root:
                    nodeTemplate = (this.RootNodeTemplate != null) ? this.RootNodeTemplate : this.NodeTemplate;
                    break;

                case SiteMapNodeItemType.Parent:
                    nodeTemplate = this.NodeTemplate;
                    break;

                case SiteMapNodeItemType.Current:
                    nodeTemplate = (this.CurrentNodeTemplate != null) ? this.CurrentNodeTemplate : this.NodeTemplate;
                    break;
            }

            if (nodeTemplate == null)
            {
                if (itemType != SiteMapNodeItemType.PathSeparator)
                {
                    if (itemType == SiteMapNodeItemType.Current)
                        item.Attributes["class"] = "active";

                    if ((itemType == SiteMapNodeItemType.Current) && !this.RenderCurrentNodeAsLink)
                    {
                        Literal literal2 = new Literal
                        {
                            Mode = LiteralMode.Encode,
                            Text = siteMapNode.Title
                        };

                        item.Controls.Add(literal2);
                    }
                    else
                    {
                        if (itemType == SiteMapNodeItemType.Root)
                        {
                            item.Controls.Add(new LiteralControl("<i class=\"icon-home\"></i> "));

                        }

                        HyperLink link = new HyperLink();

                        link.EnableTheming = false;
                        link.Enabled = this.Enabled;

                        if (string.IsNullOrEmpty(siteMapNode.Url))
                        {
                            link.NavigateUrl = "javascript:void(0);";
                            link.Enabled = false;
                        }
                        else if (siteMapNode.Url.StartsWith(@"\\", StringComparison.Ordinal))
                        {
                            link.NavigateUrl = base.ResolveClientUrl(HttpUtility.UrlPathEncode(siteMapNode.Url));
                        }
                        else
                        {
                            link.NavigateUrl = (this.Context != null) ? this.Context.Response.ApplyAppPathModifier(base.ResolveClientUrl(HttpUtility.UrlPathEncode(siteMapNode.Url))) : siteMapNode.Url;
                        }
                        link.Text = HttpUtility.HtmlEncode(siteMapNode.Title);
                        if (this.ShowToolTips)
                        {
                            link.ToolTip = siteMapNode.Description;
                        }

                        item.Controls.Add(link);
                    }
                }
            }
            else
            {
                nodeTemplate.InstantiateIn(item);
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            this.Controls.Clear();
            base.ClearChildState();
            this.CreateControlHierarchy();
            base.ChildControlsCreated = true;
        }

        protected virtual void OnItemCreated(SiteMapNodeItemEventArgs e)
        {
            SiteMapNodeItemEventHandler handler = (SiteMapNodeItemEventHandler)base.Events[_eventItemCreated];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnItemDataBound(SiteMapNodeItemEventArgs e)
        {
            SiteMapNodeItemEventHandler handler = (SiteMapNodeItemEventHandler)base.Events[_eventItemDataBound];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (base.DesignMode)
            {
                base.ChildControlsCreated = false;
                this.EnsureChildControls();
            }
            base.Render(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
        }

        [PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), TemplateContainer(typeof(ResSiteMapNodeItem)), Description("SiteMapPath_CurrentNodeTemplate"), DefaultValue((string)null)]
        public virtual ITemplate CurrentNodeTemplate
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._currentNodeTemplate;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set
            {
                this._currentNodeTemplate = value;
            }
        }

        [Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(ResSiteMapNodeItem)), Description("SiteMapPath_NodeTemplate"), DefaultValue((string)null)]
        public virtual ITemplate NodeTemplate
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._nodeTemplate;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set
            {
                this._nodeTemplate = value;
            }
        }

        [Themeable(false), Category("Behavior"), Description("SiteMapPath_ParentLevelsDisplayed"), DefaultValue(-1)]
        public virtual int ParentLevelsDisplayed
        {
            get
            {
                object obj2 = this.ViewState["ParentLevelsDisplayed"];
                if (obj2 == null)
                {
                    return -1;
                }
                return (int)obj2;
            }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["ParentLevelsDisplayed"] = value;
            }
        }

        [DefaultValue(0), Description("SiteMapPath_PathDirection"), Category("Appearance")]
        public virtual PathDirection PathDirection
        {
            get
            {
                object obj2 = this.ViewState["PathDirection"];
                if (obj2 == null)
                {
                    return PathDirection.RootToCurrent;
                }
                return (PathDirection)obj2;
            }
            set
            {
                if ((value < PathDirection.RootToCurrent) || (value > PathDirection.CurrentToRoot))
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this.ViewState["PathDirection"] = value;
            }
        }

        [Browsable(false), Description("SiteMapPath_Provider"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SiteMapProvider Provider
        {
            get
            {
                if ((this._provider == null) && !base.DesignMode)
                {
                    if (string.IsNullOrEmpty(this.SiteMapProvider))
                    {
                        this._provider = SiteMap.Provider;
                        if (this._provider == null)
                        {
                            throw new HttpException(SR.GetString("SiteMapDataSource_DefaultProviderNotFound"));
                        }
                    }
                    else
                    {
                        this._provider = SiteMap.Providers[this.SiteMapProvider];
                        if (this._provider == null)
                        {
                            throw new HttpException(SR.GetString("SiteMapDataSource_ProviderNotFound", new object[] { this.SiteMapProvider }));
                        }
                    }
                }
                return this._provider;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set
            {
                this._provider = value;
            }
        }

        [Category("Appearance"), DefaultValue(false), Description("SiteMapPath_RenderCurrentNodeAsLink")]
        public virtual bool RenderCurrentNodeAsLink
        {
            get
            {
                object obj2 = this.ViewState["RenderCurrentNodeAsLink"];
                if (obj2 == null)
                {
                    return false;
                }
                return (bool)obj2;
            }
            set
            {
                this.ViewState["RenderCurrentNodeAsLink"] = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), TemplateContainer(typeof(ResSiteMapNodeItem)), Description("SiteMapPath_RootNodeTemplate"), DefaultValue((string)null)]
        public virtual ITemplate RootNodeTemplate
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._rootNodeTemplate;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set
            {
                this._rootNodeTemplate = value;
            }
        }

        [Themeable(false), DefaultValue(true), Description("SiteMapPath_ShowToolTips"), Category("Behavior")]
        public virtual bool ShowToolTips
        {
            get
            {
                object obj2 = this.ViewState["ShowToolTips"];
                return ((obj2 == null) || ((bool)obj2));
            }
            set
            {
                this.ViewState["ShowToolTips"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(""), Description("站点地图提供者"), Themeable(false)]
        public virtual string SiteMapProvider
        {
            get
            {
                string str = this.ViewState["SiteMapProvider"] as string;
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["SiteMapProvider"] = value;
                this._provider = null;
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Ol;
            }
        }
    }

    [ToolboxItem(false)]
    public class ResSiteMapNodeItem : SiteMapNodeItem
    {
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Li;
            }
        }

        public ResSiteMapNodeItem(int itemIndex, SiteMapNodeItemType itemType)
            : base(itemIndex, itemType)
        {
        }
    }
}
