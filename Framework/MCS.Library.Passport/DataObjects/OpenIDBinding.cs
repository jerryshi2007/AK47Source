using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.Passport
{
	/// <summary>
	/// OpenID和用户的绑定关系
	/// </summary>
	[Serializable]
	[ORTableMapping("OPEN_ID_BINDINGS")]
	public class OpenIDBinding
	{
		/// <summary>
		/// OpenID
		/// </summary>
		[ORFieldMapping("OPEN_ID", PrimaryKey = true)]
		public string OpenID
		{
			get;
			set;
		}

		/// <summary>
		/// 用户
		/// </summary>
		[ORFieldMapping("USER_ID")]
		public string UserID
		{
			get;
			set;
		}

		/// <summary>
		/// OpenID的类型
		/// </summary>
		[ORFieldMapping("OPEN_ID_TYPE")]
		public string OpenIDType
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		[ORFieldMapping("CREATE_TIME")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public DateTime CreateTime
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class OpenIDBindingCollection : EditableDataObjectCollectionBase<OpenIDBinding>
	{
	}
}
