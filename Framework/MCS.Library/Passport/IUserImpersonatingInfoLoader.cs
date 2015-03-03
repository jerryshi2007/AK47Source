using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 加载用户扮演信息的接口定义
    /// </summary>
    public interface IUserImpersonatingInfoLoader
    {
        /// <summary>
        /// 加载用户扮演信息
        /// </summary>
        /// <param name="originalUserID">原始的用户ID</param>
        /// <returns>用户扮演信息</returns>
        UserImpersonatingInfo GetUserImpersonatingInfo(string originalUserID);
    }
}
