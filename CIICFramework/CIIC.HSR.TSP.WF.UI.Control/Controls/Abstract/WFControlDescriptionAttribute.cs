using MCS.Library.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.UI.Control.Controls.Abstract
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WFControlDescriptionAttribute : Attribute
    {
        public WFControlDescriptionAttribute()
        {
        }

        public WFControlDescriptionAttribute(string defaultActionUrl)
        {
            this.DefaultActionUrl = defaultActionUrl;
        }

        public WFControlDescriptionAttribute(string defaultActionUrl, string clientButtonClickScript)
        {
            this.DefaultActionUrl = defaultActionUrl;
            this.ClientButtonClickScript = clientButtonClickScript;
        }

        public WFControlDescriptionAttribute(string defaultActionUrl, string clientButtonClickScript, string defaultText)
        {
            this.DefaultActionUrl = defaultActionUrl;
            this.ClientButtonClickScript = clientButtonClickScript;
            this.DefaultText = Translator.Translate(CultureDefine.DefaultCulture, defaultText);
        }

        public WFControlDescriptionAttribute(string defaultActionUrl, string clientButtonClickScript, string defaultText, string dialogText)
        {
            this.DefaultActionUrl = defaultActionUrl;
            this.ClientButtonClickScript = clientButtonClickScript;
            this.DefaultText = defaultText;
            this.DialogText = dialogText;
        }


        public string DefaultActionUrl
        {
            get;
            set;
        }

        public string DefaultText
        {
            get;
            set;
        }

        public string DialogText
        {
            get;
            set;
        }

        public string ClientButtonClickScript
        {
            get;
            set;
        }
    }
}
