using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web.Script.Serialization;

namespace MCS.Web.Library.Script
{
	/// <summary>
	/// DataRow�����������л�ת����
	/// </summary>
	/// <remarks>DataRow�����������л�ת����</remarks>
    public class DataRowConverter : System.Web.Script.Serialization.JavaScriptConverter
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
            throw new ApplicationException("��֪�ַ����л�");
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
            Dictionary<string, object> result = new Dictionary<string, object>();

            DataRowView rowView = obj as DataRowView;
            DataRow row = rowView != null ? rowView.Row : obj as DataRow;
            if (row != null) 
            {
                foreach (DataColumn col in row.Table.Columns)
                {
                    result.Add(col.ColumnName, row[col.ColumnName]);
                }
            }

            return result;
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
                types.Add(typeof(DataRow));
                types.Add(typeof(DataRowView));

                return types;
            }
        }
    }
}
