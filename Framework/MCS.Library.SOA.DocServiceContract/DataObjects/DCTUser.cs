using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceContract
{
	/// <summary>
	/// 用户
	/// </summary>
    [DataContract(IsReference = true)]
    [Serializable]
	public class DCTUser : DCTPrincipal
	{
		/// <summary>
		/// 登录名
		/// </summary>
		[DataMember]
		public string LoginName
		{
			get;
			set;
		}
	}
}