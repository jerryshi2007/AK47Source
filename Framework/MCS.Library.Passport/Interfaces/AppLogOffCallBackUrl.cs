#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	AppLogOffCallBackUrl.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Passport
{
    /// <summary>
    /// ������Ӧ��LogOffʱ�ص���Url
    /// </summary>
    public class AppLogOffCallBackUrl
    {
        private string appID = string.Empty;
        private string logOffCallBackUrl = string.Empty;

        /// <summary>
        /// Ӧ��ID
        /// </summary>
        public string AppID
        {
            get { return this.appID; }
            set { this.appID = value; }
        }

        /// <summary>
        /// Ӧ��ע���ص���Url
        /// </summary>
        public string LogOffCallBackUrl
        {
            get { return this.logOffCallBackUrl; }
            set { this.logOffCallBackUrl = value; }
        }
    }
}
