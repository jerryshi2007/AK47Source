using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Script;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Drawing.Design;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.Responsive.WebControls.DeluxeMenu.DeluxeMenuStrip.js", "application/x-javascript")]

namespace MCS.Web.Responsive.WebControls
{
    [ToolboxData("<{0}:DeluxeMenuStrip runat=server></{0}:DeluxeMenuStrip>")]
    //[DefaultEvent("SelectionChanged")]
    //[DefaultProperty("Value")]
    //[ControlValueProperty("Value", typeof(DateTime), "1/1/0001")]
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [ClientScriptResource("MCS.Web.WebControls.DeluxeMenuStrip", "MCS.Web.Responsive.WebControls.DeluxeMenu.DeluxeMenuStrip.js")]
    [Designer(typeof(Design.DeluxeMenuStripDesigner))]
    public class DeluxeMenuStrip : ScriptControlBase
    {
        DeluxeMenuStripItemCollection items = null;

        public DeluxeMenuStrip()
            : base(HtmlTextWriterTag.Ul)
        {
        }

        [Category("Appereance")]
        [Description("控件的对齐方式")]
        [DefaultValue(TextAlign.Left)]
        public TextAlign Align
        {
            get
            {
                return ViewState.GetViewStateValue<TextAlign>("Align", TextAlign.Left);
            }
            set
            {
                ViewState.SetViewStateValue<TextAlign>("Align", value);
            }
        }

        /// <summary>
        /// 设置是否采用静态布局
        /// </summary>
        [Category("Appereance")]
        [Description("如果设置为静态布局方式，则控件会插入布局流中")]
        [DefaultValue(false)]
        public bool Static
        {
            get
            {
                return ViewState.GetViewStateValue<bool>("Static", false);
            }
            set
            {
                ViewState.SetViewStateValue<bool>("Static", value);
            }
        }


        public override bool SupportsDisabledAttribute
        {
            get
            {
                return true;
            }
        }

        protected override ControlCollection CreateControlCollection()
        {
            return new EmptyControlCollection(this);
        }

        [MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), Description("菜单项的集合"), DefaultValue((string)null)]
        //[Editor("System.Web.UI.Design.WebControls.TreeNodeCollectionEditor,System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public DeluxeMenuStripItemCollection Items
        {
            get
            {
                if (items == null)
                    items = new DeluxeMenuStripItemCollection(this);

                return items;
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            writer.AddAttribute("role", "menu");

            this.ControlStyle.CssClass += " dropdown-menu";

            if (this.Align == TextAlign.Right)
                this.ControlStyle.CssClass += " pull-right";

            if (this.Static)
            {
                this.Style[HtmlTextWriterStyle.Position] = "static";
            }

            base.AddAttributesToRender(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
            DeluxeMenuStripItem item;

            for (int i = 0; i < this.Items.Count; i++)
            {
                string className = "";
                item = this.Items[i];
                if (item.Text == "-")
                    className = "divider";

                writer.AddAttribute("role", "presentation");
                if (item.Enabled == false)
                    className += " disabled";

                writer.AddAttribute(HtmlTextWriterAttribute.Class, className);
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                item.Render(writer);
                writer.RenderEndTag();
            }
        }

        protected override void LoadViewState(object state)
        {
            if (state != null)
            {
                object[] objArray = (object[])state;
                if (objArray[0] != null)
                {
                    base.LoadViewState(objArray[0]);
                }
                if (objArray[1] != null)
                {
                    ((IStateManager)this.Items).LoadViewState(objArray[1]);
                }

            }
        }

        protected override object SaveViewState()
        {
            object[] objArray = new object[9];
            objArray[0] = base.SaveViewState();
            bool flag = objArray[0] != null; //如果有数据则保存视图状态，没有任何变化的情况下不保存
            objArray[1] = ((IStateManager)this.Items).SaveViewState();
            if (flag | (objArray[1] != null))
            {
                return objArray;
            }
            return null;

        }

        protected override void TrackViewState()
        {
            base.TrackViewState();

            ((IStateManager)this.Items).TrackViewState();
        }

        [Description("当客户端点击一项时发生")]
        [Category("Behavior")]
        [DefaultValue("")]
        [ScriptControlEvent(true)]
        [ClientPropertyName("onClientItemClick")]
        public string OnClientItemClick
        {
            get
            {
                return ViewState.GetViewStateValue<string>("OnClientItemClick", string.Empty);
            }

            set
            {
                ViewState.SetViewStateValue<string>("OnClientItemClick", value ?? string.Empty);
            }
        }
    }
}
