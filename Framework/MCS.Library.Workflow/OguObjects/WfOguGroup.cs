using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.Workflow.OguObjects
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class WfOguGroup : WfOguObject, IGroup
	{
		internal WfOguGroup()
			: base(SchemaType.Groups)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public WfOguGroup(string id)
			: base(id, SchemaType.Groups)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="group"></param>
		public WfOguGroup(IGroup group)
			: base(group, SchemaType.Groups)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected WfOguGroup(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#region IGroup ≥…‘±
		/// <summary>
		/// 
		/// </summary>
		public OguObjectCollection<IUser> Members
		{
			get
			{
				return BaseGroupObject.Members;
			}
		}

		#endregion

		private IGroup BaseGroupObject
		{
			get
			{
				return (IGroup)BaseObject;
			}
		}
	}
}
