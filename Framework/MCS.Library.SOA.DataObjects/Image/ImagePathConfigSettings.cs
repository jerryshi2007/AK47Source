using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MCS.Library.Configuration;
using MCS.Library.Core;
using System.IO;
using MCS.Library.SOA.DataObjects.Properties;

namespace MCS.Library.SOA.DataObjects
{
    public class ImagePathConfigSettings : ConfigurationSection
    {
        private ImagePathConfigSettings()
        { }
        
        private const string imagePathConfigName = "imagePathSettings";

        /// <summary>
        /// 获得路径配置信息
        /// </summary>
        /// <returns>路径配置信息</returns>
        public static ImagePathConfigSettings GetConfig()
        {
            ImagePathConfigSettings result =
                (ImagePathConfigSettings)ConfigurationBroker.GetSection(imagePathConfigName);
            ConfigurationExceptionHelper.CheckSectionNotNull(result, imagePathConfigName);
            return result;
        }

        		/// <summary>
		/// 路径集合
		/// </summary>
        [ConfigurationProperty(ImagePathSettingsCollection.Name)]
        public ImagePathSettingsCollection Paths
        {
            get
            {
                ImagePathSettingsCollection pathConfigCollection
                    = (ImagePathSettingsCollection)base[ImagePathSettingsCollection.Name];

                return pathConfigCollection;
            }
        }

        public bool IsInConfigedPath(string path)
        {
            bool result = false;

            string fullPath = Path.GetFullPath(path).ToLower();

            for (int i = 0; i < this.Paths.Count; i++)
            {
                string lowerConfigedPath = this.Paths[i].Dir.ToLower();

                if (fullPath.IndexOf(lowerConfigedPath) == 0)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public void CheckPathIsConfiged(string path)
        {
            ExceptionHelper.FalseThrow(IsInConfigedPath(path), string.Format(Resource.IllegalPath, path));
        }


    }

    /// <summary>
    /// 路径集合
    /// </summary>
    [ConfigurationCollection(typeof(ImagePathSettingsElement))]
    public sealed class ImagePathSettingsCollection : NamedConfigurationElementCollection<ImagePathSettingsElement>
    {
        //路径集合名称
        public const string Name = "paths";
    }

    public class ImagePathSettingsElement : NamedConfigurationElement
    {
        private string dir = null;
        		/// <summary>
		/// 路径
		/// </summary>
        [ConfigurationProperty("dir", IsRequired = true)]
        public string Dir
        {
            get
            {
                if (this.dir == null)
                {
                    this.dir = (string)this["dir"];

                    ExceptionHelper.CheckStringIsNullOrEmpty(this.dir, "dir");

                    if (this.dir.EndsWith("\\") == false)
                        this.dir += "\\";
                }

                return this.dir;

            }
        }
    }
}
