using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 对象图片的Key
	/// </summary>
	[Serializable]
	public struct SchemaObjectPhotoKey
	{
		public string ObjectID
		{
			get;
			set;
		}

		public string PropertyName
		{
			get;
			set;
		}

		public DateTime TimePoint
		{
			get;
			set;
		}
	}
}
