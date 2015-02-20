using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using MCS.Web.Responsive.Library;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Text;
using System.Web.UI.HtmlControls;
using MCS.Library.OGUPermission;
using MCS.Library.Globalization;
using MCS.Web.Library;
using MCS.Library.Core;

namespace MCSResponsiveOAPortal.WebControls
{
    /// <summary>
    /// 流程状态显示控件
    /// </summary>
    [ToolboxData("<{0}:WfStatusControl runat=server></{0}:WfStatusControl>")]
    public class WfStatusControl : Control
    {
        //private UserPresence innerUserPresence = new UserPresence();

        protected override void OnLoad(EventArgs e)
        {
            Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
            base.OnLoad(e);
        }

        /// <summary>
        /// 对应的流程的ResourceID，如果ResourceID和ProcessID都存在，则合并结果
        /// </summary>
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue("")]
        [Localizable(true)]
        public string ResourceID
        {
            get
            {
                return this.ViewState.GetViewStateValue("ResourceID", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("ResourceID", value);
            }
        }

        /// <summary>
        /// 对应的流程ID，如果ResourceID和ProcessID都存在，则合并结果
        /// </summary>
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue("")]
        [Localizable(true)]
        public string ProcessID
        {
            get
            {
                return this.ViewState.GetViewStateValue("ProcessID", string.Empty);
            }
            set
            {
                this.ViewState.SetViewStateValue("ProcessID", value);
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(WfStatusDisplayMode.ProcessStatus)]
        [Localizable(true)]
        public WfStatusDisplayMode DisplayMode
        {
            get
            {
                return this.ViewState.GetViewStateValue("DisplayMode", WfStatusDisplayMode.ProcessStatus);
            }
            set
            {
                this.ViewState.SetViewStateValue("DisplayMode", value);
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Localizable(true)]
        public bool EnableUserPresence
        {
            get
            {
                return this.ViewState.GetViewStateValue("EnableUserPresence", false);
            }
            set
            {
                this.ViewState.SetViewStateValue("EnableUserPresence", value);
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("状态跟踪")]
        [Localizable(true)]
        public string DefaultStatusText
        {
            get
            {
                return this.ViewState.GetViewStateValue("DefaultStatusText", "状态跟踪");
            }
            set
            {
                this.ViewState.SetViewStateValue("DefaultStatusText", value);
            }
        }

        protected override void CreateChildControls()
        {
            //Controls.Add(this.innerUserPresence);

            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),
                "WfStatusControl",
                ResourceHelper.LoadStringFromResource(Assembly.GetExecutingAssembly(),
                string.Format("{0}.Workflow.StatusControl.WfStatusControl.js", this.GetType().Namespace)), true);

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (DesignMode)
                writer.Write("Wf Status");
            else
            {
                UserIMAddressCollection extendInfo = null;

                if (EnableUserPresence && IsHtmlRender)
                    extendInfo = GetAllAssigneesExtendInfo(AppTraceProcesses);
                else
                    extendInfo = new UserIMAddressCollection();

                RenderStatus(AppTraceProcesses, extendInfo, writer);
            }
        }

        #region Private
        private List<string> ResourceIDList
        {
            get
            {
                List<string> list = (List<string>)HttpContext.Current.Items["ResourceIDList"];

                if (list == null)
                {
                    list = new List<string>(20);
                    HttpContext.Current.Items["ResourceIDList"] = list;
                }

                return list;
            }
        }

        private List<string> ProcessIDList
        {
            get
            {
                List<string> list = (List<string>)HttpContext.Current.Items["ProcessIDList"];

                if (list == null)
                {
                    list = new List<string>(20);
                    HttpContext.Current.Items["ProcessIDList"] = list;
                }

                return list;
            }
        }

        private WfProcessCurrentInfoCollection AppTraceProcesses
        {
            get
            {
                WfProcessCurrentInfoCollection atpc = (WfProcessCurrentInfoCollection)HttpContext.Current.Items["AppTraceProcesses"];

                if (atpc == null)
                {
                    atpc = new WfProcessCurrentInfoCollection();

                    WfProcessCurrentInfoCollection resourceAtpc = WfProcessCurrentInfoAdapter.Instance.Load(ResourceIDList.ToArray());
                    WfProcessCurrentInfoCollection processAtpc = WfProcessCurrentInfoAdapter.Instance.LoadByProcessID(ProcessIDList.ToArray());

                    resourceAtpc.ForEach(atp =>
                    {
                        if (atpc.Exists(p => p.InstanceID == atp.InstanceID) == false)
                            atpc.Add(atp);
                    });

                    processAtpc.ForEach(atp =>
                    {
                        if (atpc.Exists(p => p.InstanceID == atp.InstanceID) == false)
                            atpc.Add(atp);
                    });

                    HttpContext.Current.Items["AppTraceProcesses"] = atpc;
                }

                return atpc;
            }
        }

        //private static UserIMAddressCollection GetAllAssigneesExtendInfo(WfProcessCurrentInfoCollection atpc)
        //{
        //    UserIMAddressCollection result = (UserIMAddressCollection)HttpContext.Current.Items[atpc];

        //    if (result == null)
        //    {
        //        List<string> userIDs = new List<string>();

        //        foreach (WfProcessCurrentInfo atp in atpc)
        //        {
        //            atp.Assignees.ForEach(u => userIDs.Add(u.User.ID));
        //        }

        //        result = UserOUControlSettings.GetConfig().UserOUControlQuery.QueryUsersIMAddress(userIDs.ToArray());

        //        HttpContext.Current.Items[atpc] = result;
        //    }

        //    return result;
        //}

        private void Page_PreRenderComplete(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ResourceID) == false)
                ResourceIDList.Add(ResourceID);

            if (string.IsNullOrEmpty(ProcessID) == false)
                ProcessIDList.Add(ProcessID);
        }

        private void RenderStatus(WfProcessCurrentInfoCollection atpc, UserIMAddressCollection extendInfo, HtmlTextWriter writer)
        {
            WfProcessCurrentInfo atp = null;

            if (string.IsNullOrEmpty(this.ProcessID) == false)
            {
                atp = atpc.Find(a => string.Compare(a.InstanceID, this.ProcessID, true) == 0);
            }
            else
            {
                if (string.IsNullOrEmpty(this.ResourceID) == false)
                    atp = atpc.Find(a => string.Compare(a.ResourceID, this.ResourceID, true) == 0);
            }

            if (atp != null)
                writer.Write(GetControlHtml(CreateStatusDisplayControl(atp, extendInfo)));
        }

        private string GetStatusText(WfProcessCurrentInfo atp, UserIMAddressCollection extendInfo)
        {
            string result = DefaultStatusText;

            switch (DisplayMode)
            {
                case WfStatusDisplayMode.ProcessStatus:
                    result = HttpUtility.HtmlEncode(EnumItemDescriptionAttribute.GetDescription(atp.Status));
                    break;
                case WfStatusDisplayMode.CurrentUsers:
                    if (EnableUserPresence == false)
                        result = GetAllAssigneesStatusHtmlWithoutPresence(atp);
                    else
                        result = GetAllAssigneesStatusHtmlWithPresence(atp, IsHtmlRender, extendInfo, this.ClientID);

                    break;
            }

            return result;
        }

        private static string GetAllAssigneesStatusText(WfProcessCurrentInfo atp)
        {
            StringBuilder strB = new StringBuilder();

            foreach (var assignee in atp.Assignees)
            {
                strB.AppendFormat("{0}({1})",
                    assignee.User.DisplayName,
                    GetUserTopOUName(assignee.User));
            }

            return strB.ToString();
        }

        private static string GetAllAssigneesStatusHtmlWithoutPresence(WfProcessCurrentInfo atp)
        {
            HtmlGenericControl span = new HtmlGenericControl("span");

            foreach (var assignee in atp.Assignees)
            {
                span.InnerHtml = string.Format("{0}({1})",
                    HttpUtility.HtmlEncode(assignee.User.DisplayName),
                    HttpUtility.HtmlEncode(assignee.User.TopOU.DisplayName));
            }

            return GetControlHtml(span);
        }

        private static string GetAllAssigneesStatusHtmlWithPresence(WfProcessCurrentInfo atp, bool showStatusImage, UserIMAddressCollection extendInfo, string idPrefix)
        {
            HtmlGenericControl span = new HtmlGenericControl("span");

            int i = 0;
            foreach (var assignee in atp.Assignees)
            {
                //span.InnerHtml +=
                //    UserPresence.GetUsersPresenceHtml(assignee.User.ID, assignee.User.DisplayName.IsNullOrEmpty() ? assignee.User.Name : assignee.User.DisplayName,
                //                                      idPrefix + "_img_" + i, showStatusImage, false, true, StatusImageType.Ball, "", extendInfo);

                //string topOUName = GetUserTopOUName(assignee.User);

                //if (topOUName.IsNotEmpty())
                //    span.InnerHtml += string.Format("({0})", HttpUtility.HtmlEncode(topOUName));

                i++;
            }

            return GetControlHtml(span);
        }

        private static string GetControlHtml(Control ctrl)
        {
            System.IO.StringWriter w = new System.IO.StringWriter();
            ctrl.RenderControl(new HtmlTextWriter(w));
            return w.ToString();
        }

        private static string GetUserTopOUName(IUser user)
        {
            string result = string.Empty;

            try
            {
                if (user != null && user.TopOU != null)
                    result = user.TopOU.DisplayName;
            }
            catch (System.Exception)
            {
            }

            return result;
        }

        private bool IsHtmlRender
        {
            get
            {
                PageRenderMode mode = WebUtility.GetRequestPageRenderMode();

                return mode.IsHtmlRender;
            }
        }

        private Control CreateStatusDisplayControl(WfProcessCurrentInfo atp, UserIMAddressCollection extendInfo)
        {
            HtmlGenericControl Container = new HtmlGenericControl("span");

            HtmlGenericControl firstLine = new HtmlGenericControl("span");
            Container.Controls.Add(firstLine);

            HtmlAnchor statusAnchor = new HtmlAnchor();
            firstLine.Controls.Add(statusAnchor);

            statusAnchor.InnerHtml = GetStatusText(atp, extendInfo);
            statusAnchor.HRef = "#";

            string deptName = atp.Department.DisplayName;

            statusAnchor.Title = GetAllAssigneesStatusText(atp);
            statusAnchor.Attributes["onclick"] = "onWfStatusLinkClick()";

            HtmlGenericControl seperator = new HtmlGenericControl("span");
            firstLine.Controls.Add(seperator);

            seperator.InnerHtml = "&nbsp;";

            PageRenderMode mode = WebUtility.GetRequestPageRenderMode();

            if (mode.IsHtmlRender)
            {
                HtmlAnchor traceAnchor = new HtmlAnchor();
                firstLine.Controls.Add(traceAnchor);

                HtmlImage logo = new HtmlImage();
                traceAnchor.Controls.Add(logo);

                logo.Style["border"] = "none";

                logo.Alt = Translator.Translate("SOAWebControls", "流程跟踪...");

                logo.Style["vertical-align"] = "middle";
                logo.Src = GetImageByStatus(atp.Status);

                InitTraceControlEntry(traceAnchor, atp.ResourceID, atp.InstanceID);
            }

            HtmlGenericControl secondLine = new HtmlGenericControl("div");
            Container.Controls.Add(secondLine);

            secondLine.Controls.Add(CreateDetailControl(atp, extendInfo));

            secondLine.Style["display"] = "none";

            return Container;
        }

        private static string GetImageByStatus(WfProcessStatus status)
        {
            string result = ControlResources.ActivityLogoUrl;

            switch (status)
            {
                case WfProcessStatus.Aborted:
                    result = ControlResources.CancelledLogoUrl;
                    break;
                case WfProcessStatus.Paused:
                    result = ControlResources.DelayLogoUrl;
                    break;
                case WfProcessStatus.Completed:
                    result = ControlResources.CompletedActivityLogoUrl;
                    break;
            }

            return result;
        }

        private static void InitTraceControlEntry(IAttributeAccessor target, string resourceID, string processID)
        {
            target.SetAttribute("resourceID", resourceID);

            ResourceUriSettings settings = ResourceUriSettings.GetConfig();

            string path = "/MCSWebApp/OACommonPages/AppTrace/appTraceViewer.aspx";

            if (settings.Paths.ContainsKey("appTrace"))
                path = settings.Paths["appTrace"].Uri.ToString();

            target.SetAttribute("href",
                string.Format(path + "?resourceID={0}&processID={1}",
                resourceID, processID));

            target.SetAttribute("target", "WfTrace" + resourceID.Replace("-", string.Empty));

            target.SetAttribute("onclick", "onWfTraceButtonClick()");
        }

        private static WindowFeature GetWindowFeature()
        {
            WindowFeature feature = null;
            if (ResourceUriSettings.GetConfig().Features.ContainsKey("viewProcess"))
            {
                feature = ResourceUriSettings.GetConfig().Features["viewProcess"].Feature;
            }
            else
            {
                feature = new WindowFeature();

                feature.Width = 800;
                feature.Height = 650;
                feature.Resizable = true;
                feature.ShowAddressBar = false;
                feature.ShowMenuBar = false;
                feature.Center = true;
                feature.ShowStatusBar = false;
            }

            return feature;
        }

        private Control CreateDetailControl(WfProcessCurrentInfo atp, UserIMAddressCollection extendInfo)
        {
            HtmlGenericControl subContainer = new HtmlGenericControl("div");

            HtmlGenericControl divActivtity = new HtmlGenericControl("div");
            subContainer.Controls.Add(divActivtity);

            HtmlGenericControl divUserInfo = new HtmlGenericControl("div");
            subContainer.Controls.Add(divUserInfo);

            divUserInfo.InnerHtml = GetAllAssigneesStatusHtmlWithPresence(atp, IsHtmlRender, extendInfo, this.ClientID + "_DETAIL");

            return subContainer;
        }
        #endregion
    }

    /// <summary>
    /// 流程的状态显示
    /// </summary>
    public enum WfStatusDisplayMode
    {
        ProcessStatus = 0,
        CurrentUsers = 1,
        DefaultText = 2
    }
}