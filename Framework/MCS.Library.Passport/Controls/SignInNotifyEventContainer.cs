#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	SignInNotifyEventContainer.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Passport;

namespace MCS.Library.Web.Controls
{
	/// <summary>
	/// 初始化SignInControl，如果提供了认证信息，则不再显示认证页面，自动转向
	/// </summary>
	/// <returns></returns>
	public delegate ISignInUserInfo InitSignInControlDelegate();

    /// <summary>
    /// 认证之前的事件定义。可以修改待认证的用户名和口令
    /// </summary>
    public delegate void BeforeAuthenticateDelegate(LogOnIdentity loi);

    /// <summary>
    /// 认证之后的事件定义。可以修改认证之后用户登录名，也可以截获认证时产生的异常
    /// </summary>
    public delegate void AfterSignInDelegate(SignInContext context);

    /// <summary>
    /// 认证过程的事件容器
    /// </summary>
    public class SignInNotifyEventContainer
    {
        /// <summary>
        /// 认证之前的事件。可以修改需要认证用户的用户名和口令
        /// </summary>
        public event BeforeAuthenticateDelegate BeforeAuthenticate;

        /// <summary>
        /// 认证之后的事件
        /// </summary>
        public event AfterSignInDelegate AfterSignIn;

        internal void FireBeforeAuthenticate(LogOnIdentity loi)
        {
            if (BeforeAuthenticate != null)
                BeforeAuthenticate(loi);
        }

        internal void FireAfterSignIn(SignInContext context)
        {
            if (AfterSignIn != null)
                AfterSignIn(context);
        }
    }

    /// <summary>
    /// 用户登录的回调接口，会在认证过程中进行通知
    /// </summary>
    public interface ISignInNotifier
    {
        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="eventContainer">容器。</param>
        void Init(SignInNotifyEventContainer eventContainer);
    }

}
