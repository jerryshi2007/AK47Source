using System;
using System.Data;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using System.Diagnostics;
using MCS.Library.SOA.DataObjects.Security.Debugger;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 表示关系对象
	/// </summary>
	[Serializable]
	[ORTableMapping("SC.SchemaRelationObjects")]
	public class SCRelationObject : SchemaObjectBase
	{
		/// <summary>
		/// 初始化<see cref="SCRelationObject"/>的新实例
		/// </summary>
		public SCRelationObject() :
			base(StandardObjectSchemaType.RelationObjects.ToString())
		{
		}

		internal SCRelationObject(string schemaType)
			: base(schemaType)
		{
		}

		/// <summary>
		/// 使用指定的父子对象初始化<see cref="SCRelationObject"/>的新实例
		/// </summary>
		/// <param name="parent">父对象</param>
		/// <param name="child">子对象</param>
		public SCRelationObject(SchemaObjectBase parent, SchemaObjectBase child) :
			base(StandardObjectSchemaType.RelationObjects.ToString())
		{
			parent.NullCheck("parent");
			child.NullCheck("child");
			(parent is ISCRelationContainer).FalseThrow("parent必须实现ISCRelationContainer接口");
			(parent is ISCQualifiedNameObject).FalseThrow("此构造函数要求parent必须实现ISCQualifiedNameObject");
			(child is ISCQualifiedNameObject).FalseThrow("此构造函数要求parent必须实现ISCQualifiedNameObject");

			this.ParentID = parent.ID;
			this.ID = child.ID;

			CalculateFullPathAndGlobalSort(parent, child);

			this.ParentSchemaType = parent.SchemaType;
			this.ChildSchemaType = child.SchemaType;
		}

		internal void CalculateFullPathAndGlobalSort(SchemaObjectBase parent, SchemaObjectBase child)
		{
			this.InnerSort = ((ISCRelationContainer)parent).GetCurrentMaxInnerSort() + 1;

			string parentGlobalSort = "000000";
			string parentFullPath = ((ISCQualifiedNameObject)parent).GetQualifiedName();

			if (parent.ID == SCOrganization.GetRoot().ID)
			{
				parentGlobalSort = string.Empty;
				parentFullPath = string.Empty;
			}
			else
			{
				if (parent.CurrentParentRelations.Count > 0)
				{
					if (parent.CurrentParentRelations[0].FullPath.IsNotEmpty())
						parentFullPath = parent.CurrentParentRelations[0].FullPath;

					if (parent.CurrentParentRelations[0].GlobalSort.IsNotEmpty())
						parentGlobalSort = parent.CurrentParentRelations[0].GlobalSort;
				}
			}

			string childName = ((ISCQualifiedNameObject)child).GetQualifiedName();

			if (parentFullPath.IsNotEmpty())
				this.FullPath = parentFullPath + "\\" + childName;
			else
				this.FullPath = childName;

			this.GlobalSort = parentGlobalSort + string.Format("{0:000000}", this.InnerSort);
		}

		/// <summary>
		/// 获取或设置父级ID
		/// </summary>
		[ORFieldMapping("ParentID", PrimaryKey = true)]
		public string ParentID
		{
			get
			{
				return this.Properties.GetValue("ParentID", string.Empty);
			}
			set
			{
				this.Properties.SetValue("ParentID", value);
				this._Parent = null;
			}
		}

		/// <summary>
		/// 获取或设置ID
		/// </summary>
		[ORFieldMapping("ObjectID", PrimaryKey = true)]
		public override string ID
		{
			get
			{
				return base.ID;
			}
			set
			{
				base.ID = value;
				this._Child = null;
			}
		}

		/// <summary>
		/// 获取或设置一个<see cref="int"/>值，表示内部排序号
		/// </summary>
		[ORFieldMapping("InnerSort")]
		public int InnerSort
		{
			get
			{
				return this.Properties.GetValue("InnerSort", 0);
			}
			set
			{
				this.Properties.SetValue("InnerSort", value);
			}
		}

		/// <summary>
		/// 获取或设置一个表示父级模式类型的字符串
		/// </summary>
		[ORFieldMapping("ParentSchemaType")]
		public string ParentSchemaType
		{
			get
			{
				return this.Properties.GetValue("ParentSchemaType", string.Empty);
			}
			set
			{
				this.Properties.SetValue("ParentSchemaType", value);
			}
		}

		/// <summary>
		/// 获取或设置一个表示子级模式类型的字符串
		/// </summary>
		[ORFieldMapping("ChildSchemaType")]
		public string ChildSchemaType
		{
			get
			{
				return this.Properties.GetValue("ChildSchemaType", string.Empty);
			}
			set
			{
				this.Properties.SetValue("ChildSchemaType", value);
			}
		}

		/// <summary>
		/// 获取或设置一个<see cref="bool"/>值，表示关系是否是缺省关系。
		/// </summary>
		[ORFieldMapping("IsDefault")]
		public bool Default
		{
			get
			{
				return this.Properties.GetValue("Default", true);
			}
			set
			{
				this.Properties.SetValue("Default", value);
			}
		}

		/// <summary>
		/// 全路径
		/// </summary>
		[ORFieldMapping("FullPath")]
		public string FullPath
		{
			get
			{
				return this.Properties.GetValue("FullPath", string.Empty);
			}
			set
			{
				this.Properties.SetValue("FullPath", value);
			}
		}

		/// <summary>
		/// 全局序号
		/// </summary>
		[ORFieldMapping("GlobalSort")]
		public string GlobalSort
		{
			get
			{
				return this.Properties.GetValue("GlobalSort", string.Empty);
			}
			set
			{
				this.Properties.SetValue("GlobalSort", value);
			}
		}

		[NonSerialized]
		private SchemaObjectBase _Parent = null;

		/// <summary>
		/// 获取表示关系的父级对象的<see cref="SchemaObjectBase"/>派生类型的实例
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SchemaObjectBase Parent
		{
			get
			{
				if (this._Parent == null && this.ParentID.IsNotEmpty())
				{
					this._Parent = (SchemaObjectBase)SchemaObjectAdapter.Instance.Load(this.ParentID);

					(this._Parent != null).FalseThrow("不能找到ID为{0}的父对象", this.ParentID);
				}

				return this._Parent;
			}
		}

		[NonSerialized]
		private SchemaObjectBase _Child = null;

		/// <summary>
		/// 获取表示关系的子级对象的<see cref="SchemaObjectBase"/>派生类型的实例
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SchemaObjectBase Child
		{
			get
			{
				if (this._Child == null && this.ID.IsNotEmpty())
				{
					this._Child = (SchemaObjectBase)SchemaObjectAdapter.Instance.Load(this.ID);

					(this._Child != null).FalseThrow("不能找到ID为{0}的子对象", this.ID);
				}

				return this._Child;
			}
		}
	}

	/// <summary>
	/// 表示关系对象的集合
	/// </summary>
	[Serializable]
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(SCRelationObjectCollection.ListDebuggerView))]
	public class SCRelationObjectCollection : SchemaObjectCollectionBase<SCRelationObject, SCRelationObjectCollection>
	{
		/// <summary>
		/// 将关系的父级的ID附加作为<see cref="InSqlClauseBuilder"/>的条件
		/// </summary>
		/// <returns>包含了父级ID条件的<see cref="InSqlClauseBuilder"/></returns>
		public InSqlClauseBuilder ToParentIDsBuilder()
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder("ID");

			this.ForEach(r => builder.AppendItem(r.ParentID));

			return builder;
		}

		/// <summary>
		/// 将关系的孩子的ID附加作为<see cref="InSqlClauseBuilder"/>的条件
		/// </summary>
		/// <returns>包含了孩子ID条件的<see cref="InSqlClauseBuilder"/></returns>
		public InSqlClauseBuilder ToChildrenIDsBuilder()
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder("ID");

			this.ForEach(r => builder.AppendItem(r.ID));

			return builder;
		}

		/// <summary>
		/// 创建过滤器结果的集合
		/// </summary>
		/// <returns></returns>
		protected override SCRelationObjectCollection CreateFilterResultCollection()
		{
			return new SCRelationObjectCollection();
		}

		/// <summary>
		/// 获取关系的所有父级的Id，重复的会被合并
		/// </summary>
		/// <returns></returns>
		public string[] ToParentIDArray()
		{
			System.Collections.Generic.HashSet<string> set = new System.Collections.Generic.HashSet<string>();
			foreach (var item in this)
			{
				set.Add(item.ParentID);
			}
			string[] result = new string[set.Count];

			set.CopyTo(result);

			return result;
		}

		class ListDebuggerView
		{
			private SCRelationObjectCollection collection = null;

			public ListDebuggerView(SCRelationObjectCollection collection)
			{
				this.collection = collection;
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public ListRelations[] Objects
			{
				get
				{
					ListRelations[] keys = new ListRelations[this.collection.Count];

					int i = 0;
					foreach (SCRelationObject obj in collection)
					{
						keys[i] = new ListRelations(this.collection, obj.ID, obj);
						i++;
					}

					return keys;
				}
			}
		}
	}

	/// <summary>
	/// 包含父子关系的集合类的抽象基类
	/// </summary>
	[Serializable]
	public abstract class SCParentsOrChildrenRelationObjectCollectionBase : SchemaObjectEditableKeyedCollectionBase<SCRelationObject, SCParentsOrChildrenRelationObjectCollectionBase>
	{
		public int GetMaxInnerSort()
		{
			int result = 0;

			foreach (SCRelationObject relation in this)
			{
				if (relation.InnerSort > result)
					result = relation.InnerSort;
			}

			return result;
		}

		/// <summary>
		/// 得到默认的父对象
		/// </summary>
		/// <returns></returns>
		public SchemaObjectBase GetDefaultParent()
		{
			SchemaObjectBase result = null;

			SCRelationObject relation = this.Find(r => r.Default);

			if (relation != null)
				result = relation.Parent;

			return result;
		}

		/// <summary>
		/// 得到默认的子对象
		/// </summary>
		/// <returns></returns>
		public SchemaObjectBase GetDefaultChild()
		{
			SchemaObjectBase result = null;

			SCRelationObject relation = this.Find(r => r.Default);

			if (relation != null)
				result = relation.Child;

			return result;
		}
	}

	[Serializable]
	public class SCChildrenRelationObjectCollection : SCParentsOrChildrenRelationObjectCollectionBase
	{
		protected override string GetKeyForItem(SCRelationObject item)
		{
			return item.ID;
		}

		public InSqlClauseBuilder ToChildrenIDsBuilder()
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder("ID");

			this.ForEach(r => builder.AppendItem(r.ID));

			return builder;
		}

		protected override SCParentsOrChildrenRelationObjectCollectionBase CreateFilterResultCollection()
		{
			return new SCChildrenRelationObjectCollection();
		}
	}

	[Serializable]
	public class SCParentsRelationObjectCollection : SCParentsOrChildrenRelationObjectCollectionBase
	{
		protected override string GetKeyForItem(SCRelationObject item)
		{
			return item.ParentID;
		}

		public InSqlClauseBuilder ToParentIDsBuilder()
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder("ID");

			this.ForEach(r => builder.AppendItem(r.ParentID));

			return builder;
		}

		protected override SCParentsOrChildrenRelationObjectCollectionBase CreateFilterResultCollection()
		{
			return new SCParentsRelationObjectCollection();
		}
	}
}
