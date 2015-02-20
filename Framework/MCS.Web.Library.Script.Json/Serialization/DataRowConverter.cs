using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web.Script.Serialization;

namespace MCS.Web.Library.Script
{
	/// <summary>
	/// DataRow类型数据序列化转换器
	/// </summary>
	/// <remarks>DataRow类型数据序列化转换器</remarks>
    public class DataRowConverter : System.Web.Script.Serialization.JavaScriptConverter
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
            throw new ApplicationException("不知持反序列化");
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
        /// 获取此Converter支持的类别
        /// </summary>
		/// <remarks>获取此Converter支持的类别</remarks>
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
