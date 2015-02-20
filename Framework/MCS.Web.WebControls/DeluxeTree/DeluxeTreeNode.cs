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
    /// �ӽڵ�ļ��ط�ʽ
    /// </summary>
    public enum ChildNodesLoadingTypeDefine
    {
        /// <summary>
        /// ��������
        /// </summary>
        Normal = 0,

        /// <summary>
        /// �ӳټ��أ��ӷ�������̬����
        /// </summary>
        LazyLoading = 1
    }

    /// <summary>
    /// ���ڵ㶨��
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
        private string lazyLoadingText = Translator.Translate(Define.DefaultCategory, "���ڼ���...");
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
        /// ���췽��
        /// </summary>
        /// <param name="text">�ڵ����ʾ����</param>
        /// <param name="value">�ڵ��ֵ</param>
        public DeluxeTreeNode(string text, string value)
        {
            this.text = text;
            this.value = value;
        }

        /// <summary>
        /// ���췽��
        /// </summary>
        public DeluxeTreeNode()
        {
        }


        /// <summary>
        /// �ӳټ�������ʾ���ӽڵ������
        /// </summary>
        [DefaultValue("���ڼ���...")]
        [Bindable(true), Category("Appearance"), Description("�ӳټ��صĽڵ�����ʾ������")]
        public string LazyLoadingText
        {
            get { return this.lazyLoadingText; }
            set { this.lazyLoadingText = value; }
        }

        /// <summary>
        /// �ӽڵ��Ƿ��ӳټ���
        /// </summary>
        [DefaultValue(ChildNodesLoadingTypeDefine.Normal), Description("�ӽڵ��Ƿ��ӳټ���")]
        public ChildNodesLoadingTypeDefine ChildNodesLoadingType
        {
            get { return this.childNodesLoadingType; }
            set { this.childNodesLoadingType = value; }
        }

        /// <summary>
        /// �ڵ�����ʾ������
        /// </summary>
        [DefaultValue("")]
        [Bindable(true), Category("Appearance"), Description("�ڵ�����ʾ������")]
        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        /// <summary>
        /// �ڵ�����ʾ��Html�����������Ϊ�գ���ʹ��Text����
        /// </summary>
        [DefaultValue("")]
        [Bindable(true), Category("Appearance"), Description("�ڵ�����ʾ��Html")]
        public string Html
        {
            get { return this.html; }
            set { this.html = value; }
        }

        /// <summary>
        /// �ڵ��ֵ
        /// </summary>
        [DefaultValue("")]
        [Bindable(true), Category("Appearance"), Description("�ڵ��ֵ")]
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// �ڵ����ʾ��Ϣ
        /// </summary>
        [DefaultValue("")]
        [Bindable(true), Category("Appearance"), Description("�ڵ����ʾ��Ϣ")]
        public string ToolTip
        {
            get { return this.toolTip; }
            set { this.toolTip = value; }
        }

        /// <summary>
        /// �Ƿ���ʾ�ڵ����ʾ��Ϣ
        /// </summary>
        [DefaultValue(true)]
        [Bindable(true), Category("Appearance"), Description("�Ƿ���ʾ�ڵ����ʾ��Ϣ")]
        public bool EnableToolTip
        {
            get { return this.enableToolTip; }
            set { this.enableToolTip = value; }
        }

        /// <summary>
        /// �ڵ��������뷽ʽ
        /// </summary>
        [DefaultValue("")]
        [Bindable(true), Category("Appearance"), Description("�ڵ��������뷽ʽ")]
        public VerticalAlign NodeVerticalAlign
        {
            get { return this.nodeVerticalAlign; }
            set { this.nodeVerticalAlign = value; }
        }

        /// <summary>
        /// �ڵ������Ƿ�����
        /// </summary>
        [DefaultValue(true)]
        [Bindable(true), Category("Appearance"), Description("�ڵ������Ƿ�����")]
        public bool TextNoWrap
        {
            get { return this.textNoWrap; }
            set { this.textNoWrap = value; }
        }

        /// <summary>
        /// �ڵ�չ��ʱ��ͼƬ
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("�ڵ�չ��ʱ��ͼƬ")]
        [UrlProperty, Editor(typeof(UrlEditor), typeof(UITypeEditor))]
        public string NodeOpenImg
        {
            get { return this.nodeOpenImg; }
            set { this.nodeOpenImg = value; }
        }

        /// <summary>
        /// �ڵ��۵�ʱ��ͼƬ
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("�ڵ��۵�ʱ��ͼƬ")]
        [UrlProperty, Editor(typeof(UrlEditor), typeof(UITypeEditor))]
        public string NodeCloseImg
        {
            get { return this.nodeCloseImg; }
            set { this.nodeCloseImg = value; }
        }

        /// <summary>
        /// �ڵ�ͼƬ�Ŀ��
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("�ڵ�ͼƬ�Ŀ��")]
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
        /// �ڵ�ͼƬ�ĸ߶�
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("�ڵ�ͼƬ�ĸ߶�")]
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
        /// �ڵ�ͼƬ����߽����ʼλ��
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("�ڵ�ͼƬ����߽����ʼλ��")]
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
        /// �ڵ�ͼƬ����߽����ʼλ��
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("�ڵ�ͼƬ���ϱ߽����ʼλ��")]
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
        /// �ڵ��Ƿ�չ��
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("�ڵ��Ƿ�չ��"), DefaultValue(false)]
        public bool Expanded
        {
            get { return this.expanded; }
            set { this.expanded = value; }
        }

        /// <summary>
        /// �ڵ�ĸ�ѡ���Ƿ�ѡ��
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("�ڵ�ĸ�ѡ���Ƿ�ѡ��")]
        [DefaultValue(false)]
        public bool Checked
        {
            get { return this._checked; }
            set { this._checked = value; }
        }

        /// <summary>
        /// �ڵ��Ƿ���ѡ��״̬
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("�ڵ��Ƿ���ѡ��״̬")]
        [DefaultValue(false)]
        public bool Selected
        {
            get { return this._selected; }
            set { this._selected = value; }
        }

        /// <summary>
        /// �ڵ��Ƿ���ʾѡ���
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("�ڵ��Ƿ���ʾѡ���")]
        [DefaultValue(false)]
        public bool ShowCheckBox
        {
            get { return this.showCheckBox; }
            set { this.showCheckBox = value; }
        }

        /// <summary>
        /// �ӽڵ��Ƿ��Ѿ�����
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
        /// �ڵ����ֵ���ʽ
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("�ڵ����ֵ���ʽ")]
        public string CssClass
        {
            get { return this.cssClass; }
            set { this.cssClass = value; }
        }

        /// <summary>
        /// �ڵ�ѡ��ʱ����ʽ
        /// </summary>
        [Bindable(true), Category("Appearance"), Description("�ڵ�ѡ��ʱ����ʽ")]
        public string SelectedCssClass
        {
            get { return this.selectedCssClass; }
            set { this.selectedCssClass = value; }
        }

        /// <summary>
        /// �ڵ�ѡ��ʱ�ĵ�����url
        /// </summary>
        [Bindable(true), Category("Appearance"), UrlProperty, Description("�ڵ�ѡ��ʱ�ĵ�����url")]
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
        /// �ڵ㵼����url��Ŀ�괰������
        /// </summary>
        [Bindable(true), Category("Appearance"), UrlProperty, Description("�ڵ㵼����url��Ŀ�괰������")]
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
        /// �Ƿ�����ӽڵ�
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
        /// �ӽڵ�ļ���
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
        /// ǰһ���ֵܽڵ�
        /// </summary>
        [Browsable(false)]
        [ScriptIgnore()]
        public DeluxeTreeNode PreviousSibling
        {
            get { return this.previousSibling; }
            internal set { this.previousSibling = value; }
        }

        /// <summary>
        /// ��һ���ֵܽڵ�
        /// </summary>
        [Browsable(false)]
        [ScriptIgnore()]
        public DeluxeTreeNode NextSibling
        {
            get { return this.nextSibling; }
            internal set { this.nextSibling = value; }
        }

        /// <summary>
        /// ���ڵ�
        /// </summary>
        [Browsable(false)]
        [ScriptIgnore()]
        public DeluxeTreeNode Parent
        {
            get { return this.parent; }
            internal set { this.parent = value; }
        }

        /// <summary>
        /// ��չ����
        /// </summary>
        [Browsable(false)]
        [Category("Data")]
        public object ExtendedData
        {
            get { return this.extendedData; }
            set { this.extendedData = value; }
        }

        /// <summary>
        /// ��չ�����ڿͻ��˵�����key
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
    /// ���ڵ�ļ���
    /// </summary>
    public class DeluxeTreeNodeCollection : ICollection, IEnumerable
    {
        private IList<DeluxeTreeNode> list = new List<DeluxeTreeNode>();
        private DeluxeTreeNode owner;

        /// <summary>
        /// ���췽��
        /// </summary>
        /// <param name="owner">���ڵ�</param>
        public DeluxeTreeNodeCollection(DeluxeTreeNode owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// ������ŵõ��ڵ�
        /// </summary>
        /// <param name="index">�ڵ�����</param>
        /// <returns>�ڵ����</returns>
        public DeluxeTreeNode this[int index]
        {
            get
            {
                return this.list[index];
            }
        }

        /// <summary>
        /// ����һ���ڵ�
        /// </summary>
        /// <param name="node">�ڵ����</param>
        public void Add(DeluxeTreeNode node)
        {
            this.AddAt(this.list.Count, node);
        }

        /// <summary>
        /// ��ָ��λ������һ���ڵ�
        /// </summary>
        /// <param name="index">λ��</param>
        /// <param name="node">�ڵ����</param>
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
        /// ������нڵ�
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// �Ƿ����ĳ���ڵ�
        /// </summary>
        /// <param name="node">�ڵ����</param>
        /// <returns>�Ƿ����</returns>
        public bool Contains(DeluxeTreeNode node)
        {
            return this.list.Contains(node);
        }

        /// <summary>
        /// ɾ��ĳ���ڵ�
        /// </summary>
        /// <param name="node">�ڵ����</param>
        public void Remove(DeluxeTreeNode node)
        {
            int index = this.list.IndexOf(node);
            if (index != -1)
                this.RemoveAt(index);
        }

        /// <summary>
        /// ɾ��ָ��λ�õĽڵ�
        /// </summary>
        /// <param name="index">�ڵ��λ��</param>
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
        /// �õ�ĳ���ڵ��λ��
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int IndexOf(DeluxeTreeNode node)
        {
            return this.list.IndexOf(node);
        }

        #region ICollection Members
        /// <summary>
        /// ��DeluxeTreeNode��������ݸ��Ƶ�������
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            list.CopyTo((DeluxeTreeNode[])array, index);
        }

        /// <summary>
        /// �ڵ�ĸ���
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// �Ƿ�������ͬ�����Ƶļ���
        /// </summary>
        public bool IsSynchronized
        {
            get { return ((ICollection)this.list).IsSynchronized; }
        }

        /// <summary>
        /// ͬ������
        /// </summary>
        public object SyncRoot
        {
            get { return ((ICollection)this.list).SyncRoot; }
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// �õ�������
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
    /// ���ڵ��JSON���л���
    /// </summary>
    public class DeluxeTreeNodeConverter : JavaScriptConverter
    {
        /// <summary>
        /// �����л����ڵ�
        /// </summary>
        /// <param name="dictionary">�����ֵ�</param>
        /// <param name="type">��������</param>
        /// <param name="serializer">JS���л���</param>
        /// <returns>�����л����ɵĶ���</returns>
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
        /// �����ڵ����JSON���л�
        /// </summary>
        /// <param name="obj">���ڵ����</param>
        /// <param name="serializer">���л���</param>
        /// <returns>���Լ���</returns>
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
        /// �õ���֧�ֵ����л�����
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
    /// ���ڵ㼯�ϵ����л���
    /// </summary>
    public class DeluxeTreeNodeListConverter : JavaScriptConverter
    {
        /// <summary>
        /// �����л��ڵ㼯��
        /// </summary>
        /// <param name="dictionary">���Լ���</param>
        /// <param name="type">��������</param>
        /// <param name="serializer">JS���л���</param>
        /// <returns>�����л����ɵĶ���</returns>
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
        /// ���л��ڵ㼯��
        /// </summary>
        /// <param name="obj">���ڵ����</param>
        /// <param name="serializer">JS���л���</param>
        /// <returns>���Լ���</returns>
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
        /// ��֧�ֵ����л�����
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