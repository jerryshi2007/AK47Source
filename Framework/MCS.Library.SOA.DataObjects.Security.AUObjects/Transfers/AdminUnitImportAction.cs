using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PC = MCS.Library.SOA.DataObjects.Security;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
    public class AdminUnitImportAction : IImportAction
    {
        public void DoImport(MCS.Library.SOA.DataObjects.Security.SCObjectSet objectSet, IImportContext context)
        {
            var exec = this.IgnorePermission ? Operations.Facade.DefaultInstance : Operations.Facade.InstanceWithPermissions;

            ImportContextWrapper wrapper = new ImportContextWrapper(context);

            if (string.IsNullOrEmpty(this.ParentID))
                throw new InvalidOperationException("操作前必须对ParentID进行赋值");


            var parent = AUCommon.DoDbProcess<SchemaObjectBase>(() => PC.Adapters.SchemaObjectAdapter.Instance.Load(this.ParentID));
            if (parent == null || parent.Status != Schemas.SchemaProperties.SchemaObjectStatus.Normal)
                throw new AUObjectException("不存在指定的父级单元，请确认");

            if (parent is AUSchema)
                parent = null;
            else if (!(parent is AdminUnit))
            {
                throw new AUObjectException("指定的ParentID不是一个AUSchema或者AdminUnit对象的ID");
            }

            var subs = (from r in objectSet.Relations where r.ParentID == this.ParentID && r.ChildSchemaType == AUCommon.SchemaAdminUnit && r.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal join o in objectSet.Objects on r.ID equals o.ID where o.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal && o.SchemaType == AUCommon.SchemaAdminUnit select (AdminUnit)o).ToArray();
            int count = subs.Length;
            foreach (AdminUnit s in subs)
            {
                ImportOneUnit(objectSet, exec, wrapper, parent, count, s);
            }
        }

        private void ImportOneUnit(MCS.Library.SOA.DataObjects.Security.SCObjectSet objectSet, Operations.IFacade exec, ImportContextWrapper wrapper, SchemaObjectBase parent, int totalSteps, AdminUnit unit)
        {
            int currentStep = 0;
            var scopes = (from m in objectSet.Membership where m.ContainerID == unit.ID && m.MemberSchemaType == AUCommon.SchemaAUAdminScope && m.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal join o in objectSet.Objects on m.ID equals o.ID where o.SchemaType == AUCommon.SchemaAUAdminScope && o.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal select (AUAdminScope)o).ToArray();
            var roles = (from m in objectSet.Membership where m.ContainerID == unit.ID && m.MemberSchemaType == AUCommon.SchemaAdminUnitRole && m.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal join o in objectSet.Objects on m.ID equals o.ID where o.SchemaType == AUCommon.SchemaAdminUnitRole && o.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal select (AURole)o).ToArray();
            try
            {
                currentStep++;
                wrapper.SetStatusAndLog(currentStep, totalSteps, "正在导入管理单元" + unit.GetQualifiedName());
                exec.AddAdminUnitWithMembers(unit, (AdminUnit)parent, roles, scopes);
                ImportRoleMembers(objectSet, exec, wrapper, totalSteps, unit, currentStep, roles);

                ImportConditions(objectSet, exec, wrapper, totalSteps, unit, currentStep, scopes);

                if (this.ImportSubUnits)
                {
                    wrapper.IncreaseLevel();
                    var subs = (from r in objectSet.Relations where r.ParentID == unit.ID && r.ChildSchemaType == AUCommon.SchemaAdminUnit && r.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal join o in objectSet.Objects on r.ID equals o.ID where o.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal && o.SchemaType == AUCommon.SchemaAdminUnit select (AdminUnit)o).ToArray();
                    int count = subs.Length;
                    foreach (AdminUnit s in subs)
                    {
                        ImportOneUnit(objectSet, exec, wrapper, unit, count, s);
                    }

                    wrapper.DecreaseLevel();
                }
            }
            catch (Exception ex)
            {
                wrapper.IncreaseError();
                wrapper.SetStatusAndLog(currentStep, totalSteps, string.Format("导入单元 {0} 失败，原因是：{1}", unit.GetQualifiedName(), ex.ToString()));
            }
        }

        private void ImportConditions(MCS.Library.SOA.DataObjects.Security.SCObjectSet objectSet, Operations.IFacade exec, ImportContextWrapper wrapper, int totalSteps, AdminUnit unit, int currentStep, AUAdminScope[] scopes)
        {
            if (this.IncludeScopeConditions)
            {
                wrapper.SetStatusAndLog(currentStep, totalSteps, "正在导入管理单元管理范围条件" + unit.GetQualifiedName());
                foreach (AUAdminScope sc in scopes)
                {
                    var conditions = (from c in objectSet.Conditions where c.OwnerID == sc.ID select c);
                    foreach (var c in conditions)
                    {
                        DBTimePointActionContext.Current.DoActions(() => exec.UpdateScopeCondition(sc, c));
                    }
                }
            }
        }

        private void ImportRoleMembers(MCS.Library.SOA.DataObjects.Security.SCObjectSet objectSet, Operations.IFacade exec, ImportContextWrapper wrapper, int totalSteps, AdminUnit unit, int currentStep, AURole[] roles)
        {
            if (this.IncludeRoleMembers)
            {
                wrapper.SetStatusAndLog(currentStep, totalSteps, "正在替换管理单元角色成员" + unit.GetQualifiedName());
                foreach (AURole role in roles)
                {
                    var userIDS = (from c in objectSet.Membership where c.ContainerID == role.ID && c.MemberSchemaType == "Users" && c.Status == Schemas.SchemaProperties.SchemaObjectStatus.Normal select c.ID).ToArray();
                    var users = MCS.Library.OGUPermission.OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userIDS);
                    var scUsers = (from u in users select new SCUser() { }).ToArray();
                    var schemaRole = AUCommon.DoDbProcess<AUSchemaRole>(() => (AUSchemaRole)PC.Adapters.SchemaObjectAdapter.Instance.Load(role.SchemaRoleID));
                    exec.ReplaceUsersInRole(scUsers, unit, schemaRole);
                }
            }
        }

        public string ParentID { get; set; }

        /// <summary>
        /// 是否导入角色的人员
        /// </summary>
        public bool IncludeRoleMembers { get; set; }
        /// <summary>
        /// 是否导入范围成员
        /// </summary>
        public bool IncludeScopeConditions { get; set; }
        /// <summary>
        /// 是否忽略权限
        /// </summary>
        public bool IgnorePermission { get; set; }

        public bool ImportSubUnits { get; set; }
    }

    class ImportContextWrapper
    {
        private IImportContext context;

        public ImportContextWrapper(IImportContext context)
        {
            this.context = context;
        }

        int level = 0;

        public int Level
        {
            get { return level; }
        }

        public void IncreaseLevel()
        {
            if (level >= 0)
                level++;
        }

        public void IncreaseError()
        {
            this.context.ErrorCount++;
        }

        public void DecreaseLevel()
        {
            if (level > 1)
                level--;
        }

        public void SetStatusAndLog(int currentStep, int maxStep, string message)
        {
            if (level > 0)
                context.SetSubStatusAndLog(currentStep, maxStep, message);
            else
                context.SetStatusAndLog(currentStep, maxStep, message);
        }
    }
}
