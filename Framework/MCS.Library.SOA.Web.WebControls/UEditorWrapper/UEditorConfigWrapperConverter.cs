using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// UEditorConfigWrapper的序列化器，用于Config的对象序列化
	/// </summary>
	internal class UEditorConfigWrapperConverter : JavaScriptConverter
	{
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			UEditorConfigWrapper config = (UEditorConfigWrapper)obj;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			dictionary.AddNonDefaultValue("imagePath", config.ImagePath);
			dictionary.AddNonDefaultValue("compressSide", config.CompressSide);
			dictionary.AddNonDefaultValue("maxImageSideLength", config.MaxImageSideLength);
			dictionary.AddNonDefaultValue("relativePath", config.RelativePath);
			dictionary.AddNonDefaultValue("catcherUrl", config.CatcherUrl);

			dictionary.AddNonDefaultValue("UEDITOR_HOME_URL", config.UEDITOR_HOME_URL);
			dictionary.AddNonDefaultValue("toolbars", new object[] { config.Toolbars });
			dictionary.AddNonDefaultValue("initialContent", config.InitialContent);
			dictionary.AddNonDefaultValue("autoClearinitialContent", config.AutoClearInitialContent);

			dictionary.AddNonDefaultValue("pasteplain", config.PastePlain);
			dictionary.AddNonDefaultValue("textarea", config.TextArea);

			dictionary.AddNonDefaultValue("autoHeightEnabled", config.AutoHeightEnabled);
			dictionary.AddNonDefaultValue("elementPathEnabled", config.ElementPathEnabled);
			dictionary.AddNonDefaultValue("wordCount", config.WordCount);
			dictionary.AddNonDefaultValue("maximumWords", config.MaximumWords);

			dictionary.AddNonDefaultValue("highlightJsUrl", config.HighlightJsUrl);
			dictionary.AddNonDefaultValue("highlightCssUrl", config.HighlightCssUrl);

			return dictionary;
		}

		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			UEditorConfigWrapper config = new UEditorConfigWrapper();

			config.ImagePath = dictionary.GetValue("imagePath", (string)null);
			config.CompressSide = dictionary.GetValue("compressSide", 0);
			config.MaxImageSideLength = dictionary.GetValue("maxImageSideLength", 0);
			config.RelativePath = dictionary.GetValue("relativePath", true);
			config.CatcherUrl = dictionary.GetValue("catcherUrl", (string)null);

			config.UEDITOR_HOME_URL = dictionary.GetValue("UEDITOR_HOME_URL", (string)null);

			if (dictionary.ContainsKey("toolbars"))
			{
				object[] objs = JSONSerializerExecute.Deserialize<object[]>(dictionary["toolbars"]);

				if (objs.Length > 0)
				{
					object[] toolbarArray = (object[])objs[0];

					config.Toolbars = new string[toolbarArray.Length];

					for (int i = 0; i < toolbarArray.Length; i++)
						config.Toolbars[i] = (string)toolbarArray[i];
				}
			}

			config.InitialContent = dictionary.GetValue("initialContent", (string)null);
			config.AutoClearInitialContent = dictionary.GetValue("autoClearinitialContent", false);

			config.PastePlain = dictionary.GetValue("pasteplain", false);
			config.TextArea = dictionary.GetValue("textarea", (string)null);

			config.AutoHeightEnabled = dictionary.GetValue("autoHeightEnabled", false);
			config.ElementPathEnabled = dictionary.GetValue("elementPathEnabled", false);
			config.WordCount = dictionary.GetValue("wordCount", false);
			config.MaximumWords = dictionary.GetValue("maximumWords", 0);

			config.HighlightJsUrl = dictionary.GetValue("highlightJsUrl", (string)null);
			config.HighlightCssUrl = dictionary.GetValue("highlightCssUrl", (string)null);

			return config;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(UEditorConfigWrapper) };
			}
		}
	}

    internal class DocumentPropertyConverter : JavaScriptConverter
    {

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            DocumentProperty docProp = new DocumentProperty();
            docProp.Id = dictionary.GetValue("Id", (string)null);
            docProp.InitialData = dictionary.GetValue("InitialData", (string)null);
            if (dictionary.ContainsKey("DocumentImages"))
            {
                DocumentImageList docImages = JSONSerializerExecute.Deserialize<DocumentImageList>(dictionary["DocumentImages"]);
                docProp.DocumentImages = docImages;
            }

            //if (dictionary.ContainsKey("DocImageProps"))
            //{
            //    ImagePropertyCollection imageProps = JSONSerializerExecute.Deserialize<ImagePropertyCollection>(dictionary["DocImageProps"]);
            //    docProp.DocImageProps = imageProps;
            //}

            return docProp;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            DocumentProperty docProp = (DocumentProperty)obj;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            
            dictionary.AddNonDefaultValue("InitialData", docProp.InitialData);
            dictionary.AddNonDefaultValue("Id", docProp.Id);
            dictionary.AddNonDefaultValue("DocumentImages", new DocumentImageList());
            //dictionary.AddNonDefaultValue("DocImageProps", docProp.DocImageProps);
            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new Type[] { typeof(DocumentProperty) };
            }
        }
    }
}
