using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
	internal class RelationHelper
	{
		public static readonly RelationHelper Instance = new RelationHelper();

		/// <summary>
		/// 断开容器与成员的关系(不影响成员对象)
		/// </summary>
		/// <param name="obj"></param>
		public void ClearContainer(SchemaObjectBase obj)
		{
			AUCommon.DoDbAction(() =>
			{
				var memberRelations = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(obj.ID, DateTime.MinValue);
				foreach (var item in memberRelations)
				{
					if (item.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal)
						PC.Adapters.SCMemberRelationAdapter.Instance.UpdateStatus(item, Schemas.SchemaProperties.SchemaObjectStatus.Deleted);
				}
			});
		}
	}
}
