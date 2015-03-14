using MCS.Library.Caching;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Expression;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.SOA.DataObjects.Workflow.Builders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// 从一个已经存在的行构造
        /// </summary>
        /// <param name="templateRow"></param>
        /// <param name="rowNumber"></param>
        public SOARolePropertyRow(SOARolePropertyRow templateRow, int rowNumber)
        {
            templateRow.NullCheck("templateRow");

            this.Role = templateRow.Role;
            this.RowNumber = rowNumber;
            this.OperatorType = templateRow.OperatorType;
            this.Operator = templateRow.Operator;

            foreach (SOARolePropertyValue srv in templateRow.Values)
            {
                SOARolePropertyValue newSrv = new SOARolePropertyValue(srv.Column);

                newSrv.Value = srv.Value;

                this.Values.Add(newSrv);
            }
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
            return this.Values.GetValue(SOARolePropertyDefinition.IsMergeableColumn,
                this.GetPropertyDefinitions().GetColumnDefaultValue(SOARolePropertyDefinition.IsMergeableColumn, false));
        }

        /// <summary>
        /// 如果行的内容是角色，则将这些行扩展出来
        /// </summary>
        public SOARolePropertyRowCollection ExtractMatrixRows()
        {
            SOARolePropertyRowCollection extractedRows = new SOARolePropertyRowCollection();
            bool extracted = false;

            if (this.Operator.IsNotEmpty())
            {
                if (this.Operator.IndexOf(":") != -1)
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
                else
                {
                    if (this.Values.GetValue(SOARolePropertyDefinition.AutoExtractColumn, false))
                    {
                        switch (this.OperatorType)
                        {
                            case SOARoleOperatorType.Role:
                                extracted = this.ExtractDynamicRoleMatrixRows(this.Operator, extractedRows);
                                break;
                            case SOARoleOperatorType.User:
                                extracted = this.ExtractUserMatrixRows(this.Operator, extractedRows);
                                break;
                        }
                    }
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

                SOARolePropertyValue cell = this.Values.FindByColumnName(SOARolePropertyDefinition.OperatorColumn);

                if (cell != null)
                    cell.Value = this.Operator;
            }

            return result;
        }

        public T GetValue<T>(string name)
        {
            return this.Values.GetValue(name, this.GetPropertyDefinitions().GetColumnDefaultValue("name", default(T)));
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

        internal SOARolePropertyDefinitionCollection GetPropertyDefinitions()
        {
            SOARolePropertyDefinitionCollection result = SOARolePropertyDefinition.EmptyInstance;

            if (SOARoleContext.Current != null)
            {
                result = SOARoleContext.Current.PropertyDefinitions;
            }
            else
            {
                if (this.Role != null)
                    result = ((SOARole)this.Role).PropertyDefinitions;
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
                        extracted = this.ExtractSOARoleMatrixRows(innerRole, extractedRows);
                    }
                }
                finally
                {
                    SOARoleContext.Current = originalContext;
                }
            }

            return extracted;
        }

        private bool ExtractSOARoleMatrixRows(SOARole innerRole, SOARolePropertyRowCollection extractedRows)
        {
            bool extracted = false;

            SOARoleContext.DoAction(innerRole, SOARoleContext.CurrentProcess, (context) =>
            {
                SOARolePropertyRowCollection subRows = innerRole.Rows.Query(context.QueryParams);

                if (((SOARole)this.Role).MatrixType == WfMatrixType.ActivityMatrix && innerRole.MatrixType != WfMatrixType.ActivityMatrix)
                    subRows = MergeActivityRowPropertiesByRows(subRows);

                foreach (SOARolePropertyRow subRow in subRows)
                {
                    SOARolePropertyRowCollection subExtractedRows = subRow.ExtractMatrixRows();

                    extractedRows.CopyFrom(subExtractedRows);
                }

                extracted = true;
            });

            return extracted;
        }

        private bool ExtractDynamicRoleMatrixRows(string roleName, SOARolePropertyRowCollection extractedRows)
        {
            OguDataCollection<IUser> users = new OguDataCollection<IUser>();

            FillInternalDynamicRoleUsers(roleName, users);

            extractedRows.CopyFrom(this.MergeActivityRowPropertiesByUsers(users));

            return users.Count > 0;
        }

        private bool ExtractUserMatrixRows(string operators, SOARolePropertyRowCollection extractedRows)
        {
            List<string> userIDs = GenerateObjectIDs(operators);

            extractedRows.CopyFrom(this.MergeActivityRowPropertiesByOperators(userIDs));

            return userIDs.Count > 0;
        }

        internal static List<string> GenerateObjectIDs(string operators)
        {
            List<string> objIds = new List<string>();

            if (operators.IsNotEmpty())
            {
                string[] ids = operators.Split(SOARolePropertyRow.OperatorSplitters, StringSplitOptions.RemoveEmptyEntries);

                foreach (string id in ids)
                {
                    string trimmedID = id.Trim();

                    if (objIds.Exists(eid => string.Compare(eid, id, true) == 0) == false)
                        objIds.Add(trimmedID);
                }
            }

            return objIds;
        }

        private SOARolePropertyRowCollection MergeActivityRowPropertiesByUsers(IEnumerable<IUser> users)
        {
            SOARolePropertyRowCollection result = new SOARolePropertyRowCollection();

            int index = 0;
            int activitySN = this.Values.GetValue(SOARolePropertyDefinition.ActivitySNColumn, 0);

            foreach (IUser user in users)
            {
                SOARolePropertyRow newRow = new SOARolePropertyRow(this, 0);

                if (this.GetPropertyDefinitions().MatrixType == WfMatrixType.ActivityMatrix)
                {
                    SOARolePropertyValue pv = newRow.Values.Find(v => string.Compare(v.Column.Name, SOARolePropertyDefinition.ActivitySNColumn, true) == 0);

                    if (pv == null)
                    {
                        pv = new SOARolePropertyValue(this.GetPropertyDefinitions()[SOARolePropertyDefinition.ActivitySNColumn]);
                        newRow.Values.Add(pv);
                    }

                    pv.Value = string.Format("{0}.{1:0000}", activitySN, ++index);
                }

                newRow.OperatorType = SOARoleOperatorType.User;
                newRow.Operator = user.LogOnName;

                result.Add(newRow);
            }

            return result;
        }

        private SOARolePropertyRowCollection MergeActivityRowPropertiesByOperators(IEnumerable<string> operators)
        {
            SOARolePropertyRowCollection result = new SOARolePropertyRowCollection();

            int index = 0;
            int activitySN = this.Values.GetValue(SOARolePropertyDefinition.ActivitySNColumn, 0);

            foreach (string op in operators)
            {
                SOARolePropertyRow newRow = new SOARolePropertyRow(this, 0);

                if (this.GetPropertyDefinitions().MatrixType == WfMatrixType.ActivityMatrix)
                {
                    SOARolePropertyValue pv = newRow.Values.Find(v => string.Compare(v.Column.Name, SOARolePropertyDefinition.ActivitySNColumn, true) == 0);

                    if (pv == null)
                    {
                        pv = new SOARolePropertyValue(this.GetPropertyDefinitions()[SOARolePropertyDefinition.ActivitySNColumn]);
                        newRow.Values.Add(pv);
                    }

                    pv.Value = string.Format("{0}.{1:0000}", activitySN, ++index);
                }

                newRow.OperatorType = SOARoleOperatorType.User;
                newRow.Operator = op;

                result.Add(newRow);
            }

            return result;
        }

        private SOARolePropertyRowCollection MergeActivityRowPropertiesByRows(SOARolePropertyRowCollection subExtractedRows)
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

        internal static void FillInternalDynamicRoleUsers(string roleName, OguDataCollection<IUser> users)
        {
            SOARoleContext context = SOARoleContext.Current;

            WfDynamicResourceDescriptor dynResource = new WfDynamicResourceDescriptor();

            if (context != null)
                dynResource.SetProcessInstance(context.Process);

            dynResource.Condition.Expression = roleName;

            dynResource.FillUsers(users);
        }
    }
}
