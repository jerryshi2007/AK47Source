using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Web.Library.Resources;

namespace MCS.Web.Library
{
	/// <summary>
	/// WebӦ�ã������Ĵ򿪷�ʽ
	/// </summary>
	public enum WebFileOpenMode
	{
		/// <summary>
		/// ������ʽ��
		/// </summary>
		[EnumItemDescription("������ʽ��", "attachment", 0)]
		Attachment,

		/// <summary>
		/// ��Ƕ��ʽ��
		/// </summary>
		[EnumItemDescription("��Ƕ��ʽ��", "inline", 1)]
		Inline
	}

	/// <summary>
	/// �ĵ�����������Ϣ��ʵ����
	/// </summary>
	public sealed class ContentTypesSection : DeluxeConfigurationSection
	{
		/// <summary>
		/// �õ��ĵ����͵�����
		/// </summary>
		/// <returns>�ĵ����͵�������Ϣ</returns>
		public static ContentTypesSection GetConfig()
		{
			ContentTypesSection settings = (ContentTypesSection)ConfigurationBroker.GetSection("contentTypes");

			if (settings == null)
				settings = new ContentTypesSection();

			return settings;
		}

		/// <summary>
		/// �ĵ�������
		/// </summary>
		[ConfigurationProperty(null, IsDefaultCollection = true)]
		public ContentTypeConfigElementCollection ContentTypes
		{
			get
			{
				return (ContentTypeConfigElementCollection)this[(string)null];
			}
		}

		/// <summary>
		/// �õ�ȱʡ��������
		/// </summary>
		public ContentTypeConfigElement DefaultElement
		{
			get
			{
				ContentTypeConfigElement result = null;

				if (string.IsNullOrEmpty(DefaultKey) == false)
					result = this.ContentTypes[DefaultKey];
				else
					if (this.ContentTypes.Count > 0)
						result = this.ContentTypes[0];

				return result;
			}
		}

		[ConfigurationProperty("default", IsRequired = false, DefaultValue = "")]
		private string DefaultKey
		{
			get
			{
				return (string)base["default"];
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class ContentTypeConfigElementCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// �����µ�����Ԫ��
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ContentTypeConfigElement();
		}

		/// <summary>
		/// �õ�Ԫ�صļ�ֵ
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ContentTypeConfigElement)element).Key;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ContentTypeConfigElement this[int index]
		{
			get
			{
				return (ContentTypeConfigElement)base.BaseGet(index);
			}
		}

		/// <summary>
		/// ����keyֵ�������ݵ�����Ԫ��
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public new ContentTypeConfigElement this[string key]
		{
			get
			{
				return (ContentTypeConfigElement)base.BaseGet(key);
			}
		}

		/// <summary>
		/// �����ļ����õ��ļ���������Ϣ
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public ContentTypeConfigElement FindElementByFileName(string fileName)
		{
			ContentTypeConfigElement result = null;

			string fileExt = Path.GetExtension(fileName);

			if (string.IsNullOrEmpty(fileExt) == false)
			{
				fileExt = fileExt.Trim('.').ToLower();//Modify 20071010, ��Сд����

				foreach (ContentTypeConfigElement element in this)
				{
					if (element.FileExtensionNames.Contains(fileExt))
					{
						result = element;
						break;
					}
				}
			}

			return result;
		}
	}

	/// <summary>
	/// �ĵ����͵�������
	/// </summary>
	public sealed class ContentTypeConfigElement : ConfigurationElement
	{
		private ReadOnlyCollection<string> fileExtensionNames = null;
		private string logoImage = null;

		/// <summary>
		/// �������key
		/// </summary>
		[ConfigurationProperty("key", IsKey = true, IsRequired = true)]
		public string Key
		{
			get
			{
				return (string)base["key"];
			}
		}

		/// <summary>
		/// �ĵ�������
		/// </summary>
		[ConfigurationProperty("contentType", IsRequired = true)]
		public string ContentType
		{
			get
			{
				return (string)base["contentType"];
			}
		}

		/// <summary>
		/// �ļ�����չ������
		/// </summary>
		public ReadOnlyCollection<string> FileExtensionNames
		{
			get
			{
				if (this.fileExtensionNames == null)
				{
					string[] exts = InnerFileExtensionNames.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

					for (int i = 0; i < exts.Length; i++)
						exts[i] = exts[i].Trim().ToLower();

					this.fileExtensionNames =
						new ReadOnlyCollection<string>(exts);
				}

				return this.fileExtensionNames;
			}
		}

		/// <summary>
		/// �õ��ĵ���Ӧ��Logo
		/// </summary>
		public string LogoImage
		{
			get
			{
				if (this.logoImage == null)
					this.logoImage = ParseLogoImagePath(InnerLogoImage);

				return this.logoImage;
			}
		}

		/// <summary>
		/// �����Ĵ򿪷�ʽ
		/// </summary>
		[ConfigurationProperty("openMode", DefaultValue = WebFileOpenMode.Attachment)]
		public WebFileOpenMode OpenMode
		{
			get
			{
				return (WebFileOpenMode)base["openMode"];
			}
		}

		[ConfigurationProperty("fileExtensionNames", DefaultValue = "")]
		private string InnerFileExtensionNames
		{
			get
			{
				return (string)base["fileExtensionNames"];
			}
		}

		[ConfigurationProperty("logoImage", DefaultValue = "builtIn://wordpad.gif")]
		private string InnerLogoImage
		{
			get
			{
				return (string)base["logoImage"];
			}
		}

		private string ParseLogoImagePath(string originalPath)
		{
			string result = originalPath;

			if (EnvironmentHelper.Mode == InstanceMode.Web)
			{
				if (originalPath.IndexOf("builtIn://", StringComparison.OrdinalIgnoreCase) == 0)
					result = GetBuiltInResource(originalPath);
				else
					if (originalPath.IndexOf("res://", StringComparison.OrdinalIgnoreCase) == 0)
						result = GetResResource(originalPath);
					else
						result = GetRegularResource(originalPath);
			}

			return result;
		}

		private string GetRegularResource(string originalPath)
		{
			string result = originalPath;

			Uri url = new Uri(originalPath, UriKind.RelativeOrAbsolute);

			if (url.IsAbsoluteUri == false && string.IsNullOrEmpty(originalPath) == false)
			{
				if (originalPath[0] == '~')
				{
					HttpRequest request = HttpContext.Current.Request;
					string appPathAndQuery = request.ApplicationPath + originalPath.Substring(1);

					appPathAndQuery = appPathAndQuery.Replace("//", "/");

					result = request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) +
								appPathAndQuery;
				}
			}

			return result;
		}

		private string GetBuiltInResource(string originalPath)
		{
			string resPath = originalPath.Substring("builtIn://".Length);
			System.Type type = typeof(ImageContainer);

			return GetPageHandler().ClientScript.GetWebResourceUrl(type,
				type.Namespace + ".Images." + resPath);
		}

		private string GetResResource(string originalPath)
		{
			string resPath = originalPath.Substring("res://".Length);

			string[] parts = resPath.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			ExceptionHelper.FalseThrow(parts.Length >= 2, Resources.DeluxeWebResource.E_InvalidContentLogoImagePath, originalPath);

			System.Type type = TypeCreator.GetTypeInfo(parts[0].Trim());

			return GetPageHandler().ClientScript.GetWebResourceUrl(type, parts[1].Trim());
		}

		private Page GetPageHandler()
		{
			ExceptionHelper.FalseThrow(HttpContext.Current.CurrentHandler is Page, Resources.DeluxeWebResource.E_HttpHandlerMustBePageClass);

			return (Page)HttpContext.Current.CurrentHandler;
		}
	}
}
