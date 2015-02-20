#region
// -------------------------------------------------
// Assembly	：	HB.DataObjects
// FileName	：	OguUser.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    李苗	    20070628		创建
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class OguGroup : OguBase, IGroup
	{
		#region IGroup 成员

		public OguObjectCollection<IUser> Members
		{
			get
			{
				return BaseGroup.Members;
			}
		}

		/// <summary>
		/// 机构类型
		/// </summary>
		public override SchemaType ObjectType
		{
			get
			{
				return SchemaType.Groups;
			}
		}

		#endregion

		#region 基础类型

		private IGroup BaseGroup
		{
			get
			{
				return (IGroup)Ogu;
			}
		}

		#endregion 基础类型

		public OguGroup(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{

		}

		public OguGroup(string id)
			: base(id, SchemaType.Groups)
		{
		}

		public OguGroup(IGroup group)
			: base(group, SchemaType.Groups)
		{
		}

		public OguGroup()
			: base(SchemaType.Groups)
		{

		}
	}
}
