using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.Core
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ActionContextBase<T> where T : ActionContextBase<T>, new()
	{
		private Dictionary<string, object> _Context = null;

		/// <summary>
		/// 从ObjectContextCache获取上下文对象
		/// </summary>
		public static T Current
		{
			get
			{
				T result = (T)ObjectContextCache.Instance.GetOrAddNewValue(GetAndCheckContextKey(),
						(cache, key) =>
						{
							T context = new T();
							cache.Add(key, context);

							return context;
						});

				return result;
			}
		}

		/// <summary>
		/// 获取一个保存上下文对象的<see cref="T:Dictionary^2"/>。
		/// </summary>
		public Dictionary<string, object> Context
		{
			get
			{
				if (this._Context == null)
					this._Context = new Dictionary<string, object>();

				return this._Context;
			}
		}

		/// <summary>
		/// 执行<paramref name="action"/>指定的操作。
		/// </summary>
		/// <param name="action">操作方法 或 <see langword="null"/>表示无操作。</param>
		[System.Diagnostics.DebuggerNonUserCode]
		public void DoActions(Action action)
		{
			try
			{
				if (action != null)
				{
					action();
				}
			}
			finally
			{
				Clear();
			}
		}

		/// <summary>
		/// 清除上下文
		/// </summary>
		public static void Clear()
		{
			string key = GetAndCheckContextKey();

			if (ExistsInContext)
				ObjectContextCache.Instance.Remove(GetAndCheckContextKey());
		}

		/// <summary>
		/// 是否在上下文中存在
		/// </summary>
		public static bool ExistsInContext
		{
			get
			{
				return ObjectContextCache.Instance.ContainsKey(GetAndCheckContextKey());
			}
		}

		private static string GetAndCheckContextKey()
		{
			ActionContextDescriptionAttribute attr = AttributeHelper.GetCustomAttribute<ActionContextDescriptionAttribute>(typeof(T));

			(attr != null).FalseThrow("不能在类{0}上找到ActionContextDescriptionAttribute的定义", typeof(T).AssemblyQualifiedName);

			(attr.Key.IsNotEmpty()).FalseThrow("类{0}上ActionContextDescriptionAttribute的Key属性不能为空", typeof(T).AssemblyQualifiedName);

			return attr.Key;
		}
	}
}
