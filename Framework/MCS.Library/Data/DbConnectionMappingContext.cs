using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Caching;
using MCS.Library.Core;

namespace MCS.Library.Data
{
	/// <summary>
	/// 数据库联接名称的映射。在上下文中维护一个连接名称的映射字典。
	/// </summary>
	[System.Diagnostics.DebuggerNonUserCode]
	public sealed class DbConnectionMappingContext : IDisposable
	{
		private string sourceConnectionName = string.Empty;
		private string destinationConnectionName = string.Empty;
		private DbConnectionMappingContext originalContext = null;

		/// <summary>
		/// 
		/// </summary>
		public string SourceConnectionName
		{
			get { return sourceConnectionName; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string DestinationConnectionName
		{
			get { return destinationConnectionName; }
		}

		private DbConnectionMappingContext()
		{
		}

		/// <summary>
		/// 创建连接名称的对应关系
		/// </summary>
		/// <param name="srcConnectionName"></param>
		/// <param name="destConnectionName"></param>
		/// <returns></returns>
		public static DbConnectionMappingContext CreateMapping(string srcConnectionName, string destConnectionName)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(srcConnectionName, "srcConnectionName");
			ExceptionHelper.CheckStringIsNullOrEmpty(destConnectionName, "destConnectionName");

			DbConnectionMappingContext localOriginalContext = null;

			DbConnectionMappingContextCache.Instance.TryGetValue(srcConnectionName, out localOriginalContext);

			DbConnectionMappingContext context = new DbConnectionMappingContext();

			context.sourceConnectionName = srcConnectionName;
			context.destinationConnectionName = destConnectionName;
			context.originalContext = localOriginalContext;

			DbConnectionMappingContextCache.Instance[context.sourceConnectionName] = context;

			return context;
		}

		/// <summary>
		/// 在连接串映射完成后，执行映射后的动作。执行完后，恢复原有的映射
		/// </summary>
		/// <param name="srcConnectionName"></param>
		/// <param name="destConnectionName"></param>
		/// <param name="action"></param>
		public static void DoMappingAction(string srcConnectionName, string destConnectionName, Action action)
		{
			if (action != null)
			{
				using (DbConnectionMappingContext context = CreateMapping(srcConnectionName, destConnectionName))
				{
					action();
				}
			}
		}

		/// <summary>
		/// 得到映射后的连接名称
		/// </summary>
		/// <param name="srcConnectionName"></param>
		/// <returns></returns>
		public static string GetMappedConnectionName(string srcConnectionName)
		{
			return GetMappedConnectionContectRecursively(srcConnectionName, new HashSet<DbConnectionMappingContext>());
		}

		private static string GetMappedConnectionContectRecursively(string srcConnectionName, HashSet<DbConnectionMappingContext> elapsedContext)
		{
			DbConnectionMappingContext context = null;
			string result = srcConnectionName;

			if (DbConnectionMappingContextCache.Instance.TryGetValue(srcConnectionName, out context))
			{
				if (elapsedContext.Contains(context) == false)
				{
					elapsedContext.Add(context);

					result = GetMappedConnectionContectRecursively(context.destinationConnectionName, elapsedContext);
				}
			}

			return result;
		}

		/// <summary>
		/// 清除所有映射
		/// </summary>
		public static void ClearAllMappings()
		{
			DbConnectionMappingContextCache.Instance.Clear();
		}

		#region IDisposable Members
		/// <summary>
		/// 释放对应关系
		/// </summary>
		public void Dispose()
		{
			DbConnectionMappingContext context = null;

			if (DbConnectionMappingContextCache.Instance.TryGetValue(this.sourceConnectionName, out context))
			{
				if (context.originalContext != null)
					DbConnectionMappingContextCache.Instance[this.sourceConnectionName] = context.originalContext;
				else
					DbConnectionMappingContextCache.Instance.Remove(this.sourceConnectionName);
			}

            GC.SuppressFinalize(this);
		}
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	public class DbConnectionMappingContextCache : ContextCacheQueueBase<string, DbConnectionMappingContext>
	{
		/// <summary>
		/// 
		/// </summary>
		public static DbConnectionMappingContextCache Instance
		{
			get
			{
				return ContextCacheManager.GetInstance<DbConnectionMappingContextCache>();
			}
		}
	}
}
