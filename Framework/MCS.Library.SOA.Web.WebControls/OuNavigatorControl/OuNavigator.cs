using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Web.Library;
using System.Collections;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 组织机构导航控件，用于显示组织结构的路径，例如：“机构人员\远洋地产\流程管理部”。
    /// 每一部分都可以是个链接
    /// </summary>
    [ToolboxData("<{0}:OuNavigator runat=server></{0}:OuNavigator>")]
    public class OuNavigator : WebControl
    {
        private static readonly object OuNavigatorAllObjectIds = new object();

        private static readonly object OuNavigatorAllObjectParents = new object();

        private static readonly object OuNavigatorAllTermials = new object();

        #region 事件
        /// <summary>
        /// 构建锚点时引发
        /// </summary>
        public event EventHandler<RenderControlEventArgs> BuildingAnchor;

        /// <summary>
        /// 构建分隔符时引发
        /// </summary>
        public event EventHandler<RenderControlEventArgs> BuildingSplitter;
        #endregion

        #region 引发事件
        /// <summary>
        /// 引发<see cref="BuildingAnchor"/>事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBuildingAnchor(RenderControlEventArgs e)
        {
            if (BuildingAnchor != null)
            {
                this.BuildingAnchor(this, e);
            }
        }

        /// <summary>
        /// 引发<see cref="BuildingSplitter"/>事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBuildingSplitter(RenderControlEventArgs e)
        {
            if (this.BuildingSplitter != null)
                this.BuildingSplitter(this, e);
        }
        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置一个值，表示从哪一级开始显示。第一级为0
        /// </summary>
        [DefaultValue(1)]
        public int StartLevel
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "StartLevel", 1);
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "StartLevel", value);
            }
        }

        /// <summary>
        /// 获取或设置一个字符串，表示对象的ID，这个ID决定是谁的路径
        /// </summary>
        [DefaultValue("")]
        public string ObjectID
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "ObjectID", string.Empty);
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "ObjectID", value);
            }
        }

        /// <summary>
        /// 获取或设置单击 <see cref="NavPath"/> 控件时显示链接到的网页内容的目标窗口或框架。
        /// </summary>
        /// <value>
        /// <para>单击 System.Web.UI.WebControls.HyperLink 控件时加载链接到的网页的目标窗口或框架。值必须以 a 到 z 的字母（不区分大小写）开头，但下表所列以下划线开头的特殊值除外。_blank
        /// 将内容呈现在一个没有框架的新窗口中。_parent 将内容呈现在上一个框架集父级中。_search在搜索窗格中呈现内容。_self 将内容呈现在含焦点的框架中。_top
        /// 将内容呈现在没有框架的全窗口中。Note请查看浏览器文档，确定是否支持 _search 值。例如，Microsoft Internet Explorer
        /// 5.0 及更高版本支持 _search 目标值。默认值为空字符串 ("")。
        /// </para>
        /// </value>
        [DefaultValue("_self")]
        [TypeConverter(typeof(TargetConverter))]
        [Category("Navigation")]
        [Description("目标")]
        public string Target
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "Target", "_self");
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "Target", value);
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示节点链接的样式
        /// </summary>
        [DefaultValue("")]
        [CssClassProperty]
        public string LinkCssClass
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "LinkCssClass", string.Empty);
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "LinkCssClass", value ?? string.Empty);
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示节点单击时需要执行的脚本
        /// </summary>
        [DefaultValue("")]
        public string OnClientLinkClick
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "OnClientLinkClick", string.Empty);
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "OnClientLinkClick", value ?? string.Empty);
            }
        }

        /// <summary>
        /// 获取或设置一个字符串，表示供脚本使用时，要在锚标记上保存节点ID的属性名称（为空字符串时忽略）
        /// </summary>
        /// <value>为空字符串以外的值时有效，否则不呈现此属性。属性名必须是一个有效的字符。</value>
        [DefaultValue("data-id")]
        public string LinkDataTag
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "LinkDataTag", string.Empty);
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "LinkDataTag", value ?? string.Empty);
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示名称之间的分隔符
        /// </summary>
        [DefaultValue("\\")]
        [CssClassProperty]
        public string SplitterChars
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "DefaultLinkCssClass", "\\");
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "DefaultLinkCssClass", value);
            }
        }

        /// <summary>
        /// 获取或设置由 Web 服务器控件在客户端呈现的级联样式表 (CSS) 类。
        /// </summary>
        /// <value>由 Web 服务器控件在客户端呈现的 CSS 类。默认值为 <see cref="System.String.Empty"/>。</value>
        [DefaultValue("")]
        [CssClassProperty]
        public string SplitterCssClass
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "SplitterCssClass", string.Empty);
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "SplitterCssClass", value);
            }
        }

        /// <summary>
        /// 获取或设置链接的格式，ID使用（{0}代替 ）
        /// </summary>
        [DefaultValue("")]
        public string NavigateUrlFormat
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "LinkTemplate", string.Empty);
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "LinkTemplate", value);
            }
        }

        ///// <summary>
        ///// ID值在Url中的参数名称，默认为'id'
        ///// </summary>
        //[DefaultValue("id")]
        //public string IDParameterName
        //{
        //    get
        //    {
        //        return WebControlUtility.GetViewStateValue(ViewState, "IDParameterName", "id");
        //    }
        //    set
        //    {
        //        WebControlUtility.SetViewStateValue(ViewState, "IDParameterName", value);
        //    }
        //}

        /// <summary>
        /// 获取或设置一个值，表示是否启用分隔符渲染
        /// </summary>
        [DefaultValue(true)]
        public bool SplitterVisible
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "SplitterVisible", true);
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "SplitterVisible", value);
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示是否应呈现根节点
        /// </summary>
        [DefaultValue(false)]
        public bool RootVisible
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "RootVisible", false);
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "RootVisible", value);
            }
        }

        /// <summary>
        /// 获取或设置一个值，表示是否应呈现终端节点
        /// </summary>
        [DefaultValue(false)]
        public bool TerminalVisible
        {
            get
            {
                return WebControlUtility.GetViewStateValue(this.ViewState, "TerminalVisible", true);
            }

            set
            {
                WebControlUtility.SetViewStateValue(this.ViewState, "TerminalVisible", value);
            }
        }

        #endregion

        #region 构造函数

        public OuNavigator()
            : base()
        {
        }

        #endregion

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.DesignMode)
                writer.Write("OUNavigator");
            else
                base.Render(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            //base.RenderContents(writer);
            writer.Write(WebControlUtility.GetControlHtml(BuildControls()));
        }

        private Control BuildControls()
        {
            bool firstNode = true;

            Control outContainer = new Control();
            //HtmlGenericControl outContainer = new HtmlGenericControl("span");

            IEnumerable<IOrganization> parents = null;

            if (AllObjectParents.TryGetValue(this.ObjectID, out parents))
            {
                int level = 0;
                bool splitterEnabled = this.SplitterVisible;
                if (this.RootVisible && this.StartLevel == 0)
                {
                    outContainer.Controls.Add(this.BuildOrganzationAnchor((IOrganization)Terminals[""]));
                    firstNode = false;
                }

                foreach (IOrganization org in parents)
                {
                    if (level >= this.StartLevel)
                    {
                        if (splitterEnabled && firstNode == false)
                            outContainer.Controls.Add(this.BuildSplitterControl());
                        else
                            firstNode = false;

                        outContainer.Controls.Add(this.BuildOrganzationAnchor(org));
                    }

                    level++;
                }

                if (this.TerminalVisible)
                {
                    IOrganization org = Terminals[this.ObjectID] as IOrganization;
                    if (org != null)
                    {
                        if (splitterEnabled && firstNode == false)
                            outContainer.Controls.Add(this.BuildSplitterControl());

                        outContainer.Controls.Add(this.BuildOrganzationAnchor(org));
                    }
                }
            }

            return outContainer;
        }

        protected virtual Control BuildSplitterControl()
        {
            HtmlGenericControl splitter = new HtmlGenericControl("span");

            if (this.SplitterCssClass.IsNotEmpty())
                splitter.Attributes["class"] = this.SplitterCssClass;

            splitter.InnerText = this.SplitterChars;

            RenderControlEventArgs e = new RenderControlEventArgs(splitter, null);
            this.OnBuildingSplitter(e);

            return splitter;
        }

        protected virtual Control BuildOrganzationAnchor(IOrganization org)
        {
            HtmlAnchor anchor = new HtmlAnchor();

            if (this.LinkCssClass.IsNotEmpty())
                anchor.Attributes["class"] = this.LinkCssClass;

            anchor.InnerText = org.DisplayName.IsNotEmpty() ? org.DisplayName : org.Name;
            anchor.HRef = this.BuildUrl(org);
            anchor.Target = this.Target;

            string dataTag = this.LinkDataTag;
            string clickScript = this.OnClientLinkClick;

            if (string.IsNullOrEmpty(dataTag) == false)
            {
                anchor.Attributes.Add(dataTag, org.ID);
            }

            if (string.IsNullOrEmpty(clickScript) == false)
            {
                anchor.Attributes.Add("onclick", clickScript);
            }

            RenderControlEventArgs e = new RenderControlEventArgs(anchor, org);

            this.OnBuildingAnchor(e);

            return anchor;
        }

        private string BuildUrl(IOrganization org)
        {
            string urlFormat = this.NavigateUrlFormat;

            if (urlFormat.IsNotEmpty())
            {
                urlFormat = this.ResolveUrl(string.Format(urlFormat, this.Page.Server.UrlEncode(org.ID)));
            }

            return urlFormat;
        }

        private static Dictionary<string, IEnumerable<IOrganization>> AllObjectParents
        {
            get
            {
                Dictionary<string, IEnumerable<IOrganization>> result = (Dictionary<string, IEnumerable<IOrganization>>)HttpContext.Current.Items[OuNavigatorAllObjectParents];

                if (result == null)
                {
                    result = UserOUControlSettings.GetConfig().UserOUControlQuery.QueryObjectsParents(AllObjectIDs.Keys.ToArray());

                    //查询并填充
                    HttpContext.Current.Items[OuNavigatorAllObjectParents] = result;
                }

                return result;
            }
        }

        private static Dictionary<string, string> AllObjectIDs
        {
            get
            {
                Dictionary<string, string> list = (Dictionary<string, string>)HttpContext.Current.Items[OuNavigatorAllObjectIds];

                if (list == null)
                {
                    list = new Dictionary<string, string>(40);
                    HttpContext.Current.Items[OuNavigatorAllObjectIds] = list;
                }

                return list;
            }
        }

        private static Hashtable Terminals
        {
            get
            {
                Hashtable map = (Hashtable)HttpContext.Current.Items[OuNavigatorAllTermials];

                if (map == null)
                {
                    map = new Hashtable();
                    map.Add(string.Empty, UserOUControlSettings.GetConfig().UserOUControlQuery.GetOrganizationByPath(string.Empty));

                    foreach (IOguObject obj in UserOUControlSettings.GetConfig().UserOUControlQuery.GetObjects(AllObjectIDs.Values.ToArray()))
                    {
                        map[obj.ID] = obj;
                    }

                    HttpContext.Current.Items[OuNavigatorAllTermials] = map;
                }

                return map;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
            base.OnLoad(e);
        }

        private void Page_PreRenderComplete(object sender, EventArgs e)
        {
            this.EnsureInAllObjectIDs();
        }

        private void EnsureInAllObjectIDs()
        {
            if (string.IsNullOrEmpty(this.ObjectID) == false)
            {
                if (AllObjectIDs.ContainsKey(ObjectID) == false)
                {
                    AllObjectIDs.Add(this.ObjectID, this.ObjectID);
                }
            }
        }
    }

    /// <summary>
    /// 表示事件参数
    /// </summary>
    public class RenderControlEventArgs : EventArgs
    {
        private Control control;
        private object data;

        /// <summary>
        /// 获取呈现的组织数据
        /// </summary>
        public object Data
        {
            get { return this.data; }
        }

        /// <summary>
        /// 获取正要进行呈现的Html控件
        /// </summary>
        public Control Control
        {
            get { return this.control; }
        }

        /// <summary>
        /// 使用指定的<see cref="Control"/>初始化<see cref="RenderControlEventArgs"/>的新实例。
        /// </summary>
        /// <param name="htmlControl">控件</param>
        /// <param name="data">节点数据</param>
        public RenderControlEventArgs(Control htmlControl, object data)
        {
            htmlControl.NullCheck("htmlControl");
            this.control = htmlControl;
            this.data = data;
        }

    }
}
