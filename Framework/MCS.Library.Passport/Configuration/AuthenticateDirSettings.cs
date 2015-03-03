#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	AuthenticateDirSettings.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
// -------------------------------------------------
#endregion
using System;
using System.Configuration;
using System.IO;
using System.Web;
using MCS.Library.Caching;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Principal;

namespace MCS.Library.Passport
{
    /// <summary>
    /// ��WebӦ���У���ЩĿ¼��Ҫ��֤�����ý�
    /// </summary>
    public sealed class AuthenticateDirSettings : AuthenticateDirSettingsBase
    {
        private AuthorizationDirElementCollection authorizationDirs = null;

        /// <summary>
        /// ��ȡ������֤Ŀ¼��Ϣ
        /// </summary>
        /// <returns>��֤Ŀ¼����</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="AuthenticateDirConfigTest" lang="cs" title="��ȡ��֤Ŀ¼����" />
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="AnonymousDirConfigTest" lang="cs" title="��ȡ����Ŀ¼����" />
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="PageNeedAuthenticateTest" lang="cs" title="�ж�ҳ���Ƿ���Ҫ��֤" />
        /// </remarks>
        public static AuthenticateDirSettings GetConfig()
        {
            AuthenticateDirSettings settings = (AuthenticateDirSettings)ConfigurationBroker.GetSection("authenticateDirSettings");

            if (settings == null)
                settings = new AuthenticateDirSettings();

            return settings;
        }

        private AuthenticateDirSettings()
        {
        }

        /// <summary>
        /// ��Ҫ������Ȩ�ͽ�ɫ���Ƶ�Urls
        /// </summary>
        [ConfigurationProperty("authorizationDirs")]
        public AuthorizationDirElementCollection AuthorizationDirs
        {
            get
            {
                lock (this.syncRoot)
                {
                    if (this.authorizationDirs == null)
                    {
                        this.authorizationDirs = (AuthorizationDirElementCollection)this["authorizationDirs"];

                        if (this.authorizationDirs == null)
                            this.authorizationDirs = new AuthorizationDirElementCollection();
                    }

                    return this.authorizationDirs;
                }
            }
        }
    }

    /// <summary>
    /// ��Ҫ��֤����������Ŀ¼���������
    /// </summary>
    public abstract class AuthenticateDirElementCollectionBase : ConfigurationElementCollection
    {
        /// <summary>
        /// ʹ�õ�ǰ��Web Request��·������ƥ��Ľ��
        /// </summary>
        /// <typeparam name="T">���������͡�</typeparam>
        /// <returns>ƥ������</returns>
        public T GetMatchedElement<T>() where T : AuthenticateDirElementBase
        {
            Common.CheckHttpContext();

            HttpRequest request = HttpContext.Current.Request;

            string url = request.Url.GetComponents(
                UriComponents.SchemeAndServer | UriComponents.Path | UriComponents.Query,
                UriFormat.SafeUnescaped);

            return GetMatchedElement<T>(url);
        }

        /// <summary>
        /// ·��ƥ����
        /// </summary>
        /// <typeparam name="T">���������͡�</typeparam>
        /// <param name="url">Ӧ��·���µ����·��</param>
        /// <returns>ƥ������</returns>
        public T GetMatchedElement<T>(string url) where T : AuthenticateDirElementBase
        {
            T result = null;

            for (int i = 0; i < this.Count; i++)
            {
                T item = (T)BaseGet(i);
                string strTPath = item.Location;

                if (item.IsWildcharMatched(url))
                {
                    result = item;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// ��ȡ���ýڵ�ļ�ֵ��
        /// </summary>
        /// <param name="element">���ýڵ�</param>
        /// <returns>���ýڵ�ļ�ֵ��</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AuthenticateDirElementBase)element).Location;
        }
    }

    /// <summary>
    /// ��Ҫ��֤Ŀ¼���������
    /// </summary>
    public class AuthenticateDirElementCollection : AuthenticateDirElementCollectionBase
    {
        internal AuthenticateDirElementCollection()
        {
        }

        /// <summary>
        /// �����µ����ýڵ㡣
        /// </summary>
        /// <returns>�µ����ýڵ㡣</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new AuthenticateDirElement();
        }
    }

    /// <summary>
    /// ��Ҫ��������Ŀ¼���������
    /// </summary>
    public class AnonymousDirElementCollection : AuthenticateDirElementCollectionBase
    {
        internal AnonymousDirElementCollection()
        {
        }

        /// <summary>
        /// �����µ����ýڵ㡣
        /// </summary>
        /// <returns>�µ����ýڵ㡣</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new AnonymousDirElement();
        }
    }

    /// <summary>
    /// ��֤�����������ʵ�Ŀ¼�����������
    /// </summary>
    public abstract class AuthenticateDirElementBase : ConfigurationElement
    {
        /// <summary>
        /// ·����
        /// </summary>
        [ConfigurationProperty("location", IsRequired = true, IsKey = true)]
        public string Location
        {
            get
            {
                string srcLocation = (string)this["location"];
                string location;

                if (LocationContextCache.Instance.TryGetValue(srcLocation, out location) == false)
                {
                    location = NormalizePath(srcLocation);
                    LocationContextCache.Instance.Add(srcLocation, location);
                }

                return location;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            return true;
        }

        #region MatchWithAsterisk
        private static bool MatchWithAsterisk(string data, string pattern)
        {
            if (data.IsNullOrEmpty() || pattern.IsNullOrEmpty())
                return false;

            string[] ps = pattern.Split('*');

            if (ps.Length == 1) // û��*��ģ��
                return MatchWithInterrogation(data, ps[0]);

            var si = data.IndexOf(ps[0], 0);	// ��stringͷ���ҵ�һ����

            if (si != 0)
                return false; // ��һ����û�ҵ����߲���string��ͷ��

            si += ps[0].Length; // �ҵ��˴���,���ͽ�ԭ��,�Ƶ�δ��ѯ���������

            int plast = ps.Length - 1; // ���һ��Ӧ��������,Ϊ�����Ч��,������ѭ����ȡ��
            int pi = 0; // ����֮ǰ������ĵ�һ��

            while (++pi < plast)
            {
                if (ps[pi] == "")
                    continue; //������*��,���Ժ���

                si = data.IndexOf(ps[pi], si);	// ������һ���Ĳ���

                if (-1 == si)
                    return false; // û���ҵ�

                si += ps[pi].Length; // �ͽ�ԭ��
            }

            if (ps[plast] == "") // ģ��β��Ϊ*,˵��������Ч�ַ���������ȫ��ƥ��,string��������������ַ�
                return true;

            // ��β����ѯ���һ���Ƿ����
            int last_index = data.LastIndexOf(ps[plast]);

            // ���������,һ��Ҫ��string��β��, ���Ҳ���Խ���Ѳ�ѯ������
            return (last_index == data.Length - ps[plast].Length) && (last_index >= si);
        }

        private static bool MatchWithInterrogation(string data, string pattern)
        {
            bool result = false;

            if (data.Length == pattern.Length)
                result = data.IndexOf(pattern) > -1;

            return result;
        }
        #endregion MatchWithAsterisk

        /// <summary>
        /// �ж�ĳ��·���Ƿ���ƥ����BaseDirInfo�еĴ�ͨ�����·��
        /// </summary>
        /// <param name="path">��Ҫƥ���·��</param>
        /// <returns>�Ƿ�ƥ��</returns>
        public bool IsWildcharMatched(string path)
        {
            //��β��*Ϊ�˱��ּ�����
            string pattern = Location.ToLower() + "*";

            return MatchWithAsterisk(path.ToLower(), pattern);
        }

        /*
        /// <summary>
        /// �ж�ĳ��·���Ƿ���ƥ����BaseDirInfo�еĴ�ͨ�����·��
        /// </summary>
        /// <param name="path">��Ҫƥ���·��</param>
        /// <returns>�Ƿ�ƥ��</returns>
        public bool IsWildcharMatched(string path)
        {
            string strTemplate = Location;

            string srcFileName = Path.GetFileNameWithoutExtension(path);
            string srcFileExt = Path.GetExtension(path).Trim('.', ' ');
            string srcDir = Path.GetDirectoryName(path);

            string tempFileName = Path.GetFileNameWithoutExtension(strTemplate);
            string tempFileExt = Path.GetExtension(strTemplate).Trim('.', ' ');
            string tempDir = Path.GetDirectoryName(strTemplate);

            bool bResult = false;

            if (srcDir.IndexOf(tempDir, StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (CompareStringWithWildchar(srcFileName, tempFileName) &&
                    CompareStringWithWildchar(srcFileExt, tempFileExt))
                    bResult = true;
            }

            return bResult;
        }
        */

        //private bool CompareStringWithWildchar(string src, string template)
        //{
        //    bool result = false;

        //    if (src == "*" || template == "*")
        //        result = true;
        //    else
        //        result = (src == template);

        //    return result;
        //}

        private string NormalizePath(string path)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(path))
                result = "/";
            else
                result = ResolveUri(path.Trim());

            return result;
        }

        private string ResolveUri(string uriString)
        {
            Uri url = new Uri(uriString, UriKind.RelativeOrAbsolute);

            if (url.IsAbsoluteUri == false && string.IsNullOrEmpty(uriString) == false)
            {
                if (EnvironmentHelper.Mode == InstanceMode.Web)
                {
                    HttpRequest request = HttpContext.Current.Request;
                    string appPathAndQuery = string.Empty;

                    if (uriString[0] == '~')
                        appPathAndQuery = request.ApplicationPath + uriString.Substring(1);
                    else
                        if (uriString[0] != '/')
                            appPathAndQuery = request.ApplicationPath + "/" + uriString;
                        else
                            appPathAndQuery = uriString;

                    appPathAndQuery = appPathAndQuery.Replace("//", "/");

                    uriString = request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) +
                                appPathAndQuery;
                }
            }

            return uriString;
        }
    }

    /// <summary>
    /// ��Ҫ��Ȩ��Ŀ¼�������
    /// </summary>
    public class AuthorizationDirElementCollection : AuthenticateDirElementCollectionBase
    {
        internal AuthorizationDirElementCollection()
        {
        }

        /// <summary>
        /// �����µ����ýڵ㡣
        /// </summary>
        /// <returns>�µ����ýڵ㡣</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new AuthorizationDirElement();
        }
    }

    /// <summary>
    /// ��Ҫ��Ȩ��Ŀ¼������
    /// </summary>
    public class AuthorizationDirElement : AuthenticateDirElementBase
    {
        internal AuthorizationDirElement()
        {
        }

        /// <summary>
        /// RolesDefineConfig�е������������
        /// </summary>
        [ConfigurationProperty("rolesDefineName", IsRequired = false, DefaultValue = "")]
        public string RolesDefineName
        {
            get
            {
                return (string)this["rolesDefineName"];
            }
        }

        /// <summary>
        /// ��׼�Ľ�ɫ���ơ�App1:Role1,Role2;App2:Role3,Role4
        /// </summary>
        [ConfigurationProperty("roles", IsRequired = false, DefaultValue = "")]
        public string Roles
        {
            get
            {
                return (string)this["roles"];
            }
        }

        /// <summary>
        /// ��ǰ�û��Ƿ���ָ���Ľ�ɫ��
        /// </summary>
        /// <returns></returns>
        public bool IsCurrentUserInRoles()
        {
            bool result = false;

            if (RolesDefineName.IsNotEmpty())
                result = RolesDefineConfig.GetConfig().IsCurrentUserInRoles(this.RolesDefineName);

            if (result == false && this.Roles.IsNotEmpty())
                result = HttpContext.Current.User.IsInRole(this.Roles);

            return result;
        }
    }

    /// <summary>
    /// ÿһ����Ҫ�������ʵ�Ŀ¼��������
    /// </summary>
    public class AnonymousDirElement : AuthenticateDirElementBase
    {
        internal AnonymousDirElement()
        {
        }
    }

    /// <summary>
    /// ÿһ����Ҫ��֤��Ŀ¼��������
    /// </summary>
    public class AuthenticateDirElement : AuthenticateDirElementBase
    {
        internal AuthenticateDirElement()
        {
        }

        /// <summary>
        /// ��������û����ʸ�ҳ��ʱû����֤���Ƿ��Զ���ת����֤ҳ��
        /// </summary>
        [ConfigurationProperty("autoRedirect", DefaultValue = true)]
        public bool AutoRedirect
        {
            get
            {
                return (bool)this["autoRedirect"];
            }
        }
    }

    internal class LocationContextCache : ContextCacheQueueBase<string, string>
    {
        public static LocationContextCache Instance
        {
            get
            {
                return ContextCacheManager.GetInstance<LocationContextCache>();
            }
        }

        private LocationContextCache()
        {
        }
    }
}
