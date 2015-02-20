#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	SignInContext.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Passport;

namespace MCS.Library.Web.Controls
{
    /// <summary>
    /// 认证结果
    /// </summary>
    public enum SignInResultType
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None = 0,

        /// <summary>
        /// 认证成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 认证失败
        /// </summary>
        Fail = 2
    }

    /// <summary>
    /// 认证上下文
    /// </summary>
    public class SignInContext
    {
        private SignInResultType resultType = SignInResultType.None;
        private string userID = string.Empty;
        private System.Exception exception = null;
        private ISignInInfo signInInfo = null;
        private SignInClientInfo clientInfo = null;
        private SignInPageData pageData = null;

        internal SignInContext(SignInResultType resultType, string userID, ISignInInfo signInInfo, SignInPageData pageData, string clientInfo, System.Exception ex)
        {
            this.resultType = resultType;
            this.userID = userID;
            this.signInInfo = signInInfo;
            this.pageData = pageData;
            this.exception = ex;

            this.clientInfo = new SignInClientInfo(clientInfo);
        }

        /// <summary>
        /// 认证结果
        /// </summary>
        public SignInResultType ResultType
        {
            get
            {
                return this.resultType;
            }
        }

        /// <summary>
        /// 认证的用户ID
        /// </summary>
        public string UserID
        {
            get
            {
                return this.userID;
            }
        }

        /// <summary>
        /// 认证页面的信息
        /// </summary>
        public SignInPageData PageData
        {
            get
            {
                return this.pageData;
            }
        }

        /// <summary>
        /// 认证信息。仅在认证通过后有效，否则为null
        /// </summary>
        public ISignInInfo SignInInfo
        {
            get
            {
                return this.signInInfo;
            }
        }

        /// <summary>
        /// 认证过程中的客户端信息
        /// </summary>
        public SignInClientInfo ClientInfo
        {
            get
            {
                return this.clientInfo;
            }
        }

        /// <summary>
        /// 认证过程中抛出的异常
        /// </summary>
        public System.Exception Exception
        {
            get
            {
                return this.exception;
            }
        }
    }

    #region 客户端信息
    /// <summary>
    /// 客户端信息
    /// </summary>
    public class SignInClientInfo : Dictionary<string, string>
    {
        internal SignInClientInfo(string strInfo)
        {
            if (strInfo != string.Empty)
            {
                XmlDocument xmlDoc = XmlHelper.CreateDomDocument(strInfo);

                InitClientInfo(xmlDoc);
            }
        }
        /// <summary>
        /// 获取客户端信息
        /// </summary>
        /// <param name="key">信息名称</param>
        /// <returns>客户端信息</returns>
        public new string this[string key]
        {
            get
            {
                string result = string.Empty;

                if (TryGetValue(key, out result) == false)
                    result = string.Empty;

                return result;
            }
        }

        internal SignInClientInfo(XmlDocument xmlDoc)
        {
            InitClientInfo(xmlDoc);
        }

        private void InitClientInfo(XmlDocument xmlDoc)
        {
            foreach (XmlNode node in xmlDoc.DocumentElement)
                base[node.Name] = node.InnerText;
        }
    }

    #endregion
}
