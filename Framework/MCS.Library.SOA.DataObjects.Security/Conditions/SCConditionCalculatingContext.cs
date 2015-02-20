using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.Conditions
{
	public interface ISCConditionCalculatingContext
	{
		SchemaObjectBase CurrentObject { get; }

		SchemaObjectCollection AllObjects { get; }

		Dictionary<string, object> ExtendedData { get; }

		void ExecutionWrapper(string operationName, Action action);
	}


	/// <summary>
	/// 条件计算中的上下文对象
	/// </summary>
	public class SCConditionCalculatingContext : ISCConditionCalculatingContext
	{
		private Dictionary<SchemaNameAndPropertyName, SCPropertyAccessorBase> _PropertyAccessors = new Dictionary<SchemaNameAndPropertyName, SCPropertyAccessorBase>(64);
		private SchemaObjectCollection _AllUsers = null;
		private Dictionary<string, object> _ExtendedData = new Dictionary<string, object>();

		internal void EnsureInitialized()
		{
			if (this._AllUsers == null)
			{
				this.InitAllData();
			}
		}

		private void InitAllData()
		{
			this.ExecutionWrapper("加载所有用户", () => this.InitAllUsers());
		}

		private void InitAllUsers()
		{
			InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("SchemaType");

			SchemaDefineCollection schemas = SchemaExtensions.CreateSchemasDefineFromConfiguration();

			schemas.ForEach(schema =>
			{
				if (string.Compare(schema.Category, "Users", true) == 0)
					inBuilder.AppendItem(schema.Name);
			});

			this._AllUsers = SchemaObjectAdapter.Instance.Load(inBuilder);
		}

		internal Dictionary<SchemaNameAndPropertyName, SCPropertyAccessorBase> PropertyAccessors
		{
			get
			{
				return this._PropertyAccessors;
			}
		}

		/// <summary>
		/// 计算过程中的当前用户
		/// </summary>
		public SchemaObjectBase CurrentObject
		{
			get;
			internal set;
		}

		/// <summary>
		/// 所有需要计算的用户
		/// </summary>
		public SchemaObjectCollection AllObjects
		{
			get
			{
				return this._AllUsers;
			}
		}

		/// <summary>
		/// 扩展数据，用于在计算过程中缓存一些数据
		/// </summary>
		public Dictionary<string, object> ExtendedData
		{
			get
			{
				return this._ExtendedData;
			}
		}

		/// <summary>
		/// 执行操作，通过日志输出操作时间
		/// </summary>
		/// <param name="operationName"></param>
		/// <param name="action"></param>
		public void ExecutionWrapper(string operationName, Action action)
		{
			operationName.CheckStringIsNullOrEmpty("operationName");
			action.NullCheck("action");

			ProcessProgress.Current.Output.WriteLine("\t\t{0}开始：{1:yyyy-MM-dd HH:mm:ss.fff}",
					operationName, DateTime.Now);

			Stopwatch sw = new Stopwatch();

			sw.Start();
			try
			{
				action();
			}
			finally
			{
				sw.Stop();
				ProcessProgress.Current.Output.WriteLine("\t\t{0}结束：{1:yyyy-MM-dd HH:mm:ss.fff}；经过时间：{2:#,##0}毫秒",
					operationName, DateTime.Now, sw.ElapsedMilliseconds);
			}
		}
	}
}
