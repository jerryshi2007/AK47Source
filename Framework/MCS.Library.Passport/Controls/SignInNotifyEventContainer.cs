#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	SignInNotifyEventContainer.cs
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
using MCS.Library.Core;
using MCS.Library.Passport;

namespace MCS.Library.Web.Controls
{
	/// <summary>
	/// ��ʼ��SignInControl������ṩ����֤��Ϣ��������ʾ��֤ҳ�棬�Զ�ת��
	/// </summary>
	/// <returns></returns>
	public delegate ISignInUserInfo InitSignInControlDelegate();

    /// <summary>
    /// ��֤֮ǰ���¼����塣�����޸Ĵ���֤���û����Ϳ���
    /// </summary>
    public delegate void BeforeAuthenticateDelegate(LogOnIdentity loi);

    /// <summary>
    /// ��֤֮����¼����塣�����޸���֤֮���û���¼����Ҳ���Խػ���֤ʱ�������쳣
    /// </summary>
    public delegate void AfterSignInDelegate(SignInContext context);

    /// <summary>
    /// ��֤���̵��¼�����
    /// </summary>
    public class SignInNotifyEventContainer
    {
        /// <summary>
        /// ��֤֮ǰ���¼��������޸���Ҫ��֤�û����û����Ϳ���
        /// </summary>
        public event BeforeAuthenticateDelegate BeforeAuthenticate;

        /// <summary>
        /// ��֤֮����¼�
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
    /// �û���¼�Ļص��ӿڣ�������֤�����н���֪ͨ
    /// </summary>
    public interface ISignInNotifier
    {
        /// <summary>
        /// ��ʼ����
        /// </summary>
        /// <param name="eventContainer">������</param>
        void Init(SignInNotifyEventContainer eventContainer);
    }

}
