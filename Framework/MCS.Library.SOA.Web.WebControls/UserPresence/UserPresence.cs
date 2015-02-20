using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Globalization;
using MCS.Library.SOA.DataObjects;

[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.blank.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnhdr.gif", "image/gif")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnaway.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnawayoof.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnblocked.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnbusy.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnbusyoof.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imndnd.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imndndoof.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnhdr.gif", "image/gif")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnidle.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnidlebusy.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnidlebusyoof.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnidleoof.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnoff.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnoffoof.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnon.png", "image/png")]
//[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.imnonoof.png", "image/png")]

[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.IMNStatus.js", "text/javascript")]
[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.userPresenceInnerPage.htm", "text/html")]

namespace MCS.Web.WebControls
{
    /// <summary>
    /// UC状态图片类型
    /// </summary>
    public enum StatusImageType
    {
        /// <summary>
        /// 小球
        /// </summary>
        Ball,
        /// <summary>
        /// 短条
        /// </summary>
        ShortBar,
        /// <summary>
        /// 长条
        /// </summary>
        LongBar
    }

    /// <summary>
    /// 用户的在线状态显示控件
    /// </summary>
    [ToolboxData("<{0}:UserPresence runat=server></{0}:UserPresence>")]
    public class UserPresence : Control
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue("")]
        [Localizable(true)]
        [Description("用户ID")]
        public string UserID
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "UserID", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "UserID", value);
            }
        }

        /// <summary>
        /// 用户的显示名称
        /// </summary>
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue("")]
        [Localizable(true)]
        [Description("用户的显示名称")]
        public string UserDisplayName
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "UserDisplayName", string.Empty);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "UserDisplayName", value);
            }
        }

        [Bindable(true)]
        [Category("Data")]
        [DefaultValue(true)]
        [Localizable(true)]
        [Description("是否显示用户的显示名称")]
        public bool ShowUserDisplayName
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "ShowUserDisplayName", true);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "ShowUserDisplayName", value);
            }
        }

        [Bindable(true)]
        [Category("Layout")]
        [DefaultValue(StatusImageType.Ball)]
        [Localizable(true)]
        [Description("状态图片类型")]
        public StatusImageType StatusImage
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "StatusImage", StatusImageType.Ball);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "StatusImage", value);
            }
        }

        [Bindable(true)]
        [Category("Layout")]
        [DefaultValue(false)]
        [Localizable(true)]
        [Description("是否显示用户图标")]
        public bool ShowUserIcon
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "ShowUserIcon", false);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "ShowUserIcon", value);
            }
        }

        [Bindable(true)]
        [Category("Layout")]
        [DefaultValue("")]
        [Localizable(true)]
        [Description("是否显示用户图标")]
        public string UserIconUrl
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "UserIconUrl", "");
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "UserIconUrl", value);
            }
        }

        [Bindable(true)]
        [Category("Layout")]
        [DefaultValue(false)]
        [Localizable(true)]
        [Description("账号是否禁用")]
        public bool AccountDisabled
        {
            get
            {
                return WebControlUtility.GetViewStateValue(ViewState, "AccountDisabled", false);
            }
            set
            {
                WebControlUtility.SetViewStateValue(ViewState, "AccountDisabled", value);
            }
        }

        /// <summary>
        /// 得到用户状态的Html。会从当前的上下文中获取用户的SIP地址
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="uniqueID"></param>
        /// <param name="showStatusImage"></param>
        /// <param name="showUserIcon"></param>
        /// <param name="showUserName"></param>
        /// <param name="statusImageType"></param>
        /// <param name="userIconUrl"></param>
        /// <returns></returns>
        public static string GetUsersPresenceHtml(string userID, string userName, string uniqueID, bool showStatusImage,
            bool showUserIcon, bool showUserName, StatusImageType statusImageType, string userIconUrl)
        {
            UserIMAddress uie = UserExtendInfo.Find(u => u.UserID == userID);

            if (uie == null)
                UserExtendInfo.CopyFrom(UserOUControlSettings.GetConfig().UserOUControlQuery.QueryUsersIMAddress(userID));
            else
                UserExtendInfo.Add(uie);

            return GetUsersPresenceHtml(userID, userName, uniqueID, true, showUserIcon, showUserName, statusImageType, userIconUrl, UserExtendInfo);
        }

        /// <summary>
        /// 得到当前显示用户状态的Html
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="userName">用户的显示名称</param>
        /// <param name="uniqueID"></param>
        /// <param name="showStatusImage">是否显示状态图标</param>
        /// <param name="uiec">用户扩展信息的结构</param>
        /// <returns></returns>
        public static string GetUsersPresenceHtml(string userID, string userName, string uniqueID, bool showStatusImage, UserIMAddressCollection uiec)
        {
            return GetUsersPresenceHtml(userID, userName, uniqueID, showStatusImage, false, true, StatusImageType.Ball, "", uiec);
        }

        /// <summary>
        /// 得到当前显示用户状态的Html
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="userName">用户的显示名称</param>
        /// <param name="uniqueID"></param>
        /// <param name="showStatusImage">是否显示状态图标</param>
        /// <param name="showUserIcon">是否显示用户图标</param>
        /// <param name="showUserName">是否显示用户名称</param>
        /// <param name="statusImageType">状态图片类型</param>
        /// <param name="userIconUrl">用户图片路径</param>
        /// <param name="uiec">用户扩展信息的结构</param>
        /// <returns></returns>
        public static string GetUsersPresenceHtml(string userID, string userName, string uniqueID, bool showStatusImage,
            bool showUserIcon, bool showUserName, StatusImageType statusImageType, string userIconUrl, UserIMAddressCollection uiec, bool accountDisabled = false)
        {
            StringBuilder strB = new StringBuilder();

            if (string.IsNullOrEmpty(userID) == false)
            {
                if (showStatusImage)
                {
                    HtmlGenericControl imageDiv = new HtmlGenericControl("div");
                    //imageDiv.Style["position"] = "relative";

                    var userIconContainerCss = "";
                    var userIconCss = "";
                    if (statusImageType == StatusImageType.Ball)
                    {
                        imageDiv.Attributes["class"] = "uc-ball";
                    }
                    else if (statusImageType == StatusImageType.ShortBar)
                    {
                        imageDiv.Attributes["class"] = "uc-bar36";
                        userIconContainerCss = "uc-user-container-short";
                        userIconCss = "icon";
                    }
                    else if (statusImageType == StatusImageType.LongBar)
                    {
                        imageDiv.Attributes["class"] = "uc-bar52";
                        userIconContainerCss = "uc-user-container-long";
                        userIconCss = "icon";
                    }

                    HtmlImage image = new HtmlImage();
                    image.Src = ControlResources.UCStatusUrl;

                    if (accountDisabled)
                    {
                        image.Alt = Translator.Translate(Define.DefaultCulture, "用户账号禁用");
                        image.Attributes["class"] = "uc-blocked"; //禁用
                    }
                    else
                    {
                        image.Alt = Translator.Translate(Define.DefaultCulture, "无联机状态信息");
                        image.Attributes["class"] = "uc-hdr"; //默认
                        image.Attributes["ShowOfflinePawn"] = "true";
                        UserIMAddress extendInfo = uiec.Find(uie => uie.UserID == userID);

                        if (extendInfo != null)
                            image.Attributes["sip"] = NormalizeIMAddress(extendInfo.IMAddress);

                        image.ID = string.Format("{0},type=sip", uniqueID);
                        image.Attributes["name"] = "imnmark";
                    }

                    imageDiv.Controls.Add(image);

                    strB.Append(WebControlUtility.GetControlHtml(imageDiv));

                    if (showUserIcon)
                    {
                        HtmlGenericControl userIconDiv = new HtmlGenericControl("div");
                        //userIconDiv.Style["position"] = "relative";
                        userIconDiv.Attributes["class"] = userIconContainerCss;

                        var subDiv = new HtmlGenericControl("div");
                        subDiv.Attributes["class"] = userIconCss;

                        var img = new HtmlImage();
                        img.Src = userIconUrl;
                        img.Border = 0;

                        subDiv.Controls.Add(img);
                        userIconDiv.Controls.Add(subDiv);
                        if (showUserName)
                        {
                            HtmlGenericControl nameSpan = new HtmlGenericControl("span");
                            nameSpan.InnerText = userName;
                            userIconDiv.Controls.Add(nameSpan);
                        }
                        //nameSpan.Attributes["class"] = "imnStatusText";

                        strB.Append(WebControlUtility.GetControlHtml(userIconDiv));
                    }
                }

                if (statusImageType == StatusImageType.Ball && showUserName)
                {
                    HtmlGenericControl span = new HtmlGenericControl("span");
                    //span.Style["padding-left"] = "16px";
                    span.InnerText = userName;
                    span.Attributes["class"] = "imnStatusText";

                    strB.Append(WebControlUtility.GetControlHtml(span));
                }
            }

            return strB.ToString();
        }

        protected override void OnLoad(EventArgs e)
        {
            Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
            base.OnLoad(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
                writer.Write("User Presence");
            else
            {
                if (!Page.Items.Contains("UserPresenceIframe"))
                {
                    string strIframe = string.Format("<iframe style='display:none' id='{0}' name='{0}' src='{1}'></iframe>",
                       "userPresenceInnerPage",
                       Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.WebControls.UserPresence.Resources.userPresenceInnerPage.htm"));

                    Page.Items.Add("UserPresenceIframe", true);
                    writer.Write(strIframe);
                }

                RenderUsersPresence(UserID, ShowUserDisplayName ? UserDisplayName : string.Empty, this.UniqueID, IsHtmlRender, this.ShowUserIcon, this.ShowUserDisplayName, this.StatusImage, this.UserIconUrl, UserExtendInfo, writer, this.AccountDisabled);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            RegisterStatusCSS();
            RegisterUcStatusImageUrl();
            //RegisterStatusImageDict();

            Page.ClientScript.RegisterClientScriptResource(this.GetType(),
                "MCS.Web.WebControls.UserPresence.Resources.IMNStatus.js");

            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "userPresenceInnerPage",
            //    string.Format("<iframe style='display:none' id='{0}' name='{0}' src='{1}'></iframe>",
            //    "userPresenceInnerPage",
            //    Page.ClientScript.GetWebResourceUrl(this.GetType(), "MCS.Web.WebControls.UserPresence.Resources.userPresenceInnerPage.htm")));

            ScriptManager sm = ScriptManager.GetCurrent(this.Page);

            if (sm != null && sm.IsInAsyncPostBack)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Startup", "ProcessImn();", true);
            else
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Startup",
                    "if(window.addEventListener){window.addEventListener(\"load\",ProcessImn,false);}else if(window.attachEvent){ window.attachEvent(\"onload\", ProcessImn);}", true);
        }

        private bool IsHtmlRender
        {
            get
            {
                PageRenderMode mode = WebUtility.GetRequestPageRenderMode();

                return mode.IsHtmlRender;
            }
        }

        public static void RenderUsersPresence(string userID, string userName, string uniqueID, bool showStatusImage, bool showUserIcon, bool showUserName, StatusImageType statusImageType, string userIconUrl, UserIMAddressCollection uiec, HtmlTextWriter writer, bool accountDisabled = false)
        {
            writer.Write(GetUsersPresenceHtml(userID, userName, uniqueID, showStatusImage, showUserIcon, showUserName, statusImageType, userIconUrl, uiec, accountDisabled));
        }

        internal static string NormalizeIMAddress(string address)
        {
            const string pattern = "sip:";

            string result = address;

            if (result.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) == 0)
                result = result.Substring(pattern.Length);

            return result;
        }

        private void RegisterStatusCSS()
        {
            //string css = ResourceHelper.LoadStringFromResource(
            //    Assembly.GetExecutingAssembly(),
            //    "MCS.Web.WebControls.UserPresence.Resources.status.css");

            string css = ResourceHelper.LoadStringFromResource(
                Assembly.GetExecutingAssembly(),
                "MCS.Web.WebControls.UserPresence.Resources.uc.css");

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "StatusCSS",
                string.Format("<style type='text/css'>{0}</style>", css));
        }

        private void RegisterUcStatusImageUrl()
        {
            var url = ControlResources.UCStatusUrl;
            string str = string.Format("var UserPresenceStatusUrl = '{0}'", url);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "UCStatusUrl", str, true);
        }

        private void RegisterOneStatusImage(StringWriter sw, string imageID)
        {
            sw.WriteLine("StatusImageDict[\"{0}\"] = \"{1}\";",
                imageID,
                GetStatusImageUrl(imageID));
        }

        internal static string GetDefaultStatusImageUrl()
        {
            return GetStatusImageUrl("imnhdr.gif");
        }

        private static string GetStatusImageUrl(string imageID)
        {
            return WebUtility.GetCurrentPage().ClientScript.GetWebResourceUrl(typeof(UserPresence),
                    "MCS.Web.WebControls.UserPresence.Resources." + imageID);
        }

        private void Page_PreRenderComplete(object sender, EventArgs e)
        {
            EnsureInUserList();
        }

        public void EnsureInUserList()
        {
            if (string.IsNullOrEmpty(UserID) == false)
            {
                if (UserList.Exists(u => string.Compare(u.ID, this.UserID, true) == 0) == false)
                {
                    OguUser user = (OguUser)OguUser.CreateWrapperObject(UserID, SchemaType.Users);

                    if (string.IsNullOrEmpty(UserDisplayName) == false)
                    {
                        user.Name = UserDisplayName;
                        user.DisplayName = UserDisplayName;
                    }

                    UserList.Add(user);
                }
            }
        }

        internal static List<IUser> UserList
        {
            get
            {
                List<IUser> list = (List<IUser>)HttpContext.Current.Items["UserList"];

                if (list == null)
                {
                    list = new List<IUser>(40);
                    HttpContext.Current.Items["UserList"] = list;
                }

                return list;
            }
        }

        private static UserIMAddressCollection UserExtendInfo
        {
            get
            {
                UserIMAddressCollection uiec = (UserIMAddressCollection)HttpContext.Current.Items["UserIMAddressCollection"];

                if (uiec == null)
                {
                    List<string> userIDs = new List<string>(40);

                    UserList.ForEach(u => userIDs.Add(u.ID));

                    uiec = UserOUControlSettings.GetConfig().UserOUControlQuery.QueryUsersIMAddress(userIDs.ToArray());
                    HttpContext.Current.Items["UserIMAddressCollection"] = uiec;
                }

                return uiec;
            }
        }
    }
}
