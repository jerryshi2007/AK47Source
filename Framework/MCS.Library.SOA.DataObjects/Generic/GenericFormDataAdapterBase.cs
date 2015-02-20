using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 通用表单数据适配器的基类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TCollection"></typeparam>
	public abstract class GenericFormDataAdapterBase<T, TCollection> : UpdatableAndLoadableAdapterBase<T, TCollection>
		where T : GenericFormData
		where TCollection : EditableDataObjectCollectionBase<T>, new()
	{
		public T Load(string id)
		{
			return Load(id, true);
		}

		/// <summary>
		/// 根据ID加载数据
		/// </summary>
		/// <param name="id"></param>
		/// <param name="throwError">如果不存在，是否抛出异常</param>
		/// <returns></returns>
		public T Load(string id, bool throwError)
		{
			id.CheckStringIsNullOrEmpty("id");

			TCollection collection = Load(builder => builder.AppendItem("RESOURCE_ID", id));

			T result = default(T);

			if (collection.Count > 0)
				result = collection[0];
			else
				ExceptionHelper.TrueThrow<FileNotFoundException>(throwError, "不能找到ID为{0}的WF.GENERIC_FORM_DATA数据", id);

			return result;
		}

		/// <summary>
		/// 根据ResourceID和ClassName加载相关的数据
		/// </summary>
		/// <typeparam name="TRelative"></typeparam>
		/// <typeparam name="TRelativeCollection"></typeparam>
		/// <param name="resourceID"></param>
		/// <param name="className"></param>
		/// <returns></returns>
		public TRelativeCollection LoadRelativeData<TRelative, TRelativeCollection>(
			string resourceID, string className)
			where TRelative : GenericFormRelativeData, new()
			where TRelativeCollection : EditableDataObjectCollectionBase<TRelative>, new()
		{
			resourceID.CheckStringIsNullOrEmpty("resourceID");
			className.CheckStringIsNullOrEmpty("className");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("RESOURCE_ID", resourceID).AppendItem("CLASS", className);

			string sql = string.Format("SELECT * FROM WF.GENERIC_FORM_RELATIVE_DATA WHERE {0} ORDER BY SORT_ID",
				builder.ToSqlString(TSqlBuilder.Instance));

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			TRelativeCollection result = new TRelativeCollection();

			ORMapping.DataViewToCollection(result, table.DefaultView);

			foreach (TRelative item in result)
			{
				XmlDocument xmlDoc = XmlHelper.CreateDomDocument(item.XmlContent);
				XmlHelper.DeserializeToObject(xmlDoc, item);
			}

			return result;
		}

		/// <summary>
		/// 更新相关的数据。先删除某个ResouceID和ClassName下相关的数据，然后再插入集合中的数据
		/// </summary>
		/// <typeparam name="TRelative"></typeparam>
		/// <param name="resourceID"></param>
		/// <param name="className"></param>
		/// <param name="dataCollection"></param>
		public virtual void UpdateRelativeData<TRelative>(string resourceID, string className, IEnumerable<TRelative> dataCollection) where TRelative : GenericFormRelativeData
		{
			resourceID.CheckStringIsNullOrEmpty("resourceID");
			className.CheckStringIsNullOrEmpty("className");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("RESOURCE_ID", resourceID).AppendItem("CLASS", className);

			StringBuilder strB = new StringBuilder();

			strB.AppendFormat("DELETE WF.GENERIC_FORM_RELATIVE_DATA WHERE {0}", builder.ToSqlString(TSqlBuilder.Instance));

			int i = 0;
			foreach (TRelative data in dataCollection)
			{
				data.ResourceID = resourceID;
				data.Class = className;

				strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
				InsertSqlClauseBuilder insertBuilder = ORMapping.GetInsertSqlClauseBuilder(data, "ResourceID", "Class", "SortID", "XmlContent");

				data.SortID = i++;

				insertBuilder.AppendItem("RESOURCE_ID", resourceID).AppendItem("CLASS", className).AppendItem("SORT_ID", data.SortID);

				XmlDocument xdoc = XmlHelper.SerializeObjectToXml(data);
				data.XmlContent = xdoc.OuterXml;

				insertBuilder.AppendItem("XML_CONTENT", xdoc.OuterXml);

				strB.AppendFormat("INSERT WF.GENERIC_FORM_RELATIVE_DATA{0}", insertBuilder.ToSqlString(TSqlBuilder.Instance));
			}

			DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName());
		}

		/// <summary>
		/// 替换已经存在的相关子对象数据
		/// </summary>
		/// <typeparam name="TRelative"></typeparam>
		/// <param name="dataCollection"></param>
		public virtual void ReplaceRelativeData<TRelative>(IEnumerable<TRelative> dataCollection) where TRelative : GenericFormRelativeData
		{
			dataCollection.NullCheck("dataCollection");

			StringBuilder strB = new StringBuilder();

			foreach (TRelative data in dataCollection)
			{
				if (strB.Length > 0)
					strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

				UpdateSqlClauseBuilder updateBuilder = ORMapping.GetUpdateSqlClauseBuilder(data, "ResourceID", "Class", "SortID", "XmlContent");

				updateBuilder.AppendItem("RESOURCE_ID", data.ResourceID).AppendItem("CLASS", data.Class).AppendItem("SORT_ID", data.SortID);

				XmlDocument xdoc = XmlHelper.SerializeObjectToXml(data);
				data.XmlContent = xdoc.OuterXml;

				updateBuilder.AppendItem("XML_CONTENT", xdoc.OuterXml);

				WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

				whereBuilder.AppendItem("RESOURCE_ID", data.ResourceID).AppendItem("CLASS", data.Class).AppendItem("SORT_ID", data.SortID);

				strB.AppendFormat("UPDATE WF.GENERIC_FORM_RELATIVE_DATA SET {0} WHERE {1}",
					updateBuilder.ToSqlString(TSqlBuilder.Instance), whereBuilder.ToSqlString(TSqlBuilder.Instance));
			}

			if (strB.Length > 0)
				DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName());
		}

		protected override void AfterLoad(TCollection data)
		{
			foreach (T item in data)
			{
				XmlDocument xmlDoc = XmlHelper.CreateDomDocument(item.XmlContent);
				XmlHelper.DeserializeToObject(xmlDoc, item);
			}
		}

		protected override void BeforeInnerUpdate(T data, Dictionary<string, object> context)
		{
			XmlDocument xdoc = XmlHelper.SerializeObjectToXml(data);
			data.XmlContent = xdoc.OuterXml;
		}

		protected override string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}
	}
}
