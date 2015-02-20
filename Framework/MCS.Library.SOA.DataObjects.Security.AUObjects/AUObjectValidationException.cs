using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示管理单元对象的校验异常
	/// </summary>
	public class AUObjectValidationException : AUObjectException
	{
		private SchemaObjectBase schemaObjectBase;

		/// <summary>
		/// 初始化<see cref="AUObjectValidationException"/>的新实例
		/// </summary>
		public AUObjectValidationException() : base("对象校验失败") { }
		/// <summary>
		/// 根据指定的错误消息初始化<see cref="AUObjectValidationException"/>的新实例
		/// </summary>
		/// <param name="message">错误消息的字符串</param>
		public AUObjectValidationException(string message) : base(message) { }
		/// <summary>
		/// 根据指定的错误消息和内部异常初始化<see cref="AUObjectValidationException"/>的新实例
		/// </summary>
		/// <param name="message">错误消息的字符串</param>
		/// <param name="inner">内部异常</param>
		public AUObjectValidationException(string message, Exception inner) : base(message, inner) { }

		/// <summary>
		/// 根据指定的错误消息和Schema对象初始化<see cref="AUObjectValidationException"/>的新实例
		/// </summary>
		/// <param name="message">错误消息的字符串</param>
		/// <param name="schemaObjectBase">Schema对象</param>
		public AUObjectValidationException(string message, SchemaObjectBase schemaObjectBase)
			: base(message)
		{
			this.schemaObjectBase = schemaObjectBase;
		}

		/// <summary>
		/// 根据指定的错误消息，内部异常，Schema对象初始化<see cref="AUObjectValidationException"/>的新实例
		/// </summary>
		/// <param name="message">错误消息的字符串</param>
		/// <param name="schemaObjectBase">Schema对象</param>
		public AUObjectValidationException(string message, Exception inner, SchemaObjectBase schemaObjectBase)
			: base(message, inner)
		{
			this.schemaObjectBase = schemaObjectBase;
		}

		/// <summary>
		/// 获取Schema对象的实例 或 <see langword="null"/>。
		/// </summary>
		public SchemaObjectBase SchemaObject { get { return this.schemaObjectBase; } }

		protected AUObjectValidationException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
