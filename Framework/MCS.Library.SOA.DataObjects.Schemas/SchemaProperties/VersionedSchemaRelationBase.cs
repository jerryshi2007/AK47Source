using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Schemas.SchemaProperties
{
	/// <summary>
	/// 简单对象关系的基类。不像RelationObject，这种对象间的关系是不递归和级联的。
	/// 用于群组和人员、应用和角色、应用和权限、人员和秘书之间
	/// </summary>
	[Serializable]
	public abstract class VersionedSchemaRelationBase : VersionedSchemaObjectBase
	{
		[NonSerialized]
		private VersionedSchemaObjectBase _Container = null;

		[NonSerialized]
		private VersionedSchemaObjectBase _Member = null;

		/// <summary>
		/// 初始化<see cref="VersionedSchemaRelationBase"/>的新实例
		/// </summary>
		public VersionedSchemaRelationBase(string schemaType) :
			base(schemaType)
		{
		}

		/// <summary>
		/// 使用指定的容器对象和成员对象 初始化<see cref="SCMemberRelation"/>的新实例
		/// </summary>
		/// <param name="container">容器对象</param>
		/// <param name="member">成员对象</param>
		/// <param name="schemaType"></param>
		public VersionedSchemaRelationBase(VersionedSchemaObjectBase container, VersionedSchemaObjectBase member, string schemaType) :
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
		public VersionedSchemaObjectBase Container
		{
			get
			{
				if (this._Container == null && this.ContainerID.IsNotEmpty())
				{
					this._Container = LoadObjectByID(this.ContainerID);

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
		public VersionedSchemaObjectBase Member
		{
			get
			{
				if (this._Member == null && this.ID.IsNotEmpty())
				{
					this._Member = LoadObjectByID(this.ID);

					(this._Member != null).FalseThrow("不能找到ID为{0}的成员对象", this.ID);
				}

				return this._Member;
			}
		}

		/// <summary>
		/// 根据对象的ID加载对象
		/// </summary>
		/// <param name="containerID"></param>
		/// <returns></returns>
		protected abstract VersionedSchemaObjectBase LoadObjectByID(string containerID);
	}
}
