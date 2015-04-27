#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	EnvironmentHelper.cs
// Remark	��	����Ӧ�û���������� 
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace MCS.Library.Core
{
    /// <summary>
    /// Instance mode
    /// </summary>
    public enum InstanceMode
    {
        /// <summary>
        /// WindowsӦ��
        /// </summary>
        Windows,

        /// <summary>
        /// Asp.netӦ�ã��ܹ����HttpContext
        /// </summary>
        Web
    }

    /// <summary>
    /// ����ϵͳ�����л���������
    /// </summary>
    public enum EnvironmentSystemType
    {
        /// <summary>
        /// Win32ϵͳ
        /// </summary>
        Windows32 = 4,

        /// <summary>
        /// Win64ϵͳ
        /// </summary>
        Windows64 = 8,

        /// <summary>
        /// Win128ϵͳ
        /// </summary>
        Windows128 = 16
    }

    /// <summary>
    /// ����Ӧ�û����������
    /// </summary>
    /// <remarks>����Ӧ�û����������
    /// </remarks>
    public static class EnvironmentHelper
    {
        #region Private field

        private static string shortDomainName;
        private static string domainDnsName;
        private static string computerNetBIOSName;

        #endregion

        #region Constructor
        /// <summary>
        /// EnvironmentHelper�Ĺ��캯��
        /// </summary>
        /// <remarks>EnvironmentHelper�Ĺ��캯��,�ù��캯�������κβ�����
        /// </remarks>
        static EnvironmentHelper()
        {
            EnvironmentHelper.shortDomainName = GetShortDomainName();
            EnvironmentHelper.domainDnsName = GetDomainDnsName();
            EnvironmentHelper.computerNetBIOSName = GetComputerNetBIOSName();
        }
        #endregion

        #region InterOP
        internal enum COMPUTER_NAME_FORMAT
        {
            ComputerNameNetBIOS,
            ComputerNameDnsHostname,
            ComputerNameDnsDomain,
            ComputerNameDnsFullyQualified,
            ComputerNamePhysicalNetBIOS,
            ComputerNamePhysicalDnsHostname,
            ComputerNamePhysicalDnsDomain,
            ComputerNamePhysicalDnsFullyQualified,
            ComputerNameMax,
        }

        internal enum EXTENDED_NAME_FORMAT
        {
            NameUnknown = 0,
            NameFullyQualifiedDN = 1,
            NameSamCompatible = 2,
            NameDisplay = 3,
            NameUniqueId = 6,
            NameCanonical = 7,
            NameUserPrincipal = 8,
            NameCanonicalEx = 9,
            NameServicePrincipal = 10,
            NameDnsDomain = 12,
        }

        /*
        ComputerNameDnsDomain:			oa.hgzs.ain.cn
        ComputerNameDnsFullyQualified:	mcs-shenzheng.oa.hgzs.ain.cn
        ComputerNameDnsHostname:		mcs-shenzheng
        ComputerNameNetBIOS:			MCS-SHENZHENG
        ComputerNamePhysicalDnsDomain:	oa.hgzs.ain.cn
        ComputerNamePhysicalDnsFullyQualified:
                                        mcs-shenzheng.oa.hgzs.ain.cn
        ComputerNamePhysicalDnsHostname:
                                        mcs-shenzheng
        ComputerNamePhysicalNetBIOS:	MCS-SHENZHENG

        NameCanonical:			oa.hgzs.ain.cn/Computers/MCS-SHENZHENG
        NameCanonicalEx:		oa.hgzs.ain.cn/Computers
        MCS-SHENZHENG
        NameDisplay:			MCS-SHENZHENG$
        NameDnsDomain:		
        NameFullyQualifiedDN:	CN=MCS-SHENZHENG,CN=Computers,DC=oa,DC=hgzs,DC=ain,DC=cn
        NameSamCompatible:		OA\MCS-SHENZHENG$
        NameServicePrincipal:		
        NameUniqueId:			{848ed57e-afd5-49e9-8d9c-cdb12bd6cf9a}
        NameUnknown:		
        NameUserPrincipal:
        */
        #endregion InterOP

        #region Public
        /// <summary>
        /// ��ȡ��ǰ���Ե�NetBIOS����
        /// </summary>
        public static string ComputerNetBIOSName
        {
            get
            {
                return EnvironmentHelper.computerNetBIOSName;
            }
        }

        /// <summary>
        /// �Ƿ�ʹ��web.config��Ϊ�����ļ�
        /// </summary>
        public static bool IsUsingWebConfig
        {
            get
            {
                return HostingEnvironment.IsHosted;
            }
        }

        /// <summary>
        /// ��ȡ���л�����ϵͳ���ͣ�32,64...��
        /// </summary>
        public static EnvironmentSystemType SystemType
        {
            get
            {
                EnvironmentSystemType result = EnvironmentSystemType.Windows32;

                switch (IntPtr.Size)
                {
                    case 4:
                        result = EnvironmentSystemType.Windows32;
                        break;
                    case 8:
                        result = EnvironmentSystemType.Windows64;
                        break;
                    case 16:
                        result = EnvironmentSystemType.Windows128;
                        break;
                }

                return result;
            }
        }

        /// <summary>
        /// ��ǰӦ���Ƿ�ΪwebӦ�õ����� ( Windows / Web)
        /// </summary>
        /// <remarks>��������ֻ���ġ�
        /// <seealso cref="MCS.Library.Configuration.ConfigurationBroker"/>
        /// <seealso cref="MCS.Library.Configuration.MetaConfigurationSourceInstanceElement"/>
        /// <seealso cref="MCS.Library.Configuration.MetaConfigurationSourceMappingElement"/>
        /// </remarks>
        public static InstanceMode Mode
        {
            get
            {
                if (EnvironmentHelper.CheckIsWebApplication())
                    return InstanceMode.Web;
                else
                    return InstanceMode.Windows;
            }
        }

        /// <summary>
        /// �������������ע�ᣬ���ض�����
        /// </summary>
        /// <remarks>������ֻ����������������ϵĶ�������oa\hb2004-db����ôShortDomainName����oa�����û�м������򷵻ؿմ�</remarks>
        public static string ShortDomainName
        {
            get
            {
                return EnvironmentHelper.shortDomainName;
            }
        }

        /// <summary>
        /// �������������ע�ᣬ���س�����
        /// </summary>
        /// <remarks>������ֻ������ĳ�������oa.hgzs.ain.cn�����û�м������򷵻ؿմ�</remarks>
        public static string DomainDnsName
        {
            get
            {
                return EnvironmentHelper.domainDnsName;
            }
        }

        /// <summary>
        /// ����ַ����еĻ�������
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReplaceEnvironmentVariablesInString(string filePath)
        {
            string result = filePath;

            Regex r = new Regex(@"%\S+?%");

            foreach (Match m in r.Matches(filePath))
            {
                string variableName = m.Value.Trim('%');
                string variableValue = Environment.GetEnvironmentVariable(variableName);

                if (variableValue != null)
                    result = result.Replace(m.Value, variableValue);
            }

            return result;
        }

        /// <summary>
        /// �õ���ǰ������������Ϣ
        /// </summary>
        /// <returns></returns>
        public static string GetEnvironmentInfo()
        {
            StringBuilder strB = new StringBuilder(1024);

            using (StringWriter writer = new StringWriter(strB))
            {
                if (HostingEnvironment.IsHosted)
                    WriteHostInfo(writer);

                if (Mode == InstanceMode.Web)
                    WriteHttpContextInfo(writer);

                WriteProcessInfo(writer);
            }

            return strB.ToString();
        }

        /// <summary>
        /// ��ȡHttpRequest�Ŀͻ��˵�IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            IPAddress ip = null;

            if (CheckIsWebApplication())
            {
                ip = HttpContext.Current.Request.ServerVariables.GetValue("HTTP_X_FORWARDED_FOR", string.Empty).GetFirstIPAddress();

                if (ip == null)
                {
                    ip = HttpContext.Current.Request.ServerVariables.GetValue("REMOTE_ADDR", string.Empty).GetFirstIPAddress();

                    if (ip == null)
                        ip = HttpContext.Current.Request.UserHostAddress.GetFirstIPAddress();
                }
            }

            return (ip != null) ? ip.ToString() : string.Empty;
        }
        #endregion Public

        #region Private helper method
        private static void WriteProcessInfo(TextWriter writer)
        {
            try
            {
                Process process = Process.GetCurrentProcess();

                writer.WriteLine("MachineName: {0}, Process ID: {1}, Name: {2}", GetComputerNetBIOSName(), process.Id, process.ProcessName);
            }
            catch (System.Exception)
            {
            }
        }

        private static void WriteHostInfo(TextWriter writer)
        {
            writer.WriteLine("Host Info:");
            if (HostingEnvironment.ApplicationHost != null)
                writer.WriteLine("Site ID: {0}", HostingEnvironment.ApplicationHost.GetSiteID());

            writer.WriteLine("Site Name: {0}", HostingEnvironment.SiteName);
            writer.WriteLine("Application Virtual Path: {0}", HostingEnvironment.ApplicationVirtualPath);
            writer.WriteLine("Application ID: {0}", HostingEnvironment.ApplicationID);
        }

        private static void WriteHttpContextInfo(TextWriter writer)
        {
            writer.WriteLine("Request Info:");
            HttpRequest request = HttpContext.Current.Request;

            writer.WriteLine("Url: {0}", request.Url.ToString());

            if (request.UrlReferrer != null)
                writer.WriteLine("Url Referrer: {0}", request.UrlReferrer.ToString());

            writer.WriteLine("User Agent: {0}", request.UserAgent);
            writer.WriteLine("User Host Address: {0}", request.UserHostAddress);
        }

        private static bool CheckIsWebApplication()
        {
            bool isWebApp = false;

            AppDomain domain = AppDomain.CurrentDomain;
            try
            {
                if (domain.ShadowCopyFiles)
                    isWebApp = (HttpContext.Current != null);
            }
            catch (System.Exception)
            {
            }

            return isWebApp;
        }

        private static string GetShortDomainName()
        {
            string machineName = InnerGetComputerObjectName(EXTENDED_NAME_FORMAT.NameSamCompatible);

            string[] nameParts = machineName.Split('\\', '/');

            return nameParts[0];
        }

        private static string GetDomainDnsName()
        {
            return InnerGetComputerName(COMPUTER_NAME_FORMAT.ComputerNamePhysicalDnsDomain);
        }

        private static string GetComputerNetBIOSName()
        {
            return InnerGetComputerName(COMPUTER_NAME_FORMAT.ComputerNameNetBIOS);
        }

        private static string InnerGetComputerObjectName(EXTENDED_NAME_FORMAT nameFormat)
        {
            StringBuilder strB = new StringBuilder(1024);

            ulong nSize = (ulong)strB.Capacity;

            NativeMethods.GetComputerObjectName(nameFormat, strB, ref nSize);

            return strB.ToString();
        }

        private static string InnerGetComputerName(COMPUTER_NAME_FORMAT nameType)
        {
            StringBuilder strB = new StringBuilder(1024);

            uint nSize = (uint)strB.Capacity;

            NativeMethods.GetComputerNameEx(nameType, strB, ref nSize);

            return strB.ToString();
        }
        #endregion
    }
}
