using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示管理单元架构角色
	/// </summary>
	[Serializable]
	public class AUSchemaRole : SCBase
	{
		public AUSchemaRole()
			: base(AUCommon.SchemaAUSchemaRole)
		{
		}

		public AUSchemaRole(string schemaType)
			: base(schemaType)
		{
		}


		/// <summary>
		/// 获取当前的父级管理Schema
		/// </summary>
		/// <returns></returns>
		public AUSchema GetCurrentOwnerAUSchema()
		{
			var queryResult = Adapters.AUSnapshotAdapter.Instance.LoadContainers(this.ID, this.SchemaType, AUCommon.SchemaAUSchema, true, DateTime.MinValue);
			if (queryResult.Count > 1)
				throw new AUObjectException("与此角色关联的AUSchema不唯一，可能存在数据错误。");
			return (AUSchema)queryResult.FirstOrDefault();
		}

		/// <summary>
		/// 获取任何与此角色对应的Schema
		/// </summary>
		/// <returns></returns>
		public AUSchema GetOwnerAUSchema()
		{
			var queryResult = Adapters.AUSnapshotAdapter.Instance.LoadContainers(this.ID, this.SchemaType, AUCommon.SchemaAUSchema, false, DateTime.MinValue);
			if (queryResult.Count > 1)
				throw new AUObjectException("与此角色关联的AUSchema不唯一，可能存在数据错误。");
			return (AUSchema)queryResult.FirstOrDefault();
		}
	}
}
