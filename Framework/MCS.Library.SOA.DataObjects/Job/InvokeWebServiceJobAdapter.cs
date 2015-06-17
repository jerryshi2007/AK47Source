using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Xml.Linq;
using MCS.Library.Data.Builder;
using System.Transactions;
using MCS.Library.Data;
using System.Data;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects
{
    public class InvokeWebServiceJobAdapter : UpdatableAndLoadableAdapterBase<InvokeWebServiceJob, InvokeWebServiceJobCollection>
    {
        internal const string SingleData_InvokeWebService = "SELECT TOP(1) * FROM  WF.JOB_INVOKE_SERVICE WHERE {0}";

        public static readonly InvokeWebServiceJobAdapter Instance = new InvokeWebServiceJobAdapter();

        protected override void AfterInnerDelete(InvokeWebServiceJob data, Dictionary<string, object> context)
        {
            JobBaseAdapter.Instance.Delete(data);
        }

        public InvokeWebServiceJob LoadSingleDataByJobID(string jobID)
        {
            jobID.CheckStringIsNullOrEmpty("jobID");

            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("JOB_ID");

            inBuilder.AppendItem(jobID);
            ConnectiveSqlClauseCollection builder = new ConnectiveSqlClauseCollection(
                inBuilder, new WhereSqlClauseBuilder().AppendTenantCode(typeof(InvokeWebServiceJob)));

            return LoadSingleData(builder);
        }

        public InvokeWebServiceJob LoadSingleData(IConnectiveSqlClause whereClause)
        {
            InvokeWebServiceJob result = null;

            if (whereClause.IsEmpty == false)
            {
                using (DbContext dbi = DbHelper.GetDBContext(GetConnectionName()))
                {
                    using (IDataReader dr = DbHelper.RunSqlReturnDR(string.Format(SingleData_InvokeWebService,
                        whereClause.ToSqlString(TSqlBuilder.Instance)), GetConnectionName()))
                    {
                        while (dr.Read())
                        {
                            result = new InvokeWebServiceJob();
                            ORMapping.DataReaderToObject(dr, result);
                            break;
                        }
                    }

                    if (result != null)
                    {
                        XElementFormatter formatter = new XElementFormatter();
                        formatter.OutputShortType = WorkflowSettings.GetConfig().OutputShortType;
                        XElement root = XElement.Parse(result.XmlData);
                        result.SvcOperationDefs = (WfServiceOperationDefinitionCollection)formatter.Deserialize(root);

                        result.InitJobBaseData(JobBaseAdapter.Instance.LoadSingleDataByJobID(whereClause));
                    }
                }
            }

            return result;
        }

        protected override void AfterLoad(InvokeWebServiceJobCollection data)
        {
            base.AfterLoad(data);
            XElementFormatter formatter = new XElementFormatter();
            formatter.OutputShortType = WorkflowSettings.GetConfig().OutputShortType;

            foreach (var job in data)
            {
                XElement root = XElement.Parse(job.XmlData);
                job.SvcOperationDefs = (WfServiceOperationDefinitionCollection)formatter.Deserialize(root);

                var baseColl = JobBaseAdapter.Instance.Load(p => p.AppendItem("JOB_ID", job.JobID));

                if (baseColl.Count == 0)
                    return;

                job.InitJobBaseData(baseColl[0]);
            }
        }

        protected override void BeforeInnerUpdate(InvokeWebServiceJob data, Dictionary<string, object> context)
        {
            base.BeforeInnerUpdate(data, context);

            XElementFormatter formatter = new XElementFormatter();
            formatter.OutputShortType = WorkflowSettings.GetConfig().OutputShortType;
            data.XmlData = formatter.Serialize(data.SvcOperationDefs).ToString();
        }

        protected override int InnerUpdate(InvokeWebServiceJob data, Dictionary<string, object> context)
        {
            UpdateSqlClauseBuilder uBuilder = new UpdateSqlClauseBuilder();

            FillSqlBuilder(uBuilder, data);

            WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

            wBuilder.AppendItem("JOB_ID", data.JobID).AppendTenantCode(typeof(InvokeWebServiceJob));

            string sql = string.Format("UPDATE {0} SET {1} WHERE {2}",
                GetMappingInfo(context).TableName,
                uBuilder.ToSqlString(TSqlBuilder.Instance),
                wBuilder.ToSqlString(TSqlBuilder.Instance));

            int result = DbHelper.RunSql(sql, GetConnectionName());

            if (result > 0)
                JobBaseAdapter.Instance.Update(data);

            return result;
        }

        protected override void InnerInsert(InvokeWebServiceJob data, Dictionary<string, object> context)
        {
            InsertSqlClauseBuilder builder = new InsertSqlClauseBuilder();

            FillSqlBuilder(builder, data);

            string sql = string.Format("INSERT INTO {0} {1}",
                GetMappingInfo(context).TableName,
                builder.ToSqlString(TSqlBuilder.Instance));

            DbHelper.RunSql(sql, GetConnectionName());

            JobBaseAdapter.Instance.Update(data);
        }

        private static void FillSqlBuilder(SqlClauseBuilderIUW builder, InvokeWebServiceJob data)
        {
            builder.AppendItem("JOB_ID", data.JobID).AppendItem("SERVICE_DEF_DATA", data.XmlData).AppendTenantCode(typeof(InvokeWebServiceJob));
        }

        public int Delete(string[] ids)
        {
            InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("JOB_ID");
            inBuilder.AppendItem(ids);

            int result = 0;

            if (inBuilder.Count > 0)
            {
                ConnectiveSqlClauseCollection builder = new ConnectiveSqlClauseCollection(inBuilder,
                    new WhereSqlClauseBuilder().AppendTenantCode(typeof(InvokeWebServiceJob)));

                string where = builder.ToSqlString(TSqlBuilder.Instance);

                StringBuilder sqlString = new StringBuilder();

                sqlString.AppendFormat("DELETE FROM WF.JOB_SCHEDULES WHERE {0} ", where);
                sqlString.Append(TSqlBuilder.Instance.DBStatementSeperator);

                sqlString.AppendFormat("DELETE FROM WF.JOB_INVOKE_SERVICE WHERE {0} ", where);
                sqlString.Append(TSqlBuilder.Instance.DBStatementSeperator);

                sqlString.AppendFormat("DELETE FROM WF.JOB_START_WORKFLOW WHERE {0} ", where);
                sqlString.Append(TSqlBuilder.Instance.DBStatementSeperator);

                sqlString.AppendFormat("DELETE WF.JOBS WHERE {0} ", where);

                result = DbHelper.RunSqlWithTransaction(sqlString.ToString());
            }

            return result;
        }
    }
}
