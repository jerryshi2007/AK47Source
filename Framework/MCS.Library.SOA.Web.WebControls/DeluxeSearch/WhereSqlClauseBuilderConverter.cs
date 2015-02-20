
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
	public class WhereSqlClauseBuilderConverter : JavaScriptConverter
	{
		/// <summary>
		/// 反序列化
		/// </summary>
		/// <param name="dictionary"></param>
		/// <param name="type"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder
			{
				LogicOperator =
					dictionary.GetValue("LogicOperator", LogicOperatorDefine.Or)
			};

			var sqlClauseItems = dictionary.GetValue("List", new ArrayList());

			foreach (var item in sqlClauseItems)
			{
				SqlClauseBuilderItemUW builderItem = JSONSerializerExecute.Deserialize<SqlClauseBuilderItemUW>(item);

				whereBuilder.Add(builderItem);
				//IDictionary<string, object> objects = (item as IDictionary<string, object>);

				//if (objects != null)
				//    whereBuilder.AppendItem(objects["DataField"].ToString(), objects["Data"].ToString());
			}

			return whereBuilder;
		}

		/// <summary>
		/// 序列化
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			WhereSqlClauseBuilder whereBuilder = (WhereSqlClauseBuilder)obj;
			Dictionary<string, object> result = new Dictionary<string, object>();

			result.AddNonDefaultValue("LogicOperator", whereBuilder.LogicOperator);
			var itemsList = new ArrayList();

			foreach (var item in whereBuilder)
				itemsList.Add(item);

			result["List"] = itemsList;

			return result;
		}

		/// <summary>
		/// 支持类型
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new[] { typeof(WhereSqlClauseBuilder) };
			}
		}
	}
}
