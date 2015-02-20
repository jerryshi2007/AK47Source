#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	WindowsAuthenticationModule.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
// -------------------------------------------------
#endregion
using System;
using System.Web;
using System.Text;
using System.Security.Principal;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.Passport.Properties;

namespace MCS.Library.Passport
{
    /// <summary>
    /// windows������֤��
    /// </summary>
    public sealed class WindowsAuthenticationModule : AuthenticationModuleBase
    {
        /// <summary>
        /// ��ȡ��¼Ʊ��
        /// </summary>
        /// <param name="ticket">ticket</param>
        /// <returns>�û�ID</returns>
        protected override string GetLogOnName(out ITicket ticket)
        {
            ticket = null;

            HttpRequest request = HttpContext.Current.Request;

            string logonName = request.ServerVariables["LOGON_USER"];

			ExceptionHelper.TrueThrow<AuthenticateException>(string.IsNullOrEmpty(logonName),
			    Resource.PageMustForbidAnonymousAccess);

            LogOnIdentity loi = new LogOnIdentity(logonName);

			DomainMappingSettings section = DomainMappingSettings.GetConfig();

			string domainName = section.Mappings[loi.Domain];

			if (section.CheckDomainName)
                CheckDomainName(domainName);

            return logonName;
        }

        private void CheckDomainName(string domainName)
        {
            if (string.IsNullOrEmpty(EnvironmentHelper.ShortDomainName) == false)
            {
                //�����Ѿ�������
                ExceptionHelper.FalseThrow<AuthenticateException>(
                    string.Compare(domainName, EnvironmentHelper.ShortDomainName, true) == 0,
                    Resource.DomainNameMismatch, domainName, EnvironmentHelper.ShortDomainName);
            }
        }
    }
}
