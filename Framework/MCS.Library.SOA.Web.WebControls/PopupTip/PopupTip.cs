using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.PopupTip.PopupTip.js", "text/javascript")]
[assembly: WebResource("MCS.Web.WebControls.PopupTip.PopupTip.css", "text/css",PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.WebControls.PopupTip.Images.down_bubble_top.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.PopupTip.Images.down_bubble_middle.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.PopupTip.Images.down_bubble_bottom.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.PopupTip.Images.up_bubble_top.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.PopupTip.Images.up_bubble_middle.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.PopupTip.Images.up_bubble_bottom.gif", "image/gif")]

namespace MCS.Web.WebControls
{
    //显示的样式
    public enum PopupStyle
    {
        TopDialog=0,
        BottomDialog=1,
        Blue = 2,
        Custom = 3,
        Green=4
    }

    public enum PopupPostion
    {
        Up = 0,
        Down = 1     
    }

    [ClientCssResource("MCS.Web.WebControls.PopupTip.PopupTip.css")]
    [ClientScriptResource("MCS.Web.WebControls.PopupTip", "MCS.Web.WebControls.PopupTip.PopupTip.js")]
    [RequiredScript(typeof(ControlBaseScript), 1)]
    [RequiredScript(typeof(HBCommonScript), 2)]
    [Designer(typeof(PopupTipDesigner))]
    public class PopupTip : ScriptControlBase
    {
        public const string DefaultEmptyTipMessage = "没有找到相关的提示信息,TipName不存在或不匹配!";
        public PopupTip()
            : base(false, HtmlTextWriterTag.Div)
        {
        }
        
         /// <summary>
        /// 是弹出显示还是加到某一容器中显示
        /// </summary>
        [Category("Default")]
        [ScriptControlProperty]
        [ClientPropertyName("isPopup")]
        [Description("是弹出显示还是加到某一容器中显示")]
        public bool IsPopup
        {
            get { return GetPropertyValue("IsPopup", true); }
            set { SetPropertyValue("IsPopup", value); }
        }

        /// <summary>
        /// 指定关联的控件ID
        /// </summary>
        [Category("Default")]
        [ScriptControlProperty]
        [ClientPropertyName("reference")]
        [Description("指定关联的控件ID")]
        public string Reference
        {
            get { return GetPropertyValue("Reference", string.Empty); }
            set { SetPropertyValue("Reference", value); }
        }
        /// <summary>
        /// 需要显示HTML内容
        /// </summary>
        [Category("Default")]
        [ScriptControlProperty]
        [ClientPropertyName("htmlContent")]
        [Description("需要显示HTML内容")]
        public string HtmlContent
        {
            get { return GetPropertyValue("HtmlContent", string.Empty); }
            set { SetPropertyValue("HtmlContent", value); }
        }

        /// <summary>
        /// 指定嵌入到页面中的容器ID
        /// </summary>
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("fixedContainer")]
        [Description("指定嵌入到页面中的容器ID")]
        public string FixedContainer
        {
            get { return GetPropertyValue("FixedContainer", string.Empty); }
            set { SetPropertyValue("FixedContainer", value); }
        }

        /// <summary>
        /// 控件的宽度
        /// </summary>
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("tipWidth")]
        [Description("控件的宽度")]
        public int TipWidth
        {
            get { return GetPropertyValue("TipWidth", 147); }
            set { SetPropertyValue("TipWidth", value); }
        }

        /// <summary>
        /// 控件的高度
        /// </summary>
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("tipHeight")]
        [Description("控件的高度")]
        public int TipHeight
        {
            get { return GetPropertyValue("TipHeight", 106); }
            set { SetPropertyValue("TipHeight", value); }
        }

        /// <summary>
        /// 提示信息在数据中的代码
        /// </summary>
        [Category("Default")]
        [ScriptControlProperty]
        [ClientPropertyName("tipName")]
        [Description("提示信息在数据中的代码")]
        public string TipName
        {
            get { return GetPropertyValue("TipName", string.Empty); }
            set { SetPropertyValue("TipName", value); }
        } 

        /// <summary>
        /// 提示框的样式
        /// </summary>
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("popupStyle")]
        [Description("提示框的样式")]
        public PopupStyle PopupStyle
        {
            get { return GetPropertyValue("PopupStyle", PopupStyle.Blue); }
            set { SetPropertyValue("PopupStyle", value); }
        }

        /// <summary>
        /// 提示框的位置
        /// </summary>
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("popupPostion")]
        [Description("提示框的位置")]
        public PopupPostion PopupPostion
        {
            get { return GetPropertyValue("PopupPostion", PopupPostion.Up); }
            set { SetPropertyValue("PopupPostion", value); }
        }
        

        /// <summary>
        /// 是否加载时显示内容
        /// </summary>
        [Category("Default")]
        [ScriptControlProperty]
        [ClientPropertyName("preloadContent")]
        [Description("是否加载时显示内容")]
        public bool PreloadContent
        {
            get { return GetPropertyValue("PreloadContent", false); }
            set { SetPropertyValue("PreloadContent", value); }
        }

        /// <summary>
        /// 停留n毫秒后显示提示信息
        /// Popup模式默认500ms,非Popup模式为0ms.
        /// </summary>
        [Category("Default")]
        [ScriptControlProperty]
        [ClientPropertyName("timerPopWait")]
        [Description("是否加载时显示内容")]
        public int TimerPopWait
        {
            get { return GetPropertyValue("TimerPopWait", IsPopup ? 500 : 0); }
            set { SetPropertyValue("TimerPopWait", value); }
        }

         /// <summary>
        /// 显示n毫秒后关闭提示
        /// Popup模式默认4000ms,非Popup模式为-1（只有失去焦点后关闭）.
        /// </summary>
        [Category("Default")]
        [ScriptControlProperty]
        [ClientPropertyName("timerPopShow")]
        [Description("是否加载时显示内容")]
        public int TimerPopShow
        {
            get { return GetPropertyValue("TimerPopShow", IsPopup?4000:-1); }
            set { SetPropertyValue("TimerPopShow", value); }
        }

        /// <summary>
        ///是否自适应
        /// </summary>
        [Category("Appearance")]
        [ScriptControlProperty]
        [ClientPropertyName("isSelfAdaption")]
        [Description("是否自适应")]
        public bool IsSelfAdaption
        {
            get { return GetPropertyValue("IsSelfAdaption", true); }
            set { SetPropertyValue("IsSelfAdaption", value); }
        }    

        internal static List<string> TipList
        {
            get
            {
                var tipList = (List<string>)HttpContext.Current.Items["TipList"];

                if (tipList == null)
                {
                    tipList = new List<string>(10);
                    HttpContext.Current.Items["TipList"] = tipList;
                }

                return tipList;
            }
        }      

        protected override void OnLoad(EventArgs e)
        {
            if (TipList==null || TipList.Count==0)
            {
                AddTipIdToCache();
            }
        }           

        protected override void CreateChildControls()
        {
            if (TipList.Exists(t => String.Compare(t, TipName, StringComparison.OrdinalIgnoreCase) == 0))
            {
                SetHtmlContent();                 
            }            
        }        

        /// <summary>
        /// 设置HtmlContent的值
        /// </summary>
        private void SetHtmlContent()
        {
            if (string.IsNullOrEmpty(TipName))
            {
                if (!string.IsNullOrEmpty(HtmlContent)) return;
                HtmlContent = DefaultEmptyTipMessage;
            }
            else
            {
                var tip =
                    CurrentTips.Find(
                        o => o.CodeName == TipName && o.Culture.Equals(GetCultureName(), StringComparison.OrdinalIgnoreCase));

                HtmlContent = tip == null ? DefaultEmptyTipMessage : tip.Content;
            }
        }

        /// <summary>
        /// 得到所有TipControl
        /// </summary>
        private void AddTipIdToCache()
        {
            foreach (var tip in Page.Form.Controls.OfType<PopupTip>().Select(control => control as PopupTip))
            {
                var isEmpty = tip.ViewState["TipName"] == null;

                if (!isEmpty)
                    TipList.Add(tip.ViewState["TipName"].ToString());
            }
        }

        /// <summary>
        /// 区域
        /// </summary>
        /// <returns></returns>
        private  static  string GetCultureName()
        {
            var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

            return string.IsNullOrEmpty(cultureInfo.Name) ? "zh-cn" : cultureInfo.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        private static TipCollection CurrentTips
        {
            get
            {
                var result = (TipCollection) HttpContext.Current.Items["TipCollection"];
                if (null == result)
                {
                    result = TipAdapter.Instance.GetTips(TipList.ToArray(), GetCultureName());

                    HttpContext.Current.Items["TipCollection"] = result;
                }
                return result;
            }
        }
    }
}
