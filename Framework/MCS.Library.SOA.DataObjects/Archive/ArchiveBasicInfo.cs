using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Archive
{
	/// <summary>
	/// 基本的归档信息
	/// </summary>
	[Serializable]
	public class ArchiveBasicInfo
	{
		private Dictionary<string, object> context = new Dictionary<string, object>();

		/// <summary>
		/// 应用名称
		/// </summary>
		public string ApplicationName
		{
			get;
			set;
		}

		/// <summary>
		/// 应用的模块名称
		/// </summary>
		public string ProgramName
		{
			get;
			set;
		}

		/// <summary>
		/// 归档数据的ID
		/// </summary>
		public string ResourceID
		{
			get;
			set;
		}

		/// <summary>
		/// 上下文
		/// </summary>
		public Dictionary<string, object> Context
		{
			get
			{
				return this.context;
			}
		}
	}

	public static class ArchiveContextExtension
	{
		public static bool TryGetValue<T>(this Dictionary<string, object> context, string key, out T data)
		{
			object resultData = default(T);
			bool result = context.TryGetValue(key, out resultData);

			if (resultData == null)
				result = false;

			data = (T)resultData;

			return result;
		}

		public static void DoAction<T>(this Dictionary<string, object> context, string key, Action<T> action)
		{
			T data = default(T);

			if (context.TryGetValue(key, out data))
				action(data);
		}
	}
}
