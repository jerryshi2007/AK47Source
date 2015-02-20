using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Logs;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Permissions;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Executors
{
    internal class AdminUnitExecutor : AUObjectExecutor
    {
        private bool _NeedParentStatusCheck = false;
        private bool _NeedDuplicateRelationCheck = false;
        /// <summary>
        /// 传入的组织
        /// </summary>
        private AdminUnit inputParent;
        /// <summary>
        /// 实际的组织
        /// </summary>
        private SchemaObjectBase actualParent;
        private SCRelationObject targetRelation, oldRelationToUnit, oldRelationToSchema;
        private AUSchema schema;
        private AUSchemaRoleCollection existingSchemaRoles; // Schema角色定义
        private SchemaObjectCollection existingUnitScopes; //SchemaScope定义
        private SchemaObjectCollection existingUnitRoles; //现有的角色
        private PendingActionCollection pendingActions = new PendingActionCollection(); //准备数据之后需要做的操作
        /// <summary>
        /// 表示已经存在与目标的关系，无需更新
        /// </summary>
        private bool relationExists = false;
        private SCAclContainer aclContainer;
        private SCChildrenRelationObjectCollection oldRelationToChildren;

        public AURole[] InputRoles { get; set; }

        public AUAdminScope[] InputAdminScopes { get; set; }

        public AdminUnitExecutor(AUOperationType opType, AdminUnit parent, AdminUnit child)
            : base(opType, child)
        {
            child.NullCheck("child");
            child.ClearRelativeData();
            if (parent != null)
                parent.ClearRelativeData();

            if (!(opType != AUOperationType.AddAdminUnit | opType != AUOperationType.RemoveAdminUnit))
                throw new ApplicationException("此Executor不支持" + opType + "操作");

            this.inputParent = parent;

            if (this.OperationType == AUOperationType.AddAdminUnit)
                this.aclContainer = PrepareAclContainer(parent, child);
        }

        private SCAclContainer PrepareAclContainer(AdminUnit parent, AdminUnit currentData)
        {
            SCAclContainer result = null;

            if (currentData is ISCAclContainer)
            {
                result = new SCAclContainer(currentData);
                if (parent != null)
                {
                    AUCommon.DoDbAction(() =>
                        result.Members.CopyFrom(AUAclAdapter.Instance.LoadByContainerID(parent.ID, DateTime.MinValue)));
                }
            }

            return result;
        }

        /// <summary>
        /// 是否覆盖保存已经存在的关系
        /// </summary>
        public bool OverrideExistedRelation { get; set; }

        public AdminUnit ParentObject
        {
            get { return inputParent; }
        }

        public AUSchema TargetObject
        {
            get { return schema; }
        }

        public SCRelationObject TargetRelation
        {
            get { return targetRelation; }
        }

        /// <summary>
        /// 获取或设置一个值，表示当此管理单元已经配置了角色或者管理范围时，是否仍然强行删除（通常由管理员操作）
        /// </summary>
        public bool ForceDelete { get; set; }

        /// <summary>
        /// 是否需要校验对象间是否有重复的关系
        /// </summary>
        public bool NeedDuplicateRelationCheck
        {
            get
            {
                return this._NeedDuplicateRelationCheck;
            }
            set
            {
                this._NeedDuplicateRelationCheck = value;
            }
        }

        /// <summary>
        /// 是否需要检查父对象的状态
        /// </summary>
        public bool NeedParentStatusCheck
        {
            get
            {
                return this._NeedParentStatusCheck;
            }
            set
            {
                this._NeedParentStatusCheck = value;
            }
        }


        protected override void PrepareData(AUObjectOperationContext context)
        {
            AUCommon.DoDbAction(() =>
            {
                this.schema = (AUSchema)SchemaObjectAdapter.Instance.Load(((AdminUnit)Data).AUSchemaID);

                if (this.schema == null || this.schema.Status != SchemaObjectStatus.Normal)
                    throw new AUObjectValidationException(AUCommon.DisplayNameFor((AdminUnit)this.Data) + "管理单元的SchemaID无效，无法找到对应的Schema。");
            });

            this.PrepareRelationObject();

            base.PrepareData(context);

            var oldObject = (AdminUnit)SCActionContext.Current.OriginalObject;

            if (oldObject != null && oldObject.AUSchemaID != this.schema.ID)
                throw new AUObjectValidationException("一旦创建，不能以任何方式修改AdminUnit的AUSchema属性");

            this.existingSchemaRoles = Adapters.AUSnapshotAdapter.Instance.LoadAUSchemaRoles(schema.ID, false, DateTime.MinValue);
            this.existingUnitRoles = Adapters.AUSnapshotAdapter.Instance.LoadAURoles(new string[] { this.Data.ID }, new string[0], false, DateTime.MinValue);
            this.existingUnitScopes = Adapters.AUSnapshotAdapter.Instance.LoadAUScope(this.Data.ID, false, DateTime.MinValue);

            this.pendingActions.Clear();
            PrepareRolesAndScopes();
        }

        private void PrepareRelationObject()
        {
            AUCommon.DoDbAction(() =>
            {
                this.oldRelationToSchema = SchemaRelationObjectAdapter.Instance.Load(((AdminUnit)this.Data).AUSchemaID, this.Data.ID);
                this.oldRelationToUnit = SchemaRelationObjectAdapter.Instance.LoadByObjectID(this.Data.ID).Where(m => m.Status == SchemaObjectStatus.Normal && m.ParentSchemaType == AUCommon.SchemaAdminUnit).FirstOrDefault();

                if (this.Data.Status == SchemaObjectStatus.Normal) //添加操作
                {
                    this.actualParent = inputParent != null ? SchemaObjectAdapter.Instance.Load(this.inputParent.ID) : this.schema;

					if (this.actualParent.ID == this.Data.ID)
						throw new AUObjectValidationException("管理单元的父管理单元不能是其自身。");

                    if (this.actualParent == null || this.actualParent.Status != SchemaObjectStatus.Normal)
                        throw new AUObjectValidationException("指定的父管理单元在系统中不存在或已删除");

                    if (this.oldRelationToSchema != null && this.oldRelationToSchema.Status == SchemaObjectStatus.Normal && this.oldRelationToUnit != null && this.oldRelationToUnit.Status == SchemaObjectStatus.Normal)
                        throw new AUObjectValidationException("关系错误。此管理单元已经存在，但存在错误的关联。一个管理单元不能同时与Schema和另一个管理单元关联");
                    else if (this.oldRelationToSchema != null && this.oldRelationToSchema.Status == SchemaObjectStatus.Normal && actualParent.ID != this.oldRelationToSchema.ParentID)
                        throw new AUObjectValidationException("关系错误。此管理单元已经与另一个Schema关联。");
                    else if (this.oldRelationToUnit != null && this.oldRelationToUnit.Status == SchemaObjectStatus.Normal && actualParent.ID != this.oldRelationToUnit.ParentID)
                        throw new AUObjectValidationException("关系错误。此管理单元已经与另一个AdminUnit关联。");

                }
                else //删除操作，忽略传,m入的parent参数，因此需要查找真实的parent
                {
                    if (oldRelationToUnit != null && oldRelationToUnit.Status == SchemaObjectStatus.Normal)
                    {
                        this.actualParent = SchemaObjectAdapter.Instance.Load(oldRelationToUnit.ParentID);
                    }
                    else if (oldRelationToSchema != null && oldRelationToSchema.Status == SchemaObjectStatus.Normal)
                    {
                        this.actualParent = schema;
                    }
                }


                this.oldRelationToChildren = SchemaRelationObjectAdapter.Instance.LoadByParentID(this.Data.ID);

                if (actualParent is AUSchema)
                {
                    // 应该是跟Schema关联
                    if (oldRelationToSchema != null && oldRelationToSchema.Status == SchemaObjectStatus.Normal)
                    {
                        relationExists = true;
                    }

                    targetRelation = new SCRelationObject(schema, this.Data) { Status = SchemaObjectStatus.Normal };
                }
                else
                {
                    targetRelation = SchemaRelationObjectAdapter.Instance.Load(actualParent.ID, this.Data.ID);
                    if (targetRelation != null)
                    {
                        if (targetRelation.Status == SchemaObjectStatus.Normal)
                        {
                            relationExists = true;
                        }

                        targetRelation.Status = SchemaObjectStatus.Normal;
                    }
                    else
                    {
                        targetRelation = new SCRelationObject(actualParent, this.Data) { Status = SchemaObjectStatus.Normal };
                    }
                }
            });
        }

        protected override void DoValidate(Validation.ValidationResults validationResults)
        {
            base.DoValidate(validationResults);

            if (this.targetRelation.Status == SchemaObjectStatus.Normal)
            {
                ValidationResults currentResults = this.targetRelation.Validate();

                if (targetRelation.ID == targetRelation.ParentID)
                    validationResults.AddResult(new ValidationResult("关系错误，不能是自身关联的", this.targetRelation, "", "", null) { });

                foreach (ValidationResult result in currentResults)
                    validationResults.AddResult(result);
            }

            string schemaID = ((AdminUnit)this.Data).AUSchemaID;

            if (this.inputParent != null && this.inputParent.AUSchemaID != schemaID)
                validationResults.AddResult(new ValidationResult("校验父对象管理架构与子对象管理架构不同", this.Data, "AUSchemaID", "", new Validators.AUSchemaIDValidator()));
        }

        protected override void CheckStatus()
        {
            AUCommon.DoDbAction(() =>
            {
                List<SchemaObjectBase> dataToBeChecked = new List<SchemaObjectBase>();

                if (this.NeedStatusCheck)
                    dataToBeChecked.Add(this.Data);

                if (this.NeedParentStatusCheck && this.inputParent != null)
                    dataToBeChecked.Add(this.inputParent);

                dataToBeChecked.Add(this.schema);

                CheckObjectStatus(dataToBeChecked.ToArray());

                if (this.NeedDuplicateRelationCheck)
                {
                    var allParentRelations = this.Data.CurrentParentRelations;
                    var relationToSchema = (from r in allParentRelations where r.ParentSchemaType == AUCommon.SchemaAUSchema select r).FirstOrDefault();
                    var relationToUnit = (from u in allParentRelations where u.ParentSchemaType == AUCommon.SchemaAdminUnit select u).FirstOrDefault();

                    if (this.inputParent != null)
                    {
                        //添加下级管理单元
                        if (relationToSchema != null)
                        {
                            throw new SCStatusCheckException(string.Format("对象\"{0}\"已经是顶级管理单元了，不能再添加到别的管理单元中",
                            AUCommon.DisplayNameFor(this.Data)));
                        }
                        else if (relationToUnit != null && (relationToUnit.ID != this.Data.ID || relationToUnit.ParentID != inputParent.ID))
                        {
                            throw new SCStatusCheckException(string.Format("对象\"{0}\"已经属于另一管理单元{1}了，不能再添加到其他管理单元",
                            AUCommon.DisplayNameFor(this.Data)));
                        }
                    }
                    else
                    {
                        //顶级管理单元
                        if (relationToSchema != null)
                        {
                            if (relationToSchema.ParentID != schema.ID)
                                throw new SCStatusCheckException(string.Format("对象\"{0}\"已经是顶级管理单元了，不能再添加到别的管理单元中",
                                AUCommon.DisplayNameFor(this.Data)));
                        }
                        else if (relationToUnit != null)
                        {
                            throw new SCStatusCheckException(string.Format("对象\"{0}\"已经属于另一管理单元{1}了，不能作为顶级管理单元",
                            AUCommon.DisplayNameFor(this.Data)));
                        }
                    }
                }
            });
        }

        protected override object DoOperation(AUObjectOperationContext context)
        {
            using (System.Transactions.TransactionScope ts = TransactionScopeFactory.Create())
            {
                bool changeRelationToSchemaDemanded = true;
                bool changeRelationToUnitDemanded = true;

                //当不是删除操作，且需要修改关系时
                if (this.Data.Status == SchemaObjectStatus.Normal && (OverrideExistedRelation || this.relationExists == false))
                {
                    SchemaRelationObjectAdapter.Instance.Update(this.targetRelation);
                }
                else if (this.Data.Status == SchemaObjectStatus.Deleted && (OverrideExistedRelation || this.relationExists))
                {
                    if (oldRelationToChildren.Any(m => m.Status == SchemaObjectStatus.Normal))
                        throw new AUObjectValidationException("管理单元存在子对象，不能进行删除操作");

                    SchemaRelationObjectAdapter.Instance.UpdateStatus(this.targetRelation, SchemaObjectStatus.Deleted);
                }

                if (oldRelationToSchema != null && this.targetRelation.ParentID == this.oldRelationToSchema.ParentID)
                    changeRelationToSchemaDemanded = false;

                if (oldRelationToUnit != null && this.targetRelation.ParentID == this.oldRelationToUnit.ParentID)
                    changeRelationToUnitDemanded = false;

                if (changeRelationToSchemaDemanded && this.oldRelationToSchema != null && this.oldRelationToSchema.Status == SchemaObjectStatus.Normal)
                    SchemaRelationObjectAdapter.Instance.UpdateStatus(this.oldRelationToSchema, SchemaObjectStatus.Deleted);

                if (changeRelationToUnitDemanded && this.oldRelationToUnit != null && this.oldRelationToUnit.Status == SchemaObjectStatus.Normal)
                    SchemaRelationObjectAdapter.Instance.UpdateStatus(this.oldRelationToUnit, SchemaObjectStatus.Deleted);

                SchemaObjectAdapter.Instance.Update(Data);

                this.DoRelativeDataOperation(context);

                if (this.aclContainer != null)
                {
                    DBTimePointActionContext.Current.DoActions(() => SCAclAdapter.Instance.Update(this.aclContainer));
                }

                ts.Complete();
            }
            return this.TargetRelation;
        }

        protected override void DoRelativeDataOperation(MCS.Library.SOA.DataObjects.Security.AUObjects.Executors.AUObjectOperationContext context)
        {
            base.DoRelativeDataOperation(context);
            this.pendingActions.DoActions();
        }

        private void PrepareRolesAndScopes()
        {
            if (Data.Status == SchemaObjectStatus.Normal)
            {
                // 新建或更新，确定要增加哪些辅助对象：AURole，AUScopes
                PrepareRolesForAdd();

                PrepareScopesForAdd();
            }
            else
            {
                // 删除
                foreach (AURole role in this.existingUnitRoles)
                {
                    RelationHelper.Instance.ClearContainer(role); // 消除角色中的人员关系，矩阵就没办法了
                    pendingActions.Add(new RemoveMemberAction((AdminUnit)this.Data, role));
                }

                foreach (AUAdminScope scope in this.existingUnitScopes)
                {
                    RelationHelper.Instance.ClearContainer(scope);
                    AUConditionAdapter.Instance.DeleteByOwner(scope.ID, AUCommon.ConditionType);
                    pendingActions.Add(new RemoveMemberAction((AdminUnit)this.Data, scope));
                }
            }
        }

        private void PrepareRolesForAdd()
        {
            foreach (AUSchemaRole sr in this.existingSchemaRoles)
            {
                bool enableRole = sr.Status == SchemaObjectStatus.Normal;
                AURole existingRole = AUCommon.FindMatchRole(this.existingUnitRoles, sr);
                AURole inputRole = this.InputRoles != null ? AUCommon.FindMatchRole(this.InputRoles, sr) : null;
                if (inputRole != null && existingRole != null && inputRole.ID != existingRole.ID)
                    throw new AUObjectException(string.Format("导入的角色{0}与现有角色{1}的ID不一致，导致无法继续执行导入。", inputRole.ID, existingRole.ID));

                if (existingRole == null)
                {
                    if (inputRole == null)
                        inputRole = new AURole() { ID = UuidHelper.NewUuidString(), SchemaRoleID = sr.ID, Status = SchemaObjectStatus.Normal };

                    inputRole.Status = SchemaObjectStatus.Normal;

                    if (enableRole == false)
                    {
                        inputRole.Status = SchemaObjectStatus.Deleted;
                    }

                    pendingActions.Add(new AddMemberAction((AdminUnit)this.Data, inputRole));

                }
                else if (existingRole.Status != sr.Status)
                {
                    pendingActions.Add(sr.Status == SchemaObjectStatus.Normal ? (IPendingAction)new EnableMemberAction(existingRole, (AdminUnit)this.Data) : new RemoveMemberAction((AdminUnit)this.Data, existingRole));
                }
            }
        }

        private void PrepareScopesForAdd()
        {
            var scopeNames = this.schema.Scopes.Split(AUCommon.Spliter, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in scopeNames)
            {
                AUAdminScope existingScope = AUCommon.FindMatchScope(this.existingUnitScopes, name);
                AUAdminScope inputScope = this.InputAdminScopes != null ? AUCommon.FindMatchScope(this.InputAdminScopes, name) : null;

                if (inputScope != null && existingScope != null && inputScope.ID != existingScope.ID)
                    throw new AUObjectException(string.Format("导入的管理范围{0}与现有管理范围{1}的ID不一致，导致无法继续执行导入。", inputScope.ID, existingScope.ID));

                if (existingScope == null)
                {
                    if (inputScope == null)
                        inputScope = new AUAdminScope() { ID = UuidHelper.NewUuidString(), ScopeSchemaType = name };

                    pendingActions.Add(new AddMemberAction((AdminUnit)this.Data, inputScope));
                }
                else if (existingScope.Status != SchemaObjectStatus.Normal)
                {
                    pendingActions.Add(new EnableMemberAction((AdminUnit)this.Data, existingScope));
                }
            }
        }

        protected override void PrepareOperationLog(AUObjectOperationContext context)
        {
            AUOperationLog log = AUOperationLog.CreateLogFromEnvironment();

            log.ResourceID = this.Data.ID;
            log.SchemaType = this.Data.SchemaType;
            log.OperationType = this.OperationType;
            log.Category = this.Data.Schema.Category;
            log.Subject = string.Format("{0}: {1} 于 {2}",
                EnumItemDescriptionAttribute.GetDescription(this.OperationType), AUCommon.DisplayNameFor(this.Data), AUCommon.DisplayNameFor(this.actualParent));

            //log.SearchContent = this.Data.ToFullTextString() + " " + this.Container.ToFullTextString();

            context.Logs.Add(log);
        }
    }
}
