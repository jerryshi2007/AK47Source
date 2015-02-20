using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Schemas.Adapters
{
	/// <summary>
	/// 带版本信息的Schema对象读取和更新适配器的基类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TCollection"></typeparam>
	public abstract class VersionedSchemaObjectLoadableAdapterBase<T, TCollection> : VersionedSchemaObjectAdapterBase<T>
		where T : VersionedSchemaObjectBase
		where TCollection : EditableDataObjectCollectionBase<T>, new()
	{
		///// <summary>
		///// 根据<see cref="IConnectiveSqlClause"/>指定的条件和时间点载入对象
		///// </summary>
		///// <param name="condition">表示条件的<see cref="IConnectiveSqlClause"/></param>
		///// <param name="timePoint">时间点 - 或 - <see cref="DateTime.MinValue"/>表示当前时间</param>
		///// <returns>一个<see cref="SchemaObjectCollection"/>，包含条件指定的对象。</returns>
		//public TCollection Load(IConnectiveSqlClause condition, DateTime timePoint)
		//{
		//    var timePointBuilder = VersionStrategyQuerySqlBuilder.Instance.TimePointToBuilder(timePoint);

		//    TCollection result = new TCollection();

		//    if (condition.IsEmpty == false)
		//    {
		//        ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(condition, timePointBuilder);

		//        using (DbContext context = DbContext.GetContext(this.GetConnectionName()))
		//        {
		//            VersionedObjectAdapterHelper.Instance.FillData(GetMappingInfo().TableName, connectiveBuilder, this.GetConnectionName(),
		//                reader =>
		//                {
		//                    result.LoadFromDataReader(reader);
		//                });
		//        }
		//    }

		//    return result;
		//}
	}
}
