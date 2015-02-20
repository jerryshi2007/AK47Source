using System;
using System.Xml;
using System.Web.UI;
using System.Diagnostics;
using System.Web.UI.HtmlControls;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.Principal;
using MCS.OA.Portal.Services;

namespace MCS.OA.Portal.frames
{
    public partial class left : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
                BindPage();
        }

        private void BindPage()
        {
            Response.Cache.SetNoStore();
            RenderPersonalInfo();
            RenderMenu();

            string serverTag;

            if (UserTaskChangingCache.Instance.TryGetValue(DeluxeIdentity.CurrentUser.ID, out serverTag) == false)
            {
                UdpNotifierCacheDependency dependency = new UdpNotifierCacheDependency();

                serverTag = Guid.NewGuid().ToString();
                UserTaskChangingCache.Instance.Add(DeluxeIdentity.CurrentUser.ID, serverTag, dependency);
            }

            userTaskCount.Value = PortalServices.GetUserTaskCountString(serverTag);
        }

        /// <summary>
        /// 个人信息
        /// </summary>
        private void RenderPersonalInfo()
        {
            userName.Text = Server.HtmlEncode(DeluxeIdentity.CurrentUser.DisplayName + "，您好！");
        }

        /// <summary>
        /// 菜单
        /// </summary>
        private void RenderMenu()
        {
            XmlDocument xmlDoc = WebXmlDocumentCache.GetXmlDocument("../xml/functions.xml");

            Stopwatch sw = new Stopwatch();

            sw.Start();

            try
            {
                if (!xmlDoc.Equals(null))
                {
                    XmlNodeList captions = xmlDoc.DocumentElement.SelectNodes("Item");

                    foreach (XmlElement elemCaption in captions)
                    {
                        string strVisible = elemCaption.GetAttribute("visible").ToLower();

                        if ((strVisible != "false") && CheckUserPermissions(elemCaption))
                        {
                            HtmlGenericControl li = new HtmlGenericControl("li");

                            menuList.Controls.Add(li);

                            HtmlAnchor link = new HtmlAnchor();

                            link.InnerText = elemCaption.GetAttribute("name");
                            link.HRef = elemCaption.GetAttribute("url");
                            link.Target = elemCaption.GetAttribute("target");

                            li.Controls.Add(link);
                        }
                    }
                }
            }
            finally
            {
                sw.Stop();
                Debug.WriteLine(string.Format("RenderMenu: {0:#,##0}", sw.ElapsedMilliseconds));
            }
        }

        /// <summary>
        /// 判断当前用户权限
        /// </summary>
        /// <param name="elemCaption"></param>
        /// <returns></returns>
        private static bool CheckUserPermissions(XmlElement elemCaption)
        {
            bool result = true;

            string appAndRoles = elemCaption.GetAttribute("appAndRoles").ToString();

            if (appAndRoles.Length > 0)
                result = DeluxePrincipal.Current.IsInRole(appAndRoles);

            return result;
        }


        #region 取消子控件的加入
        //private void RenderCaption(XmlElement elemCaption, string menuID, Control parent)
        //{
        //    HtmlTable table = new HtmlTable();

        //    table.Attributes["class"] = "menuItem";
        //    table.CellPadding = 0;
        //    table.CellSpacing = 0;
        //    table.Style["width"] = "100%";

        //    HtmlTableRow row = new HtmlTableRow();
        //    table.Controls.Add(row);

        //    HtmlTableCell cell = new HtmlTableCell();

        //    cell.Attributes["class"] = "bgMouseOut";
        //    cell.VAlign = "middle";
        //    row.Controls.Add(cell);

        //    CreateOneCaptionTable(elemCaption, menuID, cell);

        //    parent.Controls.Add(table);
        //}

        //private void CreateOneCaptionTable(XmlElement elemCaption, string menuID, Control parent)
        //{
        //    HtmlTable table = new HtmlTable();

        //    table.CellSpacing = 0;
        //    table.CellPadding = 0;
        //    table.Style["width"] = "100%";
        //    table.Style["height"] = "100%";
        //    parent.Controls.Add(table);

        //    HtmlTableRow row = new HtmlTableRow();
        //    table.Controls.Add(row);

        //    /* 周杨20090621注释
        //    HtmlTableCell cellImg = new HtmlTableCell();

        //    cellImg.Width = "32px";
        //    cellImg.Align = "center";
        //    //项目经理指示去除该图片
        //    cellImg.Style["visibility"] = "hidden";

        //    if (elemCaption.GetAttribute("img") != string.Empty)
        //        CreateImageSpan(elemCaption.GetAttribute("img"), 18, 18, cellImg);

        //    row.Controls.Add(cellImg);
        //    */
        //    HtmlTableCell cellText = new HtmlTableCell(); 
        //    cellText.Style["text-align"] = "left";
        //    cellText.Style["width"] = "100%";

        //    CreateCaptionLink(elemCaption, menuID, cellText);
        //    row.Controls.Add(cellText);
        //}

        //private void CreateCaptionLink(XmlElement elemCaption, string menuID, Control parent)
        //{
        //    //<img src="images/icon_bulkmail_16px.gif" />

        //    HtmlImage img = new HtmlImage();
        //    img.Src = "../images/menuList.gif";
        //    parent.Controls.Add(img);
        //    img.Style["margin-left"] = "18px";

        //    HtmlAnchor a = new HtmlAnchor();

        //    //a.Attributes["onmouseover"] = "onCaptionMouseOver();";
        //    //a.Attributes["onmouseout"] = "onCaptionMouseOut();";
        //    //a.Attributes["class"] = "bgMouseOut";
        //    a.HRef = "#";
        //    a.Attributes["key"] = elemCaption.GetAttribute("id");
        //    a.Attributes["menuID"] = menuID;
        //    a.Attributes["onclick"] = "onMenuLinkClick();";

        //    parent.Controls.Add(a);

        //    HtmlGenericControl spanText = new HtmlGenericControl("span");
        //    spanText.InnerText = elemCaption.GetAttribute("name");
        //    //	spanText.Style["width"] = "80%";
        //    //spanText.Style["font-weight"] = "bold";
        //    //spanText.Attributes["class"] = "navigatefont";

        //    a.Controls.Add(spanText);

        //    //周杨注释  	CreateImageSpan("../images/jiantou01.gif", 8, 8, a);
        //    //HtmlGenericControl span = new HtmlGenericControl("span");
        //    //span.InnerText = ">>";
        //    //span.Style["font-size"] = "10pt";
        //    //a.Controls.Add(span);

        //}

        //private void CreateImageSpan(string strSrc, int nWidth, int nHeight, Control parent)
        //{
        //    HtmlGenericControl span = new HtmlGenericControl("span");

        //    span.Style["width"] = nWidth.ToString();
        //    span.Style["height"] = nHeight.ToString();
        //    span.Style["background-image"] = "url(" + strSrc + ")";
        //    span.Style["background-position"] = "center";
        //    span.Style["background-repeat"] = "no-repeat";
        //    span.InnerText = " ";
        //    parent.Controls.Add(span);
        //}

        /////// <summary>
        /////// 递归添加子菜单
        /////// </summary>
        /////// <param name="node"></param>
        /////// <param name="itemParent"></param>
        ////private void RenderSubMenu(XmlElement node, MCS.Web.WebControls.MenuItem itemParent)
        ////{
        ////    XmlNodeList captions = node.SelectNodes("Item");

        ////    foreach (XmlElement elemCaption in captions)
        ////    {
        ////        string strType = elemCaption.GetAttribute("type").ToLower();
        ////        MCS.Web.WebControls.MenuItem item = new MCS.Web.WebControls.MenuItem();
        ////        string strVisible = elemCaption.GetAttribute("visible").ToLower();

        ////        if (strType != "splitter")
        ////        {
        ////            if (strVisible == "false" || !CheckUserPermissions(elemCaption)) continue;
        ////            if (elemCaption.Attributes.GetNamedItem("name") != null)
        ////                item.Text = elemCaption.Attributes.GetNamedItem("name").Value;
        ////            if (elemCaption.Attributes.GetNamedItem("img") != null)
        ////                item.ImageUrl = elemCaption.Attributes.GetNamedItem("img").Value;
        ////            if (elemCaption.Attributes.GetNamedItem("url") != null)
        ////                item.NavigateUrl = elemCaption.Attributes.GetNamedItem("url").Value;
        ////            if (elemCaption.Attributes.GetNamedItem("target") != null)
        ////                item.Target = elemCaption.Attributes.GetNamedItem("target").Value;

        ////            itemParent.ChildItems.Add(item);
        ////        }
        ////        else
        ////        {
        ////            item.IsSeparator = true;
        ////            if (strVisible != "false")
        ////                itemParent.ChildItems.Add(item);
        ////        }
        ////        if (elemCaption.ChildNodes.Count > 0)
        ////            RenderSubMenu(elemCaption, item);
        ////    }
        ////}

        ///// <summary>
        ///// 添加子菜单
        ///// </summary>
        ///// <param name="node"></param>
        ///// <param name="deMenu"></param>
        //private void RenderSubMenu(XmlElement node, MCS.Web.WebControls.MenuItem menuData)
        //{
        //    XmlNodeList captions = node.SelectNodes("Item");

        //    if (captions.Count == 0)
        //    {
        //        MCS.Web.WebControls.MenuItem currMenuData = new MCS.Web.WebControls.MenuItem();
        //        string strVisible = node.GetAttribute("visible").ToLower();
        //        string strType = node.GetAttribute("type").ToLower();
        //        if (node.Attributes.GetNamedItem("name") != null)
        //            currMenuData.Text = node.Attributes.GetNamedItem("name").Value;
        //        /*
        //        if (node.Attributes.GetNamedItem("img") != null)
        //            currMenuData.ImageUrl = node.Attributes.GetNamedItem("img").Value;
        //         */
        //        if (node.Attributes.GetNamedItem("target") != null)
        //            currMenuData.Target = node.Attributes.GetNamedItem("target").Value;
        //        string featureID = node.Attributes.GetNamedItem("featureID") ==
        //            null ? string.Empty : node.Attributes.GetNamedItem("featureID").Value;
        //        string feature = string.IsNullOrEmpty(featureID) ? string.Empty :
        //            !ResourceUriSettings.GetConfig().Features.ContainsKey(featureID) ? string.Empty :
        //            ResourceUriSettings.GetConfig().Features[featureID].Feature.ToWindowFeatureClientString();
        //        if (node.Attributes.GetNamedItem("url") != null)
        //            currMenuData.NavigateUrl = string.Format("javascript:window.open('{0}', '{1}', '{2}');",
        //                node.Attributes.GetNamedItem("url").Value,
        //                currMenuData.Target,
        //                feature);
        //        menuData.ChildItems.Add(currMenuData);
        //    }
        //    else
        //    {
        //        foreach (XmlElement elemCaption in captions)
        //        {
        //            MCS.Web.WebControls.MenuItem currMenuData = new MCS.Web.WebControls.MenuItem();
        //            string strVisible = elemCaption.GetAttribute("visible").ToLower();
        //            string strType = elemCaption.GetAttribute("type").ToLower();
        //            if (elemCaption.Attributes.GetNamedItem("name") != null)
        //                currMenuData.Text = elemCaption.Attributes.GetNamedItem("name").Value;
        //            /*
        //            if (elemCaption.Attributes.GetNamedItem("img") != null)
        //                currMenuData.ImageUrl = elemCaption.Attributes.GetNamedItem("img").Value;
        //             */
        //            if (elemCaption.Attributes.GetNamedItem("target") != null)
        //                currMenuData.Target = elemCaption.Attributes.GetNamedItem("target").Value;
        //            string featureID = elemCaption.Attributes.GetNamedItem("featureID") ==
        //                null ? string.Empty : elemCaption.Attributes.GetNamedItem("featureID").Value;
        //            string feature = string.IsNullOrEmpty(featureID) ? string.Empty :
        //                !ResourceUriSettings.GetConfig().Features.ContainsKey(featureID) ? string.Empty :
        //                ResourceUriSettings.GetConfig().Features[featureID].Feature.ToWindowFeatureClientString();
        //            if (elemCaption.Attributes.GetNamedItem("url") != null)
        //                currMenuData.NavigateUrl = string.Format("javascript:window.open('{0}', '{1}', '{2}');",
        //                    elemCaption.Attributes.GetNamedItem("url").Value,
        //                    currMenuData.Target,
        //                    feature);


        //            if (elemCaption.ChildNodes.Count > 0)
        //            {
        //                /*
        //                foreach (MCS.Web.WebControls.MenuItem item in menuData.ChildItems)
        //                {
        //                    RenderSubMenu(elemCaption, item);
        //                }
        //                 */

        //                XmlNodeList xmlList = elemCaption.SelectNodes("Item");

        //                foreach (XmlElement ele in xmlList)
        //                {
        //                    RenderSubMenu(ele, currMenuData);
        //                }

        //            }

        //            menuData.ChildItems.Add(currMenuData);

        //        }
        //    }
        //}

        #endregion
    }
}