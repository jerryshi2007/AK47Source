using CIIC.HSR.TSP.WebComponents;
using CIIC.HSR.TSP.WebComponents.Widgets.Button;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CIIC.HSR.TSP.WF.UI.Control.Interfaces;
using MCS.Library.Globalization;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract
{
    public abstract class WFButtonControlBase : WFControlBase
    {
        public WFButtonControlBase(ViewContext vc, ViewDataDictionary vdd)
            : base(vc, vdd)
        {
        }

        protected override void InitWidgetAttributes(WidgetBase widget)
        {
            Button button = (Button)widget;

            button.ClientClick = this.ClientButtonClickScript;
            button.Enabled = this.GetEnabled();
            button.Visible = this.GetEnabled();
            InitButtonAttributes(button);
        }

        protected virtual void InitButtonAttributes(Button button)
        {
            WFControlDescriptionAttribute attr = (WFControlDescriptionAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(WFControlDescriptionAttribute));

            if (attr != null)
            {
                if (string.IsNullOrEmpty(button.Text))
                    button.Text = attr.DefaultText;

                if (string.IsNullOrEmpty(button.DialogText))
                    button.DialogText = attr.DialogText;

                button.Text = Translator.Translate(CultureDefine.DefaultCulture, button.Text);
                button.DialogText = Translator.Translate(CultureDefine.DefaultCulture, button.DialogText);
            }
        }
    }
}
