using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Expression;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using MCS.Web.Library.Script;
using System.Data;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 角色中包含的人，目前只支持人员
    /// </summary>
    public enum SOARoleOperatorType
    {
        //Organization = 1,
        User = 2,

        //Group = 4
        /// <summary>
        /// 角色和动态角色
        /// </summary>
        Role = 8,

        /// <summary>
        /// 管理单元角色
        /// </summary>
        AURole = 16,
    }

    [Serializable]
    [ORTableMapping("WF.ROLE_PROPERTIES_ROWS")]
    [TenantRelativeObject]
    public class SOARolePropertyRow : TableRowBase<SOARolePropertyDefinition, SOARolePropertyValue, SOARolePropertyValueCollection, string>
    {
        public static readonly char[] OperatorSplitters = new char[] { ',', '，' };

        public SOARolePropertyRow()
        {
        }

        public SOARolePropertyRow(IRole role)
        {
            this.Role = role;
        }

        [ORFieldMapping("ROW_NUMBER")]
        public int RowNumber
        {
            get;
            set;
        }

        [NoMapping]
        internal IRole Role
        {
            get;
            set;
        }

        [ORFieldMapping("OPERATOR")]
        public string Operator
        {
            get;
            set;
        }

        private SOARoleOperatorType _OperatorType = SOARoleOperatorType.User;

        [ORFieldMapping("OPERATOR_TYPE")]
        public SOARoleOperatorType OperatorType
        {
            get
            {
                return this._OperatorType;
            }
            set
            {
                if (value == 0)
                {
                    this._OperatorType = SOARoleOperatorType.User;
                }
                else
                {
                    this._OperatorType = value;
                }
            }
        }

        /// <summary>
        /// 是否可以合并(判断是否有IsMergeable属性)
        /// </summary>
        /// <returns></returns>
        public bool IsMergeable()
        {
            bool result = false;

            string mergeable = this.Values.GetValue("IsMergeable", "False");

            try
            {
                result = (bool)DataConverter.ChangeType(mergeable, typeof(bool));
            }
            catch (System.Exception)
            {
            }

            return result;
        }

        /// <summary>
        /// 如果行的内容是角色，则将这些行扩展出来
        /// </summary>
        public SOARolePropertyRowCollection ExtractMatrixRows()
        {
            SOARolePropertyRowCollection extractedRows = new SOARolePropertyRowCollection();
            bool extracted = false;

            if (this.Operator.IsNotEmpty() && this.Operator.IndexOf(":") != -1)
            {
                switch (this.OperatorType)
                {
                    case SOARoleOperatorType.Role:
                        extracted = this.ExtractSOARoleRows(new SOARole(this.Operator), extractedRows);
                        break;
                    case SOARoleOperatorType.AURole:
                        WrappedAUSchemaRole wrappedRole = WrappedAUSchemaRole.FromCodeName(this.Operator);

                        if (wrappedRole != null)
                            wrappedRole.DoCurrentRoleAction(SOARoleContext.CurrentProcess,
                                (role, auCodeName) => extracted = this.ExtractSOARoleRows(role, extractedRows));
                        break;
                }
            }

            //如果没有展开行，则将当前行添加到结果中
            if (extracted == false)
                extractedRows.Add(this);

            return extractedRows;
        }

        /// <summary>
        /// 将某个操作人，换成某几个操作人
        /// </summary>
        /// <param name="originalOperator"></param>
        /// <param name="replaceOperators"></param>
        /// <returns>替换了几次。最多是一次替换。没有替换则返回0</returns>
        public int ReplaceOperator(string originalOperator, params string[] replaceOperators)
        {
            int result = 0;

            if (this.Operator.IsNotEmpty() && originalOperator.IsNotEmpty())
            {
                string[] originalUsers = this.Operator.Split(SOARolePropertyRow.OperatorSplitters, StringSplitOptions.RemoveEmptyEntries);

                //先整理一下需要被替换的人
                List<string> replaceUsers = new List<string>();

                if (replaceOperators != null)
                {
                    foreach (string user in replaceOperators)
                    {
                        if (originalUsers.Exists(s => string.Compare(s, user, true) == 0) == false)
                            replaceUsers.Add(user);
                    }
                }

                List<string> targetUsers = new List<string>();

                bool added = false;

                foreach (string user in originalUsers)
                {
                    if (string.Compare(user, originalOperator, true) == 0)
                    {
                        result = 1;

                        if (added == false)
                        {
                            targetUsers.AddRange(replaceUsers);
                            added = true;
                        }
                    }
                    else
                        targetUsers.Add(user);
                }

                this.Operator = string.Join(",", targetUsers.ToArray());

                SOARolePropertyValue cell = this.Values.FindByColumnName("Operator");

                if (cell != null)
                    cell.Value = this.Operator;
            }

            return result;
        }

        public bool EvaluateCondition(WfConditionDescriptor condition)
        {
            bool result = condition.IsEmpty;

            try
            {
                ObjectContextCache.Instance["RoleMatrixCurrentRow"] = this;

                try
                {
                    result = condition.Evaluate(EvaluateRoleMatrixCondition);
                }
                catch (System.Exception ex)
                {
                    ExceptionHelper.FalseThrow<WfDynamicResourceEvaluationException>(false,
                        Translator.Translate(WfHelper.CultureCategory, "角色矩阵表达式解析错误：({0})\n{1}", condition, ex.Message));
                }
                finally
                {
                    ObjectContextCache.Instance.Remove("RoleMatrixCurrentRow");
                }
            }
            catch (WfDynamicResourceEvaluationException ex)
            {
                ex.WriteToLog();
            }

            return result;
        }

        /// <summary>
        /// 展开内部角色的行
        /// </summary>
        /// <param name="innerRole"></param>
        /// <param name="extractedRows"></param>
        /// <returns></returns>
        private bool ExtractSOARoleRows(SOARole innerRole, SOARolePropertyRowCollection extractedRows)
        {
            bool extracted = false;

            if (innerRole.PropertyDefinitions.Count > 0)
            {
                SOARoleContext originalContext = SOARoleContext.Current;
                SOARoleContext.Current = null;
                try
                {
                    using (SOARoleContext innerContext = SOARoleContext.CreateContext(innerRole, originalContext.Process))
                    {
                        SOARoleContext.DoAction(innerRole, SOARoleContext.CurrentProcess, (context) =>
                        {
                            SOARolePropertyRowCollection subRows = innerRole.Rows.Query(context.QueryParams);

                            if (((SOARole)this.Role).MatrixType == WfMatrixType.ActivityMatrix && innerRole.MatrixType != WfMatrixType.ActivityMatrix)
                                subRows = MergeActivityRowProperties(subRows);

                            foreach (SOARolePropertyRow subRow in subRows)
                            {
                                SOARolePropertyRowCollection subExtractedRows = subRow.ExtractMatrixRows();

                                extractedRows.CopyFrom(subExtractedRows);
                            }

                            extracted = true;
                        });
                    }
                }
                finally
                {
                    SOARoleContext.Current = originalContext;
                }
            }

            return extracted;
        }

        private SOARolePropertyRowCollection MergeActivityRowProperties(SOARolePropertyRowCollection subExtractedRows)
        {
            SOARolePropertyRowCollection result = new SOARolePropertyRowCollection();

            foreach (SOARolePropertyRow subRow in subExtractedRows)
            {
                SOARolePropertyRow newRow = new SOARolePropertyRow(this.Role);

                newRow.Values.CopyFrom(this.Values);
                newRow.OperatorType = subRow.OperatorType;
                newRow.Operator = subRow.Operator;

                result.Add(newRow);
            }

            return result;
        }

        private static object EvaluateRoleMatrixCondition(string funcName, ParamObjectCollection paramObjects, object callerContext)
        {
            object result = null;

            switch (funcName.ToLower())
            {
                case "rowoperators":
                    result = CalculateRowOperators();
                    break;
                case "rowoperators.count":
                    result = CalculateRowOperators().Count;
                    break;
                default:
                    result = WfRuntime.ProcessContext.FireEvaluateRoleMatrixCondition(funcName, paramObjects, callerContext);
                    break;
            }

            return result;
        }

        private static OguDataCollection<IUser> CalculateRowOperators()
        {
            OguDataCollection<IUser> result = new OguDataCollection<IUser>();

            if (ObjectContextCache.Instance.ContainsKey("RoleMatrixCurrentRow"))
            {
                SOARolePropertyRow row = (SOARolePropertyRow)ObjectContextCache.Instance["RoleMatrixCurrentRow"];

                SOARolePropertyRowCollection rows = new SOARolePropertyRowCollection();
                rows.Add(row);

                SOARolePropertyRowUsersCollection rowsUsers = rows.GenerateRowsUsers();

                rowsUsers.ForEach(ru => result.CopyFrom(ru.Users));
            }

            return result;
        }
    }

    [Serializable]
    public class SOARolePropertyRowCollection : TableRowCollectionBase<SOARolePropertyRow, SOARolePropertyDefinition, SOARolePropertyValue, SOARolePropertyValueCollection, string>
    {
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

        public SOARolePropertyRowCollection Query(Predicate<SOARolePropertyRow> predicate)
        {
            SOARolePropertyRowCollection result = new SOARolePropertyRowCollection(this._Role);

            foreach (SOARolePropertyRow row in this)
            {
                bool matched = true;

                matched = predicate(row);

                if (matched)
                    result.Add(row);
            }

            return result;
        }

        public SOARolePropertyRowCollection Query(IEnumerable<SOARolePropertiesQueryParam> queryParams)
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

            return Query(row =>
            {
                bool matched = true;

                foreach (SOARolePropertiesQueryParam queryParam in queryParams)
                {
                    if (row.Values.MatchQueryValue(queryParam) == false)
                    {
                        matched = false;
                        break;
                    }
                }

                if (matched)
                {
                    if (condition != null)
                    {
                        condition.Expression = row.Values.GetValue("Condition", (string)null);

                        matched = row.EvaluateCondition(condition);
                    }
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

                    rowUsers.ObjectIDs = GenerateObjectIDs(row);

                    result.Add(rowUsers);
                }
            }

            FillPersonTypeUsers(result);

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

                    rowUsers.ObjectIDs = GenerateObjectIDs(row);

                    result.Add(rowUsers);
                }
            }

            FillPersonTypeUsers(result);
            FillRoleTypeUsers(result);

            result.SortByActivitySN();
            result.RemoveMergeableRows();

            return result;
        }

        public void FillCreateActivityParams(WfCreateActivityParamCollection capc, PropertyDefineCollection definedProperties)
        {
            SOARolePropertyDefinitionCollection definitions = null;

            SOARole role = this._Role as SOARole;

            if (role != null)
                definitions = role.PropertyDefinitions;
            else
                definitions = new SOARolePropertyDefinitionCollection();

            FillCreateActivityParams(capc, definitions, definedProperties);
        }

        public void FillCreateActivityParams(WfCreateActivityParamCollection capc, SOARolePropertyDefinitionCollection definitions, PropertyDefineCollection definedProperties)
        {
            capc.NullCheck("capc");
            definitions.NullCheck("definitions");

            SOARolePropertyRowUsersCollection rowsUsers = GenerateRowsUsers();

            rowsUsers.ForEach(rowUsers => capc.Add(WfCreateActivityParam.FromRowUsers(rowUsers, definitions, definedProperties)));

            if (definitions.MatrixType == WfMatrixType.ActivityMatrix)
                capc.MergeSameActivityParamBySN();

            capc.ForEach(cap => InitTransitionTemplatesProperties(cap, cap.Source));
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

                result.CopyFrom(extractedRows);
            }

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
                        case "Operator":
                            mRow.Operator = row[definition.Name].ToString();
                            break;
                        case "OperatorType":
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
        private static void InitTransitionTemplatesProperties(WfCreateActivityParam cap, SOARolePropertyRow row)
        {
            string json = row.Values.GetValue("Transitions", string.Empty);

            if (json.IsNotEmpty())
                cap.TransitionTemplates.FromJson(json);
        }

        /// <summary>
        /// 在SOARolePropertyRowUsersCollection中填充人员
        /// </summary>
        /// <param name="existedRowsUsers"></param>
        private static void FillPersonTypeUsers(SOARolePropertyRowUsersCollection existedRowsUsers)
        {
            Dictionary<string, string> userIDs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (SOARolePropertyRowUsers rowUsers in existedRowsUsers)
            {
                if (rowUsers.Row.OperatorType == SOARoleOperatorType.User)
                {
                    foreach (string id in rowUsers.ObjectIDs)
                        userIDs[id] = id;
                }
            }

            List<string> logonNames = new List<string>();

            foreach (KeyValuePair<string, string> kp in userIDs)
                logonNames.Add(kp.Key);

            OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, logonNames.ToArray());

            if (WfRuntime.ProcessContext.CurrentProcess != null)
                users = users.FilterUniqueSidelineUsers(WfRuntime.ProcessContext.CurrentProcess.OwnerDepartment);

            Dictionary<string, IUser> userDicts = GenerateUserDictionary(users);

            foreach (SOARolePropertyRowUsers rowUsers in existedRowsUsers)
            {
                if (rowUsers.Row.OperatorType == SOARoleOperatorType.User)
                {
                    foreach (string id in rowUsers.ObjectIDs)
                    {
                        IUser user = null;
                        if (userDicts.TryGetValue(id, out user))
                            rowUsers.Users.Add((IUser)OguUser.CreateWrapperObject(user));
                    }
                }
            }
        }

        /// <summary>
        /// 在SOARolePropertyRowUsersCollection中填充角色中的人员
        /// </summary>
        /// <param name="existedRowsUsers"></param>
        private void FillRoleTypeUsers(SOARolePropertyRowUsersCollection existedRowsUsers)
        {
            foreach (SOARolePropertyRowUsers rowUsers in existedRowsUsers)
            {
                OguDataCollection<IUser> users = new OguDataCollection<IUser>();

                switch (rowUsers.Row.OperatorType)
                {
                    case SOARoleOperatorType.Role:
                        if (rowUsers.Row.Operator.IndexOf(":") < 0)
                            FillInternalDynamicRoleUsers(rowUsers.Row.Operator, users);
                        else
                            FillRoleUsers(rowUsers.Row.Operator, users);
                        break;
                    case SOARoleOperatorType.AURole:
                        FillAURoleUsers(rowUsers.Row.Operator, users);
                        break;
                }

                foreach (IUser userInRole in users)
                {
                    if (rowUsers.Users.Exists(u => string.Compare(u.ID, userInRole.ID, true) == 0) == false)
                        rowUsers.Users.Add(userInRole);
                }
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

        private static void FillInternalDynamicRoleUsers(string roleName, OguDataCollection<IUser> users)
        {
            SOARoleContext context = SOARoleContext.Current;

            WfDynamicResourceDescriptor dynResource = new WfDynamicResourceDescriptor();

            if (context != null)
                dynResource.SetProcessInstance(context.Process);

            dynResource.Condition.Expression = roleName;

            dynResource.FillUsers(users);
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

        private static List<string> GenerateObjectIDs(SOARolePropertyRow row)
        {
            List<string> objIds = new List<string>();

            if (row.Operator != null)
            {
                string[] ids = row.Operator.Split(SOARolePropertyRow.OperatorSplitters, StringSplitOptions.RemoveEmptyEntries);

                foreach (string id in ids)
                {
                    string trimmedID = id.Trim();

                    objIds.Add(trimmedID);
                }
            }

            return objIds;
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
