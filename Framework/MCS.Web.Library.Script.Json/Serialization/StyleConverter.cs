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
    /// ��Style�������JSON���л��ͷ����л���JavaScriptConverter
    /// </summary>
    /// <remarks>��Style�������JSON���л��ͷ����л���JavaScriptConverter</remarks>
    public class StyleConverter : System.Web.Script.Serialization.JavaScriptConverter
    {

        private readonly StyleCollectionConverter styleCollectionConverter = new StyleCollectionConverter();

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
            //Style style = new Style();

            //CssStyleCollection styles = style.GetStyleAttributes(new UrlResolutionService());

            //return styleCollectionConverter.Deserialize(dictionary, type, serializer);

            return styleCollectionConverter.Deserialize(dictionary, type, serializer);
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
            Style style = obj as Style;

            if (style != null && style.IsEmpty == false)
                return styleCollectionConverter.Serialize(style.GetStyleAttributes(new UrlResolutionService()), serializer);
            else
                return null;
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
                types.Add(typeof(Style));

                return types;
            }
        }
    }
}
