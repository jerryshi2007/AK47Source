using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.Core
{
	/// <summary>
	/// 未知的序列化类型。用于反序列化时，接住无法获取的类型信息。与UnknownTypeStrategyBinder配合使用
	/// </summary>
	[Serializable]
	public sealed class UnknownSerializationType : ISerializable
	{
		/// <summary>
		/// 
		/// </summary>
		public UnknownSerializationType()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private UnknownSerializationType(SerializationInfo info, StreamingContext context)
		{
		}

		/// <summary>
		/// 输出未知类型
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "未知类型";
		}

		#region ISerializable Members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		#endregion
	}
}
