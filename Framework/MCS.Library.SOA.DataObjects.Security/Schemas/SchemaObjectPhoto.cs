using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 对象的图片信息
	/// </summary>
	[Serializable]
	public class SchemaObjectPhoto
	{
		private byte[] _ContentData = null;

		private ImageProperty _ImageInfo = null;

		/// <summary>
		/// 图片的基本信息
		/// </summary>
		public ImageProperty ImageInfo
		{
			get
			{
				return this._ImageInfo;
			}
			set
			{
				this._ImageInfo = value;
			}
		}

		/// <summary>
		/// 图片数据
		/// </summary>
		public byte[] ContentData
		{
			get
			{
				return this._ContentData;
			}
			set
			{
				this._ContentData = value;
			}
		}
	}
}
