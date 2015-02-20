using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace MCS.Web.Library.Script
{
    /// <summary>
    /// 将StyleCollection对象进行JSON序列化和反序列化的JavaScriptConverter
    /// </summary>
	/// <remarks>将StyleCollection对象进行JSON序列化和反序列化的JavaScriptConverter</remarks>
    public class StyleCollectionConverter : System.Web.Script.Serialization.JavaScriptConverter
    {
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
            CssStyleCollection styles = (new AttributeCollection(null)).CssStyle;

            foreach (KeyValuePair<string, object> item in dictionary)
            {
                styles.Add(CastKey(item.Key), (string)item.Value);
            }

            return styles;
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
            CssStyleCollection style = obj as CssStyleCollection;
            if (style != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (string key in style.Keys)
                {
                    result.Add(CastKey(key), style[key]);
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// 把html的style格式转换成javascript的style格式
        /// 如：background-color转换成backgroundColor
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		/// <remarks>把html的style格式转换成javascript的style格式
		/// 如：background-color转换成backgroundColor</remarks>
        private string CastKey(string key)
        {
            if (  key.IndexOf("-")  == -1  ) 
                return   key ;

            string[] strs = key.Split('-');

            return strs[0] + strs[1].Substring(0, 1).ToUpper() + strs[1].Substring(1);
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
                types.Add(typeof(CssStyleCollection));

                return types;
            }
        }
    }
}
