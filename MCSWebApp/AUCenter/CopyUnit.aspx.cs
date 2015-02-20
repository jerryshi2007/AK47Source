using System;
using System.Web;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Web.WebControls;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using PC = MCS.Library.SOA.DataObjects.Security;
using System.Linq;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using System.Web.Services;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.SOA.DataObjects.Security.Validators;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Web.Library;

namespace AUCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class CopyUnit : System.Web.UI.Page
	{
		internal class StepContext
		{
			/// <summary>
			/// 总步数
			/// </summary>
			public int TotalSteps { get; set; }
			/// <summary>
			/// 已完成的总步数
			/// </summary>
			public int PassedSteps { get; set; }
			/// <summary>
			/// 分部步数系数
			/// </summary>
			public double Div { get; set; }
			/// <summary>
			/// 分部完成步数
			/// </summary>
			public int InnerStep { get; set; }
			/// <summary>
			/// 分部总步数
			/// </summary>
			public int InnerTotalSteps { get; set; }

			public System.IO.TextWriter Logger { get { return ProcessProgress.Current.Output; } }

			internal void ResetInnerSteps()
			{
				InnerStep = 0;
				InnerTotalSteps = 0;
			}

			internal void Response()
			{
				if (InnerTotalSteps == 0)
					ProcessProgress.Current.CurrentStep = Range100(((PassedSteps) * Div));
				else
					ProcessProgress.Current.CurrentStep = Range100((PassedSteps + (InnerStep / (double)InnerTotalSteps)) * Div);
				ProcessProgress.Current.Response();
			}

			private static int Range100(double value)
			{
				if (value < 1)
					value = 1;
				else if (value > 100)
					value = 100;

				return (int)value;
			}

			public void ResetInnerSteps(int totalInnerSteps)
			{
				InnerStep = 0;
				InnerTotalSteps = totalInnerSteps;
			}
		}

		[Serializable]
		public class ValidationResult
		{
			public ValidationResult()
			{
				this.ObjectValidationResult = this.NameValidationResult = this.CodeNameValidationResult = this.TargetValidationResult = "未验证";
				this.Passed = true;
			}

			public string NameValidationResult { get; set; }

			public string CodeNameValidationResult { get; set; }

			public string TargetValidationResult { get; set; }

			public string ObjectValidationResult { get; set; }

			public bool Passed { get; set; }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Util.EnsureOperationSafe();
			if (Page.IsPostBack == false)
			{
				Initialize();
			}
		}

		private void Initialize()
		{
			string fromUnitID = Request.QueryString["fromUnit"];
			if (string.IsNullOrEmpty(fromUnitID))
				throw new HttpException("必须提供fromUnit");

			var unit = DbUtil.GetEffectiveObject<AU.AdminUnit>(fromUnitID);
			this.txtNewCodeName.Text = unit.CodeName + "1";
			this.txtNewName.Text = unit.Name + "1";

			this.hfFromUnit.Value = fromUnitID;
			this.hfSchemaID.Value = unit.AUSchemaID;
			this.tree.CallBackContext = unit.AUSchemaID;

			var schema = unit.GetUnitSchema();

			this.InitizeTree(fromUnitID, schema);
		}

		[WebMethod]
		public static ValidationResult DoClientValidation(string schemaID, string parentID, string name, string codeName)
		{
			ValidationResult result = new ValidationResult();

			ValidateCodeName(codeName, parentID, result);

			if (result.Passed)
			{
				var schema = (AU.AUSchema)AU.Adapters.AUSnapshotAdapter.Instance.LoadAUSchema(schemaID, true, DateTime.MinValue).FirstOrDefault();
				SchemaObjectBase targetUnit = null;
				if (schema == null)
				{
					result.Passed = false;
					result.TargetValidationResult = "管理架构不存在或已删除";
				}
				else
				{
					bool hasPermission;
					hasPermission = AUPermissionHelper.IsSupervisor(DeluxePrincipal.Current) || (string.IsNullOrEmpty(schema.MasterRole) == false && DeluxePrincipal.Current.IsInRole(schema.MasterRole));
					if (parentID == schemaID || parentID == null)
					{
						targetUnit = schema;
					}
					else
					{
						targetUnit = AUCommon.DoDbProcess(() => (AdminUnit)PC.Adapters.SchemaObjectAdapter.Instance.Load(parentID));
						if (hasPermission == false)
							hasPermission = CheckAddSubPermission(schema, (AdminUnit)targetUnit);
					}

					result.Passed &= hasPermission;
					result.TargetValidationResult = hasPermission ? "通过" : "没有在目标添加子单元的权限";

					if (result.Passed)
					{
						ValidateName(name, codeName, result, schema, targetUnit);
					}
				}
			}

			return result;
		}

		private static void ValidateName(string name, string codeName, ValidationResult result, AUSchema schema, SchemaObjectBase targetUnit)
		{
			var actualParent = (SchemaObjectBase)targetUnit ?? schema;

			var adminUnit = new AdminUnit()
			{
				ID = UuidHelper.NewUuidString(),
				CodeName = codeName,
				Name = name,
				AUSchemaID = schema.ID
			};

			var validationObjResult = adminUnit.Validate();
			result.ObjectValidationResult = validationObjResult.ResultCount > 0 ? ToMessage(validationObjResult.First()) : "通过";
			result.Passed &= validationObjResult.ResultCount == 0;

			if (result.Passed)
			{
				SCRelationObject relation = new SCRelationObject(actualParent, adminUnit);
				var existedObj = AU.Adapters.AUSnapshotAdapter.Instance.LoadAUByChildName(name, actualParent.ID, AUCommon.SchemaAdminUnit, true, DateTime.MinValue);
				result.NameValidationResult = existedObj.Count > 0 ? "此名称已经被占用，请使用其他名称" : "通过";
				result.Passed &= existedObj.Count == 0;
			}
		}

		private static string ToMessage(MCS.Library.Validation.ValidationResult validationResult)
		{
			return string.Format("属性验证“{0}”:{1}", validationResult.Key, validationResult.Message);
		}

		private static void ValidateCodeName(string codeName, string parentID, ValidationResult result)
		{
			if (string.IsNullOrEmpty(codeName))
			{
				result.CodeNameValidationResult = "代码名称不得为空";
				result.Passed = false;
			}
			else
			{
				var codeNameValid = AUCommon.DoDbProcess<bool>(() => CodeNameUniqueValidatorFacade.Validate(codeName, "SomeNewID", AUCommon.SchemaAdminUnit, parentID, false, false, DateTime.MinValue));

				result.Passed &= codeNameValid;
				result.CodeNameValidationResult = codeNameValid ? "通过" : string.Format("指定的代码名称 {0} 已经被其他对象使用，请使用其他名称。", codeName);
			}
		}

		private static bool CheckAddSubPermission(AUSchema schema, AdminUnit targetUnit)
		{
			bool result = false;

			result = AU.AUPermissionHelper.IsSupervisor(DeluxePrincipal.Current);
			if (result == false)
			{
				if (string.IsNullOrEmpty(schema.MasterRole) == false)
				{
					result = DeluxePrincipal.Current.IsInRole(schema.MasterRole);

					if (result == false && targetUnit != null)
					{
						var permissions = AU.Adapters.AUAclAdapter.Instance.LoadCurrentContainerAndPermissions(DeluxeIdentity.CurrentUser.ID, new string[] { targetUnit.ID });

						result = Util.ContainsPermission(permissions, targetUnit.ID, "AddSubUnit");
					}
				}
			}

			return result;
		}

		private void InitizeTree(string fromUnitID, AUSchema schema)
		{
			tree.Nodes.Clear();

			var root = AddSchemaToTree(schema, tree.Nodes);
			var subUnits = AU.Adapters.AUSnapshotAdapter.Instance.LoadSubUnits(this.hfSchemaID.Value, this.hfSchemaID.Value, true, DateTime.MinValue);
			foreach (AU.AdminUnit item in subUnits)
			{
				AddUnitToTree(item, root.Nodes);
			}
		}

		private void AddUnitToTree(AU.AdminUnit item, MCS.Web.WebControls.DeluxeTreeNodeCollection treeNodes)
		{
			treeNodes.Add(new MCS.Web.WebControls.DeluxeTreeNode(item.Name, item.ID)
			{
				NodeOpenImg = ControlResources.OULogoUrl,
				NodeCloseImg = ControlResources.OULogoUrl,
				CssClass = "au-catenode",
				ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading,
				ExtendedData = "AU",
				SubNodesLoaded = false
			});
		}

		private DeluxeTreeNode AddSchemaToTree(AU.AUSchema item, MCS.Web.WebControls.DeluxeTreeNodeCollection treeNodes)
		{
			DeluxeTreeNode node = new MCS.Web.WebControls.DeluxeTreeNode(item.Name, item.ID)
			{
				NodeOpenImg = ControlResources.OULogoUrl,
				NodeCloseImg = ControlResources.OULogoUrl,
				CssClass = "au-catenode",
				ChildNodesLoadingType = ChildNodesLoadingTypeDefine.Normal,
				ExtendedData = "Schema",
				Expanded = true
			};

			treeNodes.Add(node);

			return node;
		}

		protected void tree_GetChildrenData(MCS.Web.WebControls.DeluxeTreeNode parentNode, MCS.Web.WebControls.DeluxeTreeNodeCollection result, string callBackContext)
		{
			var subUnits = AU.Adapters.AUSnapshotAdapter.Instance.LoadSubUnits(callBackContext, parentNode.Value, true, DateTime.MinValue);

			foreach (AU.AdminUnit item in subUnits)
			{
				AddUnitToTree(item, result);
			}
		}

		protected void Processing(object sender, MCS.Web.WebControls.PostProgressDoPostedDataEventArgs e)
		{
			ProcessProgress.Current.MinStep = 0;
			ProcessProgress.Current.MaxStep = ProcessProgress.Current.CurrentStep = 1;
			StepContext context = new StepContext();

			try
			{
				string fromUnitID = (string)e.Steps[0];
				string newName = (string)e.Steps[1];
				string newCodeName = (string)e.Steps[2];
				string toUnitID = (string)e.Steps[3];
				bool copyRoleMembers = (bool)e.Steps[4];
				bool copyScopeMembers = (bool)e.Steps[5];
				bool copyScopeConditions = (bool)e.Steps[6];

				var fromUnit = DbUtil.GetEffectiveObject<AU.AdminUnit>(fromUnitID);
				string schemaID = fromUnit.AUSchemaID;
				if (string.IsNullOrEmpty(schemaID))
					throw new AUObjectException("无法获取要复制单元的架构ID");

				var targetParent = toUnitID == schemaID ? null : DbUtil.GetEffectiveObject<AU.AdminUnit>(toUnitID);
				if (targetParent != null && fromUnit.AUSchemaID != targetParent.AUSchemaID)
					throw new AU.AUObjectException("选择的目标父级单元与子级单元架构不同");


				DoCopyUnit(context, fromUnit, targetParent, newName, newCodeName, copyRoleMembers, copyScopeMembers, copyScopeConditions);
			}
			catch (AUObjectValidationException vex)
			{
				ProcessProgress.Current.Output.WriteLine(vex.Message);
			}
			catch (Exception ex)
			{
				ProcessProgress.Current.Output.WriteLine(ex.ToString());
			}

			e.Result.ProcessLog = context.Logger.ToString();
			ProcessProgress.Current.StatusText = "结束";
			ProcessProgress.Current.Response();
			SCCacheHelper.InvalidateAllCache();

			e.Result.DataChanged = true;
			e.Result.CloseWindow = false;
			e.Result.ProcessLog = ProcessProgress.Current.GetDefaultOutput();
		}

		private static void DoCopyUnit(StepContext context, AU.AdminUnit fromUnit, AU.AdminUnit targetParent, string newName, string newCodeName, bool copyRoleMembers, bool copyScopeMembers, bool copyScopeConditions)
		{
			AU.AdminUnit newUnit = CreateUnit(fromUnit, newName, newCodeName);

			int totalSteps = 1;
			if (copyRoleMembers)
				totalSteps++;
			if (copyScopeConditions)
				totalSteps++;
			if (copyScopeMembers)
				totalSteps++;

			context.TotalSteps = totalSteps;
			context.PassedSteps = 0;
			context.Div = 100.0 / totalSteps;

			ProcessProgress.Current.MinStep = 1;
			ProcessProgress.Current.CurrentStep = 1;
			ProcessProgress.Current.MaxStep = 100;

			context.Logger.WriteLine(ProcessProgress.Current.StatusText = "正在添加管理单元");

			AU.Operations.Facade.InstanceWithPermissions.AddAdminUnit(newUnit, targetParent);
			context.ResetInnerSteps();
			context.PassedSteps++;
			context.Response();

			CopyRoleMembers(fromUnit, copyRoleMembers, newUnit, context);
			context.PassedSteps++;
			context.ResetInnerSteps();
			context.Response();

			if (copyScopeMembers || copyScopeConditions)
			{
				var srcScopes = fromUnit.GetNormalScopes();
				var scopes = newUnit.GetNormalScopes();

				if (copyScopeMembers)
				{
					foreach (AU.AUAdminScope srcScope in srcScopes)
					{
						var targetScope = scopes.GetScope(srcScope.ScopeSchemaType);
						if (targetScope != null)
						{
							CopyMembers(copyScopeMembers, srcScope, targetScope, context);
						}
					}
					context.PassedSteps++;
					context.ResetInnerSteps();
					context.Response();
				}

				if (copyScopeConditions)
				{
					foreach (AU.AUAdminScope srcScope in srcScopes)
					{
						var targetScope = scopes.GetScope(srcScope.ScopeSchemaType);
						if (targetScope != null)
						{
							CopyConditions(copyScopeConditions, srcScope, targetScope, context);
							context.PassedSteps++;
							context.ResetInnerSteps();
							context.Response();
						}
					}
					context.PassedSteps++;
					context.ResetInnerSteps();
				}
			}
		}

		private static void CopyConditions(bool copyScopeConditions, AU.AUAdminScope srcScope, AU.AUAdminScope targetScope, StepContext context)
		{
			if (copyScopeConditions)
			{
				context.Logger.WriteLine(ProcessProgress.Current.StatusText = "正在复制管理范围条件成员");
				ProcessProgress.Current.Response();

				var srcCondition = AU.Adapters.AUConditionAdapter.Instance.Load(srcScope.ID, AU.AUCommon.ConditionType).Where(m => m.Status == SchemaObjectStatus.Normal).FirstOrDefault();
				if (srcCondition != null)
				{
					AU.Operations.Facade.InstanceWithPermissions.UpdateScopeCondition(targetScope, new PC.Conditions.SCCondition()
					{
						Condition = srcCondition.Condition,
						Description = srcCondition.Description,
						OwnerID = targetScope.ID,
						Type = AU.AUCommon.ConditionType,
						SortID = 0
					});
				}
			}
		}

		private static void CopyMembers(bool copyMembers, AU.AUAdminScope item, AU.AUAdminScope targetScope, StepContext context)
		{
			if (copyMembers)
			{
				context.Logger.WriteLine(ProcessProgress.Current.StatusText = "正在复制管理范围固定成员");
				ProcessProgress.Current.Response();
				var memberIDs = AU.AUCommon.DoDbProcess(() => PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(item.ID, item.ScopeSchemaType)).ToIDArray();
				var actualMembers = AU.Adapters.AUSnapshotAdapter.Instance.LoadScopeItems(memberIDs, item.ScopeSchemaType, true, DateTime.MinValue);
				context.ResetInnerSteps(actualMembers.Count);

				foreach (AU.AUAdminScopeItem scopeItem in actualMembers)
				{
					context.Logger.WriteLine("正在添加" + scopeItem.AUScopeItemName);
					AU.Operations.Facade.InstanceWithPermissions.AddObjectToScope(scopeItem, targetScope);
					context.InnerStep++;
					context.Response();
				}
			}
		}

		private static void CopyRoleMembers(AU.AdminUnit fromUnit, bool copyRoleMembers, AU.AdminUnit newUnit, StepContext context)
		{
			if (copyRoleMembers)
			{
				var roles = AU.Adapters.AUSnapshotAdapter.Instance.LoadAURoles(new string[] { fromUnit.ID }, true, DateTime.MinValue);
				var schemaRoles = AU.Adapters.AUSnapshotAdapter.Instance.LoadAUSchemaRoles(fromUnit.AUSchemaID, true, DateTime.MinValue);
				double allCount = roles.Count;
				foreach (AU.AURole r in roles)
				{
					var schemaRole = schemaRoles[r.SchemaRoleID];
					if (schemaRole == null)
						throw new AUObjectException(string.Format("未能找到对应角色{0}的管理架构角色{1}", r.ID, r.SchemaRoleID));

					context.Logger.WriteLine(ProcessProgress.Current.StatusText = string.Format("正在设置管理单元角色 {0} 成员", schemaRole.Name));
					ProcessProgress.Current.Response();
					var targetRole = AU.Adapters.AUSnapshotAdapter.Instance.LoadAURole(r.SchemaRoleID, newUnit.ID, true, DateTime.MinValue);

					if (targetRole != null)
					{
						var usersIDs = AU.AUCommon.DoDbProcess(() => PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(r.ID, "Users")).FilterByStatus(SchemaObjectStatusFilterTypes.Normal).ToIDArray();

						var users = (from p in usersIDs select new PC.SCUser() { ID = p, Name = "Demo", CodeName = "Demo" }).ToArray();

						AU.Operations.Facade.InstanceWithPermissions.ReplaceUsersInRole(users, newUnit, DbUtil.GetEffectiveObject<AU.AUSchemaRole>(targetRole.SchemaRoleID));
						context.Logger.Write("已经添加{0}个人员\r\n", users.Length);
					}
				}
			}
		}

		private static AU.AdminUnit CreateUnit(AU.AdminUnit fromUnit, string newName, string newCodeName)
		{
			AU.AdminUnit newUnit = new AU.AdminUnit();
			foreach (var item in fromUnit.Properties)
			{
				if (newUnit.Properties.ContainsKey(item.Definition.Name) == false)
					newUnit.Properties.Add(new SchemaPropertyValue(item.Definition));

				newUnit.Properties.SetValue<string>(item.Definition.Name, item.StringValue);

				var ppt = newUnit.Properties[item.Definition.Name];
				if (ppt == null)
					throw new AUObjectException(string.Format("管理单元的属性：{0}不匹配", ppt.Definition.Name));
				else
					ppt.StringValue = item.StringValue;
			}

			newUnit.ID = UuidHelper.NewUuidString();
			newUnit.AUSchemaID = fromUnit.AUSchemaID;
			newUnit.Name = newName;
			newUnit.CodeName = newCodeName;



			return newUnit;
		}

		protected void btnOk_Click(object sender, EventArgs e)
		{
			this.Initialize();
		}

		protected void calcProgress_LoadingDialogContent(object sender, LoadingDialogContentEventArgs e)
		{

		}
	}
}