using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Web;

using MCS.Library.Globalization;
using System.Xml;
using MCS.Library.Caching;
using MCS.Library.Core;

namespace MCS.Web.Library
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class WebTranslator : ITranslator
    {
        #region ITranslator Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="sourceCulture"></param>
        /// <param name="sourceText"></param>
        /// <param name="targetCulture"></param>
        /// <param name="objParams"></param>
        /// <returns></returns>
        public string Translate(string category, CultureInfo sourceCulture, string sourceText, CultureInfo targetCulture, params object[] objParams)
        {
            string targetText = sourceText;

            if (targetCulture.Name != sourceCulture.Name && string.IsNullOrEmpty(category) == false)
            {
                DictionaryItemKey key = new DictionaryItemKey()
                {
                    SourceText = sourceText,
                    SourceCultureName = sourceCulture.Name,
                    TargetCultureName = targetCulture.Name,
                    Category = category
                };

                if (DictionaryCache.Instance.TryGetValue(key, out targetText) == false)
                {
                    string fileName = GetCultureFilePath(category, targetCulture) + ".xml";
                    string physicalFileName = string.Empty;

                    try
                    {
                        if (EnvironmentHelper.Mode != InstanceMode.Web)
                        {
                            return sourceText;
                        }

                        XmlDocument xmlDoc = WebXmlDocumentCache.GetXmlDocument(fileName);

                        XmlElement item = FindMatchedItem(sourceText, xmlDoc.DocumentElement.SelectNodes("Item"));

                        if (item != null)
                            targetText = item.GetAttribute("target");
                        else
                            targetText = sourceText;

                        physicalFileName = HttpContext.Current.Server.MapPath(fileName);
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        targetText = sourceText;
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        targetText = sourceText;
                    }

                    if (string.IsNullOrEmpty(physicalFileName) == false)
                    {
                        FileCacheDependency dependency = new FileCacheDependency(physicalFileName);

                        DictionaryCache.Instance.Add(key, targetText, dependency);
                    }
                    else
                    {
                        DictionaryCache.Instance.Add(key, targetText);
                    }
                }
            }

            return targetText;
        }

        #endregion

        private XmlElement FindMatchedItem(string sourceText, XmlNodeList nodes)
        {
            XmlElement result = null;

            foreach (XmlElement elem in nodes)
            {
                if (elem.GetAttribute("source") == sourceText)
                {
                    result = elem;
                    break;
                }
            }

            return result;
        }

        private static string GetCultureFilePath(string category, CultureInfo sourceCulture)
        {
            string rootPath = WebTranslatorConfigSettings.GetConfig().CultureFileRoot;

            rootPath = UriHelper.ResolveRelativeUri(rootPath);

            return Path.Combine(rootPath, sourceCulture.Name + "/" + category);
        }
    }
}
