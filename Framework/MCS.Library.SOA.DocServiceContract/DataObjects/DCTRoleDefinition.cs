using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
	/// <summary>
	/// 角色定义
	/// </summary>
	[DataContract]
	[Serializable]
	public class DCTRoleDefinition
	{
		/// <summary>
		/// ID
		/// </summary>
		[DataMember]
		public int ID
		{
			get;
			set;
		}

		/// <summary>
		/// 名称
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// 描述
		/// </summary>
		[DataMember]
		public string Description
		{
			get;
			set;
		}
	}
}