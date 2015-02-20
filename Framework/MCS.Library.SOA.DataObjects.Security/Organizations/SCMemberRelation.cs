using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using System.Data;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 表示成员关系
	/// </summary>
	[Serializable]
	public class SCMemberRelation : SCSimpleRelationBase
	{
		/// <summary>
		/// 初始化<see cref="SCMemberRelation"/>的新实例
		/// </summary>
		public SCMemberRelation() :
			base(StandardObjectSchemaType.MemberRelations.ToString())
		{
		}

		public SCMemberRelation(string schemaType)
			: base(schemaType)
		{
		}

		/// <summary>
		/// 使用指定的容器对象和成员对象 初始化<see cref="SCMemberRelation"/>的新实例
		/// </summary>
		/// <param name="container">容器对象</param>
		/// <param name="member">成员对象</param>
		public SCMemberRelation(SchemaObjectBase container, SchemaObjectBase member) :
			base(container, member, StandardObjectSchemaType.MemberRelations.ToString())
		{
		}
	}
}
