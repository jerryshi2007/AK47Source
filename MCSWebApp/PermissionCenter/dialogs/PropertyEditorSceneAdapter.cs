using System;
using MCS.Library.Core;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace PermissionCenter
{
	internal abstract class PropertyEditorSceneAdapter
	{
		private string parentID = null;

		private PC.Permissions.SCContainerAndPermissionCollection containerPermissions = null;

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

		protected PC.Permissions.SCContainerAndPermissionCollection ContainerPermissions
		{
			get
			{
				if (this.containerPermissions == null)
				{
					if (this.ParentID != null)
					{
						this.containerPermissions = PC.Adapters.SCAclAdapter.Instance.LoadCurrentContainerAndPermissions(Util.CurrentUser.ID, new string[] { this.ParentID });
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
				case "Users":
					return new UsersPropertyEditorSceneAdapter();
				case "Organizations":
					return new OrganizationPropertyEditorSceneAdapter();
				case "Groups":
					return new GroupPropertyEditorSceneAdapter();
				case "Roles":
					return new RolePropertyEditorSceneAdapter();
				case "Applications":
					return new ApplicationPropertyEditorSceneAdapter();
				case "Permissions":
					return new PermissionPropertyEditorSceneAdapter();
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
						if (this.ParentMustPresent && this.ParentID == null)
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

	internal sealed class UsersPropertyEditorSceneAdapter : PropertyEditorSceneAdapter
	{
		protected override bool IsEditEnabled()
		{
			return this.HasPermission("UpdateChildren");
		}

		protected override bool IsCreateEnabled()
		{
			return this.HasPermission("AddChildren");
		}

		/// <summary>
		/// 初始化主职
		/// </summary>
		protected override void InitializeParent()
		{
			var parentRelation = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadByObjectID(this.ObjectID);

			this.ParentID = null;

			foreach (var item in parentRelation)
			{
				if (item.ParentSchemaType == "Organizations" && item.Status == SchemaObjectStatus.Normal && item.Default)
				{
					this.ParentID = item.ParentID;
					break;
				}
			}
		}
	}

	internal sealed class GroupPropertyEditorSceneAdapter : PropertyEditorSceneAdapter
	{
		public override bool ParentMustPresent
		{
			get
			{
				return true;
			}
		}

		protected override bool IsEditEnabled()
		{
			return this.HasPermission("UpdateChildren");
		}

		protected override bool IsCreateEnabled()
		{
			return this.HasPermission("AddChildren");
		}

		protected override void InitializeParent()
		{
			this.ParentID = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadByObjectID(this.ObjectID).Find(m => m.Status == SchemaObjectStatus.Normal && m.ParentSchemaType == "Organizations").ParentID;
		}
	}

	internal sealed class OrganizationPropertyEditorSceneAdapter : PropertyEditorSceneAdapter
	{
		public override bool ParentMustPresent
		{
			get
			{
				return true;
			}
		}

		protected override bool IsEditEnabled()
		{
			return this.HasPermission("UpdateChildren");
		}

		protected override bool IsCreateEnabled()
		{
			return this.HasPermission("AddChildren");
		}

		protected override void InitializeParent()
		{
			this.ParentID = PC.Adapters.SchemaRelationObjectAdapter.Instance.LoadByObjectID(this.ObjectID).Find(m => m.Status == SchemaObjectStatus.Normal && m.ParentSchemaType == "Organizations").ParentID;
		}
	}

	internal sealed class ApplicationPropertyEditorSceneAdapter : PropertyEditorSceneAdapter
	{
		protected override void InitializeParent()
		{
			this.ParentID = this.ObjectID;
		}

		protected override bool IsEditEnabled()
		{
			return this.HasPermission("UpdateApplications");
		}

		protected override bool IsCreateEnabled()
		{
			return false;
		}
	}

	internal sealed class RolePropertyEditorSceneAdapter : PropertyEditorSceneAdapter
	{
		public override bool ParentMustPresent
		{
			get
			{
				return true;
			}
		}

		protected override void InitializeParent()
		{
			this.ParentID = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByMemberID(this.ObjectID).Find(m => m.Status == SchemaObjectStatus.Normal && m.ContainerSchemaType == "Applications").ContainerID;
		}

		protected override bool IsEditEnabled()
		{
			return this.HasPermission("UpdateRoles");
		}

		protected override bool IsCreateEnabled()
		{
			return this.HasPermission("AddRoles");
		}
	}

	internal sealed class PermissionPropertyEditorSceneAdapter : PropertyEditorSceneAdapter
	{
		public override bool ParentMustPresent
		{
			get
			{
				return true;
			}
		}

		protected override void InitializeParent()
		{
			this.ParentID = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByMemberID(this.ObjectID).Find(m => m.Status == SchemaObjectStatus.Normal && m.ContainerSchemaType == "Applications").ContainerID;
		}

		protected override bool IsEditEnabled()
		{
			return this.HasPermission("UpdatePermissions");
		}

		protected override bool IsCreateEnabled()
		{
			return this.HasPermission("AddPermissions");
		}
	}
}