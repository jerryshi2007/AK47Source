using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using System.Web.Script.Serialization;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Permissions;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示管理单元
	/// </summary>
	[Serializable]
	public class AdminUnit : SCBase, ISCAclContainer, ISCRelationContainer, ISCContainerObject, IAllowAclInheritance, IPropertyExtendedObject
	{
		public AdminUnit()
			: base(AUCommon.SchemaAdminUnit)
		{
		}

		public AdminUnit(string schemaType)
			: base(schemaType)
		{

		}

		/// <summary>
		/// 获取或设置所属的管理单元Schema的ID
		/// </summary>
		[NoMapping]
		public string AUSchemaID
		{
			get { return (string)this.Properties.GetValue("AUSchemaID", string.Empty); }

			set
			{
				this.Properties.SetValue("AUSchemaID", value);
				EnsureExtendedPropertyLoaded(null);
			}
		}

		public AUSchema GetUnitSchema()
		{
			AUSchema schema = null;
			AUCommon.DoDbAction(() =>
			{
				schema = (AUSchema)SchemaObjectAdapter.Instance.Load(this.AUSchemaID, DateTime.MinValue);
			});

			return schema;
		}

		/// <summary>
		/// 获取此管理单元的管理范围的集合
		/// </summary>
		/// <returns></returns>
		public AUAdminScopeCollection GetNormalScopes()
		{
			return new AUAdminScopeCollection(Adapters.AUSnapshotAdapter.Instance.LoadAUScope(this.ID, true, DateTime.MinValue));
		}

		/// <summary>
		/// 获取此管理单元的管理范围的集合，包含删除的
		/// </summary>
		/// <returns></returns>
		public AUAdminScopeCollection GetAllScopes()
		{
			return new AUAdminScopeCollection(Adapters.AUSnapshotAdapter.Instance.LoadAUScope(this.ID, false, DateTime.MinValue));
		}

		protected override void OnIDChanged()
		{
			base.OnIDChanged();
			this._AllChildren = null;
			this._AllChildrenRelations = null;
			this._AllMemberOfRelations = null;
			this._AllMembersRelations = null;
			this._CurrentChildren = this._CurrentMembers = null;
		}

		[NonSerialized]
		private SCObjectMemberRelationCollection _AllMembersRelations = null;

		[ScriptIgnore]
		[NoMapping]
		public SCObjectMemberRelationCollection AllMembersRelations
		{
			get
			{
				if (this._AllMembersRelations == null && string.IsNullOrEmpty(this.ID) == false)
					AUCommon.DoDbAction(() =>
					this._AllMembersRelations = SCMemberRelationAdapter.Instance.LoadByContainerID(this.ID));

				return this._AllMembersRelations;
			}
		}

		/// <summary>
		/// 获取一个<see cref="SCObjectMemberRelationCollection"/>，表示当前用户的成员
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCObjectMemberRelationCollection CurrentMembersRelations
		{
			get
			{
				return (SCObjectMemberRelationCollection)AllMembersRelations.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			}
		}

		[NonSerialized]
		private SchemaObjectCollection _CurrentMembers = null;

		/// <summary>
		/// 获取用户的当前成员，一般是秘书关系
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SchemaObjectCollection CurrentMembers
		{
			get
			{
				if (this._CurrentMembers == null && string.IsNullOrEmpty(this.ID) == false)
				{
					AUCommon.DoDbAction(() =>
						this._CurrentMembers = SchemaObjectAdapter.Instance.Load(CurrentMembersRelations.ToMemberIDsBuilder()));
				}

				return _CurrentMembers;
			}
		}

		[NonSerialized]
		private SCObjectContainerRelationCollection _AllMemberOfRelations = null;

		/// <summary>
		/// 获取用户的所有成员关系的集合
		/// </summary>
		/// <value> 一个<see cref="SCObjectContainerRelationCollection"/></value>
		[ScriptIgnore]
		[NoMapping]
		public SCObjectContainerRelationCollection AllMemberOfRelations
		{
			get
			{
				if (this._AllMemberOfRelations == null && string.IsNullOrEmpty(this.ID) == false)
					AUCommon.DoDbAction(() =>
						this._AllMemberOfRelations = SCMemberRelationAdapter.Instance.LoadByMemberID(this.ID));

				return this._AllMemberOfRelations;
			}
		}

		/// <summary>
		/// 获取一个<see cref="SCObjectContainerRelationCollection"/>，表示当前用户的成员关系
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCObjectContainerRelationCollection CurrentMemberOfRelations
		{
			get
			{
				return (SCObjectContainerRelationCollection)AllMemberOfRelations.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			}
		}

		public SCObjectMemberRelationCollection GetCurrentMembersRelations()
		{
			return this.CurrentMembersRelations;
		}

		public SchemaObjectCollection GetCurrentMembers()
		{
			return this.CurrentMembers;
		}

		public SCAclMemberCollection GetAclMembers()
		{
			return AUAclAdapter.Instance.LoadByContainerID(this.ID, SchemaObjectStatus.Normal, DateTime.MinValue);
		}

		[NonSerialized]
		private SCChildrenRelationObjectCollection _AllChildrenRelations = null;

		/// <summary>
		/// 获取一个<see cref="SCChildrenRelationObjectCollection"/>，表示所有子级关系
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCChildrenRelationObjectCollection AllChildrenRelations
		{
			get
			{
				if (this._AllChildrenRelations == null && string.IsNullOrEmpty(this.ID) == false)
				{
					AUCommon.DoDbAction(() =>
						this._AllChildrenRelations = SchemaRelationObjectAdapter.Instance.LoadByParentID(this.ID));
				}

				return _AllChildrenRelations;
			}
		}

		/// <summary>
		/// 获取一个<see cref="SCChildrenRelationObjectCollection"/>，表示当前子级关系
		/// </summary>
		[ScriptIgnore]
		[NoMapping]
		public SCChildrenRelationObjectCollection CurrentChildrenRelations
		{
			get
			{
				return (SCChildrenRelationObjectCollection)AllChildrenRelations.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
			}
		}

		[NonSerialized]
		private SchemaObjectCollection _AllChildren = null;

		/// <summary>
		/// 获取一个<see cref="SchemaObjectCollection"/>，表示所有的子级
		/// </summary>
		[NoMapping]
		[ScriptIgnore]
		public SchemaObjectCollection AllChildren
		{
			get
			{
				if (this._AllChildren == null && string.IsNullOrEmpty(this.ID))
					AUCommon.DoDbAction(() =>
					this._AllChildren = SchemaObjectAdapter.Instance.Load(AllChildrenRelations.ToChildrenIDsBuilder()));

				return this._AllChildren;
			}
		}

		[NonSerialized]
		private SchemaObjectCollection _CurrentChildren = null;

		/// <summary>
		/// 获取一个表示当前的子级的<see cref="SchemaObjectCollection"/>
		/// </summary>
		[NoMapping]
		[ScriptIgnore]
		public SchemaObjectCollection CurrentChildren
		{
			get
			{
				if (this._CurrentChildren == null && string.IsNullOrEmpty(this.ID) == false)
				{
					AUCommon.DoDbAction(() =>
						this._CurrentChildren = SchemaObjectAdapter.Instance.Load(CurrentChildrenRelations.ToChildrenIDsBuilder()));
					this._CurrentChildren = this._CurrentChildren.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
				}

				return this._CurrentChildren;
			}
		}

		int ISCRelationContainer.GetCurrentChildrenCount()
		{
			return AUCommon.DoDbProcess(() => SchemaRelationObjectAdapter.Instance.GetChildrenCount(this.ID, null, DateTime.MinValue));
		}

		int ISCRelationContainer.GetCurrentMaxInnerSort()
		{
			return AUCommon.DoDbProcess(() => SchemaRelationObjectAdapter.Instance.GetMaxInnerSort(this.ID, null, DateTime.MinValue));
		}

		/// <summary>
		/// 获取最接近的父级（Schema或者另一个Unit）
		/// </summary>
		/// <returns></returns>
		public SCRelationObject GetCurrentVeryParentRelation()
		{
			SCRelationObject result = null;
			SCParentsRelationObjectCollection parents = null;
			AUCommon.DoDbAction(() =>
			{
				parents = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadByObjectID(this.ID);
			});

			result = FindActualParent(result, parents);

			return result;
		}

		private static SCRelationObject FindActualParent(SCRelationObject result, SCParentsRelationObjectCollection parents)
		{
			SCRelationObject toSchema = null, toUnit = null;
			foreach (SCRelationObject r in parents)
			{
				if (r.ParentSchemaType == AUCommon.SchemaAUSchema)
				{
					if (toSchema == null || r.Status == SchemaObjectStatus.Normal)
						toSchema = r;
				}
				else if (r.ParentSchemaType == AUCommon.SchemaAdminUnit)
				{
					if (toUnit == null || r.Status == SchemaObjectStatus.Normal)
						toUnit = r;
				}
			}

			if (toSchema == null && toUnit != null)
				result = toUnit;
			else if (toUnit == null && toSchema != null)
				result = toSchema;
			else if (toUnit == null && toSchema == null)
				result = null;
			else if (toUnit.Status == SchemaObjectStatus.Normal && toSchema.Status == SchemaObjectStatus.Normal)
				throw new AUObjectValidationException("管理单元存在错误的关系");
			else if (toUnit.Status == SchemaObjectStatus.Normal)
				result = toUnit;
			else
				result = toSchema;
			return result;
		}

		public override void ToXElement(System.Xml.Linq.XElement element)
		{
			base.ToXElement(element);
		}

		public override void FromXElement(System.Xml.Linq.XElement element)
		{
			base.FromXElement(element); //基础属性
			EnsureExtendedPropertyLoaded(element);
		}

		/// <summary>
		/// 确保扩展属性被加载
		/// </summary>
		/// <param name="element">当不为<see langword="null"/> 时 ，顺便从其中提取属性值</param>
		private void EnsureExtendedPropertyLoaded(System.Xml.Linq.XElement element)
		{
			if (string.IsNullOrEmpty(this.AUSchemaID) == false)
			{
				var key = new SchemaPropertyExtensionKey(this.AUSchemaID, this.SchemaType);
				var extension = (SchemaPropertyExtension)MCS.Library.Caching.ObjectContextCache.Instance.GetOrAddNewValue(key, (cache, vk) =>
				{
					SchemaPropertyExtensionKey k = (SchemaPropertyExtensionKey)vk;
					return SchemaPropertyExtensionAdapter.Instance.Load(k.TargetSchemaType, k.SourceID);
				});

				if (extension != null)
				{
					foreach (var item in extension.Properties)
					{
						if (this.Properties.ContainsKey(item.Name) == false)
						{
							this.Properties.Add(new SchemaPropertyValue(item));
						}
					}

					if (element != null)
					{
						foreach (var item in extension.Properties)
						{
							this.Properties[item.Name].FromXElement(element);
						}
					}
				}
			}
		}

		void IPropertyExtendedObject.EnsureExtendedProperties()
		{
			this.EnsureExtendedPropertyLoaded(null);
		}
	}
}
