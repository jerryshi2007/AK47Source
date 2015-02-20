using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DocServiceContract;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint;
using MCS.Library.SOA.DocServiceContract.Exceptions;
using MCS.Library.Services.Converters;
using Microsoft.SharePoint.Client.Utilities;
using MCS.Library.Core;
using MCS.Library.CamlBuilder;
using System.Threading;

namespace MCS.Library.Services
{
    /// <summary>
    /// 权限管理实现
    /// </summary>
    public partial class DCSStorageService
    {
        #region consts
        private const string IgnoreRole = "受限访问";
        #endregion

        public DCTRoleDefinition DCMCreateRole(string rolename, string descriptioin)
        {
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                RoleDefinitionCreationInformation roleCreationInfo = new RoleDefinitionCreationInformation() { BasePermissions = new BasePermissions(), Name = rolename, Description = descriptioin };
                context.Web.RoleDefinitions.Add(roleCreationInfo);
                context.Load(context.Web.RoleDefinitions);
                context.ExecuteQuery();

                RoleDefinition roleDefinition = context.Web.RoleDefinitions.GetByName(rolename);
                context.Load(roleDefinition);
                context.ExecuteQuery();

                DCTRoleDefinition dctRoleDefinition = new DCTRoleDefinition();
                DCTConverterHelper.Convert(roleDefinition, dctRoleDefinition);

                return dctRoleDefinition;
            }
        }

        public void DCMRemoveRole(string rolename)
        {
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                RoleDefinition roleDefinition = context.Web.RoleDefinitions.GetByName(rolename);
                roleDefinition.DeleteObject();

                context.ExecuteQuery();
            }
        }

        public BaseCollection<DCTPermission> DCMGetRolePermissions(DCTRoleDefinition role)
        {
            (role.ID <= 0 && string.IsNullOrEmpty(role.Name)).TrueThrow<ArgumentException>("角色中必须包含名称或ID.");

			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                RoleDefinition roleDef = null;

                if (!string.IsNullOrEmpty(role.Name))
                    roleDef = context.Web.RoleDefinitions.GetByName(role.Name);
                else
                    roleDef = context.Web.RoleDefinitions.GetById(role.ID);

                context.Load(roleDef);
                context.ExecuteQuery();

                BaseCollection<DCTPermission> permissions = new BaseCollection<DCTPermission>();

                if (roleDef.BasePermissions.Has(PermissionKind.ViewListItems))
                    permissions.Add(DCTPermission.ViewFileOrFolder);

                if (roleDef.BasePermissions.Has(PermissionKind.AddListItems))
                    permissions.Add(DCTPermission.AddFileOrFolder);

                if (roleDef.BasePermissions.Has(PermissionKind.EditListItems))
                    permissions.Add(DCTPermission.UpdateFileOrFolder);

                if (roleDef.BasePermissions.Has(PermissionKind.DeleteListItems))
                    permissions.Add(DCTPermission.DeleteFileOrFolder);


                return permissions;

            }
        }

        public void DCMSetRolePermissions(DCTRoleDefinition role, BaseCollection<DCTPermission> permissions)
        {
            (role.ID <= 0 && string.IsNullOrEmpty(role.Name)).TrueThrow<ArgumentException>("角色中必须包含名称或ID.");


            if (permissions == null || permissions.Count == 0)
                return;

			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                RoleDefinition roleDef = null;

                if (!string.IsNullOrEmpty(role.Name))
                    roleDef = context.Web.RoleDefinitions.GetByName(role.Name);
                else
                    roleDef = context.Web.RoleDefinitions.GetById(role.ID);

                context.Load(roleDef);
                context.ExecuteQuery();

                BasePermissions basePerm = new BasePermissions();

                foreach (DCTPermission permission in permissions)
                {
                    basePerm.Set((PermissionKind)permission);
                }

                roleDef.BasePermissions = basePerm;
                roleDef.Update();
                context.ExecuteQuery();
            }
        }

        public BaseCollection<DCTRoleAssignment> DCMGetRoleAssignments(int storageObjID)
        {
            (storageObjID <= 0).TrueThrow<ArgumentException>("ID:{0}无效，请传入大于0的值.", storageObjID);

			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                ListItem listItem = GetListItemById(storageObjID, context);
                if (null == listItem)
                    return new BaseCollection<DCTRoleAssignment>();
                context.Load(listItem);
                context.Load(listItem.RoleAssignments);
                context.ExecuteQuery();

                RoleAssignmentCollection roleAssignments = listItem.RoleAssignments;
                BaseCollection<DCTRoleAssignment> results = new BaseCollection<DCTRoleAssignment>();

                foreach (RoleAssignment roleAssignment in roleAssignments)
                {
                    DCTRoleAssignment dctRoleAssignment = new DCTRoleAssignment();
                    context.Load(roleAssignment.Member);
                    context.Load(roleAssignment.RoleDefinitionBindings);
                    context.ExecuteQuery();

                    dctRoleAssignment.Member = GetPrinciple(roleAssignment.Member.PrincipalType);
                    DCTConverterHelper.Convert(roleAssignment.Member, dctRoleAssignment.Member);

                    dctRoleAssignment.RoleDefinitions = new BaseCollection<DCTRoleDefinition>();
                    RoleDefinitionBindingCollection bindingCollection = roleAssignment.RoleDefinitionBindings;
                    foreach (RoleDefinition roleDefinition in bindingCollection)
                    {
                        DCTRoleDefinition dctRoleDefinition = new DCTRoleDefinition();
                        DCTConverterHelper.Convert(roleDefinition, dctRoleDefinition);
                        dctRoleAssignment.RoleDefinitions.Add(dctRoleDefinition);
                    }

                    results.Add(dctRoleAssignment);
                }

                return results;
            }
        }


        public BaseCollection<string> DCMGetUserRoles(int storageObjID, string userAccount)
        {
            (storageObjID > 0).FalseThrow<ArgumentException>("ID值{0}无效，请传入大于0的值.", storageObjID);
            BaseCollection<string> results = new BaseCollection<string>();
            BaseCollection<DCTRoleAssignment> roleAssignments = DCMGetRoleAssignments(storageObjID);
            foreach (DCTRoleAssignment assignment in roleAssignments)
            {
                if (assignment.Member is DCTUser)
                {
                    DCTUser user = assignment.Member as DCTUser;
                    if (!IsSameUser(user.LoginName, userAccount))
                        continue;
                    foreach (DCTRoleDefinition definition in assignment.RoleDefinitions)
                    {
                        results.Add(definition.Name);
                    }
                }
            }
            return results;
        }

        private bool IsSameUser(string userAccount1, string userAccount2)
        {
            return userAccount1 == userAccount2 || userAccount1.EndsWith("\\" + userAccount2, true, Thread.CurrentThread.CurrentUICulture);
        }


        public void DCMAddUserRole(int storageObjID, string userAccount, string rolename)
        {
            (storageObjID > 0).FalseThrow<ArgumentException>("ID值{0}无效，请传入大于0的值.", storageObjID);
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                context.Load(context.Web.RoleDefinitions);
                context.ExecuteQuery();
                RoleDefinition selectedRoleDef = null;
                foreach (RoleDefinition role in context.Web.RoleDefinitions)
                {
                    if (role.Name == rolename)
                    {
                        selectedRoleDef = role;
                        break;
                    }
                }
                if (null == selectedRoleDef)
                    return;
                context.Load(selectedRoleDef);
                ListItem listItem = GetListItemById(storageObjID, context);
                if (null == listItem)
                    return;
                context.Load(listItem);
                context.Load(listItem.RoleAssignments);
                context.ExecuteQuery();

                RoleAssignmentCollection roleAssignments = listItem.RoleAssignments;

                bool userFound = false;
                foreach (RoleAssignment roleAssignment in roleAssignments)
                {
                    context.Load(roleAssignment.Member);
                    context.ExecuteQuery();
                    if (roleAssignment.Member.LoginName != userAccount)
                        continue;
                    userFound = true;
                    roleAssignment.RoleDefinitionBindings.Add(selectedRoleDef);
                    roleAssignment.Update();
                }
                if (!userFound)
                {
                    RoleDefinitionBindingCollection newBinding = new RoleDefinitionBindingCollection(context);
                    newBinding.Add(selectedRoleDef);
                    User user = context.Web.EnsureUser(userAccount);
                    listItem.RoleAssignments.Add(user, newBinding);
                    listItem.Update();
                }
                context.ExecuteQuery();
            }
        }


        public void DCMRemoveUserRoles(int storageObjID, string userAccount, BaseCollection<string> rolesToRemove)
        {
            (storageObjID > 0).FalseThrow<ArgumentException>("ID值{0}无效，请传入大于0的值.", storageObjID);
			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                ListItem listItem = GetListItemById(storageObjID, context);
                if (null == listItem)
                    return;
                context.Load(listItem);
                context.Load(listItem.RoleAssignments);
                context.ExecuteQuery();

                RoleAssignmentCollection roleAssignments = listItem.RoleAssignments;

                foreach (RoleAssignment roleAssignment in roleAssignments)
                {
                    context.Load(roleAssignment.Member);
                    context.Load(roleAssignment.RoleDefinitionBindings);
                    context.ExecuteQuery();
                    if (!IsSameUser(roleAssignment.Member.LoginName, userAccount))
                        continue;
                    for (int i = 0; i < roleAssignment.RoleDefinitionBindings.Count; i++)
                    {
                        RoleDefinition roleDefinition = roleAssignment.RoleDefinitionBindings[i];
                        string rolename = roleDefinition.Name;
                        if (rolesToRemove.Contains(rolename))
                        {
                            roleAssignment.RoleDefinitionBindings.Remove(roleDefinition);
                            i--;
                        }
                    }
                    roleAssignment.Update();
                }
                context.ExecuteQuery();
            }
        }


        private DCTPrincipal GetPrinciple(PrincipalType principalType)
        {
            DCTPrincipal result = null;

            if (principalType == PrincipalType.SharePointGroup)
                result = new DCTGroup();
            else
                result = new DCTUser();

            return result;
        }

        public void DCMSetRoleAssignments(int storageObjID, BaseCollection<DCTRoleAssignment> roleAssignments)
        {
            (storageObjID > 0).FalseThrow<ArgumentException>("ID值{0}无效，请传入大于0的值.", storageObjID);

			using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                ListItem listItem = GetListItemById(storageObjID, context);
                if (null == listItem)
                    return;
                context.Load(listItem);

                listItem.BreakRoleInheritance(true, true);

                context.Load(listItem.RoleAssignments);
                context.ExecuteQuery();

                foreach (RoleAssignment roleAssignment in listItem.RoleAssignments)
                {
                    roleAssignment.RoleDefinitionBindings.RemoveAll();
                    roleAssignment.Update();
                }

                context.ExecuteQuery();

                foreach (DCTRoleAssignment dctRoleAssignment in roleAssignments)
                {
                    bool ignore = false;
                    RoleDefinitionBindingCollection binding = BuildRoleDefninitionBinding(context, dctRoleAssignment, out ignore);

                    Principal principal = BuildSharepointPrincipal(context, dctRoleAssignment);

                    if (ignore)
                        continue;

                    listItem.RoleAssignments.Add(principal, binding);
                    listItem.Update();
                }

                context.ExecuteQuery();
            }
        }

        private Principal BuildSharepointPrincipal(DocLibContext context, DCTRoleAssignment dctRoleAssignment)
        {
            Principal principal = null;

            if (dctRoleAssignment.Member.PricinpalType == DCTPrincipalType.SharePointGroup)
            {
                Group group = context.Web.SiteGroups.GetById(dctRoleAssignment.Member.ID);
                principal = group;
            }
            else
            {
                User user = context.Web.EnsureUser((dctRoleAssignment.Member as DCTUser).LoginName);
                principal = user;
            }

            return principal;
        }

        private RoleDefinitionBindingCollection BuildRoleDefninitionBinding(DocLibContext context, DCTRoleAssignment dctRoleAssignment, out bool ignore)
        {
            RoleDefinitionBindingCollection binding = new RoleDefinitionBindingCollection(context);
            int count = 0;

            foreach (DCTRoleDefinition dctRoleDefinition in dctRoleAssignment.RoleDefinitions)
            {
                if (dctRoleDefinition.Name == IgnoreRole)
                    continue;
                RoleDefinition spRoleDefinition = null;

                if (!string.IsNullOrEmpty(dctRoleDefinition.Name))
                {
                    spRoleDefinition = context.Web.RoleDefinitions.GetByName(dctRoleDefinition.Name);
                }
                else
                {
                    spRoleDefinition = context.Web.RoleDefinitions.GetById(dctRoleDefinition.ID);
                }

                context.Load(spRoleDefinition);
                context.ExecuteQuery();
                binding.Add(spRoleDefinition);
                count++;
            }

            ignore = (count == 0);

            return binding;
        }
    }
}