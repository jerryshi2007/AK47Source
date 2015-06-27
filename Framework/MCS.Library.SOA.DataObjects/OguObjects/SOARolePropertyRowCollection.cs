using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects
{
    [Serializable]
    public class SOARolePropertyRowCollection : TableRowCollectionBase<SOARolePropertyRow, SOARolePropertyDefinition, SOARolePropertyValue, SOARolePropertyValueCollection, string>
    {
        private readonly static List<string> _EmptyStringList = new List<string>();

        private IRole _Role = null;

        /// <summary>
        /// 
        /// </summary>
        public SOARolePropertyRowCollection()
        {
        }

        public SOARolePropertyRowCollection(IRole role)
        {
            this._Role = role;
        }

        public SOARolePropertyRowCollection Query(params SOARolePropertiesQueryParam[] queryParams)
        {
            queryParams.NullCheck("queryParams");

            return Query((IEnumerable<SOARolePropertiesQueryParam>)queryParams);
        }

        public IRole Role
        {
            get
            {
                return this._Role;
            }
        }

        public SOARolePropertyRowCollection Clone()
        {
            SOARolePropertyRowCollection cloned = new SOARolePropertyRowCollection();

            this.FillClonedRows(cloned);

            return cloned;
        }

        public void FillClonedRows(SOARolePropertyRowCollection cloned)
        {
            foreach (SOARolePropertyRow row in this)
                cloned.Add(new SOARolePropertyRow(row, row.RowNumber));
        }

        public SOARolePropertyRowCollection Query(Predicate<SOARolePropertyRow> predicate)
        {
            SOARolePropertyRowCollection result = new SOARolePropertyRowCollection(this._Role);

            foreach (SOARolePropertyRow row in this)
            {
                bool matched = true;

                matched = predicate(row);

                //if (matched)
                //    result.Add(row);
                if (matched)
                    result.Add(new SOARolePropertyRow(row, row.RowNumber));
            }

            return result;
        }

        public SOARolePropertyRowCollection Query(IEnumerable<SOARolePropertiesQueryParam> queryParams)
        {
            return this.Query(queryParams, false);
        }

        public SOARolePropertyRowCollection Query(IEnumerable<SOARolePropertiesQueryParam> queryParams, bool matchAny = false)
        {
            WfConditionDescriptor condition = null;

            SOARoleContext context = SOARoleContext.Current;

            if (context != null)
            {
                if (context.Process != null)
                    condition = new WfConditionDescriptor(context.Process.Descriptor);
                else
                    condition = new WfConditionDescriptor(null);
            }

            return this.Query(row =>
            {
                bool matched = row.Values.MatchQueryValues(queryParams, matchAny);

                if (matched)
                {
                    if (condition != null)
                    {
                        condition.Expression = row.Values.GetValue(SOARolePropertyDefinition.ConditionColumn,
                            row.GetPropertyDefinitions().GetColumnDefaultValue(SOARolePropertyDefinition.ConditionColumn, string.Empty));

                        matched = row.EvaluateCondition(condition);
                    }
                }

                return matched;
            });
        }

        /// <summary>
        /// 不带条件地筛选
        /// </summary>
        /// <param name="queryParams"></param>
        /// <param name="matchAny"></param>
        /// <returns></returns>
        public SOARolePropertyRowCollection QueryWithoutCondition(IEnumerable<SOARolePropertiesQueryParam> queryParams, bool matchAny = false)
        {
            WfConditionDescriptor condition = null;

            SOARoleContext context = SOARoleContext.Current;

            if (context != null)
            {
                if (context.Process != null)
                    condition = new WfConditionDescriptor(context.Process.Descriptor);
                else
                    condition = new WfConditionDescriptor(null);
            }

            return this.Query(row => row.Values.MatchQueryValues(queryParams, matchAny));
        }

        /// <summary>
        /// 按照条件列进行数据行的筛选，返回筛选后的行集合
        /// </summary>
        /// <returns></returns>
        public SOARolePropertyRowCollection FilterByConditionColumn()
        {
            WfConditionDescriptor condition = null;

            SOARoleContext context = SOARoleContext.Current;

            if (context != null)
            {
                if (context.Process != null)
                    condition = new WfConditionDescriptor(context.Process.Descriptor);
                else
                    condition = new WfConditionDescriptor(null);
            }

            return this.Query(row =>
            {
                bool matched = true;

                if (condition != null)
                {
                    condition.Expression = row.Values.GetValue(SOARolePropertyDefinition.ConditionColumn,
                        row.GetPropertyDefinitions().GetColumnDefaultValue(SOARolePropertyDefinition.ConditionColumn, string.Empty));

                    matched = row.EvaluateCondition(condition);
                }

                return matched;
            });
        }

        public int ReplaceOperators(SOARoleOperatorType typeFilter, string originalOperator, params string[] replaceOperators)
        {
            int result = 0;

            foreach (SOARolePropertyRow row in this)
            {
                if (row.OperatorType == typeFilter)
                    result += row.ReplaceOperator(originalOperator, replaceOperators);
            }

            return result;
        }

        /// <summary>
        /// 直接生成行和用户信息，只有用户信息，不包含OperationType为角色的信息
        /// </summary>
        /// <returns></returns>
        public SOARolePropertyRowUsersCollection GenerateRowsUsersDirectly()
        {
            SOARolePropertyRowUsersCollection result = new SOARolePropertyRowUsersCollection();

            foreach (SOARolePropertyRow row in this)
            {
                if (row.OperatorType == SOARoleOperatorType.User)
                {
                    SOARolePropertyRowUsers rowUsers = new SOARolePropertyRowUsers(row);

                    rowUsers.ObjectIDs = GenerateObjectIDs(row, (r) => r.Operator);

                    result.Add(rowUsers);
                }
            }

            FillPersonTypeUsers(result,
                (rowUsers) => rowUsers.ObjectIDs,
                (rowUsers) => rowUsers.Row.OperatorType,
                (rowUsers, userDicts) => FillMatchedUsers(rowUsers.ObjectIDs, userDicts, rowUsers.Users));

            return result;
        }

        /// <summary>
        /// 得到OperationType为Role的角色信息
        /// </summary>
        /// <returns></returns>
        public SOARolePropertyRowRolesCollection GenerateRowsRolesDirectly()
        {
            SOARolePropertyRowRolesCollection result = new SOARolePropertyRowRolesCollection();

            foreach (SOARolePropertyRow row in this)
            {
                if (row.OperatorType == SOARoleOperatorType.Role)
                {
                    SOARolePropertyRowRoles rowRoles = new SOARolePropertyRowRoles(row);

                    //不考虑动态角色
                    if (rowRoles.Row.Operator.IndexOf(":") >= 0)
                    {
                        SOARole role = new SOARole(rowRoles.Row.Operator);

                        ExceptionHelper.DoSilentAction(() =>
                        {
                            //尝试读取一下，如果没有角色则吃掉异常
                            Trace.WriteLine(role.CodeName);

                            rowRoles.Roles.Add(role);
                        });
                    }

                    result.Add(rowRoles);
                }
            }

            return result;
        }

        public SOARolePropertyRowUsersCollection GenerateRowsUsers()
        {
            SOARolePropertyRowUsersCollection result = new SOARolePropertyRowUsersCollection();

            foreach (SOARolePropertyRow row in this)
            {
                //内嵌矩阵，有可能行信息（原始矩阵的行）重复
                if (result.ContainsKey(row) == false)
                {
                    SOARolePropertyRowUsers rowUsers = new SOARolePropertyRowUsers(row);

                    rowUsers.ObjectIDs = GenerateObjectIDs(row, r => r.Operator);
                    rowUsers.EnterNotifyIDs = GenerateObjectIDs(row, r => r.Values.GetValue(SOARolePropertyDefinition.EnterNotifyReceiverColumn, string.Empty));
                    rowUsers.LeaveNotifyIDs = GenerateObjectIDs(row, r => r.Values.GetValue(SOARolePropertyDefinition.LeaveNotifyReceiverColumn, string.Empty));

                    result.Add(rowUsers);
                }
            }

            FillPersonTypeUsers(result,
                (rowUsers) => rowUsers.ObjectIDs,
                (rowUsers) => rowUsers.Row.OperatorType,
                (rowUsers, userDicts) => FillMatchedUsers(rowUsers.ObjectIDs, userDicts, rowUsers.Users));

            FillRoleTypeUsers(result,
                (rowUsers) => rowUsers.Row.OperatorType,
                (rowUsers) => rowUsers.Row.Operator,
                (rowUsers, user) => rowUsers.Users.AddNotExistsItem(user, u => string.Compare(u.ID, user.ID, true) == 0));

            FillPersonTypeUsers(result,
                (rowUsers) => rowUsers.EnterNotifyIDs,
                (rowUsers) => rowUsers.Row.Values.GetValue(SOARolePropertyDefinition.EnterNotifyReceiverTypeColumn, SOARoleOperatorType.User),
                (rowUsers, userDicts) => FillMatchedUsers(rowUsers.EnterNotifyIDs, userDicts, rowUsers.EnterNotifyUsers));

            FillRoleTypeUsers(result,
                (rowUsers) => rowUsers.Row.Values.GetValue(SOARolePropertyDefinition.EnterNotifyReceiverTypeColumn, SOARoleOperatorType.Role),
                (rowUsers) => rowUsers.Row.Values.GetValue(SOARolePropertyDefinition.EnterNotifyReceiverColumn, string.Empty),
                (rowUsers, user) => rowUsers.EnterNotifyUsers.AddNotExistsItem(user, u => string.Compare(u.ID, user.ID, true) == 0));

            FillPersonTypeUsers(result,
                (rowUsers) => rowUsers.LeaveNotifyIDs,
                (rowUsers) => rowUsers.Row.Values.GetValue(SOARolePropertyDefinition.LeaveNotifyReceiverTypeColumn, SOARoleOperatorType.User),
                (rowUsers, userDicts) => FillMatchedUsers(rowUsers.LeaveNotifyIDs, userDicts, rowUsers.LeaveNotifyUsers));

            FillRoleTypeUsers(result,
                (rowUsers) => rowUsers.Row.Values.GetValue(SOARolePropertyDefinition.LeaveNotifyReceiverTypeColumn, SOARoleOperatorType.Role),
                (rowUsers) => rowUsers.Row.Values.GetValue(SOARolePropertyDefinition.LeaveNotifyReceiverColumn, string.Empty),
                (rowUsers, user) => rowUsers.LeaveNotifyUsers.AddNotExistsItem(user, u => string.Compare(u.ID, user.ID, true) == 0));

            result.SortByActivitySN();
            result.RemoveMergeableRows();

            return result;
        }

        /// <summary>
        /// 重新按照ActivitySN排序
        /// </summary>
        public void SortActivitySN()
        {
            int index = 0;

            Dictionary<string, string> snDictionary = new Dictionary<string, string>();

            foreach (SOARolePropertyRow row in this)
            {
                SOARolePropertyValue snValue = row.Values.Find(pv => string.Compare(SOARolePropertyDefinition.ActivitySNColumn, pv.Column.Name) == 0);

                if (snValue != null)
                {
                    string existedSN = string.Empty;

                    if (snDictionary.TryGetValue(snValue.Value, out existedSN))
                    {
                        snValue.Value = existedSN;
                    }
                    else
                    {
                        string originalValue = snValue.Value;
                        snValue.Value = (++index * 10).ToString();

                        snDictionary.Add(originalValue, snValue.Value);
                    }
                }
                //snValue.Value = (++index * 10).ToString();
            }
        }

        //沈峥注释掉，2015/6/21.必须提供列定义
        //public void FillCreateActivityParams(WfCreateActivityParamCollection capc, PropertyDefineCollection definedProperties)
        //{
        //    SOARolePropertyDefinitionCollection definitions = null;

        //    SOARole role = this._Role as SOARole;

        //    if (role != null)
        //        definitions = role.PropertyDefinitions;
        //    else
        //        definitions = new SOARolePropertyDefinitionCollection();

        //    this.FillCreateActivityParams(capc, definitions, definedProperties);
        //}

        public void FillCreateActivityParams(WfCreateActivityParamCollection capc, SOARolePropertyDefinitionCollection definitions, PropertyDefineCollection definedProperties)
        {
            capc.NullCheck("capc");
            definitions.NullCheck("definitions");

            SOARolePropertyRowUsersCollection rowsUsers = GenerateRowsUsers();

            rowsUsers.ForEach(rowUsers => capc.Add(WfCreateActivityParam.FromRowUsers(rowUsers, definitions, definedProperties)));

            if (definitions.MatrixType == WfMatrixType.ActivityMatrix)
                capc.MergeSameActivityParamBySN();

            capc.ForEach(cap => InitTransitionTemplatesProperties(cap, definitions, cap.Source));
        }

        /// <summary>
        /// 将包含的子角色矩阵进行展开
        /// </summary>
        /// <returns></returns>
        public SOARolePropertyRowCollection ExtractMatrixRows()
        {
            SOARolePropertyRowCollection result = new SOARolePropertyRowCollection();

            foreach (SOARolePropertyRow row in this)
            {
                SOARolePropertyRowCollection extractedRows = row.ExtractMatrixRows();

                extractedRows.FillClonedRows(result);
                //result.CopyFrom(extractedRows);
            }

            result.SortActivitySN();

            return result;
        }

        /// <summary>
        /// 从DataRow构造行信息
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="definitions"></param>
        internal void FromDataTable(DataRowCollection rows, IEnumerable<SOARolePropertyDefinition> definitions)
        {
            rows.NullCheck("rows");
            definitions.NullCheck("definitions");

            this.Clear();

            int rowIndex = 1;

            foreach (DataRow row in rows)
            {
                SOARolePropertyRow mRow = new SOARolePropertyRow() { RowNumber = rowIndex++ };

                foreach (SOARolePropertyDefinition definition in definitions)
                {
                    SOARolePropertyValue mCell = new SOARolePropertyValue(definition);
                    mCell.Value = row[definition.Name].ToString();

                    switch (definition.Name)
                    {
                        case SOARolePropertyDefinition.OperatorColumn:
                            mRow.Operator = row[definition.Name].ToString();
                            break;
                        case SOARolePropertyDefinition.OperatorTypeColumn:
                            SOARoleOperatorType opType = SOARoleOperatorType.User;
                            Enum.TryParse(row[definition.Name].ToString(), out opType);
                            mRow.OperatorType = opType;
                            break;
                        default:
                            break;
                    }

                    mRow.Values.Add(mCell);
                }

                this.Add(mRow);
            }
        }

        /// <summary>
        /// 初始化出线的属性
        /// </summary>
        /// <param name="cap"></param>
        /// <param name="row"></param>
        private static void InitTransitionTemplatesProperties(WfCreateActivityParam cap, SOARolePropertyDefinitionCollection definitions, SOARolePropertyRow row)
        {
            string json = row.Values.GetValue(SOARolePropertyDefinition.TransitionsColumn,
                row.GetPropertyDefinitions().GetColumnDefaultValue(SOARolePropertyDefinition.TransitionsColumn, string.Empty));

            if (json.IsNotEmpty())
                cap.TransitionTemplates.FromJson(json);
        }

        ///// <summary>
        ///// 在SOARolePropertyRowUsersCollection中填充人员
        ///// </summary>
        ///// <param name="existedRowsUsers"></param>
        //private static void FillPersonTypeUsers(SOARolePropertyRowUsersCollection existedRowsUsers)
        //{
        //    List<string> userLogonNames = new List<string>();

        //    foreach (SOARolePropertyRowUsers rowUsers in existedRowsUsers)
        //        rowUsers.ObjectIDs.ForEach(logonName => userLogonNames.Add(logonName));

        //    Dictionary<string, IUser> userDicts = GetUsersDictionary(userLogonNames);
        //    foreach (SOARolePropertyRowUsers rowUsers in existedRowsUsers)
        //    {
        //        if (rowUsers.Row.OperatorType == SOARoleOperatorType.User)
        //        {
        //            foreach (string id in rowUsers.ObjectIDs)
        //            {
        //                IUser user = null;
        //                if (userDicts.TryGetValue(id, out user))
        //                    rowUsers.Users.Add((IUser)OguUser.CreateWrapperObject(user));
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 在SOARolePropertyRowUsersCollection中填充人员
        /// </summary>
        /// <param name="existedRowsUsers"></param>
        /// <param name="getLogonNames"></param>
        /// <param name="predicate"></param>
        /// <param name="action"></param>
        private static void FillPersonTypeUsers(SOARolePropertyRowUsersCollection existedRowsUsers,
            Func<SOARolePropertyRowUsers,
            IEnumerable<string>> getLogonNames,
            Func<SOARolePropertyRowUsers, SOARoleOperatorType> predicate,
            Action<SOARolePropertyRowUsers, Dictionary<string, IUser>> action)
        {
            List<string> userLogonNames = new List<string>();

            //foreach (SOARolePropertyRowUsers rowUsers in existedRowsUsers)
            //    rowUsers.ObjectIDs.ForEach(logonName => userLogonNames.Add(logonName));
            foreach (SOARolePropertyRowUsers rowUsers in existedRowsUsers)
                getLogonNames(rowUsers).ForEach(logonName => userLogonNames.Add(logonName));

            Dictionary<string, IUser> userDicts = GetUsersDictionary(userLogonNames);

            foreach (SOARolePropertyRowUsers rowUsers in existedRowsUsers)
            {
                if (predicate(rowUsers) == SOARoleOperatorType.User)
                    action(rowUsers, userDicts);
            }
        }

        private static void FillMatchedUsers(IEnumerable<string> objectIDs, Dictionary<string, IUser> userDicts, OguDataCollection<IUser> target)
        {
            foreach (string id in objectIDs)
            {
                IUser user = null;
                if (userDicts.TryGetValue(id, out user))
                    target.Add((IUser)OguUser.CreateWrapperObject(user));
            }
        }

        /// <summary>
        /// 根据用户的登录名，去除重复的数据，生成登录名和用户对象的字典
        /// </summary>
        /// <param name="userLogonNames"></param>
        /// <returns></returns>
        private static Dictionary<string, IUser> GetUsersDictionary(IEnumerable<string> userLogonNames)
        {
            Dictionary<string, string> userLogonNamesDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (string logonName in userLogonNames)
                userLogonNamesDict[logonName] = logonName;

            OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, userLogonNamesDict.Keys.ToArray());

            if (WfRuntime.ProcessContext.CurrentProcess != null)
                users = users.FilterUniqueSidelineUsers(WfRuntime.ProcessContext.CurrentProcess.OwnerDepartment);

            return GenerateUserDictionary(users);
        }

        /// <summary>
        /// 在SOARolePropertyRowUsersCollection中填充角色中的人员
        /// </summary>
        /// <param name="existedRowsUsers"></param>
        /// <param name="getType"></param>
        /// <param name="getOp"></param>
        /// <param name="action"></param>
        private void FillRoleTypeUsers(SOARolePropertyRowUsersCollection existedRowsUsers, Func<SOARolePropertyRowUsers, SOARoleOperatorType> getType, Func<SOARolePropertyRowUsers, string> getOp, Action<SOARolePropertyRowUsers, IUser> action)
        {
            foreach (SOARolePropertyRowUsers rowUsers in existedRowsUsers)
            {
                OguDataCollection<IUser> users = new OguDataCollection<IUser>();

                FillRoleTypeOperatorToUsers(getType(rowUsers), getOp(rowUsers), users);

                foreach (IUser userInRole in users)
                    action(rowUsers, userInRole);
            }
        }

        /// <summary>
        /// 将角色类型的操作人填充到用户集合中
        /// </summary>
        /// <param name="users"></param>
        /// <param name="operatorType"></param>
        /// <param name="operatorDesp"></param>
        private static void FillRoleTypeOperatorToUsers(SOARoleOperatorType operatorType, string operatorDesp, OguDataCollection<IUser> users)
        {
            switch (operatorType)
            {
                case SOARoleOperatorType.Role:
                    if (operatorDesp.IndexOf(":") < 0)
                        SOARolePropertyRow.FillInternalDynamicRoleUsers(operatorDesp, users);
                    else
                        FillRoleUsers(operatorDesp, users);
                    break;
                case SOARoleOperatorType.AURole:
                    FillAURoleUsers(operatorDesp, users);
                    break;
            }
        }

        private static void FillRoleUsers(string roleName, OguDataCollection<IUser> users)
        {
            SOARole role = new SOARole(roleName);

            WfRoleResourceDescriptor roleResource = new WfRoleResourceDescriptor(role);

            roleResource.FillUsers(users);
        }

        private static void FillAURoleUsers(string roleFullCodeName, OguDataCollection<IUser> users)
        {
            WrappedAUSchemaRole role = WrappedAUSchemaRole.FromCodeName(roleFullCodeName);

            if (role != null)
                role.FillUsers(SOARoleContext.CurrentProcess, users);
        }

        private static Dictionary<string, IUser> GenerateUserDictionary(IEnumerable<IUser> users)
        {
            Dictionary<string, IUser> result = new Dictionary<string, IUser>(StringComparer.OrdinalIgnoreCase);

            foreach (IUser user in users)
            {
                IUser originalUser = null;

                if (result.TryGetValue(user.LogOnName, out originalUser))
                {
                    if (originalUser.IsSideline && user.IsSideline == false)
                        result[user.LogOnName] = user;
                }
                else
                {
                    result.Add(user.LogOnName, user);
                }
            }

            return result;
        }

        private static List<string> GenerateObjectIDs(SOARolePropertyRow row, Func<SOARolePropertyRow, string> getUserID)
        {
            List<string> result = _EmptyStringList;

            string userIDs = getUserID(row);

            if (userIDs.IsNotEmpty())
                result = SOARolePropertyRow.GenerateObjectIDs(userIDs);

            return result;
        }

        protected override void OnInsert(int index, object value)
        {
            base.OnInsert(index, value);

            if (this._Role != null)
                ((SOARolePropertyRow)value).Role = this._Role;
        }

        protected override void OnSet(int index, object oldValue, object newValue)
        {
            base.OnSet(index, oldValue, newValue);

            if (this._Role != null)
                ((SOARolePropertyRow)newValue).Role = this._Role;
        }
    }
}
