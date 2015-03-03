using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 用户的扮演信息
    /// </summary>
    public class UserImpersonatingInfo
    {
        private string originalUserID = string.Empty;
        private string originalUserName = string.Empty;
        private string impersonatingUserID = string.Empty;
        private string impersonatingUserName = string.Empty;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="oUserID">原始的用户ID</param>
        /// <param name="oUserName">原始的用户名</param>
        /// <param name="iUserID">正在扮演的用户ID</param>
        /// <param name="iUserName">正在扮演的用户名称</param>
        public UserImpersonatingInfo(string oUserID, string oUserName, string iUserID, string iUserName)
        {
            this.originalUserID = oUserID;
            this.originalUserName = oUserName;
            this.impersonatingUserID = iUserID;
            this.impersonatingUserName = iUserName;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public UserImpersonatingInfo()
        {
        }

        /// <summary>
        /// 原始的用户ID
        /// </summary>
        public string OriginalUserID
        {
            get { return this.originalUserID; }
            set { this.originalUserID = value; }
        }

        /// <summary>
        /// 原始的用户名
        /// </summary>
        public string OriginalUserName
        {
            get { return this.originalUserName; }
            set { this.originalUserName = value; }
        }

        /// <summary>
        /// 正在扮演的用户ID
        /// </summary>
        public string ImpersonatingUserID
        {
            get { return impersonatingUserID; }
            set { impersonatingUserID = value; }
        }

        /// <summary>
        /// 正在扮演的用户名称
        /// </summary>
        public string ImpersonatingUserName
        {
            get { return impersonatingUserName; }
            set { impersonatingUserName = value; }
        }
    }
}
