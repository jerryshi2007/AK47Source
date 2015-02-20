using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Script;
using MCS.Library.Core;
using MCS.Web.Responsive.WebControls.Properties;

#region
[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeTree.minus.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeTree.plus.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeTree.openImg.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeTree.closeImg.gif", "image/gif")]
//[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeTree.hourglass.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeTree.Tree.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeTree.DeluxeTree.css", "text/css")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeTree.DeluxeTree_No_Lines.css", "text/css")]
#endregion

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// ���ؼ�
    /// </summary>
    //���û����ű�
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(DeluxeTreeNode), 8)]
    //���ñ��ؼ��ű�����һ��Ϊ�ͻ��˿ؼ�������
    [ClientScriptResource("MCS.Web.WebControls.DeluxeTree", "MCS.Web.Responsive.WebControls.DeluxeTree.Tree.js")]

    [ToolboxBitmap(typeof(DeluxeTree), "Resources.Tree.bmp")]
    [Designer(typeof(DeluxeTreeItemsDesigner))]
    [PersistChildren(false)]
    [ParseChildren(true)]
    //[ClientCssResource("MCS.Web.Responsive.WebControls.DeluxeTree.DeluxeTree.css")]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DeluxeTree : ScriptControlBase
    {
        private int scrollTop = 0;
        private int scrollLeft = 0;

        /// <summary>
        /// �õ��ӽڵ��ί�ж���
        /// </summary>
        /// <param name="parentNode">���ڵ�</param>
        /// <param name="callBackContext">������</param>
        /// <param name="result">������ӽڵ㼯�ϣ�</param>
        public delegate void GetChildrenDataDelegate(DeluxeTreeNode parentNode, DeluxeTreeNodeCollection result, string callBackContext);

        /// <summary>
        /// �õ��ӽڵ���¼�����
        /// </summary>
        public event GetChildrenDataDelegate GetChildrenData;

        /// <summary>
        /// ���췽��
        /// </summary>
        public DeluxeTree()
            : base(true, HtmlTextWriterTag.Div)
        {
            //DeluxeTreeNodeListConverter
            JSONSerializerExecute.RegisterConverter(typeof(DeluxeTreeNodeConverter));
            JSONSerializerExecute.RegisterConverter(typeof(DeluxeTreeNodeListConverter));
        }

        #region ���ط���
        /// <summary>
        /// ��Ⱦ���ͻ���
        /// </summary>
        /// <param name="writer">HtmlTextWriter</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
                writer.Write(this.CreateDesignTimeControlsHtml());
            else
                base.Render(writer);
        }

        /// <summary>
        /// ��Ⱦ���ͻ���֮ǰ�Ĳ������ڱ��ؼ��У�Ԥ�ȼ���ͼƬ
        /// </summary>
        /// <param name="e">�¼�����</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            this.RegisterCSS();

            if (this.DesignMode == false)
                this.PreloadAllImages();

            //this.Style["overflow"] = EnumItemDescriptionAttribute.GetDescription(this.Overflow);
        }

        private void RegisterCSS()
        {
            string cssUrl = string.Empty;

            if (this.ShowLines)
            {
                cssUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.Responsive.WebControls.DeluxeTree.DeluxeTree.css");
            }
            else
            {
                cssUrl = this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.Responsive.WebControls.DeluxeTree.DeluxeTree_No_Lines.css");

            }

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "treeCss",
                string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", cssUrl));
        }

        /// <summary>
        /// ����ClientState
        /// </summary>
        /// <param name="clientState">�ͻ���״̬</param>
        protected override void LoadClientState(string clientState)
        {
            if (string.IsNullOrEmpty(clientState) == false)
            {
                object[] state = (object[])JSONSerializerExecute.DeserializeObject(clientState, typeof(object[]));

                this.scrollLeft = (int)state[0];
                this.scrollTop = (int)state[1];
                DeluxeTreeNode[] nodes = JSONSerializerExecute.Deserialize<DeluxeTreeNode[]>(state[2]);

                this.nodes.Clear();

                foreach (DeluxeTreeNode node in nodes)
                    this.nodes.Add(node);
            }
        }

        /// <summary>
        /// ����ClientState
        /// </summary>
        /// <returns>�ڵ����л����ַ���</returns>
        protected override string SaveClientState()
        {
            if (this.Page.IsCallback == false)
            {
                object[] state = new object[3];

                state[0] = this.scrollLeft;
                state[1] = this.scrollTop;
                state[2] = this.Nodes;

                return JSONSerializerExecute.Serialize(state);
            }
            else
                return string.Empty;
        }
        #endregion

        #region ScriptControlMethods
        /// <summary>
        /// �õ��ӽڵ�Ŀͻ��˻ص�����
        /// </summary>
        /// <param name="paretnNode">���ڵ�</param>
        /// <param name="callBackContext">������</param>
        /// <returns>������ӽڵ㼯�ϣ�</returns>
        [ScriptControlMethod]
        public DeluxeTreeNodeCollection GetChildren(DeluxeTreeNode paretnNode, string callBackContext)
        {
            DeluxeTreeNodeCollection result = new DeluxeTreeNodeCollection(paretnNode);

            if (GetChildrenData != null)
                GetChildrenData(paretnNode, result, callBackContext);

            return result;
        }
        #endregion

        #region Properties
        private DeluxeTreeNodeCollection nodes = new DeluxeTreeNodeCollection(null);

        /// <summary>
        /// �ӽڵ�ļ���
        /// </summary>
        [MergableProperty(false), DefaultValue((string)null)]
        [PersistenceMode(PersistenceMode.InnerProperty), Description("�ӽڵ�ļ���")]
        [Editor(typeof(DeluxeTreeCollectionItemsEditor), typeof(UITypeEditor))]
        public DeluxeTreeNodeCollection Nodes
        {
            get
            {
                return this.nodes;
            }
        }

        /// <summary>
        /// �ڵ��ʱ��ͼƬ
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("nodeOpenImg")]
        [Bindable(true), Category("Appearance"), Description("�ڵ��ʱ��ͼƬ")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
        public string NodeOpenImg
        {
            get
            {
                return GetPropertyValue<string>("NodeOpenImg", string.Empty);
            }
            set
            {
                SetPropertyValue<string>("NodeOpenImg", value);
            }
        }

        /// <summary>
        /// �ڵ�ر�ʱ��ͼƬ
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("nodeCloseImg")]
        [Bindable(true), Category("Appearance"), Description("�ڵ�ر�ʱ��ͼƬ")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
        public string NodeCloseImg
        {
            get
            {
                return GetPropertyValue<string>("NodeCloseImg", string.Empty);
            }
            set
            {
                SetPropertyValue<string>("NodeCloseImg", value);
            }
        }

        ///// <summary>
        ///// ȱʡ��չ���ڵ��ͼƬ
        ///// </summary>
        //[ScriptControlProperty]
        //[ClientPropertyName("defaultWaitingForImage")]
        //[Browsable(false)]
        //private string DefaultWaitingForImage
        //{
        //    get
        //    {
        //        return this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.Responsive.WebControls.DeluxeTree.hourglass.gif");
        //    }
        //}

        /// <summary>
        /// ȱʡ��չ���ڵ��ͼƬ
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("defaultExpandImage")]
        [Browsable(false)]
        private string DefaultExpandImage
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.Responsive.WebControls.DeluxeTree.plus.gif");
            }
        }

        /// <summary>
        /// ȱʡ�Ľڵ��۵�ͼƬ
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("defaultCollapseImage")]
        [Browsable(false)]
        public string DefaultCollapseImage
        {
            get
            {
                return this.Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.Responsive.WebControls.DeluxeTree.minus.gif");
            }
        }

        /// <summary>
        /// �ӽڵ����������
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("nodeIndent")]
        [Bindable(true), Category("Appearance"), Description("�ӽڵ����������")]
        [DefaultValue(16)]
        public int NodeIndent
        {
            get
            {
                return GetPropertyValue<int>("NodeIndent", 16);
            }
            set
            {
                SetPropertyValue<int>("NodeIndent", value);
            }
        }

        /// <summary>
        /// �ڵ���ѡ��ǰ�Ŀͻ��˽ű������������ڽű���ȡ��ѡ�в���
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("nodeSelecting")]
        [Bindable(true), Category("ClientEventsHandler"), Description("�ڵ���ѡ��ǰ�Ŀͻ��˽ű�")]
        public string OnNodeSelecting
        {
            get
            {
                return GetPropertyValue("onNodeSelecting", string.Empty);
            }
            set
            {
                SetPropertyValue("onNodeSelecting", value);
            }
        }

        /// <summary>
        /// �ڽڵ��ϵ����Ҽ��˵��Ŀͻ��˽ű�������ȱʡ�����ε���������Ҽ��˵��ģ������ڽű��д򿪡�
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("nodeContextMenu")]
        [Bindable(true), Category("ClientEventsHandler"), Description("�ڽڵ��ϵ����Ҽ��˵��Ŀͻ��˽ű�������ȱʡ�����ε���������Ҽ��˵��ģ������ڽű��д�")]
        public string OnNodeContextMenu
        {
            get
            {
                return GetPropertyValue("nodeContextMenu", string.Empty);
            }
            set
            {
                SetPropertyValue("nodeContextMenu", value);
            }
        }

        /// <summary>
        /// �ڽڵ���˫���Ŀͻ��˽ű�������
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("nodeDblClick")]
        [Bindable(true), Category("ClientEventsHandler"), Description("�ڽڵ���˫���Ŀͻ��˽ű�����")]
        public string OnNodeDblClick
        {
            get
            {
                return GetPropertyValue("nodeDblClick", string.Empty);
            }
            set
            {
                SetPropertyValue("nodeDblClick", value);
            }
        }

        /// <summary>
        /// ���нڵ��ǰ�Ŀͻ��˽ű�����������ȡ���ڵ�İ�
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("beforeAllDataBind")]
        [Bindable(true), Category("ClientEventsHandler"), Description("���нڵ��ǰ�Ŀͻ��˽ű�����������ȡ���ڵ�İ�")]
        public string OnBeforeAllDataBind
        {
            get
            {
                return GetPropertyValue("beforeAllDataBind", string.Empty);
            }
            set
            {
                SetPropertyValue("beforeAllDataBind", value);
            }
        }

        /// <summary>
        /// ���нڵ�󶨺�Ŀͻ��˽ű�����
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("afterAllDataBind")]
        [Bindable(true), Category("ClientEventsHandler"), Description("���нڵ�󶨺�Ŀͻ��˽ű�����")]
        public string OnAfterAllDataBind
        {
            get
            {
                return GetPropertyValue("afterAllDataBind", string.Empty);
            }
            set
            {
                SetPropertyValue("afterAllDataBind", value);
            }
        }

        /// <summary>
        /// �ڵ��ǰ�Ŀͻ��˽ű�����������ȡ���ڵ�İ�
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("beforeDataBind")]
        [Bindable(true), Category("ClientEventsHandler"), Description("�ڵ��ǰ�Ŀͻ��˽ű�����������ȡ���ڵ�İ�")]
        public string OnNodeBeforeDataBind
        {
            get
            {
                return GetPropertyValue("beforeDataBind", string.Empty);
            }
            set
            {
                SetPropertyValue("beforeDataBind", value);
            }
        }

        /// <summary>
        /// �ڵ�󶨺�Ŀͻ��˽ű�����
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("afterDataBind")]
        [Bindable(true), Category("ClientEventsHandler"), Description("�ڵ�󶨺�Ŀͻ��˽ű�����")]
        public string OnNodeAfterDataBind
        {
            get
            {
                return GetPropertyValue("afterDataBind", string.Empty);
            }
            set
            {
                SetPropertyValue("afterDataBind", value);
            }
        }

        /// <summary>
        /// �ڵ�չ��ǰ�Ŀͻ��˽ű�����������ȡ���ڵ��չ��
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("nodeBeforeExpand")]
        [Bindable(true), Category("ClientEventsHandler"), Description("�ڵ�չ��ǰ�Ŀͻ��˽ű�����������ȡ���ڵ��չ��")]
        public string OnNodeBeforeExpand
        {
            get
            {
                return GetPropertyValue("nodeBeforeExpand", string.Empty);
            }
            set
            {
                SetPropertyValue("nodeBeforeExpand", value);
            }
        }

        /// <summary>
        /// �ڵ�չ����Ŀͻ��˽ű�����
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("nodeAfterExpand")]
        [Bindable(true), Category("ClientEventsHandler"), Description("�ڵ�չ����Ŀͻ��˽ű�����")]
        public string OnNodeAfterExpand
        {
            get
            {
                return GetPropertyValue("nodeAfterExpand", string.Empty);
            }
            set
            {
                SetPropertyValue("nodeAfterExpand", value);
            }
        }

        /// <summary>
        /// �ڵ㸴ѡ��ѡ��ǰ�Ŀͻ��˽ű�����������ȡ����ѡ���ѡ��
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("nodeCheckBoxBeforeClick")]
        [Bindable(true), Category("ClientEventsHandler"), Description("�ڵ㸴ѡ��ѡ��ǰ�Ŀͻ��˽ű�����������ȡ����ѡ���ѡ��")]
        public string OnNodeCheckBoxBeforeClick
        {
            get
            {
                return GetPropertyValue("nodeCheckBoxBeforeClick", string.Empty);
            }
            set
            {
                SetPropertyValue("nodeCheckBoxBeforeClick", value);
            }
        }

        /// <summary>
        /// �ڵ㸴ѡ��ѡ�к�Ŀͻ��˽ű�����
        /// </summary>
        [DefaultValue("")]
        [ScriptControlEvent]
        [ClientPropertyName("nodeCheckBoxAfterClick")]
        [Bindable(true), Category("ClientEventsHandler"), Description("�ڵ㸴ѡ��ѡ�к�Ŀͻ��˽ű�����")]
        public string OnNodeCheckBoxAfterClick
        {
            get
            {
                return GetPropertyValue("nodeCheckBoxAfterClick", string.Empty);
            }
            set
            {
                SetPropertyValue("nodeCheckBoxAfterClick", value);
            }
        }

        /// <summary>
        /// �Ƿ�֧��PostBack(���ֽڵ��״̬)
        /// </summary>
        [DefaultValue(false)]
        [ScriptControlProperty]
        [ClientPropertyName("supportPostBack")]
        [Bindable(true), Description("�Ƿ�֧��PostBack(���ֽڵ��״̬)")]
        public bool SupportPostBack
        {
            get
            {
                return GetPropertyValue("supportPostBack", false);
            }
            set
            {
                SetPropertyValue("supportPostBack", value);
            }
        }

        /// <summary>
        /// �ص�ʱ�������ģ���ʹ�����ṩ
        /// </summary>
        [DefaultValue("")]
        [ScriptControlProperty]
        [ClientPropertyName("callBackContext")]
        [Bindable(true), Description("�ص�ʱ�������ģ���ʹ�����ṩ")]
        public string CallBackContext
        {
            get
            {
                return GetPropertyValue("callBackContext", string.Empty);
            }
            set
            {
                SetPropertyValue("callBackContext", value);
            }
        }

        /// <summary>
        /// �Ƿ���ʾ��
        /// </summary>
        [DefaultValue(true)]
        [Bindable(true), Description("�Ƿ���ʾ��")]
        public bool ShowLines
        {
            get
            {
                return GetPropertyValue("ShowLines", true);
            }
            set
            {
                SetPropertyValue("ShowLines", value);
            }
        }

        ///// <summary>
        ///// ��ʽ��Overflow���Ե�ֵ
        ///// </summary>
        //[DefaultValue(HtmlStyleOverflowDefine.Auto)]
        //[Bindable(true), Description("��ʽ��Overflow���Ե�ֵ")]
        //public HtmlStyleOverflowDefine Overflow
        //{
        //    get
        //    {
        //        return GetPropertyValue("Overflow", HtmlStyleOverflowDefine.Auto);
        //    }
        //    set
        //    {
        //        SetPropertyValue("Overflow", value);
        //    }
        //}

        #endregion

        #region ˽�з���
        private void PreloadAllImages()
        {
            //this.PreloadImage(this.DefaultWaitingForImage, this.DefaultWaitingForImage);
            this.PreloadImage(this.DefaultCollapseImage, this.DefaultCollapseImage);
            this.PreloadImage(this.DefaultExpandImage, this.DefaultExpandImage);

            this.PreloadImage(this.NodeOpenImg, this.NodeOpenImg);
            this.PreloadImage(this.NodeCloseImg, this.NodeCloseImg);

            this.PreloadTreeNodesImages(this.Nodes);
        }

        private void PreloadTreeNodesImages(DeluxeTreeNodeCollection treeNodes)
        {
            foreach (DeluxeTreeNode node in treeNodes)
            {
                this.PreloadImage(node.NodeOpenImg, node.NodeOpenImg);
                this.PreloadImage(node.NodeCloseImg, node.NodeCloseImg);

                this.PreloadTreeNodesImages(node.Nodes);
            }
        }

        private void PreloadImage(string key, string imgSrc)
        {
            if (string.IsNullOrEmpty(imgSrc) == false)
                Page.ClientScript.RegisterClientScriptBlock(
                    this.GetType(),
                    key,
                    string.Format("<img src=\"{0}\" style=\"display:none\"/>", HttpUtility.UrlPathEncode(imgSrc)));
        }

        private string CreateDesignTimeControlsHtml()
        {
            HtmlGenericControl container = new HtmlGenericControl("div");
            container.Style["border"] = "1px solid gray";

            foreach (DeluxeTreeNode node in Nodes)
                this.CreateDesignTimeTreeNode(node, container);
            if (Nodes.Count == 0)
            {
                return Resources.EnptyTreeDesignTimeHtml;
            }
            StringBuilder strB = new StringBuilder(512);
            using (StringWriter sw = new StringWriter(strB))
            {
                HtmlTextWriter writer = new HtmlTextWriter(sw);
                container.RenderControl(writer);
            }

            return strB.ToString();
        }

        private void CreateDesignTimeTreeNode(DeluxeTreeNode node, Control parent)
        {
            HtmlGenericControl container = new HtmlGenericControl("div");
            parent.Controls.Add(container);

            HtmlTable nodeTable = new HtmlTable();
            nodeTable.CellPadding = 0;
            nodeTable.CellSpacing = 0;

            container.Controls.Add(nodeTable);

            HtmlTableRow row = new HtmlTableRow();
            nodeTable.Controls.Add(row);

            HtmlTableCell cellExpandImg = new HtmlTableCell();
            cellExpandImg.Style["vertical-align"] = "middle";
            cellExpandImg.Style["text-align"] = "center";

            row.Controls.Add(cellExpandImg);

            HtmlImage imgExpand = new HtmlImage();

            imgExpand.Src = this.DefaultExpandImage;

            cellExpandImg.Controls.Add(imgExpand);

            HtmlTableCell cellImg = new HtmlTableCell();
            cellImg.Style["vertical-align"] = "middle";
            cellImg.Style["text-align"] = "center";

            row.Controls.Add(cellImg);

            HtmlImage img = new HtmlImage();

            HtmlTableCell cellText = new HtmlTableCell();
            cellText.Style["text-align"] = "left";

            if (string.IsNullOrEmpty(node.Html))
                cellText.InnerText = node.Text;
            else
                cellText.InnerHtml = node.Html;

            cellText.Attributes["class"] = "tree-item";// node.CssClass;

            row.Controls.Add(cellText);

            img.Src = this.GetDesignTimeNodeImage(node, false);

            if (node.Expanded)
            {
                if (node.HasChildren)
                {
                    imgExpand.Src = this.DefaultCollapseImage;

                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.Style["margin-left"] = this.NodeIndent.ToString() + "px";

                    foreach (DeluxeTreeNode subNode in node.Nodes)
                        this.CreateDesignTimeTreeNode(subNode, div);

                    container.Controls.Add(div);

                    img.Src = this.GetDesignTimeNodeImage(node, true);
                }
                else
                    imgExpand.Style["visibility"] = "visible";
            }

            if (img.Src != string.Empty)
            {
                HtmlGenericControl imgSpan = new HtmlGenericControl("span");
                imgSpan.Style["display"] = "inline-block";

                if (node.ImgWidth != Unit.Empty)
                    imgSpan.Style["width"] = node.ImgWidth.ToString();

                if (node.ImgHeight != Unit.Empty)
                    imgSpan.Style["height"] = node.ImgHeight.ToString();

                if (node.ImgMarginLeft != Unit.Empty)
                    img.Style["margin-left"] = node.ImgMarginLeft.ToString();

                if (node.ImgMarginTop != Unit.Empty)
                    img.Style["margin-top"] = node.ImgMarginTop.ToString();

                imgSpan.Controls.Add(img);
                cellImg.Controls.Add(imgSpan);
            }
        }

        private string GetDesignTimeNodeImage(DeluxeTreeNode node, bool expanded)
        {
            string result = string.Empty;

            if (expanded)
            {
                result = node.NodeOpenImg;

                if (result == string.Empty)
                    result = this.NodeOpenImg;
            }
            else
            {
                result = node.NodeCloseImg;

                if (result == string.Empty)
                    result = this.NodeCloseImg;
            }

            return result;
        }
        #endregion
    }
}
