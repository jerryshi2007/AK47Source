using System;
using System.Collections.Generic;
using System.Transactions;
using System.Configuration;

using MCS.Library.Configuration;
using MCS.Library.Data.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Data
{
	/// <summary>
	/// 为了定制TransactionScope的配置参数设计的专用工厂类。
	/// </summary>
	public static class TransactionScopeFactory
	{
		private static readonly TimeSpan DefaultTimeout = TimeSpan.Parse("00:10:00");
		private const IsolationLevel DefaultIsolationLevel = IsolationLevel.ReadCommitted;

		private static TransactionConfigurationSection GetConfiguration()
		{
			TransactionConfigurationSection section = ConfigurationBroker.GetSection("transactions") as TransactionConfigurationSection;

			if (section == null)//系统允许默认设置
				section = new TransactionConfigurationSection();

			return section;
		}

		/// <summary>
		/// 工厂类对 TransactionScope 设置的超时参数
		/// </summary>
		public static TimeSpan Timeout
		{
			get
			{
				TransactionConfigurationSection section = GetConfiguration();
				if (section == null)
					return DefaultTimeout;
				else
					return section.TimeOut;
			}
		}

		/// <summary>
		/// 工厂类对 TransactionScope 设置的交易隔离度
		/// </summary>
		public static IsolationLevel IsolationLevel
		{
			get
			{
				TransactionConfigurationSection section = GetConfiguration();
				if (section == null)
					return DefaultIsolationLevel;
				else
					return section.IsolationLevel;
			}
		}

		/// <summary>
		/// 在事物对象上绑定提交成功后的事件
		/// </summary>
		/// <param name="action"></param>
		/// <param name="throwError">是否抛出异常</param>
		public static void AttachCommittedAction(Action<TransactionEventArgs> action, bool throwError)
		{
			if (Transaction.Current != null && action != null)
			{
				Transaction.Current.TransactionCompleted += (sender, e) =>
				{
					if (e.Transaction.TransactionInformation.Status == TransactionStatus.Committed)
					{
						try
						{
							action(e);
						}
						catch (System.Exception)
						{
							if (throwError)
								throw;
						}
					}
				};
			}
		}

		/// <summary>
		/// 创建 TransactionScope 对象实例
		/// </summary>
		/// <returns>事务性范围对象</returns>
		public static TransactionScope Create()
		{
			return Create(TransactionScopeOption.Required);
		}

		/// <summary>
		/// 创建 TransactionScope 对象实例，设置超时时间
		/// </summary>
		/// <param name="timeout">超时时间</param>
		/// <returns></returns>
		public static TransactionScope Create(TimeSpan timeout)
		{
			return Create(TransactionScopeOption.Required, timeout, TransactionScopeFactory.IsolationLevel);
		}

		/// <summary>
		/// 创建 TransactionScope 对象实例
		/// </summary>
		/// <param name="scopeOption">事务范围复选项</param>
		/// <returns>事务性范围对象</returns>
		public static TransactionScope Create(TransactionScopeOption scopeOption)
		{
			return Create(scopeOption, TransactionScopeFactory.Timeout, TransactionScopeFactory.IsolationLevel);
		}

		/// <summary>
		/// 创建 TransactionScope 对象实例
		/// </summary>
		/// <param name="scopeOption">事务范围复选项</param>
		/// <param name="scopeTimeout">TimeSpan方式表示的超时时间</param>
		/// <returns>事务性范围对象</returns>
		public static TransactionScope Create(TransactionScopeOption scopeOption, TimeSpan scopeTimeout)
		{
			ExceptionHelper.TrueThrow<ArgumentOutOfRangeException>(scopeTimeout <= TimeSpan.Zero, "scopeTimeout");

			TransactionOptions options = new TransactionOptions();
			options.Timeout = scopeTimeout;
			options.IsolationLevel = TransactionScopeFactory.IsolationLevel;

			return Create(scopeOption, options);
		}

		/// <summary>
		/// 创建 TransactionScope 对象实例
		/// </summary>
		/// <param name="scopeOption">事务范围复选项</param>
		/// <param name="scopeTimeout">超时时间</param>
		/// <param name="isolationLevel">隔离级别</param>
		/// <returns>事务性范围对象</returns>
		public static TransactionScope Create(TransactionScopeOption scopeOption, TimeSpan scopeTimeout, IsolationLevel isolationLevel)
		{
			if (scopeTimeout <= TimeSpan.Zero)
				throw new ArgumentOutOfRangeException("scopeTimeout");

			// 组装TS参数
			TransactionOptions options = new TransactionOptions();
			options.Timeout = scopeTimeout;
			options.IsolationLevel = isolationLevel;

			return Create(scopeOption, options);
		}

		/// <summary>
		/// 创建TransactionScope并且执行操作
		/// </summary>
		/// <param name="action"></param>
		public static void DoAction(Action action)
		{
			DoAction(TransactionScopeOption.Required, action);
		}

		/// <summary>
		/// 创建TransactionScope并且执行操作
		/// </summary>
		/// <param name="scopeOption"></param>
		/// <param name="action"></param>
		public static void DoAction(TransactionScopeOption scopeOption, Action action)
		{
			if (action != null)
			{
				using (TransactionScope scope = Create(scopeOption))
				{
					action();

					scope.Complete();
				}
			}
		}

		/// <summary>
		/// 创建 TransactionScope 对象实例
		/// </summary>
		/// <param name="scopeOption">事务复选项</param>
		/// <param name="transactionOptions">事务附加信息</param>
		/// <returns>事务性范围对象</returns>
		private static TransactionScope Create(TransactionScopeOption scopeOption, TransactionOptions transactionOptions)
		{
			return new TransactionScope(scopeOption, transactionOptions);
		}

		/// <summary>
		/// 创建 TransactionScope 对象实例
		/// </summary>
		/// <param name="scopeOption">事务范围复选项</param>
		/// <param name="isolationLevel">隔离级别</param>
		/// <returns>事务性范围对象</returns>
		private static TransactionScope Create(TransactionScopeOption scopeOption, IsolationLevel isolationLevel)
		{
			return Create(scopeOption, TransactionScopeFactory.Timeout, isolationLevel);
		}

		/// <summary>
		/// 创建 TransactionScope 对象实例
		/// 由于 Transaction 自带交易隔离度说明，因此不涉及从配置提取该参数的操作
		/// </summary>
		/// <param name="transactionToUse">事务</param>
		/// <param name="scopeTimeout">超时范围</param>
		/// <returns>事务性范围对象</returns>
		private static TransactionScope Create(Transaction transactionToUse, TimeSpan scopeTimeout)
		{
			if (scopeTimeout < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException("scopeTimeout");

			return new TransactionScope(transactionToUse, scopeTimeout);
		}
	}
}
