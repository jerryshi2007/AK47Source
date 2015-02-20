using System;
using System.Drawing.Design;
using System.Text;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Web.UI;
using System.ComponentModel;
using MCS.Web.Library.Script;
using System.Web;
using System.Security.Permissions;
using System.Web.UI.Design;
using MCS.Library.Core;
using MCS.Library.Globalization;

[assembly: WebResource("MCS.Web.WebControls.DeluxeTree.TreeNode.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 子节点的加载方式
    /// </summary>
    public enum ChildNodesLoadingTypeDefine
    {
        /// <summary>
        /// 正常加载
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 延迟加载，从服务器动态加载
        /// </summary>
        LazyLoading = 1
    }

    /// <summary>
    /// 树节点定义
    /// </summary>
    [ClientScriptResource("", "MCS.Web.WebControls.DeluxeTree.TreeNode.js")]
    [ParseChildren(true, "Nodes")]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DeluxeTreeNode
    {
        private string text = string.Empty;
        private string html = string.Empty;
        private string value = string.Empty;
        private string toolTip = string.Empty;
        private bool enableToolTip = true;
        private string nodeOpenImg = string.Empty;
        private string nodeCloseImg = string.Empty;

        private VerticalAlign nodeVerticalAlign = VerticalAlign.NotSet;

        private Unit imgWidth = Unit.Empty;
        private Unit imgHeight = Unit.Empty;
        private Unit imgMarginLeft = Unit.Empty;
        private Unit imgMarginTop = Unit.Empty;

        private bool expanded = false;
        private bool _checked = false;
        private bool _selected = false;
        private bool showCheckBox = false;
        private bool subNodesLoaded = false;
        private bool textNoWrap = true;

        private ChildNodesLoadingTypeDefine childNodesLoadingType = ChildNodesLoadingTypeDefine.Normal;
        private string lazyLoadingText = Translator.Translate(Define.DefaultCategory, "正在加载...");
        private string cssClass = string.Empty;
        private string selectedCssClass = string.Empty;
        private string navigateUrl = string.Empty;
        private string target = string.Empty;
        private string extendedDataKey = string.Empty;

        private DeluxeTreeNodeCollection nodes;
        private DeluxeTreeNode previousSibling;
        private DeluxeTreeNode nextSibling;
        private DeluxeTreeNode parent;

        internal object extendedData;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="text">节点的显示文字</param>
        /// <param name="value">节点的值</param>
        public DeluxeTreeNode(string text, string value)
        {
            this.text = text;
            this.value = value;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public DeluxeTreeNode()
        {
        }


        /// <summary>
        /// 延迟加载所显示的子节点的名称
        /// </summary>
        [DefaultValue("正在加载...")]
        [Bindable(true), Category("Appearance"), Description("延迟加载的节点所显示的文字")]
        public string LazyLoadingText
        {
            get { return this.lazyLoadingText; }
            set { this.lazyLoadingText = value; }
        }

        /// <summary>
        /// 子节点是否延迟加载
        /// </summary>
        [DefaultValue(ChildNodesLoadingTypeDefine.Normal), Description("子节点是否延迟加载")]
        public ChildNodesLoadingTypeDefine ChildNodesLoadingType
        {
            get { return this.childNodesLoadingType; }
            set { this.childNodesLoadingType = value; }
        }

        /// <summary>
        /// 节点所显示的文字
        /// </summary>
        [DefaultValue("")]
        [Bindable(true), Category("Appearance"), Description("节点所显示的文字")]
        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        /// <summary>
        /// 节点所显示的Html，如果此属性为空，则使用Text属性
        /// </summary>
        [DefaultValue("")]
        [Bindable(true), Category("Appearance"), Description("节点所显示的Html")]
        public string Html
        {
            get { return this.html; }
            set { this.html = value; }
        }

        /// <summary>
        /// 节点的值
        /// </summary>
        [DefaultValue("")]
        [Bindable(true), Category("Appearance"), Description("节点的值")]
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// 节点的提示信息
        /// </summary>
        [DefaultValue("")]
        [Bindable(true), Category("Appearance"), Description("节点的提示信息")]
        public string ToolTip
        {
            get { return this.toolTip; }
            set { this.toolTip = value; }
        }

        /// <summary>
        /// 是否显示节点的提示信息
        /// </summary>
        [DefaultValue(true)]
        [Bindable(true), Category("Appearance"), Description("是否显示节点的提示信息")]
        public bool EnableToolTip
        {
            get { return this.enableToolTip; }
            set { this.enableToolTip = value; }
        }

        /// <summary>
        /// 节点的纵向对齐方式
        /// </summary>
        [DefaultValue("")]
        [Bindable(true), Category("Appearance"), Description("节点的纵向对齐方式")]
        public VerticalAlign NodeVerticalAlign
        {
            get { return this.nodeVerticalAlign; }
            set { this.nodeVerticalAlign = value; }
        }

        /// <summary>
        /// 节点文字是否折行
        /// </summary>
        [DefaultValue(true)]
        [Bindable(true), Category("Appearance"), Description("节点文字是否折行")]
        public bool TextNoWrap
        {
            get { return this.textNoWrap; }
            set { this.textNoWrap = value; }
        }

        /// <summary>
        /// 节点展开时的图片
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("节点展开时的图片")]
        [UrlProperty, Editor(typeof(UrlEditor), typeof(UITypeEditor))]
        public string NodeOpenImg
        {
            get { return this.nodeOpenImg; }
            set { this.nodeOpenImg = value; }
        }

        /// <summary>
        /// 节点折叠时的图片
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("节点折叠时的图片")]
        [UrlProperty, Editor(typeof(UrlEditor), typeof(UITypeEditor))]
        public string NodeCloseImg
        {
            get { return this.nodeCloseImg; }
            set { this.nodeCloseImg = value; }
        }

        /// <summary>
        /// 节点图片的宽度
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("节点图片的宽度")]
        public Unit ImgWidth
        {
            get
            {
                return this.imgWidth;
            }
            set
            {
                this.imgWidth = value;
            }
        }

        /// <summary>
        /// 节点图片的高度
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("节点图片的高度")]
        public Unit ImgHeight
        {
            get
            {
                return this.imgHeight;
            }
            set
            {
                this.imgHeight = value;
            }
        }

        /// <summary>
        /// 节点图片的左边界的起始位置
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("节点图片的左边界的起始位置")]
        public Unit ImgMarginLeft
        {
            get
            {
                return this.imgMarginLeft;
            }
            set
            {
                this.imgMarginLeft = value;
            }
        }

        /// <summary>
        /// 节点图片的左边界的起始位置
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("节点图片的上边界的起始位置")]
        public Unit ImgMarginTop
        {
            get
            {
                return this.imgMarginTop;
            }
            set
            {
                this.imgMarginTop = value;
            }
        }

        /// <summary>
        /// 节点是否展开
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("节点是否展开"), DefaultValue(false)]
        public bool Expanded
        {
            get { return this.expanded; }
            set { this.expanded = value; }
        }

        /// <summary>
        /// 节点的复选框是否被选中
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("节点的复选框是否被选中")]
        [DefaultValue(false)]
        public bool Checked
        {
            get { return this._checked; }
            set { this._checked = value; }
        }

        /// <summary>
        /// 节点是否处于选中状态
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("节点是否处于选中状态")]
        [DefaultValue(false)]
        public bool Selected
        {
            get { return this._selected; }
            set { this._selected = value; }
        }

        /// <summary>
        /// 节点是否显示选择框
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("节点是否显示选择框")]
        [DefaultValue(false)]
        public bool ShowCheckBox
        {
            get { return this.showCheckBox; }
            set { this.showCheckBox = value; }
        }

        /// <summary>
        /// 子节点是否已经加载
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        public bool SubNodesLoaded
        {
            get
            {
                return this.subNodesLoaded;
            }
            set
            {
                this.subNodesLoaded = value;
            }
        }

        /// <summary>
        /// 节点文字的样式
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("节点文字的样式")]
        public string CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }

        /// <summary>
        /// 节点选中时的样式
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("节点选中时的样式")]
        public string SelectedCssClass
        {
            get { return this.selectedCssClass; }
            set { this.selectedCssClass = value; }
        }

        /// <summary>
        /// 节点选中时的导航的url
        /// </summary>
        [Bindable(true), Category("Appearance"), UrlProperty, Description("节点选中时的导航的url")]
        public string NavigateUrl
        {
            get
            {
                return this.navigateUrl;
            }
            set
            {
                this.navigateUrl = value;
            }
        }

        /// <summary>
        /// 节点导航的url的目标窗口名称
        /// </summary>
        [Bindable(true), Category("Appearance"), UrlProperty, Description("节点导航的url的目标窗口名称")]
        public string Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = value;
            }
        }

        /// <summary>
        /// 是否存在子节点
        /// </summary>
        [Browsable(false)]
        public bool HasChildren
        {
            get
            {
                return this.Nodes.Count > 0;
            }
        }

        /// <summary>
        /// 子节点的集合
        /// </summary>
        [Browsable(false)]
        [MergableProperty(false)]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        [Category("Data")]
        public DeluxeTreeNodeCollection Nodes
        {
            get
            {
                if (this.nodes == null)
                    this.nodes = new DeluxeTreeNodeCollection(this);

                return this.nodes;
            }
        }

        /// <summary>
        /// 前一个兄弟节点
        /// </summary>
        [Browsable(false)]
        [ScriptIgnore()]
        public DeluxeTreeNode PreviousSibling
        {
            get { return this.previousSibling; }
            internal set { this.previousSibling = value; }
        }

        /// <summary>
        /// 下一个兄弟节点
        /// </summary>
        [Browsable(false)]
        [ScriptIgnore()]
        public DeluxeTreeNode NextSibling
        {
            get { return this.nextSibling; }
            internal set { this.nextSibling = value; }
        }

        /// <summary>
        /// 父节点
        /// </summary>
        [Browsable(false)]
        [ScriptIgnore()]
        public DeluxeTreeNode Parent
        {
            get { return this.parent; }
            internal set { this.parent = value; }
        }

        /// <summary>
        /// 扩展数据
        /// </summary>
        [Browsable(false)]
        [Category("Data")]
        public object ExtendedData
        {
            get { return this.extendedData; }
            set { this.extendedData = value; }
        }

        /// <summary>
        /// 扩展数据在客户端的类型key
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("extendedDataKey")]
        [Browsable(false)]
        public string ExtendedDataKey
        {
            get
            {
                return this.extendedDataKey;
            }
            set
            {
                this.extendedDataKey = value;
            }
        }
    }

    /// <summary>
    /// 树节点的集合
    /// </summary>
    public class DeluxeTreeNodeCollection : ICollection, IEnumerable
    {
        private IList<DeluxeTreeNode> list = new List<DeluxeTreeNode>();
        private DeluxeTreeNode owner;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="owner">父节点</param>
        public DeluxeTreeNodeCollection(DeluxeTreeNode owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// 按照序号得到节点
        /// </summary>
        /// <param name="index">节点的序号</param>
        /// <returns>节点对象</returns>
        public DeluxeTreeNode this[int index]
        {
            get
            {
                return this.list[index];
            }
        }

        /// <summary>
        /// 增加一个节点
        /// </summary>
        /// <param name="node">节点对象</param>
        public void Add(DeluxeTreeNode node)
        {
            this.AddAt(this.list.Count, node);
        }

        /// <summary>
        /// 在指定位置增加一个节点
        /// </summary>
        /// <param name="index">位置</param>
        /// <param name="node">节点对象</param>
        public void AddAt(int index, DeluxeTreeNode node)
        {
            node.Parent = this.owner;
            this.list.Insert(index, node);
            if (index == 0)
            {
                node.PreviousSibling = null;
                if (this.list.Count > 1)
                {
                    node.NextSibling = this[1];
                    this[1].PreviousSibling = node;
                }
                else
                    node.NextSibling = null;
            }
            else if (index == (this.list.Count - 1))
            {
                node.NextSibling = null;
                if (this.list.Count > 1)
                {
                    node.PreviousSibling = this[index - 1];
                    this[index - 1].NextSibling = node;
                }
                else
                    node.PreviousSibling = null;
            }
            else if (index > 0 && index < this.list.Count)
            {
                node.PreviousSibling = this[index - 1];
                this[index - 1].NextSibling = node;

                node.NextSibling = this[index + 1];
                this[index + 1].PreviousSibling = node;
            }
        }

        /// <summary>
        /// 清除所有节点
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// 是否包含某个节点
        /// </summary>
        /// <param name="node">节点对象</param>
        /// <returns>是否包含</returns>
        public bool Contains(DeluxeTreeNode node)
        {
            return this.list.Contains(node);
        }

        /// <summary>
        /// 删除某个节点
        /// </summary>
        /// <param name="node">节点对象</param>
        public void Remove(DeluxeTreeNode node)
        {
            int index = this.list.IndexOf(node);
            if (index != -1)
                this.RemoveAt(index);
        }

        /// <summary>
        /// 删除指定位置的节点
        /// </summary>
        /// <param name="index">节点的位置</param>
        public void RemoveAt(int index)
        {
            if (index == 0)
            {
                if (this.Count > 1)
                    this[1].PreviousSibling = null;
            }
            else if (index == this.Count - 1)
            {
                if (this.Count > 1)
                    this[index - 1].NextSibling = null;
            }
            else if (index > 0 && index < this.Count - 1)
            {
                this[index - 1].NextSibling = this[index + 1];
                this[index + 1].PreviousSibling = this[index - 1];
            }
            this[index].Parent = this[index].PreviousSibling = this[index].NextSibling = null;
            this.list.RemoveAt(index);
        }

        /// <summary>
        /// 得到某个节点的位置
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int IndexOf(DeluxeTreeNode node)
        {
            return this.list.IndexOf(node);
        }

        #region ICollection Members
        /// <summary>
        /// 将DeluxeTreeNode数组的内容复制到集合中
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            list.CopyTo((DeluxeTreeNode[])array, index);
        }

        /// <summary>
        /// 节点的个数
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// 是否是用于同步复制的集合
        /// </summary>
        public bool IsSynchronized
        {
            get { return ((ICollection)this.list).IsSynchronized; }
        }

        /// <summary>
        /// 同步对象
        /// </summary>
        public object SyncRoot
        {
            get { return ((ICollection)this.list).SyncRoot; }
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// 得到迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            foreach (DeluxeTreeNode node in this.list)
                yield return node;
        }
        #endregion
    }

    /// <summary>
    /// 树节点的JSON序列化器
    /// </summary>
    public class DeluxeTreeNodeConverter : JavaScriptConverter
    {
        /// <summary>
        /// 反序列化数节点
        /// </summary>
        /// <param name="dictionary">属性字典</param>
        /// <param name="type">对象类型</param>
        /// <param name="serializer">JS序列化器</param>
        /// <returns>反序列化生成的对象</returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            DeluxeTreeNode node = new DeluxeTreeNode();

            node.Text = DictionaryHelper.GetValue(dictionary, "text", string.Empty);
            node.Html = DictionaryHelper.GetValue(dictionary, "html", string.Empty);
            node.Value = DictionaryHelper.GetValue(dictionary, "value", string.Empty);
            node.ToolTip = DictionaryHelper.GetValue(dictionary, "toolTip", string.Empty);
            node.EnableToolTip = DictionaryHelper.GetValue(dictionary, "enableToolTip", true);
            node.NodeOpenImg = DictionaryHelper.GetValue(dictionary, "nodeOpenImg", string.Empty);
            node.NodeCloseImg = DictionaryHelper.GetValue(dictionary, "nodeCloseImg", string.Empty);
            node.ImgWidth = Unit.Parse(DictionaryHelper.GetValue(dictionary, "imgWidth", string.Empty));
            node.ImgHeight = Unit.Parse(DictionaryHelper.GetValue(dictionary, "imgHeight", string.Empty));
            node.ImgMarginLeft = Unit.Parse(DictionaryHelper.GetValue(dictionary, "imgMarginLeft", string.Empty));
            node.ImgMarginTop = Unit.Parse(DictionaryHelper.GetValue(dictionary, "imgMarginTop", string.Empty));

            if (dictionary.ContainsKey("extendedData"))
                node.ExtendedData = dictionary["extendedData"];

            node.Checked = DictionaryHelper.GetValue(dictionary, "checked", false);
            node.Selected = DictionaryHelper.GetValue(dictionary, "selected", false);
            node.Expanded = DictionaryHelper.GetValue(dictionary, "expanded", false);

            node.SubNodesLoaded = DictionaryHelper.GetValue(dictionary, "subNodesLoaded", false);

            node.ShowCheckBox = DictionaryHelper.GetValue(dictionary, "showCheckBox", false);
            node.CssClass = DictionaryHelper.GetValue(dictionary, "cssClass", string.Empty);
            node.SelectedCssClass = DictionaryHelper.GetValue(dictionary, "selectedCssClass", string.Empty);
            node.ChildNodesLoadingType = (ChildNodesLoadingTypeDefine)dictionary["childNodesLoadingType"];
            node.LazyLoadingText = DictionaryHelper.GetValue(dictionary, "lazyLoadingText", string.Empty);
            node.NavigateUrl = DictionaryHelper.GetValue(dictionary, "navigateUrl", string.Empty);
            node.Target = DictionaryHelper.GetValue(dictionary, "target", string.Empty);
            node.ExtendedDataKey = DictionaryHelper.GetValue(dictionary, "extendedDataKey", string.Empty);

            node.TextNoWrap = DictionaryHelper.GetValue(dictionary, "textNoWrap", true);
            node.NodeVerticalAlign = DictionaryHelper.GetValue(dictionary, "nodeVerticalAlign", VerticalAlign.NotSet);

            object objNodes;

            if (dictionary.TryGetValue("nodes", out objNodes))
            {
                ArrayList nodes = (ArrayList)objNodes;
                for (int i = 0; i < nodes.Count; i++)
                    node.Nodes.Add((DeluxeTreeNode)Deserialize((IDictionary<string, object>)nodes[i], type, serializer));
            }

            return node;
        }

        /// <summary>
        /// 将树节点进行JSON序列化
        /// </summary>
        /// <param name="obj">树节点对象</param>
        /// <param name="serializer">序列化器</param>
        /// <returns>属性集合</returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            DeluxeTreeNode node = (DeluxeTreeNode)obj;

            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "text", node.Text);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "html", node.Html);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "value", node.Value);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "toolTip", node.ToolTip);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "enableToolTip", node.EnableToolTip, true);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "nodeOpenImg", node.NodeOpenImg);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "nodeCloseImg", node.NodeCloseImg);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "imgWidth", node.ImgWidth.ToString());
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "imgHeight", node.ImgHeight.ToString());
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "imgMarginLeft", node.ImgMarginLeft.ToString());
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "imgMarginTop", node.ImgMarginTop.ToString());
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "extendedData", node.ExtendedData);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "expanded", node.Expanded);

            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "subNodesLoaded", node.SubNodesLoaded);

            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "textNoWrap", node.TextNoWrap, true);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "nodeVerticalAlign", node.NodeVerticalAlign, VerticalAlign.NotSet);

            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "showCheckBox", node.ShowCheckBox);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "checked", node.Checked);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "selected", node.Selected);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "childNodesLoadingType", node.ChildNodesLoadingType);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "lazyLoadingText", node.LazyLoadingText);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "cssClass", node.CssClass);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "selectedCssClass", node.SelectedCssClass);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "navigateUrl", node.NavigateUrl);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "target", node.Target);
            DictionaryHelper.AddNonDefaultValue<string, object>(dict, "extendedDataKey", node.ExtendedDataKey);

            DeluxeTreeNode[] nodes = new DeluxeTreeNode[node.Nodes.Count];
            for (int i = 0; i < nodes.Length; i++)
                nodes[i] = node.Nodes[i];

            dict.Add("nodes", nodes);

            return dict;
        }

        /// <summary>
        /// 得到所支持的序列化类型
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new System.Type[] { typeof(DeluxeTreeNode) };
            }
        }
    }

    /// <summary>
    /// 树节点集合的序列化器
    /// </summary>
    public class DeluxeTreeNodeListConverter : JavaScriptConverter
    {
        /// <summary>
        /// 反序列化节点集合
        /// </summary>
        /// <param name="dictionary">属性集合</param>
        /// <param name="type">对象类型</param>
        /// <param name="serializer">JS序列化器</param>
        /// <returns>反序列化生成的对象</returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            List<DeluxeTreeNode> rootNode = new List<DeluxeTreeNode>();

            ArrayList nodes = (ArrayList)dictionary["nodes"];
            DeluxeTreeNodeConverter nodeConvert = new DeluxeTreeNodeConverter();
            for (int i = 0; i < nodes.Count; i++)
                rootNode.Add((DeluxeTreeNode)nodeConvert.Deserialize((IDictionary<string, object>)nodes[i], typeof(DeluxeTreeNode), serializer));

            return rootNode;
        }

        /// <summary>
        /// 序列化节点集合
        /// </summary>
        /// <param name="obj">树节点对象</param>
        /// <param name="serializer">JS序列化器</param>
        /// <returns>属性集合</returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            IList list = (IList)obj;
            DeluxeTreeNode[] nodes = new DeluxeTreeNode[list.Count];

            for (int i = 0; i < list.Count; i++)
                nodes[i] = (DeluxeTreeNode)((IList)obj)[i];

            dict.Add("treeNodes", nodes);

            return dict;
        }

        /// <summary>
        /// 所支持的序列化类型
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new System.Type[] { typeof(List<DeluxeTreeNode>) };
            }
        }
    }
}