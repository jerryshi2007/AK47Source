#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	AuthenticateDirSettings.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
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
	/// 在Web应用中，哪些目录需要认证的配置节
	/// </summary>
	public sealed class AuthenticateDirSettings : AuthenticateDirSettingsBase
	{
		private AuthorizationDirElementCollection authorizationDirs = null;

		/// <summary>
		/// 获取配置认证目录信息
		/// </summary>
		/// <returns>认证目录配置</returns>
		/// <remarks>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="AuthenticateDirConfigTest" lang="cs" title="获取认证目录配置" />
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="AnonymousDirConfigTest" lang="cs" title="获取匿名目录配置" />
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Passport.Test\ConfigurationTest.cs" region="PageNeedAuthenticateTest" lang="cs" title="判断页面是否需要认证" />
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
		/// 需要进行授权和角色控制的Urls
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

		/// <summary>
		/// Principal创建者的定义，如果没有定义，使用缺省的Builder
		/// </summary>
		/// <remarks>
		/// <![CDATA[
		/// <typeFactories>
		/// <add name="pricipalBuilder" type="MCS.Library.Principal.DefaultPrincipalBuilder, DeluxeWorks.Library.Passport" />
		/// </typeFactories>
		/// ]]>
		/// </remarks>
		public IPrincipalBuilder PrincipalBuilder
		{
			get
			{
				IPrincipalBuilder result = null;

				if (TypeFactories.ContainsKey("pricipalBuilder"))
					result = (IPrincipalBuilder)TypeFactories["pricipalBuilder"].CreateInstance();
				else
					result = new DefaultPrincipalBuilder();

				return result;
			}
		}

		/// <summary>
		/// 用户身份扮演信息的加载器，如果没有定义，则不使用身份扮演
		/// </summary>
		/// <remarks>
		/// <![CDATA[
		/// <typeFactories>
		/// <add name="impersonatingInfoLoader" type="typeinfo..., assembly info..." />
		/// </typeFactories>
		/// ]]>
		/// </remarks>
		public IUserImpersonatingInfoLoader ImpersonatingInfoLoader
		{
			get
			{
				IUserImpersonatingInfoLoader loader = null;

				if (TypeFactories.ContainsKey("impersonatingInfoLoader"))
					loader = (IUserImpersonatingInfoLoader)TypeFactories["impersonatingInfoLoader"].CreateInstance();

				return loader;
			}
		}

		[ConfigurationProperty("typeFactories", IsRequired = false)]
		private TypeConfigurationCollection TypeFactories
		{
			get
			{
				return (TypeConfigurationCollection)this["typeFactories"];
			}
		}
	}

	/// <summary>
	/// 需要认证或匿名访问目录的配置项集合
	/// </summary>
	public abstract class AuthenticateDirElementCollectionBase : ConfigurationElementCollection
	{
		/// <summary>
		/// 使用当前的Web Request的路径进行匹配的结果
		/// </summary>
		/// <typeparam name="T">期望的类型。</typeparam>
		/// <returns>匹配结果。</returns>
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
		/// 路径匹配结果
		/// </summary>
		/// <typeparam name="T">期望的类型。</typeparam>
		/// <param name="url">应用路径下的相对路径</param>
		/// <returns>匹配结果。</returns>
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
		/// 获取配置节点的键值。
		/// </summary>
		/// <param name="element">配置节点</param>
		/// <returns>配置节点的键值。</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((AuthenticateDirElementBase)element).Location;
		}
	}

	/// <summary>
	/// 需要认证目录的配置项集合
	/// </summary>
	public class AuthenticateDirElementCollection : AuthenticateDirElementCollectionBase
	{
		internal AuthenticateDirElementCollection()
		{
		}

		/// <summary>
		/// 创建新的配置节点。
		/// </summary>
		/// <returns>新的配置节点。</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new AuthenticateDirElement();
		}
	}

	/// <summary>
	/// 需要匿名访问目录的配置项集合
	/// </summary>
	public class AnonymousDirElementCollection : AuthenticateDirElementCollectionBase
	{
		internal AnonymousDirElementCollection()
		{
		}

		/// <summary>
		/// 创建新的配置节点。
		/// </summary>
		/// <returns>新的配置节点。</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new AnonymousDirElement();
		}
	}

	/// <summary>
	/// 认证或者匿名访问的目录的配置项基类
	/// </summary>
	public abstract class AuthenticateDirElementBase : ConfigurationElement
	{
		/// <summary>
		/// 路径。
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

			if (ps.Length == 1) // 没有*的模型
				return MatchWithInterrogation(data, ps[0]);

			var si = data.IndexOf(ps[0], 0);	// 从string头查找第一个串

			if (si != 0)
				return false; // 第一个串没找到或者不在string的头部

			si += ps[0].Length; // 找到了串后,按就近原则,移到未查询过的最左边

			int plast = ps.Length - 1; // 最后一串应单独处理,为了提高效率,将它从循环中取出
			int pi = 0; // 跳过之前处理过的第一串

			while (++pi < plast)
			{
				if (ps[pi] == "")
					continue; //连续的*号,可以忽略

				si = data.IndexOf(ps[pi], si);	// 继续下一串的查找

				if (-1 == si)
					return false; // 没有找到

				si += ps[pi].Length; // 就近原则
			}

			if (ps[plast] == "") // 模型尾部为*,说明所有有效字符串部分已全部匹配,string后面可以是任意字符
				return true;

			// 从尾部查询最后一串是否存在
			int last_index = data.LastIndexOf(ps[plast]);

			// 如果串存在,一定要在string的尾部, 并且不能越过已查询过部分
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
		/// 判断某个路径是否能匹配上BaseDirInfo中的带通配符的路径
		/// </summary>
		/// <param name="path">需要匹配的路径</param>
		/// <returns>是否匹配</returns>
		public bool IsWildcharMatched(string path)
		{
			//结尾加*为了保持兼容性
			string pattern = Location.ToLower() + "*";

			return MatchWithAsterisk(path.ToLower(), pattern);
		}

		/*
		/// <summary>
		/// 判断某个路径是否能匹配上BaseDirInfo中的带通配符的路径
		/// </summary>
		/// <param name="path">需要匹配的路径</param>
		/// <returns>是否匹配</returns>
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
	/// 需要授权的目录配置项集合
	/// </summary>
	public class AuthorizationDirElementCollection : AuthenticateDirElementCollectionBase
	{
		internal AuthorizationDirElementCollection()
		{
		}

		/// <summary>
		/// 创建新的配置节点。
		/// </summary>
		/// <returns>新的配置节点。</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new AuthorizationDirElement();
		}
	}

	/// <summary>
	/// 需要授权的目录配置项
	/// </summary>
	public class AuthorizationDirElement : AuthenticateDirElementBase
	{
		internal AuthorizationDirElement()
		{
		}

		/// <summary>
		/// RolesDefineConfig中的配置项的名称
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
		/// 标准的角色名称。App1:Role1,Role2;App2:Role3,Role4
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
		/// 当前用户是否在指定的角色中
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
	/// 每一个需要匿名访问的目录的配置项
	/// </summary>
	public class AnonymousDirElement : AuthenticateDirElementBase
	{
		internal AnonymousDirElement()
		{
		}
	}

	/// <summary>
	/// 每一个需要认证的目录的配置项
	/// </summary>
	public class AuthenticateDirElement : AuthenticateDirElementBase
	{
		internal AuthenticateDirElement()
		{
		}

		/// <summary>
		/// 如果发现用户访问该页面时没有认证，是否自动跳转到认证页面
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
