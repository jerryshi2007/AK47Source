using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;
using PC = MCS.Library.SOA.DataObjects.Security;
using System.Web.Script.Serialization;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	/// <summary>
	/// 表示管理单元架构
	/// </summary>
	[Serializable]
	public class AUSchema : SCBase, ISCRelationContainer
	{
		private SchemaObjectCollection _AllChildren;
		private SchemaObjectCollection _CurrentlChildren;
		private SCChildrenRelationObjectCollection _AllChildrenRelations;
		public AUSchema()
			: base(AUCommon.SchemaAUSchema)
		{
		}

		public AUSchema(string schemaType)
			: base(schemaType)
		{
		}

		/// <summary>
		/// 获取或设置架构上级分类的ID
		/// </summary>
		[NoMapping]
		public string CategoryID
		{
			get
			{
				return (string)this.Properties.GetValue("CategoryID", string.Empty);
			}

			set
			{
				this.Properties.SetValue("CategoryID", value);
			}
		}

		/// <summary>
		/// 获取或设置以逗号分隔的管理范围类型名
		/// </summary>
		[NoMapping]
		public string Scopes
		{
			get
			{
				return (string)this.Properties.GetValue("Scopes", string.Empty);
			}

			set
			{
				this.Properties.SetValue("Scopes", value);
			}
		}

		/// <summary>
		/// 获取或设置此管理架构的管理角色（AppName:CodeName）
		/// </summary>
		[NoMapping]
		public string MasterRole
		{
			get
			{
				return (string)this.Properties.GetValue("MasterRole", string.Empty);
			}

			set
			{
				this.Properties.SetValue("MasterRole", value);
			}
		}

		/// <summary>
		/// 获取一个集合，包含此架构中定义的角色(含删除状态的)
		/// </summary>
		[NoMapping]
		public AUSchemaRoleCollection SchemaRoles
		{
			get
			{
				return Adapters.AUSnapshotAdapter.Instance.LoadAUSchemaRoles(this.ID, false, DateTime.MinValue);
			}
		}

		[ScriptIgnore]
		[NoMapping]
		public SCChildrenRelationObjectCollection AllChildrenRelations
		{
			get
			{
				if (this._AllChildrenRelations == null && this.ID.IsNotEmpty())
				{
					AUCommon.DoDbAction(() =>
						this._AllChildrenRelations = SchemaRelationObjectAdapter.Instance.LoadByParentID(this.ID));
				}

				return _AllChildrenRelations;
			}
		}

		[NoMapping]
		[ScriptIgnore]
		public SchemaObjectCollection AllChildren
		{
			get
			{
				if (this._AllChildren == null && this.ID.IsNotEmpty())
					AUCommon.DoDbAction(() => this._AllChildren = SchemaObjectAdapter.Instance.Load(AllChildrenRelations.ToChildrenIDsBuilder()));

				return this._AllChildren;
			}
		}

		[NoMapping]
		[ScriptIgnore]
		public SchemaObjectCollection CurrentChildren
		{
			get
			{
				if (this._CurrentlChildren == null && this.ID.IsNotEmpty())
				{
					AUCommon.DoDbAction(() =>
					this._CurrentlChildren = SchemaObjectAdapter.Instance.Load(CurrentChildrenRelations.ToChildrenIDsBuilder()));
					this._CurrentlChildren = this._CurrentlChildren.FilterByStatus(SchemaObjectStatusFilterTypes.Normal);
				}

				return this._CurrentlChildren;
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

		protected override void OnIDChanged()
		{
			base.OnIDChanged();

			this._AllChildrenRelations = null;
			this._AllChildren = null;
			this._CurrentlChildren = null;
		}

		int ISCRelationContainer.GetCurrentChildrenCount()
		{
			return AUCommon.DoDbProcess(() => SchemaRelationObjectAdapter.Instance.GetChildrenCount(this.ID, null, DateTime.MinValue));
		}

		int ISCRelationContainer.GetCurrentMaxInnerSort()
		{
			return AUCommon.DoDbProcess(() => SchemaRelationObjectAdapter.Instance.GetMaxInnerSort(this.ID, null, DateTime.MinValue));
		}
	}
}
