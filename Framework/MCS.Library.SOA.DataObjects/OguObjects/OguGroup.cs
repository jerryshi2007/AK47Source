#region
// -------------------------------------------------
// Assembly	��	HB.DataObjects
// FileName	��	OguUser.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070628		����
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
		#region IGroup ��Ա

		public OguObjectCollection<IUser> Members
		{
			get
			{
				return BaseGroup.Members;
			}
		}

		/// <summary>
		/// ��������
		/// </summary>
		public override SchemaType ObjectType
		{
			get
			{
				return SchemaType.Groups;
			}
		}

		#endregion

		#region ��������

		private IGroup BaseGroup
		{
			get
			{
				return (IGroup)Ogu;
			}
		}

		#endregion ��������

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
