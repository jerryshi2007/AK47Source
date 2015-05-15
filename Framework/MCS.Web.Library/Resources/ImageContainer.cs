using System;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;

[assembly: WebResource("MCS.Web.Library.Resources.Images.word.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.excel.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.htm.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.image.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.pdf.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.ppt.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.shusheng.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.sound.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.visio.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.winrar.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.winzip.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.wmp.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.wordpad.gif", "image/gif")]

//Org Trees
[assembly: WebResource("MCS.Web.Library.Resources.Images.cross.png", "image/png")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.hline.png", "image/png")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.lc.png", "image/png")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.rc.png", "image/png")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.reverseTee.png", "image/png")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.tee.png", "image/png")]
[assembly: WebResource("MCS.Web.Library.Resources.Images.vline.png", "image/png")]

namespace MCS.Web.Library.Resources
{
    internal sealed class ImageContainer
    {
        public static string CrossUrl
        {
            get
            {
                return GetWebResourceUrl("MCS.Web.Library.Resources.Images.cross.png");
            }
        }

        public static string HLineUrl
        {
            get
            {
                return GetWebResourceUrl("MCS.Web.Library.Resources.Images.hline.png");
            }
        }

        public static string LeftCornerUrl
        {
            get
            {
                return GetWebResourceUrl("MCS.Web.Library.Resources.Images.lc.png");
            }
        }

        public static string RightCornerUrl
        {
            get
            {
                return GetWebResourceUrl("MCS.Web.Library.Resources.Images.rc.png");
            }
        }

        public static string ReverseTeeUrl
        {
            get
            {
                return GetWebResourceUrl("MCS.Web.Library.Resources.Images.reverseTee.png");
            }
        }

        public static string TeeUrl
        {
            get
            {
                return GetWebResourceUrl("MCS.Web.Library.Resources.Images.tee.png");
            }
        }

        public static string VLineUrl
        {
            get
            {
                return GetWebResourceUrl("MCS.Web.Library.Resources.Images.vline.png");
            }
        }

        private static string GetWebResourceUrl(string resourceName)
        {
            Page page = WebUtility.GetCurrentPage();

            if (page == null)
                page = new Page();

            return page.ClientScript.GetWebResourceUrl(typeof(ImageContainer), resourceName);
        }
    }
}
