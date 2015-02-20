using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Web.Library.Script
{
    class UrlResolutionService : IUrlResolutionService
    {
        public string ResolveClientUrl(string relativeUrl)
        {
            return relativeUrl;
        }
    }
    /// <summary>
    /// 将Style对象进行JSON序列化和反序列化的JavaScriptConverter
    /// </summary>
    /// <remarks>将Style对象进行JSON序列化和反序列化的JavaScriptConverter</remarks>
    public class StyleConverter : System.Web.Script.Serialization.JavaScriptConverter
    {

        private readonly StyleCollectionConverter styleCollectionConverter = new StyleCollectionConverter();

        /// <summary>
        /// 将dictionary反序列化转换成特定类型的对象
        /// </summary>
        /// <param name="dictionary">要反序列化的数据</param>
        /// <param name="type">要反序列化的类型</param>
        /// <param name="serializer">JavaScriptSerializer对象</param>
        /// <returns>反序列化出的对象</returns>
        /// <remarks>将dictionary反序列化转换成特定类型的对象</remarks>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
        {
            //Style style = new Style();

            //CssStyleCollection styles = style.GetStyleAttributes(new UrlResolutionService());

            //return styleCollectionConverter.Deserialize(dictionary, type, serializer);

            return styleCollectionConverter.Deserialize(dictionary, type, serializer);
        }

        /// <summary>
        /// 将对象序列化成dictionary类型数据
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="serializer">JavaScriptSerializer对象</param>
        /// <returns>序列化出的dictionary类型数据</returns>
        /// <remarks>将对象序列化成dictionary类型数据</remarks>
        public override IDictionary<string, object> Serialize(object obj, System.Web.Script.Serialization.JavaScriptSerializer serializer)
        {
            Style style = obj as Style;

            if (style != null && style.IsEmpty == false)
                return styleCollectionConverter.Serialize(style.GetStyleAttributes(new UrlResolutionService()), serializer);
            else
                return null;
        }

        /// <summary>
        /// 获取此Converter支持的类别
        /// </summary>
        /// <remarks>获取此Converter支持的类别</remarks>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(Style));

                return types;
            }
        }
    }
}
