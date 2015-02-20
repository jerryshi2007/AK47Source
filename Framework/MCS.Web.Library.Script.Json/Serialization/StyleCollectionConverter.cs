using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace MCS.Web.Library.Script
{
    /// <summary>
    /// ��StyleCollection�������JSON���л��ͷ����л���JavaScriptConverter
    /// </summary>
	/// <remarks>��StyleCollection�������JSON���л��ͷ����л���JavaScriptConverter</remarks>
    public class StyleCollectionConverter : System.Web.Script.Serialization.JavaScriptConverter
    {
        /// <summary>
        /// ��dictionary�����л�ת�����ض����͵Ķ���
        /// </summary>
        /// <param name="dictionary">Ҫ�����л�������</param>
        /// <param name="type">Ҫ�����л�������</param>
        /// <param name="serializer">JavaScriptSerializer����</param>
        /// <returns>�����л����Ķ���</returns>
		/// <remarks>��dictionary�����л�ת�����ض����͵Ķ���</remarks>
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
        /// ���������л���dictionary��������
        /// </summary>
        /// <param name="obj">Ҫ���л��Ķ���</param>
        /// <param name="serializer">JavaScriptSerializer����</param>
        /// <returns>���л�����dictionary��������</returns>
		/// <remarks>���������л���dictionary��������</remarks>
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
        /// ��html��style��ʽת����javascript��style��ʽ
        /// �磺background-colorת����backgroundColor
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
		/// <remarks>��html��style��ʽת����javascript��style��ʽ
		/// �磺background-colorת����backgroundColor</remarks>
        private string CastKey(string key)
        {
            if (  key.IndexOf("-")  == -1  ) 
                return   key ;

            string[] strs = key.Split('-');

            return strs[0] + strs[1].Substring(0, 1).ToUpper() + strs[1].Substring(1);
        }

        /// <summary>
        /// ��ȡ��Converter֧�ֵ����
        /// </summary>
		/// <remarks>��ȡ��Converter֧�ֵ����</remarks>
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
