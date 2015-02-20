using System;
using System.Xml;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using MCS.Library.Core;
using MCS.Web.Library;
using System.Reflection;

#region assembly
[assembly: WebResource("MCS.Web.WebControls.SimpleTabStrip.SimpleTabStrip.css", "text/css")]
[assembly: WebResource("MCS.Web.WebControls.SimpleTabStrip.simpleTabStrip.htc", "text/x-component")]
#endregion assembly

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 简单的页签控件
    /// </summary>
    [ParseChildren(true)]
    [ToolboxData("<{0}:SimpleTabStrip runat=server></{0}:SimpleTabStrip>")]
    public class SimpleTabStrip : Control, INamingContainer
    {
        private HtmlGenericControl clientTabStrip = new HtmlGenericControl("HB:simpleStrip");
        private TabStripItemCollection tabStrips = new TabStripItemCollection();
        private CustomValidator submitValidator = new CustomValidator();
        private HtmlInputHidden hiddenData = new HtmlInputHidden();

        /// <summary>
        /// 建立子控件
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Controls.Add(this.submitValidator);

            LiteralControl nameSpaceScript = new LiteralControl();

            nameSpaceScript.Text = string.Format("<script type=\"text/javascript\">addNamespace(\"HB\");</script>");

            Controls.Add(nameSpaceScript);

            clientTabStrip.Style["behavior"] = string.Format("url({0})",
                Page.ClientScript.GetWebResourceUrl(typeof(SimpleTabStrip),
                "MCS.Web.WebControls.SimpleTabStrip.simpleTabStrip.htc"));

            this.clientTabStrip.ID = "tabStripHtc";
            Controls.Add(clientTabStrip);

            this.hiddenData.ID = "hiddenData";
            Controls.Add(this.hiddenData);
        }

        /// <summary>
        /// 点击页签的客户端事件
        /// </summary>
        public string OnClientTableStripClick
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "OnClientTableStripClick", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "OnClientTableStripClick", value);
            }
        }

        /// <summary>
        /// 页签集合
        /// </summary>
        [MergableProperty(false)]
        [DefaultValue((string)null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [PersistenceMode(PersistenceMode.InnerProperty), Description("绑定数据的映射")]
        public TabStripItemCollection TabStrips
        {
            get
            {
                return this.tabStrips;
            }
        }

        /// <summary>
        /// 被选中的页签的Key
        /// </summary>
        [DefaultValue("")]
        public string SelectedKey
        {
            get
            {
                return this.hiddenData.Value;
            }
            set
            {
                this.hiddenData.Value = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            HBCommonScript.RegisterAddNamespaceScript();

            string cssUrl = Page.ClientScript.GetWebResourceUrl(typeof(SimpleTabStrip),
                "MCS.Web.WebControls.SimpleTabStrip.SimpleTabStrip.css");

            Page.ClientScript.RegisterClientScriptBlock(typeof(SimpleTabStrip), "SimpleTabStripCss",
                string.Format("<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />", cssUrl));

            Page.ClientScript.RegisterClientScriptBlock(typeof(SimpleTabStrip), "SimpleTabStripScript",
                ResourceHelper.LoadStringFromResource(
                    Assembly.GetExecutingAssembly(),
                    "MCS.Web.WebControls.SimpleTabStrip.simpleTabStrip.js"),
                true);

            if (string.IsNullOrEmpty(this.SelectedKey) == false)
            {
                string attachEvent = string.Format("window.attachEvent(\"onload\", new Function('if (document.all(\"{0}\").strips.getItemByKey(\"{1}\")) document.all(\"{0}\").strips.getItemByKey(\"{1}\").setSelected()'));", this.clientTabStrip.ClientID, this.SelectedKey);
                string addEventListener = string.Format("window.addEventListener(\"onload\", new Function('if (document.all(\"{0}\").strips.getItemByKey(\"{1}\")) document.all(\"{0}\").strips.getItemByKey(\"{1}\").setSelected()'));", this.clientTabStrip.ClientID, this.SelectedKey);

                Page.ClientScript.RegisterStartupScript(typeof(SimpleTabStrip), this.ClientID,
                    string.Format("if (window.attachEvent) {0} else {1}", attachEvent, addEventListener),
                    true);
            }
            InitValidatorAttributes();

            if (string.IsNullOrEmpty(OnClientTableStripClick) == false)
                this.clientTabStrip.Attributes["onStripClick"] = OnClientTableStripClick;

            RenderHtcInnerXml();

            Page.ClientScript.RegisterStartupScript(typeof(SimpleTabStrip), this.ClientID + "_BaseObj",
                "var " + this.ClientID + "= { get_control: function() { return document.getElementById(\"" + this.clientTabStrip.ClientID + "\") } };",
                true);

            base.OnPreRender(e);
        }

        private void InitValidatorAttributes()
        {
            this.submitValidator.Attributes["tabStripControlID"] = this.clientTabStrip.ClientID;
            this.submitValidator.Attributes["hiddenDataControlID"] = this.hiddenData.ClientID;
            this.submitValidator.ClientValidationFunction = "tabStripClientValidate";
        }

        private void RenderHtcInnerXml()
        {
            XmlDocument xmlDoc = XmlHelper.CreateDomDocument("<TabStrips/>");

            XmlNode root = xmlDoc.DocumentElement;

            foreach (TabStripItem item in this.tabStrips)
            {
                XmlNode itemNode = XmlHelper.AppendNode(root, "TabStrip");

                XmlHelper.AppendNotNullAttr(itemNode, "key", item.Key);
                XmlHelper.AppendNotNullAttr(itemNode, "image", item.Logo);
                XmlHelper.AppendNotNullAttr(itemNode, "text", item.Text);
                XmlHelper.AppendNotNullAttr(itemNode, "tag", item.Tag);
                XmlHelper.AppendNotNullAttr(itemNode, "width", item.Width.ToString());

                if (string.IsNullOrEmpty(item.ControlID) == false)
                {
                    Control panel = WebControlUtility.FindControlByID(this.Page, item.ControlID, true);

                    if (panel != null)
                        XmlHelper.AppendNotNullAttr(itemNode, "elementID", panel.ClientID);
                }
            }

            this.clientTabStrip.InnerHtml = root.OuterXml;
        }
    }
}
