using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security
{
	[Serializable]
	public class SCSecretaryRelation : SCSimpleRelationBase
	{
		/// <summary>
		/// 初始化<see cref="SCMemberRelation"/>的新实例
		/// </summary>
		public SCSecretaryRelation() :
			base(StandardObjectSchemaType.SecretaryRelations.ToString())
		{
		}

		internal SCSecretaryRelation(string schemaType)
			: base(schemaType)
		{
		}

		/// <summary>
		/// 使用指定的容器对象和成员对象 初始化<see cref="SCMemberRelation"/>的新实例
		/// </summary>
		/// <param name="container">容器对象</param>
		/// <param name="member">成员对象</param>
		public SCSecretaryRelation(SchemaObjectBase container, SchemaObjectBase member) :
			base(container, member, StandardObjectSchemaType.SecretaryRelations.ToString())
		{
		}
	}
}
