using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using System.Collections;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 简单对象关系的基类。不像RelationObject，这种对象间的关系是不递归和级联的。
	/// 用于群组和人员、应用和角色、应用和权限、人员和秘书之间
	/// </summary>
	[Serializable]
	[ORTableMapping("SC.SchemaMembers")]
	public abstract class SCSimpleRelationBase : SchemaObjectBase
	{
		[NonSerialized]
		private SchemaObjectBase _Container = null;

		[NonSerialized]
		private SchemaObjectBase _Member = null;

		/// <summary>
		/// 初始化<see cref="SCMemberRelation"/>的新实例
		/// </summary>
		public SCSimpleRelationBase(string schemaType) :
			base(schemaType)
		{
		}

		/// <summary>
		/// 使用指定的容器对象和成员对象 初始化<see cref="SCMemberRelation"/>的新实例
		/// </summary>
		/// <param name="container">容器对象</param>
		/// <param name="member">成员对象</param>
		/// <param name="schemaType"></param>
		public SCSimpleRelationBase(SchemaObjectBase container, SchemaObjectBase member, string schemaType) :
			base(schemaType)
		{
			container.NullCheck("container");
			member.NullCheck("member");

			this.ContainerID = container.ID;
			this.ID = member.ID;

			this.ContainerSchemaType = container.SchemaType;
			this.MemberSchemaType = member.SchemaType;
		}

		/// <summary>
		/// 获取或设置容器的ID
		/// </summary>
		[ORFieldMapping("ContainerID", PrimaryKey = true)]
		public string ContainerID
		{
			get
			{
				return this.Properties.GetValue("ContainerID", string.Empty);
			}
			set
			{
				this.Properties.SetValue("ContainerID", value);
				this._Container = null;
			}
		}

		/// <summary>
		/// 获取或设置ID
		/// </summary>
		[ORFieldMapping("MemberID", PrimaryKey = true)]
		public override string ID
		{
			get
			{
				return base.ID;
			}
			set
			{
				base.ID = value;
				this._Member = null;
			}
		}

		/// <summary>
		/// 获取或设置内部排序号
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
		/// 获取或设置表示容器模式类型的字符串
		/// </summary>
		[ORFieldMapping("ContainerSchemaType")]
		public string ContainerSchemaType
		{
			get
			{
				return this.Properties.GetValue("ContainerSchemaType", string.Empty);
			}
			set
			{
				this.Properties.SetValue("ContainerSchemaType", value);
			}
		}

		/// <summary>
		/// 获取或设置表示成员模式类型的字符串
		/// </summary>
		[ORFieldMapping("MemberSchemaType")]
		public string MemberSchemaType
		{
			get
			{
				return this.Properties.GetValue("MemberSchemaType", string.Empty);
			}
			set
			{
				this.Properties.SetValue("MemberSchemaType", value);
			}
		}

		/// <summary>
		/// 获取关系的容器对象
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SchemaObjectBase Container
		{
			get
			{
				if (this._Container == null && this.ContainerID.IsNotEmpty())
				{
					this._Container = SchemaObjectAdapter.Instance.Load(this.ContainerID);

					(this._Container != null).FalseThrow("不能找到ID为{0}的父对象", this.ContainerID);
				}

				return this._Container;
			}
		}

		/// <summary>
		/// 获取关系中的成员对象
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SchemaObjectBase Member
		{
			get
			{
				if (this._Member == null && this.ID.IsNotEmpty())
				{
					this._Member = SchemaObjectAdapter.Instance.Load(this.ID);

					(this._Member != null).FalseThrow("不能找到ID为{0}的成员对象", this.ID);
				}

				return this._Member;
			}
		}
	}

	#region Collections
	/// <summary>
	/// 表示成员和容器的关系的集合
	/// </summary>
	[Serializable]
	public class SCMemberRelationCollection : SchemaObjectCollectionBase<SCSimpleRelationBase, SCMemberRelationCollection>
	{
		protected override SCMemberRelationCollection CreateFilterResultCollection()
		{
			return new SCMemberRelationCollection();
		}
	}

	/// <summary>
	/// 表示成员和容器的集合的基类
	/// </summary>
	[Serializable]
	public abstract class SCMemberRelationCollectionBase : SchemaObjectEditableKeyedCollectionBase<SCSimpleRelationBase, SCMemberRelationCollectionBase>
	{
		/// <summary>
		/// 获取根据关系类型过滤的结果集合
		/// </summary>
		/// <param name="schemaType"></param>
		/// <returns></returns>
		public SCMemberRelationCollectionBase FilterBySchemaType(string schemaType)
		{
			SCMemberRelationCollectionBase result = CreateFilterResultCollection();

			foreach (SCSimpleRelationBase mr in this)
			{
				if (mr.SchemaType == schemaType)
					result.Add(mr);
			}

			return result;
		}

		/// <summary>
		/// 获取根据容器类型过滤的结果集合
		/// </summary>
		/// <param name="schemaType"></param>
		/// <returns></returns>
		public SCMemberRelationCollectionBase FilterByContainerSchemaType(string schemaType)
		{
			SCMemberRelationCollectionBase result = CreateFilterResultCollection();

			foreach (SCSimpleRelationBase mr in this)
			{
				if (mr.ContainerSchemaType == schemaType)
					result.Add(mr);
			}

			return result;
		}

		/// <summary>
		/// 获取根据成员类型过滤的结果集合
		/// </summary>
		/// <param name="schemaType">模式类型的字符串</param>
		/// <returns><see cref="SCMemberRelationCollectionBase"/>对象</returns>
		public SCMemberRelationCollectionBase FilterByMemberSchemaType(string schemaType)
		{
			SCMemberRelationCollectionBase result = CreateFilterResultCollection();

			foreach (SCSimpleRelationBase mr in this)
			{
				if (mr.MemberSchemaType == schemaType)
					result.Add(mr);
			}

			return result;
		}
	}

	/// <summary>
	/// 对象的容器的关系集合，类似于MemberOf的集合
	/// </summary>
	[Serializable]
	public class SCObjectContainerRelationCollection : SCMemberRelationCollectionBase
	{
		public InSqlClauseBuilder ToContainerIDsBuilder()
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder("ID");

			this.ForEach(r => builder.AppendItem(r.ContainerID));

			return builder;
		}

		public string[] ToContainerIDArray()
		{
			HashSet<string> arr = new HashSet<string>();
			foreach (var item in this)
			{
				arr.Add(item.ContainerID);
			}

			return arr.ToArray();
		}

		protected override string GetKeyForItem(SCSimpleRelationBase item)
		{
			return item.ContainerID;
		}

		protected override SCMemberRelationCollectionBase CreateFilterResultCollection()
		{
			return new SCObjectContainerRelationCollection();
		}
	}

	/// <summary>
	/// 容器所包含的成员的关系集合，类似于Members集合
	/// </summary>
	[Serializable]
	public class SCObjectMemberRelationCollection : SCMemberRelationCollectionBase
	{
		public InSqlClauseBuilder ToMemberIDsBuilder()
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder("ID");

			this.ForEach(r => builder.AppendItem(r.ID));

			return builder;
		}

		/// <summary>
		/// 获取集合中指定对象的键
		/// </summary>
		/// <param name="item">要获取其键的<see cref="SCMemberRelation"/>对象</param>
		/// <returns></returns>
		protected override string GetKeyForItem(SCSimpleRelationBase item)
		{
			return item.ID;
		}

		/// <summary>
		/// 创建过滤器结果的集合
		/// </summary>
		/// <returns></returns>
		protected override SCMemberRelationCollectionBase CreateFilterResultCollection()
		{
			return new SCObjectMemberRelationCollection();
		}
	}
	#endregion Collections
}
