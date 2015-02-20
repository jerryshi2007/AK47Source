using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
	/// <summary>
	/// 授权实体基类
	/// </summary>
    [DataContract(IsReference = true)]
    [Serializable]
	public abstract class DCTPrincipal
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
		/// 类型（用户或用户组）
		/// </summary>
		[DataMember]
		public DCTPrincipalType PricinpalType
		{
			get;
			set;
		}

		/// <summary>
		/// 标题
		/// </summary>
		[DataMember]
		public string Title
		{
			get;
			set;
		}
	}
}