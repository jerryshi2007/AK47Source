#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	SignInPageData.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Passport;

namespace MCS.Library.Web.Controls
{
    /// <summary>
    /// 登录页面的配置信息
    /// </summary>
    public class SignInPageData
    {
        private bool dontSaveUserID = false;
        private bool autoSignIn = false;
        private string userID = string.Empty;
        private Dictionary<string, object> _Properties;

        /// <summary>
        /// 是否保存用户名
        /// </summary>
        public bool DontSaveUserID
        {
            get
            {
                return this.dontSaveUserID;
            }
            set
            {
                this.dontSaveUserID = value;
            }
        }

        /// <summary>
        /// 是否自动登录
        /// </summary>
        public bool AutoSignIn
        {
            get
            {
                return this.autoSignIn;
            }
            set
            {
                this.autoSignIn = value;
            }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID
        {
            get
            {
                return this.userID;
            }
            set
            {
                this.userID = value;
            }
        }

        /// <summary>
        /// 扩展的属性信息
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get
            {
                if (this._Properties == null)
                    this._Properties = new Dictionary<string, object>();

                return this._Properties;
            }
        }

        /// <summary>
        /// 从Cookie中装载页面配置信息
        /// </summary>
        public void LoadFromCookie()
        {
            HttpContext context = HttpContext.Current;

            HttpCookie cookie = context.Request.Cookies[PassportSignInSettings.GetConfig().SignInPageDataCookieKey];

            if (cookie != null)
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();

                    xmlDoc.LoadXml(Base64StringToCookieValue(cookie.Value));

                    this.userID = XmlHelper.GetSingleNodeText(xmlDoc.DocumentElement, "UID");
                    this.dontSaveUserID = XmlHelper.GetSingleNodeValue(xmlDoc.DocumentElement, "DSUID", false);
                    this.autoSignIn = XmlHelper.GetSingleNodeValue(xmlDoc.DocumentElement, "ASI", false);

                    this.LoadProperties(xmlDoc.DocumentElement);
                }
                catch (System.Exception)
                {
                    //忽略cookie中的内容错误
                }
            }
        }

        /// <summary>
        /// 存入Cookie中
        /// </summary>
        public void SaveToCookie()
        {
            HttpContext context = HttpContext.Current;

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml("<Data/>");

            XmlHelper.AppendNode(xmlDoc.DocumentElement, "UID", this.userID);
            XmlHelper.AppendNode(xmlDoc.DocumentElement, "DSUID", this.dontSaveUserID.ToString());
            XmlHelper.AppendNode(xmlDoc.DocumentElement, "ASI", this.autoSignIn.ToString());
            this.AppendPropertiesNode(xmlDoc.DocumentElement);

            HttpCookie cookie = new HttpCookie(PassportSignInSettings.GetConfig().SignInPageDataCookieKey);
            cookie.Value = CookieValueToBase64String(xmlDoc.InnerXml);
            cookie.Expires = DateTime.MaxValue;

            context.Response.Cookies.Add(cookie);
        }

        private void AppendPropertiesNode(XmlNode parent)
        {
            if (this._Properties != null)
            {
                XmlNode propNode = parent.AppendNode("Properties");

                foreach (KeyValuePair<string, object> kp in this._Properties)
                {
                    if (kp.Value != null)
                    {
                        XmlNode itemNode = propNode.AppendNode("Property");

                        itemNode.AppendAttr("key", kp.Key);
                        itemNode.AppendAttr("value", kp.Value.ToString());
                    }
                }
            }
        }

        private void LoadProperties(XmlNode parent)
        {
            foreach (XmlNode itemNode in parent.SelectNodes("Properties/Property"))
            {
                string key = itemNode.GetAttributeText("key");
                string value = itemNode.GetAttributeText("value");

                this.Properties[key] = value;
            }
        }

        private static string CookieValueToBase64String(string v)
        {
            byte[] data = Encoding.UTF8.GetBytes(v);

            return Convert.ToBase64String(data);
        }

        private static string Base64StringToCookieValue(string v)
        {
            string cookieValue = v;

            try
            {
                byte[] data = Convert.FromBase64String(v);

                MemoryStream ms = new MemoryStream(data);
                try
                {
                    StreamReader sr = new StreamReader(ms, Encoding.UTF8);

                    cookieValue = sr.ReadToEnd();
                }
                finally
                {
                    ms.Close();
                }
            }
            catch (System.FormatException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return cookieValue;
        }
    }
}
