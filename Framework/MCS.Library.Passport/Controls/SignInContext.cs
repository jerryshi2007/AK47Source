#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	SignInContext.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
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
    /// ��֤���
    /// </summary>
    public enum SignInResultType
    {
        /// <summary>
        /// ��״̬
        /// </summary>
        None = 0,

        /// <summary>
        /// ��֤�ɹ�
        /// </summary>
        Success = 1,

        /// <summary>
        /// ��֤ʧ��
        /// </summary>
        Fail = 2
    }

    /// <summary>
    /// ��֤������
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
        /// ��֤���
        /// </summary>
        public SignInResultType ResultType
        {
            get
            {
                return this.resultType;
            }
        }

        /// <summary>
        /// ��֤���û�ID
        /// </summary>
        public string UserID
        {
            get
            {
                return this.userID;
            }
        }

        /// <summary>
        /// ��֤ҳ�����Ϣ
        /// </summary>
        public SignInPageData PageData
        {
            get
            {
                return this.pageData;
            }
        }

        /// <summary>
        /// ��֤��Ϣ��������֤ͨ������Ч������Ϊnull
        /// </summary>
        public ISignInInfo SignInInfo
        {
            get
            {
                return this.signInInfo;
            }
        }

        /// <summary>
        /// ��֤�����еĿͻ�����Ϣ
        /// </summary>
        public SignInClientInfo ClientInfo
        {
            get
            {
                return this.clientInfo;
            }
        }

        /// <summary>
        /// ��֤�������׳����쳣
        /// </summary>
        public System.Exception Exception
        {
            get
            {
                return this.exception;
            }
        }
    }

    #region �ͻ�����Ϣ
    /// <summary>
    /// �ͻ�����Ϣ
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
        /// ��ȡ�ͻ�����Ϣ
        /// </summary>
        /// <param name="key">��Ϣ����</param>
        /// <returns>�ͻ�����Ϣ</returns>
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
