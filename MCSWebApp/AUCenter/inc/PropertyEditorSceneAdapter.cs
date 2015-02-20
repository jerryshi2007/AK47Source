using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;

namespace AUCenter
{
	internal abstract class PropertyEditorSceneAdapter
	{
		private string parentID = null;

		private SCContainerAndPermissionCollection containerPermissions = null;

		public SCObjectOperationMode Mode { get; set; }

		public string ParentID
		{
			get
			{
				return this.parentID;
			}

			set
			{
				this.parentID = value;
				this.containerPermissions = null;
			}
		}

		public virtual string ObjectID { get; set; }

		/// <summary>
		/// 在派生类中重写时，返回表示创建对象时是否必须指定父级ID
		/// </summary>
		public virtual bool ParentMustPresent
		{
			get { return false; }
		}

		protected SCContainerAndPermissionCollection ContainerPermissions
		{
			get
			{
				if (this.containerPermissions == null)
				{
					if (this.ParentID != null)
					{
						this.containerPermissions = AUAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.ParentID });
					}
				}

				return this.containerPermissions;
			}
		}

		public static PropertyEditorSceneAdapter Create(MCS.Library.SOA.DataObjects.Security.SchemaObjectBase schemaObj)
		{
			var category = ObjectSchemaSettings.GetConfig().Schemas[schemaObj.SchemaType].Category;
			return PropertyEditorSceneAdapter.CreateByCategory(category);
		}

		public static PropertyEditorSceneAdapter Create(string schemaType)
		{
			var category = ObjectSchemaSettings.GetConfig().Schemas[schemaType].Category;
			return PropertyEditorSceneAdapter.CreateByCategory(category);
		}

		public static PropertyEditorSceneAdapter CreateByCategory(string category)
		{
			switch (category)
			{
				case "AUSchemas":
					return new AUSchemasPropertyEditorSceneAdapter();
				case "AUSchemaRoles":
					return new AUSchemaRolesPropertyEditorSceneAdapter();
				case "AdminUnits":
					return new AdminUnitsPropertyEditorSceneAdapter();
				default:
					throw new NotSupportedException("不支持的Schema类型");
			}
		}

		/// <summary>
		/// 检查当前用户是否可编辑对象（不考虑godMode）
		/// </summary>
		/// <returns></returns>
		public virtual bool IsEditable()
		{
			if (TimePointContext.Current.UseCurrentTime)
			{
				switch (this.Mode)
				{
					case SCObjectOperationMode.Add:
						if (this.ParentMustPresent && string.IsNullOrEmpty(this.ParentID))
							throw new InvalidOperationException("创建此类对象时必须指定父级ID");
						return this.IsCreateEnabled();
					case SCObjectOperationMode.Update:
						this.InitializeParent();
						return this.IsEditEnabled();
					default:
						throw new InvalidOperationException("操作模式必须为Add或者Update值之一");
				}
			}

			return false;
		}

		/// <summary>
		/// 编辑对象时，初始化父级ID
		/// </summary>
		protected virtual void InitializeParent()
		{
		}

		protected abstract bool IsEditEnabled();

		protected abstract bool IsCreateEnabled();

		protected bool HasPermission(string permission)
		{
			return Util.ContainsPermission(this.ContainerPermissions, this.ParentID, permission);
		}
	}

	internal sealed class AUSchemasPropertyEditorSceneAdapter : PropertyEditorSceneAdapter
	{

		protected override bool IsEditEnabled()
		{
			return AUPermissionHelper.IsSupervisor(DeluxePrincipal.Current);
		}

		protected override bool IsCreateEnabled()
		{
			return AUPermissionHelper.IsSupervisor(DeluxePrincipal.Current);
		}
	}

	internal sealed class AUSchemaRolesPropertyEditorSceneAdapter : PropertyEditorSceneAdapter
	{
		protected override void InitializeParent()
		{
			//this.ParentID = AUCommon.DoDbProcess(() => SCMemberRelationAdapter.Instance.LoadByMemberID(this.ObjectID, AUCommon.SchemaAUSchema).Find(m => m.Status == SchemaObjectStatus.Normal).ContainerID);
		}

		protected override bool IsEditEnabled()
		{
			return AUPermissionHelper.IsSupervisor(DeluxePrincipal.Current);
		}

		protected override bool IsCreateEnabled()
		{
			return AUPermissionHelper.IsSupervisor(DeluxePrincipal.Current);
		}
	}

	internal sealed class AdminUnitsPropertyEditorSceneAdapter : PropertyEditorSceneAdapter
	{
		private AUSchema schema;

		internal AUSchema Schema
		{
			get
			{
				if (this.schema == null)
				{
					this.schema = DbUtil.GetEffectiveObject<AdminUnit>(this.ObjectID).GetUnitSchema();
				}

				return schema;
			}
		}

		public AdminUnitsPropertyEditorSceneAdapter()
		{
		}

		public AdminUnitsPropertyEditorSceneAdapter(AUSchema schema)
		{
			(this.schema = schema).NullCheck("schmea");
		}

		public override bool ParentMustPresent
		{
			get
			{
				return false;
			}
		}

		protected override void InitializeParent()
		{
			var relation = AUCommon.DoDbProcess(() => SchemaRelationObjectAdapter.Instance.LoadByObjectID(this.ObjectID, AUCommon.SchemaAdminUnit).Find(m => m.Status == SchemaObjectStatus.Normal));
			if (relation != null)
				this.ParentID = relation.ParentID;
			else
				this.ParentID = string.Empty;
		}

		protected override bool IsEditEnabled()
		{
			bool enabled = IsASuperVisior();
			if (enabled == false)
			{
				var acls = AUAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.ObjectID });
				enabled = Util.ContainsPermission(acls, this.ParentID, "EditProperty");
			}

			return enabled;
		}

		private bool IsASuperVisior()
		{
			bool enabled = AUPermissionHelper.IsSupervisor(DeluxePrincipal.Current);
			if (enabled == false)
			{
				if (string.IsNullOrEmpty(Schema.MasterRole) == false)
				{
					enabled = DeluxePrincipal.Current.IsInRole(this.Schema.MasterRole);
				}
			}

			return enabled;
		}

		protected override bool IsCreateEnabled()
		{
			bool enabled = IsASuperVisior();
			if (enabled == false)
			{
				enabled = this.HasPermission("AddSubUnit");
			}

			return enabled;
		}
	}
}