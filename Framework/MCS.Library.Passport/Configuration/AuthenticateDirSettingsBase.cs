using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 待认证的目录设置的基类
	/// </summary>
	public abstract class AuthenticateDirSettingsBase : ConfigurationSection
	{
		private AuthenticateDirElementCollection authenticateDirs = null;
		private AnonymousDirElementCollection anonymousDirs = null;

		/// <summary>
		/// 同步对象
		/// </summary>
		protected object syncRoot = new object();

		/// <summary>
		/// 缺省是否是需要验证的
		/// </summary>
		[ConfigurationProperty("defaultAnonymous", DefaultValue = false)]
		public bool DefaultAnonymous
		{
			get
			{
				return (bool)this["defaultAnonymous"];
			}
		}

		/// <summary>
		/// 需要认证的目录定义
		/// </summary>
		[ConfigurationProperty("authenticateDirs")]
		public AuthenticateDirElementCollection AuthenticateDirs
		{
			get
			{
				lock (this.syncRoot)
				{
					if (this.authenticateDirs == null)
					{
						this.authenticateDirs = (AuthenticateDirElementCollection)this["authenticateDirs"];

						if (this.authenticateDirs == null)
							this.authenticateDirs = new AuthenticateDirElementCollection();
					}

					return this.authenticateDirs;
				}
			}
		}

		/// <summary>
		/// 匿名访问的目录定义
		/// </summary>
		[ConfigurationProperty("anonymousDirs")]
		public AnonymousDirElementCollection AnonymousDirs
		{
			get
			{
				lock (this.syncRoot)
				{
					if (this.anonymousDirs == null)
					{
						this.anonymousDirs = (AnonymousDirElementCollection)this["anonymousDirs"];
						if (this.anonymousDirs == null)
							this.anonymousDirs = new AnonymousDirElementCollection();
					}

					return this.anonymousDirs;
				}
			}
		}

		/// <summary>
		/// 缺省需要认证的后缀名
		/// </summary>
		[ConfigurationProperty("defualtAuthenticateExts", IsRequired = false, DefaultValue = ".aspx;.ashx;.asmx")]
		public string DefualtAuthenticateExts
		{
			get
			{
				return (string)this["defualtAuthenticateExts"];
			}
		}

		/// <summary>
		/// 页面是否需要认证
		/// </summary>
		/// <param name="appRelativePath">应用路径下的相对路径</param>
		/// <returns>是否需要认证</returns>
		public bool PageNeedAuthenticate(string appRelativePath)
		{
			bool result = false;

			if (DefaultAnonymous)
				result = this.AuthenticateDirs.GetMatchedElement<AuthenticateDirElement>(appRelativePath) != null;
			else
				result = this.AnonymousDirs.GetMatchedElement<AnonymousDirElement>(appRelativePath) == null;

			return result;
		}

		/// <summary>
		/// 当前页面是否需要认证
		/// </summary>
		/// <returns>当前页面是否需要认证</returns>
		public bool PageNeedAuthenticate()
		{
			bool result = NeedAuthenticateByExt();

			if (result)
			{
				if (DefaultAnonymous)
					result = this.AuthenticateDirs.GetMatchedElement<AuthenticateDirElement>() != null;
				else
					result = this.AnonymousDirs.GetMatchedElement<AnonymousDirElement>() == null;
			}

			return result;
		}

		private bool NeedAuthenticateByExt()
		{
			string[] exts = this.DefualtAuthenticateExts.Split(',', ';');

			string url = HttpContext.Current.Request.Url.GetComponents(
							UriComponents.SchemeAndServer | UriComponents.Path,
							UriFormat.SafeUnescaped);

			string ext = Path.GetExtension(url);

			bool result = true;

			if (string.IsNullOrEmpty(ext) == false)
			{
				string matched = Array.Find(exts, delegate(string data) { return string.Compare(data, ext, true) == 0; });
				result = (matched != null);
			}

			return result;
		}
	}
}
