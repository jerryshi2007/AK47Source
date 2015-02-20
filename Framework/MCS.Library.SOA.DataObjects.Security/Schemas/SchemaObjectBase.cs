using System;
using System.Data;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Validators;
using MCS.Library.Validation;
using System.Collections.Generic;
using System.Collections;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 表示模式对象的基类
	/// </summary>
	[Serializable]
	[ORTableMapping("SC.SchemaObject")]
	public abstract class SchemaObjectBase : VersionedSchemaObjectBase, ISCStatusObject
	{
		private SchemaPropertyValueCollection _Properties = null;

		/// <summary>
		/// 初始化<see cref="SchemaObjectBase"/>成员
		/// </summary>
		protected SchemaObjectBase()
		{
		}

		/// <summary>
		/// 使用指定的模式名初始化<see cref="SchemaObjectBase"/>成员
		/// </summary>
		/// <param name="schemaType">模式的名称</param>
		public SchemaObjectBase(string schemaType)
			: base(schemaType)
		{
		}

		/// <summary>
		/// 获取模式定义
		/// </summary>
		/// <value>表示模式定义的<see cref="SchemaDefine"/>对象</value>
		[NoMapping]
		public new SchemaDefine Schema
		{
			get
			{
				return (SchemaDefine)base.Schema;
			}
		}

		[NonSerialized]
		private SCParentsRelationObjectCollection _AllParentRelations = null;

		/// <summary>
		/// 获取表示所有父级关系的<see cref="SCParentsRelationObjectCollection"/>
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCParentsRelationObjectCollection AllParentRelations
		{
			get
			{
				if (this._AllParentRelations == null && this.ID.IsNotEmpty())
				{
					this._AllParentRelations = SchemaRelationObjectAdapter.Instance.LoadByObjectID(this.ID);
				}

				return _AllParentRelations;
			}
		}

		/// <summary>
		/// 获取一个表示当前父级关系的<see cref="SCParentsRelationObjectCollection"/>
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCParentsRelationObjectCollection CurrentParentRelations
		{
			get
			{
				return (SCParentsRelationObjectCollection)AllParentRelations.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			}
		}

		[NonSerialized]
		private SchemaObjectCollection _AllParents = null;

		/// <summary>
		/// 获取一个<see cref="SchemaObjectCollection"/>，表示所有父级关系
		/// </summary>
		[NoMapping]
		[ScriptIgnore]
		public SchemaObjectCollection AllParents
		{
			get
			{
				if (_AllParents == null && this.ID.IsNotEmpty())
				{
					this._AllParents = SchemaObjectAdapter.Instance.Load(AllParentRelations.ToParentIDsBuilder());

					SCOrganization root = SCOrganization.GetRoot();

					if (AllParentRelations.Exists(r => r.ParentID == root.ID))
						this._AllParents.Add(root);
				}

				return this._AllParents;
			}
		}

		[NonSerialized]
		private SchemaObjectCollection _CurrentlParents = null;

		/// <summary>
		/// 获取一个<see cref="SchemaObjectCollection"/>，表示所有父级。
		/// </summary>
		[NoMapping]
		[ScriptIgnore]
		public SchemaObjectCollection CurrentParents
		{
			get
			{
				if (this._CurrentlParents == null && this.ID.IsNotEmpty())
				{
					this._CurrentlParents = SchemaObjectAdapter.Instance.Load(CurrentParentRelations.ToParentIDsBuilder());
					this._CurrentlParents = this._CurrentlParents.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);

					SCOrganization root = SCOrganization.GetRoot();

					if (CurrentParentRelations.Exists(r => r.ParentID == root.ID))
						this._CurrentlParents.Add(root);
				}

				return this._CurrentlParents;
			}
		}

		/// <summary>
		/// 生成表示当前对象的简单对象
		/// </summary>
		/// <returns></returns>
		public SCSimpleObject ToSimpleObject()
		{
			SCSimpleObject result = new SCSimpleObject();

			result.ID = this.ID;
			result.VersionStartTime = this.VersionStartTime;
			result.VersionEndTime = this.VersionEndTime;
			result.Tag = this.Tag;
			result.SchemaType = this.SchemaType;
			result.Status = this.Status;

			result.Name = this.Properties.GetValue("Name", string.Empty);
			result.DisplayName = this.Properties.GetValue("DisplayName", string.Empty);
			result.CodeName = this.Properties.GetValue("CodeName", string.Empty);

			if (result.DisplayName.IsNullOrEmpty())
				result.DisplayName = result.Name;

			return result;
		}

		/// <summary>
		/// 校验数据
		/// </summary>
		/// <returns></returns>
		public ValidationResults Validate()
		{
			SchemaObjectValidator validator = new SchemaObjectValidator();

			return validator.Validate(this);
		}

		/// <summary>
		/// 清除相关的数据，例如AllParents、AllChildren等
		/// </summary>
		public void ClearRelativeData()
		{
			OnIDChanged();
		}

		/// <summary>
		/// 处理ID的变更
		/// </summary>
		protected override void OnIDChanged()
		{
			this._AllParentRelations = null;
			this._AllParents = null;
			this._CurrentlParents = null;
		}

		protected override SchemaPropertyValueCollection GetProperties()
		{
			if (this._Properties == null)
				this._Properties = ((SchemaDefine)this.Schema).ToProperties();

			return this._Properties;
		}

		protected override SchemaDefineBase GetSchema(string schemaType)
		{
			return SchemaDefine.GetSchema(this.SchemaType);
		}
	}

	/// <summary>
	/// 表示平面化模式对象的集合
	/// </summary>
	[Serializable]
	public class SchemaObjectPlainCollection : SchemaObjectCollectionBase<SchemaObjectBase, SchemaObjectPlainCollection>
	{
		/// <summary>
		/// 获取简单对象的集合
		/// </summary>
		/// <returns><see cref="SCSimpleObjectCollection"/></returns>
		public SCSimpleObjectCollection ToSimpleObjects()
		{
			SCSimpleObjectCollection result = new SCSimpleObjectCollection();

			this.ForEach(obj => result.Add(obj.ToSimpleObject()));

			return result;
		}

		/// <summary>
		/// 创建过滤器结果的集合
		/// </summary>
		/// <returns></returns>
		protected override SchemaObjectPlainCollection CreateFilterResultCollection()
		{
			return new SchemaObjectPlainCollection();
		}
	}

	/// <summary>
	/// 表示模式对象的集合
	/// </summary>
	[Serializable]
	public class SchemaObjectCollection : SchemaObjectEditableKeyedCollectionBase<SchemaObjectBase, SchemaObjectCollection>
	{
		public SchemaObjectCollection()
		{
		}

		public SchemaObjectCollection(int capacity)
			: base(capacity)
		{
		}

		public SchemaObjectCollection(IEnumerable<SchemaObjectBase> collection)
			: base(DetectCapacity(collection))
		{
			foreach (var item in collection)
			{
				this.AddNotExistsItem(item);
			}
		}

		private static int DetectCapacity(IEnumerable<SchemaObjectBase> collection)
		{
			int result = 0;

			if (collection is ICollection<SchemaObjectBase>)
			{
				result = ((ICollection<SchemaObjectBase>)collection).Count;
			}
			else if (collection is ICollection)
			{
				result = ((ICollection)collection).Count;
			}

			return result;
		}

		/// <summary>
		/// 合并两个集合。结果是两个集合的并集
		/// </summary>
		/// <param name="source"></param>
		public virtual void Merge(SchemaObjectCollection source)
		{
			if (source != null)
				source.ForEach(obj => this.AddNotExistsItem(obj));
		}

		/// <summary>
		/// 找出所有用户对象。如果是用户的容器对象，由recursively参数决定是否挖出容器内的用户。
		/// 这个操作有可能开销很大。
		/// </summary>
		/// <param name="recursively">决定是否挖出容器内的用户</param>
		/// <returns></returns>
		public virtual SchemaObjectCollection ToUsers(bool recursively)
		{
			SchemaObjectCollection result = new SchemaObjectCollection();

			foreach (SchemaObjectBase obj in this)
			{
				if (obj is SCUser)
				{
					result.AddNotExistsItem(obj);
				}
				else
				{
					if (recursively)
					{
						//如果带表达式计算的（角色和群组）
						if (obj is ISCUserContainerWithConditionObject)
						{
							result.Merge(((ISCUserContainerWithConditionObject)obj).GetAllCurrentAndCalculatedUsers());
						}
						else
						{
							//如果是一般容器（组织）
							if (obj is ISCUserContainerObject)
								result.Merge(((ISCUserContainerObject)obj).GetCurrentUsers());
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 获取简单对象的集合
		/// </summary>
		/// <returns></returns>
		public SCSimpleObjectCollection ToSimpleObjects()
		{
			SCSimpleObjectCollection result = new SCSimpleObjectCollection();

			this.ForEach(obj => result.Add(obj.ToSimpleObject()));

			return result;
		}

		/// <summary>
		/// 获取指定对象的键
		/// </summary>
		/// <param name="item">要获取其键的<see cref="SchemaObjectBase"/>的派生类型</param>
		/// <returns>表示键的字符串</returns>
		protected override string GetKeyForItem(SchemaObjectBase item)
		{
			return item.ID;
		}

		/// <summary>
		/// 创建过滤器结果的集合
		/// </summary>
		/// <returns></returns>
		protected override SchemaObjectCollection CreateFilterResultCollection()
		{
			return new SchemaObjectCollection();
		}

		/// <summary>
		/// 合并两个集合的元素（这两个集合元素应无交集），结果是一个新的集合
		/// </summary>
		/// <param name="obj">源集合</param>
		/// <param name="that">目标集合</param>
		/// <returns>新的集合</returns>
		public static SchemaObjectCollection operator +(SchemaObjectCollection obj, SchemaObjectCollection that)
		{
			SchemaObjectCollection a = new SchemaObjectCollection(obj.Count + that.Count);
			a.CopyFrom(obj);
			a.Merge(that);

			return a;
		}
	}
}
