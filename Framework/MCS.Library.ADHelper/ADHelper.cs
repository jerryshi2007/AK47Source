using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using MCS.Library.Core;

namespace MCS.Library
{
    #region 枚举定义
    [Flags]
    public enum ADSchemaType
    {
        /// <summary>
        /// 未指定
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// 组织机构
        /// </summary>
        [EnumItemDescription("组织", ShortName = "organizationalUnit")]
        Organizations = 1,

        /// <summary>
        /// 用户
        /// </summary>
        [EnumItemDescription("用户", ShortName = "user")]
        Users = 2,

        /// <summary>
        /// 组
        /// </summary>
        [EnumItemDescription("群组", ShortName = "group")]
        Groups = 4,

        /// <summary>
        /// Domain
        /// </summary>
        [EnumItemDescription("域", ShortName = "domain")]
        Domain = 16,

        /// <summary>
        /// Container
        /// </summary>
        [EnumItemDescription("容器", ShortName = "container")]
        Container = 32,

        /// <summary>
        /// 所有条件
        /// </summary>
        All = 65535
    }
    #endregion

    #region AD查询的附加过滤条件
    /// <summary>
    /// AD查询的附加过滤条件
    /// </summary>
    public class ExtraFilter
    {
        private string _OUFilter = string.Empty;
        private string _UserFilter = string.Empty;
        private string _GroupFilter = string.Empty;
        private string _ContainerFilter = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string OUFilter
        {
            get
            {
                return _OUFilter;
            }

            set
            {
                _OUFilter = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserFilter
        {
            get
            {
                return _UserFilter;
            }

            set
            {
                _UserFilter = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string GroupFilter
        {
            get
            {
                return _GroupFilter;
            }

            set
            {
                _GroupFilter = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ContainerFilter
        {
            get
            {
                return _ContainerFilter;
            }

            set
            {
                _ContainerFilter = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ouFilter"></param>
        /// <param name="userFilter"></param>
        /// <param name="groupFilter"></param>
        public ExtraFilter(string ouFilter, string userFilter, string groupFilter)
        {
            _OUFilter = ouFilter;
            _UserFilter = userFilter;
            _GroupFilter = groupFilter;
        }

        /// <summary>
        /// 
        /// </summary>
        public ExtraFilter()
        {
        }
    }

    #endregion

    #region 与AD查询有关的条件设置
    /// <summary>
    /// 用于查询结果产生后的回调
    /// </summary>
    /// <param name="searcher"></param>
    /// <param name="resultList"></param>
    /// <param name="context"></param>
    public delegate void ADSearchCompletedHandler(DirectorySearcher searcher, List<SearchResult> resultList, object context);

    /// <summary>
    /// 与AD查询有关的条件设置
    /// </summary>
    public class ADSearchConditions
    {
        private SortOption _Sort = new SortOption();
        private SearchScope _Scope = SearchScope.OneLevel;
        private int _SizeLimit = 0;
        private int _PageSize = 0;

        public ADSearchConditions()
        {
            _Sort.PropertyName = string.Empty;
        }

        public ADSearchConditions(ADSearchConditions condition)
        {
            _Sort.Direction = condition.Sort.Direction;
            _Sort.PropertyName = condition.Sort.PropertyName;
            _Scope = condition.Scope;
            _SizeLimit = condition.SizeLimit;
            SearchCompletedContext = condition.SearchCompletedContext;
        }

        public ADSearchConditions(SearchScope scope)
        {
            _Scope = scope;
            _Sort.PropertyName = string.Empty;
        }

        public ADSearchConditions(SortOption sort)
        {
            _Sort = sort;
        }

        public SortOption Sort
        {
            get
            {
                return _Sort;
            }

            set
            {
                _Sort = value;
            }
        }

        public SearchScope Scope
        {
            get
            {
                return _Scope;
            }

            set
            {
                _Scope = value;
            }
        }

        public int SizeLimit
        {
            get
            {
                return _SizeLimit;
            }

            set
            {
                _SizeLimit = value;
            }
        }

        public int PageSize
        {
            get
            {
                return _PageSize;
            }

            set
            {
                _PageSize = value;
            }
        }

        public object SearchCompletedContext
        {
            get;
            set;
        }

        public event ADSearchCompletedHandler ADSearchCompleted;

        internal void OnADSearchCompleted(DirectorySearcher searcher, List<SearchResult> resultList)
        {
            if (ADSearchCompleted != null)
                ADSearchCompleted(searcher, resultList, SearchCompletedContext);
        }

        public static string GetFilterByMask(ADSchemaType nMask)
        {
            return GetFilterByMask(nMask, new ExtraFilter());
        }

        public static string GetFilterByMask(ADSchemaType nMask, ExtraFilter ef)
        {
            StringBuilder strB = new StringBuilder(1024);

            strB.Append("(|");

            if ((ADSchemaType.Domain & nMask) != 0)
                strB.Append("(objectCategory=domain)");

            if ((ADSchemaType.Organizations & nMask) != 0)
            {
                string strOU = string.Empty;

                strOU += "(objectCategory=organizationalUnit)";

                if (ef.OUFilter != string.Empty)
                    strOU += ef.OUFilter;

                strB.Append("(&" + strOU + ")");
            }

            if ((ADSchemaType.Container & nMask) != 0)
            {
                string strContainer = "(objectCategory=person)(objectClass=user)";

                if (ef.ContainerFilter != string.Empty)
                    strContainer += ef.ContainerFilter;

                strB.Append("(&" + strContainer + ")");
            }

            if ((ADSchemaType.Users & nMask) != 0)
            {
                string strUser = "(objectCategory=person)(objectClass=user)";

                if (ef.UserFilter != string.Empty)
                    strUser += ef.UserFilter;

                strB.Append("(&" + strUser + ")");
            }

            if ((ADSchemaType.Groups & nMask) != 0)
            {
                string strGroup = "(objectCategory=group)(objectClass=group)";

                if (ef.GroupFilter != string.Empty)
                    strGroup += ef.GroupFilter;

                strB.Append("(&" + strGroup + ")");
            }

            strB.Append(")");

            return strB.ToString();
        }
    }
    #endregion

    public class ADSearchResultParams
    {
        private DirectoryEntry _Parent = null;
        private object _Tag = null;
        private object _LastResult = null;

        internal ADSearchResultParams(DirectoryEntry parent)
        {
            _Parent = parent;
        }

        public DirectoryEntry Parent
        {
            get
            {
                return _Parent;
            }
        }

        public object Tag
        {
            get
            {
                return _Tag;
            }

            set
            {
                _Tag = value;
            }
        }

        public object LastResult
        {
            get
            {
                return _LastResult;
            }

            set
            {
                _LastResult = value;
            }
        }
    }

    public delegate object ADSearchRecursivelyDelegate(SearchResult sr, ADSearchResultParams asrp, object oParams, ref bool bContinue);

    /// <summary>
    /// AD相关的访问方法
    /// </summary>
    public class ADHelper
    {
        public static readonly string[] DefaultADObjProperties = new string[]
												  {
													  "objectGUID",
													  "distinguishedName",
													  "objectClass",
													  "name",
													  "samAccountName",
													  "displayName",
													  "extensionName",
													  "adminDisplayName",
													  "internationalISDNNumber",
													  "description",
													  "title",
													  "personalTitle",
													  "accountExpires",
													  "userAccountControl",
													  "lastLogon",
													  "lastLogoff",
													  "logonCount",
													  "whenCreated",
													  "whenChanged",
													  "x121Address",
													  "primaryTelexNumber",
													  "co",
													  "c",
													  "l",
													  "primaryInternationalISDNNumber",
													  "physicalDeliveryOfficeName",
													  "givenName",
                                                      "member",
                                                      "mail",
													  "sn",
                                                      "msRTCSIP-PrimaryUserAddress",
                                                      "mobile",
                                                      "telephoneNumber"
												  };

        private static ADHelper _InnerInstance = null;

        private ServerInfo _Server = null;

        private static string[] specialCharacters = new string[] { @"\", "*", "(", ")", "NUL", "/" };
        private static string[] replacedCharacters = new string[] { @"\5c", @"\2a", @"\28", @"\29", @"\00", @"\2f" };

        private enum COMPUTER_NAME_FORMAT
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

        private enum EXTENDED_NAME_FORMAT
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

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetComputerNameEx(COMPUTER_NAME_FORMAT NameType, StringBuilder nameBuffer, ref int bufferSize);

        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern int GetComputerObjectName(EXTENDED_NAME_FORMAT nameFormat, StringBuilder nameBuffer, ref int bufferSize);

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

        private ADHelper()
        {
        }

        /// <summary>
        /// 得到ADHelper的实例
        /// </summary>
        /// <returns></returns>
        public static ADHelper GetInstance()
        {
            if (_InnerInstance == null)
                _InnerInstance = new ADHelper();

            return _InnerInstance;
        }

        public static ADHelper GetInstance(ServerInfo si)
        {
            ADHelper inst = new ADHelper();

            inst._Server = si;

            return inst;
        }

        public string DomainShortName
        {
            get
            {
                string machineName = InnerGetComputerObjectName(EXTENDED_NAME_FORMAT.NameSamCompatible);

                string[] nameParts = machineName.Split('\\', '/');

                return nameParts[0];
            }
        }

        public string DomainDNSName
        {
            get
            {
                return InnerGetComputerName(COMPUTER_NAME_FORMAT.ComputerNamePhysicalDnsDomain);
            }
        }

        #region Entry系列
        public DirectoryEntry GetRootDSE()
        {
            return InnerNewEntry("rootDSE");
        }

        public string GetNamingContext()
        {
            string authenticationMsg = string.Empty;
            try
            {
                string result = string.Empty;

                using (DirectoryEntry root = GetRootDSE())
                {

                    if (root != null)
                        authenticationMsg = string.Format("authenticationType: {0}, ", root.AuthenticationType);

                    PropertyValueCollection values = root.Properties["defaultNamingContext"];

                    if (values.Value == null)
                    {
                        values = root.Properties["namingContexts"];

                        foreach (string v in values)
                        {
                            if (v.IndexOf("CN=Schema", StringComparison.OrdinalIgnoreCase) != 0 &&
                                v.IndexOf("CN=Configuration", StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                result = v;
                                break;
                            }
                        }
                    }
                    else
                        result = values.Value.ToString();

                    return result;
                }
            }
            catch (Exception ex)
            {
                if (this._Server != null)
                {
                    if (this._Server.Identity != null)
                        authenticationMsg += string.Format("serverName: {0}, logonName: {1}", this._Server.ServerName, this._Server.Identity.LogOnName);
                    else
                        authenticationMsg += string.Format("serverName: {0}", this._Server.ServerName);
                }

                ex.Data.Add("authenticationMsg", authenticationMsg);

                throw ex;
            }
        }

        public DirectoryEntry GetRootEntry()
        {
            using (DirectoryEntry root = GetRootDSE())
            {
                return InnerNewEntry(root.Properties["defaultNamingContext"].Value.ToString());
            }
        }

        public DirectoryEntry NewEntry(string strDN)
        {
            return InnerNewEntry(AppendNamingContext(strDN));
        }

        public DirectoryEntry CreateEntry(string path, string username, string password)
        {
            DirectoryEntry entry = new DirectoryEntry(path);

            ServerInfo si = GetServer();

            entry.Username = username;
            entry.Password = password;

            return entry;
        }

        public DirectoryEntry CreateEntry(string path)
        {
            DirectoryEntry entry = new DirectoryEntry(path);

            ServerInfo si = GetServer();

            if (si.Identity != null)
            {
            entry.Username = si.Identity.LogOnNameWithDomain;
            entry.Password = si.Identity.Password;
            }

            return entry;
        }

        public bool EntryExists(string strDN)
        {
            bool result = false;
            try
            {
                using (DirectoryEntry entry = InnerNewEntry(AppendNamingContext(strDN)))
                {
                    return entry.NativeObject != null;
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                if (ex.ErrorCode == -2147016656)
                    result = false;
                else
                    throw;
            }

            return result;
        }
        #endregion

        #region ExecuteSearch系列
        public void EnumGroupMembers(string strDN, Action<SearchResult> action)
        {
            strDN.CheckStringIsNullOrEmpty("strDN");

            if (action != null)
            {
                using (DirectoryEntry groupEntry = this.NewEntry(strDN))
                {
                    EnumGroupMembers(groupEntry, action);
                }
            }
        }

        public void EnumGroupMembers(DirectoryEntry groupEntry, Action<SearchResult> action)
        {
            groupEntry.NullCheck("groupEntry");

            StringBuilder strB = new StringBuilder(256);

            foreach (string dn in groupEntry.Properties["member"])
            {
                strB.AppendFormat("(distinguishedName={0})", dn);
            }

            if (strB.Length > 0 && action != null)
            {
                ADSearchConditions condition = new ADSearchConditions();
                condition.Scope = SearchScope.Subtree;

                string filter = string.Format("(&(objectCategory=Person)(objectClass=person)(|{0}))", strB.ToString());
                using (DirectoryEntry entry = this.GetRootEntry())
                {
                    foreach (SearchResult sr in this.ExecuteSearch(entry, filter, condition, DefaultADObjProperties))
                    {
                        action(sr);
                    }
                }
            }
        }

        public List<SearchResult> ExecuteSearch(DirectoryEntry root, string filter, params string[] properties)
        {
            using (DirectorySearcher searcher = new DirectorySearcher(root))
            {
                ADSearchConditions condition = new ADSearchConditions();

                searcher.Filter = filter;
                searcher.SearchScope = condition.Scope;
                searcher.Sort = condition.Sort;
                searcher.SizeLimit = condition.SizeLimit;
                searcher.PageSize = condition.PageSize;

                for (int i = 0; i < properties.Length; i++)
                {
                    searcher.PropertiesToLoad.Add(properties[i]);
                }

                return searcher.FindAll().ToList();
            }
        }

        public List<SearchResult> ExecuteSearch(DirectoryEntry root, string filter, ADSearchConditions condition, params string[] properties)
        {
            using (DirectorySearcher searcher = new DirectorySearcher(root))
            {
                searcher.Filter = filter;
                searcher.SearchScope = condition.Scope;
                searcher.Sort = condition.Sort;
                searcher.SizeLimit = condition.SizeLimit;
                searcher.PageSize = condition.PageSize;

                for (int i = 0; i < properties.Length; i++)
                {
                    searcher.PropertiesToLoad.Add(properties[i]);
                }

                List<SearchResult> result = searcher.FindAll().ToList();

                if (condition != null)
                    condition.OnADSearchCompleted(searcher, result);

                return result;
            }
        }
        #endregion

        #region Recursively系列
        public void ExecuteSearchRecursively(DirectoryEntry root, string filter, ADSearchConditions condition, ADSearchRecursivelyDelegate callback, object oParams, params string[] properties)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(properties != null, "properties");

            if (Array.Exists(properties, data => data == "objectClass") == false)
                properties = AppendToStringArray(properties, new string[] { "objectClass" });

            InnerExecuteSearchRecursively(root, filter, condition, callback, oParams, null, properties);
        }

        public bool InnerExecuteSearchRecursively(DirectoryEntry root, string filter, ADSearchConditions condition, ADSearchRecursivelyDelegate callback, object oParams, object lastResult, string[] properties)
        {
            bool bContinue = true;

            ADSearchConditions newCondition = new ADSearchConditions(condition);

            newCondition.Scope = SearchScope.OneLevel;

            ADSearchResultParams asrp = new ADSearchResultParams(root);

            asrp.LastResult = lastResult;

            List<SearchResult> resultList = ExecuteSearch(root, filter, condition, properties);

            foreach (SearchResult sr in resultList)
            {
                if (bContinue)
                {
                    if (callback != null)
                    {
                        asrp.LastResult = callback(sr, asrp, oParams, ref bContinue);

                        if (bContinue == false)
                            break;
                    }

                    string objClass = sr.Properties["objectClass"][1].ToString();

                    if (string.Compare(objClass, "organizationalUnit", true) == 0)
                    {
                        bContinue = InnerExecuteSearchRecursively(sr.GetDirectoryEntry(), filter, condition, callback, oParams, asrp.LastResult, properties);
                    }
                }
            }

            return bContinue;
        }

        #endregion

        /// <summary>
        /// 登录名称是否合法
        /// </summary>
        /// <param name="logonName">登录名称</param>
        /// <returns></returns>
        public bool IsLogOnNameValid(string logonName)
        {
            LogOnIdentity identity = new LogOnIdentity(logonName);

            ExceptionHelper.FalseThrow(string.Compare(identity.Domain, DomainShortName, true) == 0,
                string.Format("帐号“{0}”的域名与当前域“{1}”不匹配", identity.LogOnName, DomainShortName));

            using (DirectoryEntry root = GetRootEntry())
            {
                return ExecuteSearch(root,
                                ADSearchConditions.GetFilterByMask(ADSchemaType.Users,
                                    new ExtraFilter(string.Empty,
                                                    string.Format("(samAccountName={0})", EscapeString(identity.LogOnNameWithoutDomain)),
                                                    string.Empty)),
                                new ADSearchConditions(SearchScope.Subtree), "samAccountName").Count > 0;
            }
        }

        [Obsolete("此方法不支持转义，使用有风险,考虑使用GetParentRdnSequence方法")]
        public string GetParentDN(string strDN)
        {
            string[] arrParts = strDN.Split(',');

            StringBuilder strB = new StringBuilder(256);
            bool bGotOU = false;

            for (int i = 1; i < arrParts.Length; i++)
            {
                if (bGotOU == false)
                {
                    string[] strKeyValue = arrParts[i].Split('=');

                    if (strKeyValue[0] == "OU" || strKeyValue[0] == "DC")
                        bGotOU = true;
                }

                if (bGotOU)
                {
                    if (strB.Length > 0)
                        strB.Append(',');
                    strB.Append(arrParts[i]);
                }
            }

            return strB.ToString();
        }

        /// <summary>
        /// 提取当前RDN序列的上一级的RDN序列
        /// </summary>
        /// <param name="rdnSequence">当前级的RDN序列</param>
        /// <returns></returns>
        public static string GetParentRdnSequence(string rdnSequence)
        {
            var ie = new RdnSequencePartEnumerator(rdnSequence).GetEnumerator();
            if (ie.MoveNext())
            {
                int nextIndex = ie.Current.Length + 1;
                if (nextIndex < rdnSequence.Length)
                {
                    return rdnSequence.Substring(nextIndex);
                }
                else
                    return string.Empty;
            }
            else
            {
                throw new FormatException("无法提取上级路径");
            }
        }

        #region 对象或查询结果输出
        public void OutputEntryProperties(DirectoryEntry entry, TraceListener listener)
        {
            foreach (string strName in entry.Properties.PropertyNames)
            {
                listener.WriteLine(string.Format("Name: {0},\tValue: {1}", strName, entry.Properties[strName].Value.ToString()));
            }
        }

        public string GetEntryPropertiesDescription(DirectoryEntry entry)
        {
            StringBuilder strB = new StringBuilder(1024);
            StringWriter sw = new StringWriter(strB);

            TextWriterTraceListener listener = new TextWriterTraceListener(sw);

            OutputEntryProperties(entry, listener);

            return strB.ToString();
        }

        public void OutputSearchResult(SearchResultCollection result, TraceListener listener)
        {
            int n = 0;

            foreach (SearchResult sr in result)
            {
                listener.WriteLine(string.Format("第{0}行结果开始", n));

                listener.IndentLevel++;

                foreach (string propName in sr.Properties.PropertyNames)
                {
                    string strValue = string.Empty;

                    for (int i = 0; i < sr.Properties[propName].Count; i++)
                    {
                        if (strValue != string.Empty)
                            strValue += ", ";

                        strValue += sr.Properties[propName][i].ToString();
                    }

                    listener.WriteLine(string.Format("Property Name: {0}\tProperty Value: {1}", propName, strValue));
                }

                listener.IndentLevel--;
                listener.WriteLine(string.Format("第{0}行结果结束", n));

                n++;
            }
        }

        public string GetSearchResultDescription(SearchResultCollection result)
        {
            StringBuilder strB = new StringBuilder(1024);
            StringWriter sw = new StringWriter(strB);

            TextWriterTraceListener listener = new TextWriterTraceListener(sw);

            OutputSearchResult(result, listener);

            return strB.ToString();
        }

        public string AppendNamingContext(string strDN)
        {
            string upperCaseDN = strDN.ToUpper();

            if (upperCaseDN.IndexOf("DC=") == -1)
            {
                if (strDN != string.Empty)
                    strDN = strDN + "," + GetNamingContext();
                else
                    strDN = GetNamingContext();
            }

            return strDN;
        }

        /// <summary>
        /// 删除DN中的DC部分
        /// </summary>
        /// <param name="strDN"></param>
        /// <returns></returns>
        public string RemoveDCPart(string strDN)
        {
            string result = string.Empty;

            int start = strDN.IndexOf("DC=");

            if (start > 0)
            {
                result = strDN.Substring(0, start - 1);
            }
            else
                if (start < 0)
                    result = strDN;

            return result;
        }

        /// <summary>
        /// 从DN分析出对象的深度。从零开始
        /// </summary>
        /// <param name="strDN"></param>
        /// <returns></returns>

        public int GetLevelFromDN(string strDN)
        {
            string dnShort = RemoveDCPart(strDN);

            return new RdnSequencePartEnumerator(dnShort).Count();
        }

        [Obsolete("此方法不识别名称中的逗号，如果要提取不包含转义符的名称，请换用GetFirstRdnValue")]
        public string GetNameFromDN(string strDN)
        {
            string strResult = string.Empty;

            int nFirstSplit = strDN.IndexOf(",");

            if (nFirstSplit != -1)
            {
                string strFirstPart = strDN.Substring(0, nFirstSplit);

                string[] arrParts = strFirstPart.Split('=');

                strResult = arrParts[1];
            }

            return strResult;
        }

        /// <summary>
        /// 获取第一个RDN的字符串值
        /// </summary>
        /// <param name="rdnSequence"></param>
        /// <returns>第一个RDN的字符串值</returns>
        /// <remarks>例如，对于OU=天津上华,OU=机构人员，返回天津上华</remarks>
        public string GetFirstRdnValue(string rdnSequence)
        {
            return RdnAttribute.Parse(new RdnSequencePartEnumerator(rdnSequence).First()).StringValue;
        }

        public bool IsBelongTo(string subDN, string parentDN, bool directSubObj)
        {
            string pDN = RemoveDCPart(parentDN);
            string sDN = RemoveDCPart(subDN);

            bool belongTo = sDN.Length > pDN.Length && string.CompareOrdinal(pDN, 0, sDN, sDN.Length - pDN.Length, pDN.Length) == 0; // a right manner

            if (belongTo && directSubObj)
                belongTo = GetLevelFromDN(sDN) == GetLevelFromDN(pDN) + 1;

            return belongTo;
        }

        #region GetSearchResultPropertyValue

        public string GetSearchResultPropertyStrValue(string propName, SearchResult sr)
        {
            return GetSearchResultPropertyStrValue(propName, sr, 0);
        }

        public string GetSearchResultPropertyStrValue(string propName, SearchResult sr, int valueIndex)
        {
            string result = string.Empty;

            result = (string)GetSearchResultPropertyValue(propName, sr, valueIndex, string.Empty);

            return result;
        }

        public object GetSearchResultPropertyValue(string propName, SearchResult sr, object oDefault)
        {
            return GetSearchResultPropertyValue(propName, sr, 0, oDefault);
        }

        public string GetSearchResultPropertyStrValues(string propName, SearchResult sr)
        {
            return GetSearchResultPropertyStrValues(propName, sr, string.Empty);
        }

        public string GetSearchResultPropertyStrValues(string propName, SearchResult sr, object oDefault)
        {
            StringBuilder strB = new StringBuilder();

            ResultPropertyValueCollection valueCollection = sr.Properties[propName];

            if (valueCollection != null)
            {
                foreach (object o in valueCollection)
                {
                    object data = o;

                    if (data != null)
                    {
                        data = ChangeSinglePropertyValueType(o, oDefault);

                        if (strB.Length > 0)
                            strB.Append(";");

                        strB.Append(data.ToString());
                    }
                }
            }

            return strB.ToString();
        }

        public object GetSearchResultPropertyValue(string propName, SearchResult sr, int valueIndex, object oDefault)
        {
            try
            {
                object result = oDefault;

                ResultPropertyValueCollection valueCollection = sr.Properties[propName];

                if (valueCollection != null && valueCollection.Count > 0)
                {
                    object o = valueCollection[valueIndex];

                    result = ChangeSinglePropertyValueType(o, oDefault);
                }

                return result;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException(
                    string.Format("读取查询结果属性{0}错误: {1}", propName, ex.Message),
                    ex
                    );
            }
        }

        private static object ChangeSinglePropertyValueType(object o, object oDefault)
        {
            object result = o;

            if (o != null)
            {
                if (o is byte[])
                    result = AttributeHelper.Hex((byte[])o);
                else
                    if (o is Int64 && oDefault is DateTime)
                    {
                        try
                        {
                            if ((Int64)o == 0)
                                result = DateTime.MinValue;
                            else
                            {
                                DateTime dt = System.DateTime.FromFileTime((Int64)o);
                                // 忽略掉毫秒
                                result = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
                            }
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                        }
                    }
                    else
                    {
                        try
                        {
                            result = Convert.ChangeType(o, oDefault.GetType());
                        }
                        catch (System.Exception)
                        {
                            result = oDefault;
                        }
                    }
            }

            return result;
        }
        #endregion

        #region GetPropertyStrValue
        public string GetPropertyStrValue(string propName, DirectoryEntry entry)
        {
            return GetPropertyStrValue(propName, entry, 0);
        }

        public string GetPropertyStrValue(string propName, DirectoryEntry entry, int valueIndex)
        {
            try
            {
                string result = string.Empty;
                PropertyValueCollection valueCollection = entry.Properties[propName];

                if (valueCollection != null && valueCollection.Count > 0)
                {
                    object o = valueCollection[valueIndex];

                    if (o != null)
                    {
                        if (o is byte[])
                            result = AttributeHelper.Hex((byte[])o);
                        else
                            result = o.ToString();
                    }
                }

                return result;
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException(
                        string.Format("读取对象属性{0}错误: {1}", propName, ex.Message),
                        ex
                    );
            }
        }
        #endregion

        #endregion

        private DirectoryEntry InnerNewEntry(string strDN)
        {
            DirectoryEntry entry = new DirectoryEntry(GetPath(strDN));

            ServerInfo si = GetServer();

            //entry.AuthenticationType = AuthenticationTypes.Secure;
            if (si.Identity != null)
            {
                entry.Username = si.Identity.LogOnNameWithDomain;
                entry.Password = si.Identity.Password;
            }

            return entry;
        }

        private ServerInfo GetServer()
        {
            if (_Server == null)
                _Server = new ServerInfo();

            return _Server;
        }

        private string GetPath(string strDN)
        {
            string path = string.Empty;

            ServerInfo si = GetServer();

            if (string.IsNullOrEmpty(si.ServerName) == false)
                path = "LDAP://" + si.ServerName + "/" + strDN;
            else
                path = "LDAP://" + strDN;

            return path;
        }

        /// <summary>
        /// 旧的实现用在LDAP查询中，几乎不可靠
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceSpecialCharacter(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < specialCharacters.Length; i++)
                    str = str.Replace(specialCharacters[i], replacedCharacters[i]);
            }

            return str;
        }

        /// <summary>
        /// 进行字符转义，转义后的字符用于属性的字符串值，但不适合用于LDAP查询
        /// </summary>
        /// <param stringValue="name"></param>
        /// <returns></returns>
        public static string EscapeString(string name)
        {
            return Regex.Replace(name, @"([\#\+\=\\;\""\,\<\>])", @"\$1");
        }

        /// <summary>
        /// 进行转义，转义后的字符用于属性的字符串值，但不适合LDAP查询。
        /// </summary>
        /// <param stringValue="name"></param>
        /// <returns></returns>
        /// <remarks>仅适用于标量值的基元类型</remarks>
        public static string EscapeValue(object value)
        {
            string result;
            if (value is string)
                result = EscapeString((string)value);
            else if (value is byte[])
                result = EscapeBytes((byte[])value);
            else if (value is char)
                result = EscapeValue(value.ToString());
            else if (value.GetType().IsPrimitive)
                result = value.ToString();
            else
                throw new NotSupportedException("提供的对象必须是基元类型或二进制数组");
            return result;
        }

        /// <summary>
        /// 进行转义，转义后的字符，用于精确的LDAP查询。
        /// </summary>
        /// <param name="value"></param>
        /// <returns>
        /// <list type="number">
        /// <item>NULL→NUL</item>
        /// <item>字符串</item>
        /// </list></returns>
        public static string EscapeValueForLdapQuery(object value)
        {
            string result;
            if (value == null)
                result = "NUL";
            if (value is string)
                result = ReplaceSpecialCharacter((string)value);
            else
                result = EscapeValue(value);

            return result;
        }

        /// <summary>
        /// 进行二进制转义，转义后的字符用于属性的二进制值
        /// </summary>
        /// <param stringValue="name"></param>
        /// <returns>转义后的序列</returns>
        /// <remarks>转义序列为转义符后跟字节的十六进制值，例如0x8392转义为\83\92</remarks>
        public static string EscapeBytes(byte[] bytes)
        {
            char[] buffer = new char[bytes.Length * 3];
            int baseIndex = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                buffer[baseIndex++] = '\\';
                buffer[baseIndex++] = AttributeHelper.Hex(bytes[i], true);
                buffer[baseIndex++] = AttributeHelper.Hex(bytes[i], false);
            }

            return new string(buffer);
        }

        private string InnerGetComputerObjectName(EXTENDED_NAME_FORMAT nameFormat)
        {
            StringBuilder strB = new StringBuilder(1024);

            int nSize = strB.Capacity;

            GetComputerObjectName(nameFormat, strB, ref nSize);

            return strB.ToString();
        }

        private string InnerGetComputerName(COMPUTER_NAME_FORMAT nameType)
        {
            StringBuilder strB = new StringBuilder(1024);

            int nSize = strB.Capacity;

            GetComputerNameEx(nameType, strB, ref nSize);

            return strB.ToString();
        }

        private string[] AppendToStringArray(string[] dataDest, string[] data)
        {
            string[] result = new string[dataDest.Length + data.Length];

            dataDest.CopyTo(result, 0);
            data.CopyTo(result, dataDest.Length);

            return result;
        }

        #region UserAccountPolicy相关

        public UserAccountPolicy GetUserAccountPolicy(SearchResult sr)
        {
            UserAccountPolicy policy = new UserAccountPolicy();

            ResultPropertyValueCollection values = sr.Properties["userAccountControl"];

            if (values != null && values.Count > 0)
            {
                object data = values[0];

                if (data != null)
                {
                    int userAccountControl = (int)data;

                    policy.UserAccountDisabled = this.GetUserAccountDisabled(userAccountControl);
                    policy.UserPasswordNeverExpires = this.GetUserPasswordNeverExpires(userAccountControl);
                    policy.UserPwdLastSet = this.GetUserPwdLastSet(sr);
                    policy.UserAccountExpirationDate = this.GetUserAccountExpirationDate(sr);
                }
            }

            return policy;
        }

        /// <summary>
        /// 从SearchResult中拿到的pwdLastSet属性类型为：long
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        private bool GetUserPwdLastSet(SearchResult sr)
        {
            bool pwdLastSet = false;

            if (IsUserSearchResult(sr))
            {
                object data = this.GetSearchResultPropertyValue("pwdLastSet", sr, 0, -1);

                if (data != null && data is long)
                {
                    if ((long)data == 0)
                        pwdLastSet = true;
                    else
                        pwdLastSet = false;
                }
                // else
                //    pwdLastSet = GetUserPwdLastSet(data as IADsLargeInteger);
            }

            return pwdLastSet;
        }

        private DateTime GetUserAccountExpirationDate(SearchResult sr)
        {
            DateTime dtResult = DateTime.MinValue;

            if (IsUserSearchResult(sr))
                dtResult = (DateTime)this.GetSearchResultPropertyValue("accountExpires", sr, 0, DateTime.MinValue);

            if (IsDefaultExpiredDate(dtResult))
                dtResult = DateTime.MinValue;

            return dtResult;
        }

        public DateTime GetUserAccountExpirationDate(object accountExpires)
        {
            DateTime dtResult = DateTime.MinValue;
            dtResult = (DateTime)accountExpires;

            if (IsDefaultExpiredDate(dtResult))
                dtResult = DateTime.MinValue;

            return dtResult;
        }

        /// <summary>
        /// ADAdmin中判断PasswordNeverExpires属性的逻辑
        /// </summary>
        /// <param name="userAccountControl"></param>
        /// <returns></returns>
        private bool GetUserPasswordNeverExpires(int userAccountControl)
        {
            if ((userAccountControl & (int)ADS_USER_FLAG.ADS_UF_DONT_EXPIRE_PASSWD) != 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// ADAdmin中判断AccountDisabled属性的逻辑
        /// </summary>
        /// <param name="userAccountControl"></param>
        /// <returns></returns>
        public bool GetUserAccountDisabled(int userAccountControl)
        {
            if ((userAccountControl & (int)ADS_USER_FLAG.ADS_UF_ACCOUNTDISABLE) != 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// ADAdmin中判断PasswordNotRequired属性的逻辑
        /// </summary>
        /// <param name="userAccountControl"></param>
        /// <returns></returns>
        public bool GetUserPasswordNotRequired(int userAccountControl)
        {
            if ((userAccountControl & (int)ADS_USER_FLAG.ADS_UF_PASSWD_NOTREQD) != 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// ADAdmin中判断DontExpirePassword属性的逻辑
        /// </summary>
        /// <param name="userAccountControl"></param>
        /// <returns></returns>
        public bool GetUserDontExpirePassword(int userAccountControl)
        {
            if ((userAccountControl & (int)ADS_USER_FLAG.ADS_UF_DONT_EXPIRE_PASSWD) != 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// ADAdmin中缺省的过期时间是new DateTime(1970, 1, 1)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool IsDefaultExpiredDate(DateTime dt)
        {
            DateTime defaultExpiredDate = new DateTime(1970, 1, 1);

            if (dt == defaultExpiredDate)
                return true;
            else
                return false;
        }

        private bool IsUserSearchResult(SearchResult sr)
        {
            bool result = false;

            if (sr != null)
            {
                if (this.GetSearchResultPropertyStrValue("objectClass", sr, 1).ToLower() == "person")
                    result = true;
            }

            return result;
        }

        #endregion
    }
}
