using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter
{
	/// <summary>
	/// 指定允许使用的数据的基数类型
	/// </summary>
	[Flags]
	public enum BatchExecuteCardinality
	{
		/// <summary>
		/// 最多指定一个对象
		/// </summary>
		/// <remarks>设置此标志，表示限制一个</remarks>
		One = 1,

		/// <summary>
		/// 对象个数没有上限
		/// </summary>
		Many = 0,

		/// <summary>
		/// 必须至少含有一个
		/// </summary>
		Mandatory = 2,
	}

	public abstract class BatchExecutor
	{
		#region 字段

		private string[] keysFrom = null;
		private string[] keysTo = null;
		private IBatchErrorAdapter errors;

		#endregion

		#region 构造函数

		public BatchExecutor(IEnumerable src, IEnumerable target)
			: this(BatchExecutor.ToArray(src), BatchExecutor.ToArray(target))
		{
		}

		public BatchExecutor(string[] src, string[] target)
		{
			this.errors = this.InitErrorAdapter();
			this.keysFrom = src;
			this.keysTo = target;
		}

		protected virtual IBatchErrorAdapter InitErrorAdapter()
		{
			return new ProgressBatchErrorAdapter();
		}

		#endregion

		#region 受保护的属性

		/// <summary>
		/// 包含错误时，向其中写入异常或消息
		/// </summary>
		protected IBatchErrorAdapter Errors
		{
			get
			{
				return this.errors;
			}
		}

		protected string[] SourceKeys
		{
			get { return this.keysFrom; }
		}

		protected string[] TargetKeys
		{
			get { return this.keysTo; }
		}

		/// <summary>
		/// 不为<see langword = "null"/>时，表示源对象的类型描述
		/// </summary>
		protected abstract string DescriptionForSource { get; }

		/// <summary>
		/// 不为<see langword = "null"/>时，表示目标对象的类型描述
		/// </summary>
		protected abstract string DescriptionForTarget { get; }

		/// <summary>
		/// 指定来源对象的基数，缺省是至少含有一个，但无上限
		/// </summary>
		protected virtual BatchExecuteCardinality CardinalityOfSource
		{
			get { return BatchExecuteCardinality.Mandatory; }
		}

		/// <summary>
		/// 指定目标对象的基数，缺省是至少含有一个，但无上限
		/// </summary>
		protected virtual BatchExecuteCardinality CardinalityOfDestination
		{
			get { return BatchExecuteCardinality.Mandatory; }
		}

		#endregion

		#region 公开的静态方法
		public static string[] ToArray(IEnumerable keysTo)
		{
			string[] result = null;
			if (keysTo is string[])
			{
				result = (string[])keysTo;
			}
			else if (keysTo is IList<string>)
			{
				result = ((IList<string>)keysTo).ToArray();
			}
			else
			{
				result = (from string k in keysTo select k).ToArray();
			}

			return result;
		}
		#endregion

		#region 公开的方法
		/// <summary>
		/// 执行操作
		/// </summary>
		/// <returns></returns>
		public virtual bool Execute()
		{
			bool result = false;
			try
			{
				var srcObjects = BatchExecutor.GetObjects(this.SourceKeys, this.CardinalityOfSource, this.Errors);
				var targetObjects = BatchExecutor.GetObjects(this.TargetKeys, this.CardinalityOfDestination, this.Errors);

				Util.EnsureOperationSafe();
				this.ValidateObjects();

				result = this.DoExecute(srcObjects, targetObjects);
			}
			catch (Exception ex)
			{
				result = false;
				if (this.HandleError(ex) == false)
				{
					throw;
				}
			}

			return result;
		}

		#endregion

		#region 受保护的方法

		protected virtual void ValidateObjects()
		{
		}

		protected virtual bool DoExecute(SchemaObjectCollection srcObjects, SchemaObjectCollection targetObjects)
		{
			bool result = true;
			foreach (SchemaObjectBase src in srcObjects)
			{
				foreach (SchemaObjectBase target in targetObjects)
				{
					try
					{
						this.DoExecuteItem(src, target);
					}
					catch (Exception ex)
					{
						if (this.HandleItemError(src, target, ex) == false)
						{
							throw;
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 处理错误
		/// </summary>
		/// <param name="ex"></param>
		/// <returns>如果处理了，应返回<see langword="true"/>，否则会导致异常被再次抛出</returns>
		protected virtual bool HandleError(Exception ex)
		{
			return false;
		}

		/// <summary>
		/// 处理项错误
		/// </summary>
		/// <param name="ex"></param>
		/// <returns>如果处理了，应返回<see langword="true"/>，否则会导致异常被再次抛出</returns>
		protected virtual bool HandleItemError(SchemaObjectBase src, SchemaObjectBase target, Exception ex)
		{
			return false;
		}

		protected abstract void DoExecuteItem(SchemaObjectBase src, SchemaObjectBase target);

		#endregion

		#region 私有的静态方法

		private static SchemaObjectCollection GetObjects(string[] keys, BatchExecuteCardinality cardinality, IBatchErrorAdapter errors)
		{
			SchemaObjectCollection result = null;
			if ((cardinality & BatchExecuteCardinality.Mandatory) == BatchExecuteCardinality.Mandatory)
			{
				// 至少含有一个
				if (keys == null || keys.Length == 0)
				{
					throw new ArgumentOutOfRangeException("keys", keys, "需要至少一个对象");
				}
			}

			if ((cardinality & BatchExecuteCardinality.One) == BatchExecuteCardinality.One)
			{
				if (keys != null && keys.Length > 1)
				{
					throw new ArgumentOutOfRangeException("keys", keys, "只能指定一个对象");
				}
			}

			if (keys != null && keys.Length > 0)
			{
				result = DbUtil.LoadAndCheckObjects("对象", errors, keys);
			}
			else
			{
				result = new SchemaObjectCollection();
			}

			return result;
		}
		#endregion
	}
}