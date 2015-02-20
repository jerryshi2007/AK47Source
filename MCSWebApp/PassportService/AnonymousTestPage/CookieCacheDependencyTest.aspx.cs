using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MCS.Library.Caching;

namespace MCS.Web.Passport.TestPages
{
    public partial class CookieCacheDependencyTest : System.Web.UI.Page
    {
        private const string CookieName = "CookieCacheDependencyTest";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void setCacheDataBtn_Click(object sender, EventArgs e)
        {
            if (ObjectCacheQueue.Instance.ContainsKey(CookieName))
                ObjectCacheQueue.Instance.Remove(CookieName);

            HttpCookie cookie = new HttpCookie(CookieName);
            cookie.Expires = DateTime.MinValue;
            CookieCacheDependency dependency = new CookieCacheDependency(cookie);

            ObjectCacheQueue.Instance.Add(CookieName, DateTime.Now, dependency);

            ShowCacheData();
        }

        protected void showCacheDataBtn_Click(object sender, EventArgs e)
        {
            ShowCacheData();
        }

        protected void clearCookieBtn_Click(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie(CookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);

            Response.Cookies.Add(cookie);

            if (Request.Cookies[CookieName] != null)
                Request.Cookies.Remove(CookieName);

            ShowCacheData();
        }

        private void ShowCacheData()
        {
            object data;

            if (ObjectCacheQueue.Instance.TryGetValue(CookieName, out data) == true)
                cacheData.Text = data.ToString();
            else
                cacheData.Text = string.Empty;
        }
    }
}
