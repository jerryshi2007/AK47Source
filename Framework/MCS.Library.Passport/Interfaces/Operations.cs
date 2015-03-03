#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	Operations.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Security.Principal;
using System.Collections.Generic;

namespace MCS.Library.Passport
{
    /// <summary>
    /// ʵ����֤�Ľӿڣ�һ�����ṩPassport��֤����ĳ�����ʹ��
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// ��֤
        /// </summary>
        /// <param name="strUserID">�û���ʶ��ͨ���ǵ�¼��</param>
        /// <param name="strPassword">����</param>
        /// <returns></returns>
        ISignInUserInfo Authenticate(string strUserID, string strPassword);

        /// <summary>
        /// ����û��Ƿ����
        /// </summary>
        /// <param name="strUserID">�û���ʶ��ͨ���ǵ�¼��</param>
        /// <returns>�û��Ƿ����</returns>
        bool CheckUserExists(string strUserID);
    }

    /// <summary>
    /// ʵ����֤�Ľӿڣ�һ�����ṩPassport��֤����ĳ�����ʹ�á�����ӿ���IAuthenticator����չ
    /// </summary>
    public interface IAuthenticator2
    {
        /// <summary>
        /// ��֤
        /// </summary>
        /// <param name="strUserID"></param>
        /// <param name="strPassword"></param>
        /// <param name="context">һЩ����Ĳ���</param>
        /// <returns></returns>
        ISignInUserInfo Authenticate(string strUserID, string strPassword, Dictionary<string, object> context);

        /// <summary>
        /// ����û��Ƿ����
        /// </summary>
        /// <param name="strUserID">�û���ʶ��ͨ���ǵ�¼��</param>
        /// <param name="context">һЩ����Ĳ���</param>
        /// <returns>�û��Ƿ����</returns>
        bool CheckUserExists(string strUserID, Dictionary<string, object> context);
    }

    /// <summary>
    /// �û�ID��ת������һ������£���¼ʹ�õ��ǵ�¼����ʵ�ʵ��û�����һ��ID��
    /// �˽ӿھ�������ת��������
    /// </summary>
    public interface IUserIDConverter
    {
        /// <summary>
        /// ���ݵ�¼���õ��û�����ģ��ȶ���ID
        /// </summary>
        /// <returns></returns>
        string GetUserConsistentID(string logonName);

        /// <summary>
        /// �����û��ĳ־�ID�õ���¼��
        /// </summary>
        /// <param name="consistentID"></param>
        /// <returns></returns>
        string GetUserLogonName(string consistentID);
    }

    /// <summary>
    /// �û���Ϣ���޸���
    /// </summary>
    public interface IUserInfoUpdater
    {
        /// <summary>
        /// �޸�����
        /// </summary>
        /// <param name="strUserID">�û���ID��ͨ���ǵ�¼��</param>
        /// <param name="oldPassword">ԭ����</param>
        /// <param name="newPassword">������</param>
        void ChangePassword(string strUserID, string oldPassword, string newPassword);
    }

    /// <summary>
    /// SignInInfo��Ticket�����ݷ��ʲ���
    /// </summary>
    public interface IPersistSignInInfo
    {
        /// <summary>
        /// �����¼��Ϣ
        /// </summary>
        /// <param name="signInInfo">�û���֤�����ɵĵ�¼��Ϣ</param>
        void SaveSignInInfo(ISignInInfo signInInfo);

        /// <summary>
        /// Ӧ����֤�����ɵ�Ticket��Ϣ
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="signInUrl"></param>
        /// <param name="logOffCBUrl"></param>
        void SaveTicket(ITicket ticket, Uri signInUrl, Uri logOffCBUrl);

        /// <summary>
        /// ɾ����ص���֤��Ϣ
        /// </summary>
        /// <param name="signInID">��֤��SessionID</param>
        void DeleteRelativeSignInInfo(string signInID);

        /// <summary>
        /// �õ�ĳ����¼Session������ע���Ļص�Url
        /// </summary>
        /// <param name="sessionID">��¼SessionID</param>
        /// <returns>ĳ����¼Session������ע���Ļص�Url</returns>
        List<AppLogOffCallBackUrl> GetAllRelativeAppsLogOffCallBackUrl(string sessionID);
    }

    /// <summary>
    /// ����OpenID���û���Ϣӳ���
    /// </summary>
    public interface IPersistOpenIDBinding
    {
        /// <summary>
        /// ����OpenIDBinding
        /// </summary>
        /// <param name="binding"></param>
        void SaveOpenIDBinding(OpenIDBinding binding);

        /// <summary>
        /// ɾ��OpenID�İ���Ϣ
        /// </summary>
        /// <param name="openID"></param>
        void RemoveOpenIDBinding(string openID);

        /// <summary>
        /// ����OpenID�õ�Binding��Ϣ
        /// </summary>
        /// <param name="openID"></param>
        OpenIDBinding GetBindingByOpenID(string openID);

        /// <summary>
        /// �õ�ÿ���û������а���Ϣ
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        OpenIDBindingCollection GetBindingsByUserID(string userID);
    }
}
