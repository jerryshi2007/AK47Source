using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	/// <summary>
	/// 按照不同的ID（CodeName、GUID、FullPath）查询对象并返回结果的基类，这个类主要用于保持原来的应用面板和WebService的兼容性
	/// </summary>
	public abstract class QueryByIDsAdapterBase<TResult>
	{
		private string[] _IDs = null;
		private bool _IncludeDeleted = false;
		private string[] _SchemaTypes = null;

		protected QueryByIDsAdapterBase(string[] schemaTypes, string[] ids, bool includeDeleted)
		{
			schemaTypes.NullCheck("schemaTypes");
			ids.NullCheck("ids");

			this._SchemaTypes = schemaTypes;
			this._IDs = ids;
			this._IncludeDeleted = includeDeleted;
		}

		#region Properties
		public string[] SchemaTypes
		{
			get
			{
				return this._SchemaTypes;
			}
		}

		public bool IncludeDeleted
		{
			get
			{
				return this._IncludeDeleted;
			}

			set
			{
				this._IncludeDeleted = value;
			}
		}

		public string[] IDs
		{
			get
			{
				return this._IDs;
			}
		}
		#endregion Properties

		#region Public methods
		/// <summary>
		/// 查询出SCObjectAndRelationCollection对象
		/// </summary>
		/// <returns></returns>
		public abstract SCObjectAndRelationCollection QueryObjectsAndRelations();

		/// <summary>
		/// 执行查询并且转换为所期待的结果类型
		/// </summary>
		/// <returns></returns>
		public TResult Query()
		{
			SCObjectAndRelationCollection relations = this.QueryObjectsAndRelations();

			return this.ConvertToResult(relations);
		}
		#endregion Public methods

		#region Protected methods
		/// <summary>
		/// 修饰ID
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		protected virtual string[] DecorateIDs(string[] ids)
		{
			return ids;
		}

		/// <summary>
		/// 转换成所需要的结果
		/// </summary>
		/// <param name="relations"></param>
		/// <returns></returns>
		protected abstract TResult ConvertToResult(SCObjectAndRelationCollection relations);

		#endregion Protected methods
	}
}
