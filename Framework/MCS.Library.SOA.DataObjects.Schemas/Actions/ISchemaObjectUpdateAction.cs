using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Schemas.Actions
{
	/// <summary>
	/// 模式对象更新操作的方法
	/// </summary>
	public interface ISchemaObjectUpdateAction
	{
		/// <summary>
		/// 准备数据
		/// </summary>
		/// <param name="obj">一个<see cref="VersionedSchemaObjectBase"/>实例</param>
		void Prepare(VersionedSchemaObjectBase obj);  

		/// <summary>
		/// 保存数据
		/// </summary>
		/// <param name="obj">一个<see cref="VersionedSchemaObjectBase"/>实例</param>
		void Persist(VersionedSchemaObjectBase obj);
	}

	/// <summary>
	/// 表示模式对象更新操作的集合
	/// </summary>
	[Serializable]
	public class SchemaObjectUpdateActionCollection : EditableDataObjectCollectionBase<ISchemaObjectUpdateAction>
	{
		/// <summary>
		/// 准备数据
		/// </summary>
		/// <param name="obj">一个<see cref="VersionedSchemaObjectBase"/>实例</param>
		public void Prepare(VersionedSchemaObjectBase obj)
		{
			this.ForEach(action => action.Prepare(obj));
		}

		/// <summary>
		/// 保存数据
		/// </summary>
		/// <param name="obj">一个<see cref="VersionedSchemaObjectBase"/>实例</param>
		public void Persist(VersionedSchemaObjectBase obj)
		{
			this.ForEach(action => action.Persist(obj));
		}
	}
}
