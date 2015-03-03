#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	Operations.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Security.Principal;
using System.Collections.Generic;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 实现认证的接口，一般由提供Passport认证服务的程序来使用
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="strUserID">用户标识，通常是登录名</param>
        /// <param name="strPassword">口令</param>
        /// <returns></returns>
        ISignInUserInfo Authenticate(string strUserID, string strPassword);

        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="strUserID">用户标识，通常是登录名</param>
        /// <returns>用户是否存在</returns>
        bool CheckUserExists(string strUserID);
    }

    /// <summary>
    /// 实现认证的接口，一般由提供Passport认证服务的程序来使用。这个接口是IAuthenticator的扩展
    /// </summary>
    public interface IAuthenticator2
    {
        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="strUserID"></param>
        /// <param name="strPassword"></param>
        /// <param name="context">一些额外的参数</param>
        /// <returns></returns>
        ISignInUserInfo Authenticate(string strUserID, string strPassword, Dictionary<string, object> context);

        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="strUserID">用户标识，通常是登录名</param>
        /// <param name="context">一些额外的参数</param>
        /// <returns>用户是否存在</returns>
        bool CheckUserExists(string strUserID, Dictionary<string, object> context);
    }

    /// <summary>
    /// 用户ID的转换器。一般情况下，登录使用的是登录名，实际的用户另有一个ID。
    /// 此接口就是用来转换这两个
    /// </summary>
    public interface IUserIDConverter
    {
        /// <summary>
        /// 根据登录名得到用户不变的，稳定的ID
        /// </summary>
        /// <returns></returns>
        string GetUserConsistentID(string logonName);

        /// <summary>
        /// 根据用户的持久ID得到登录名
        /// </summary>
        /// <param name="consistentID"></param>
        /// <returns></returns>
        string GetUserLogonName(string consistentID);
    }

    /// <summary>
    /// 用户信息的修改器
    /// </summary>
    public interface IUserInfoUpdater
    {
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="strUserID">用户的ID，通常是登录名</param>
        /// <param name="oldPassword">原密码</param>
        /// <param name="newPassword">新密码</param>
        void ChangePassword(string strUserID, string oldPassword, string newPassword);
    }

    /// <summary>
    /// SignInInfo和Ticket的数据访问操作
    /// </summary>
    public interface IPersistSignInInfo
    {
        /// <summary>
        /// 保存登录信息
        /// </summary>
        /// <param name="signInInfo">用户认证后生成的登录信息</param>
        void SaveSignInInfo(ISignInInfo signInInfo);

        /// <summary>
        /// 应用认证后生成的Ticket信息
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="signInUrl"></param>
        /// <param name="logOffCBUrl"></param>
        void SaveTicket(ITicket ticket, Uri signInUrl, Uri logOffCBUrl);

        /// <summary>
        /// 删除相关的认证信息
        /// </summary>
        /// <param name="signInID">认证的SessionID</param>
        void DeleteRelativeSignInInfo(string signInID);

        /// <summary>
        /// 得到某个登录Session的所有注销的回调Url
        /// </summary>
        /// <param name="sessionID">登录SessionID</param>
        /// <returns>某个登录Session的所有注销的回调Url</returns>
        List<AppLogOffCallBackUrl> GetAllRelativeAppsLogOffCallBackUrl(string sessionID);
    }

    /// <summary>
    /// 访问OpenID和用户信息映射的
    /// </summary>
    public interface IPersistOpenIDBinding
    {
        /// <summary>
        /// 保存OpenIDBinding
        /// </summary>
        /// <param name="binding"></param>
        void SaveOpenIDBinding(OpenIDBinding binding);

        /// <summary>
        /// 删除OpenID的绑定信息
        /// </summary>
        /// <param name="openID"></param>
        void RemoveOpenIDBinding(string openID);

        /// <summary>
        /// 根据OpenID得到Binding信息
        /// </summary>
        /// <param name="openID"></param>
        OpenIDBinding GetBindingByOpenID(string openID);

        /// <summary>
        /// 得到每个用户的所有绑定信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        OpenIDBindingCollection GetBindingsByUserID(string userID);
    }
}
