using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.SOA.DataObjects.Workflow.Conditions
{
    /// <summary>
    /// 查询流程实例的条件类
    /// </summary>
    [Serializable]
    public class WfProcessQueryCondition
    {
        [ConditionMapping("APPLICATION_NAME", EscapeLikeString = true, Prefix = "%", Postfix = "%", Operation = "LIKE")]
        public string ApplicationName
        {
            get;
            set;
        }

        [ConditionMapping("PROGRAM_NAME", EscapeLikeString = true, Prefix = "%", Postfix = "%", Operation = "LIKE")]
        public string ProgramName
        {
            get;
            set;
        }

        [ConditionMapping("PROCESS_NAME", EscapeLikeString = true, Prefix = "%", Postfix = "%", Operation = "LIKE")]
        public string ProcessName
        {
            get;
            set;
        }

        [ConditionMapping("DEPARTMENT_NAME", EscapeLikeString = true, Prefix = "%", Postfix = "%", Operation = "LIKE")]
        public string DepartmentName
        {
            get;
            set;
        }

        [ConditionMapping("START_TIME", ">=")]
        public DateTime BeginStartTime
        {
            get;
            set;
        }

        [ConditionMapping("START_TIME", "<", AdjustDays = 1)]
        public DateTime EndStartTime
        {
            get;
            set;
        }

        [ConditionMapping("STATUS")]
        public string ProcessStatus
        {
            get;
            set;
        }

        [NoMapping]
        private WfAssigneeExceptionFilterType _AssigneeExceptionFilterType = WfAssigneeExceptionFilterType.All;

        [NoMapping]
        public WfAssigneeExceptionFilterType AssigneeExceptionFilterType
        {
            get
            {
                return this._AssigneeExceptionFilterType;
            }
            set
            {
                this._AssigneeExceptionFilterType = value;
            }
        }

        private WfAssigneesFilterType _AssigneesSelectType = WfAssigneesFilterType.CurrentActivity;

        [NoMapping]
        public WfAssigneesFilterType AssigneesSelectType
        {
            get
            {
                return this._AssigneesSelectType;
            }
            set
            {
                this._AssigneesSelectType = value;
            }
        }

        [NoMapping]
        public string AssigneesUserName
        {
            get;
            set;
        }

        private OguDataCollection<IUser> _CurrentAssignees = null;

        [NoMapping]
        public OguDataCollection<IUser> CurrentAssignees
        {
            get
            {
                if (this._CurrentAssignees == null)
                    this._CurrentAssignees = new OguDataCollection<IUser>();

                return this._CurrentAssignees;
            }
        }

        public ConnectiveSqlClauseCollection ToSqlBuilder()
        {
            ConnectiveSqlClauseCollection connectiveBuilder = new ConnectiveSqlClauseCollection(LogicOperatorDefine.And);

            connectiveBuilder.Add(ConditionMapping.GetWhereSqlClauseBuilder(this));
            connectiveBuilder.Add(GetCurrentAssigneesSubQuery());
            connectiveBuilder.Add(GetAssigneeExceptionSubQuery());

            return connectiveBuilder;
        }

        private WhereSqlClauseBuilder GetCurrentAssigneesSubQuery()
        {
            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("USER_ID");

            this.CurrentAssignees.ForEach(u => inBuilder.AppendItem(u.ID));

            WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

            if (this.AssigneesUserName.IsNotEmpty())
                wBuilder.AppendItem("USER_NAME", TSqlBuilder.Instance.EscapeLikeString(this.AssigneesUserName) + "%", "LIKE");

            ConnectiveSqlClauseCollection subQueryBuilder = new ConnectiveSqlClauseCollection();

            subQueryBuilder.Add(inBuilder);
            subQueryBuilder.Add(wBuilder);

            WhereSqlClauseBuilder resultBuilder = new WhereSqlClauseBuilder();

            if (subQueryBuilder.IsEmpty == false)
            {
                string processCondition = string.Empty;

                switch (this.AssigneesSelectType)
                {
                    case WfAssigneesFilterType.CurrentActivity:
                        processCondition = "CA.ACTIVITY_ID = CURRENT_ACTIVITY_ID";
                        break;
                    case WfAssigneesFilterType.AllActivities:
                        processCondition = "CA.PROCESS_ID = INSTANCE_ID";
                        break;
                }

                string subQuery = string.Format("EXISTS(SELECT USER_ID FROM WF.PROCESS_CURRENT_ASSIGNEES CA (NOLOCK) WHERE {0} AND {1})",
                    processCondition, subQueryBuilder.ToSqlString(TSqlBuilder.Instance));

                resultBuilder.AppendItem("CurrentAssignees", subQuery, "=", "${Data}$", true);
            }

            return resultBuilder;
        }

        private WhereSqlClauseBuilder GetAssigneeExceptionSubQuery()
        {
            StringBuilder sqlResult = new StringBuilder();
            string sqlCondition = string.Empty;

            switch (this.AssigneeExceptionFilterType)
            {
                case WfAssigneeExceptionFilterType.CurrentActivityError:	//当前环节人员异常
                    sqlCondition = "IU.ACTIVITY_ID = CURRENT_ACTIVITY_ID";
                    break;
                case WfAssigneeExceptionFilterType.ExsitedActivitiesError:	//当前环节和后续环节人员异常
                    sqlCondition = "1 = 1";
                    break;
            }

            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

            if (sqlCondition.IsNotEmpty())
            {
                sqlResult.AppendFormat("EXISTS(SELECT * FROM WF.INVALID_ASSIGNEES AS IU WHERE IU.PROCESS_ID = INSTANCE_ID AND {0})", sqlCondition);

                builder.AppendItem("AssigneeException", sqlResult.ToString(), "=", "${Data}$", true);
            }

            return builder;
        }
    }
}
