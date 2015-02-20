using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.SyncLibrary
{
    public class SqlLogger : LogProviderBase
    {
        private string connectionName = null;
        private string sourceName = null;
        SqlIncomeSyncLog log = null;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection parameters)
        {
            if (string.IsNullOrEmpty((string)parameters["connectionName"]))
            {
                parameters.Remove("connectionName");
                parameters.Add("connectionName", "PermissionsCenter");

            }

            if (string.IsNullOrEmpty((string)parameters["sourceName"]))
            {
                parameters.Remove("sourceName");
                parameters.Add("sourceName", "DefaultSource");
            }

            this.connectionName = parameters["connectionName"];
            this.sourceName = parameters["sourceName"];

            base.Initialize(name, parameters);

            parameters.Remove("connectionName");
            parameters.Remove("sourceName");

            if (parameters.Count > 0)
            {
                string key = parameters.GetKey(0);
                if (!string.IsNullOrEmpty(key))
                    throw new ProviderException(string.Format("不识别的属性： {0} ", key));
            }
        }

        public override void WriteLog(SyncSession syncSession, SchemaObjectBase scObj)
        {
            CheckLogPresent();
            this.log.NumberOfModifiedItems++;
        }

        public override void WriteErrorLog(SyncSession syncSession, SchemaObjectBase scObj, Exception ex)
        {
            CheckLogPresent();
            this.log.NumberOfExceptions++;
            var detail = new SqlIncomeSyncLogDetail();
            detail.LogDetailID = UuidHelper.NewUuidString();
            detail.LogID = this.log.LogID;
            detail.SCObjectID = scObj.ID;
            detail.Summary = ex.Message;
            detail.Detail = ex.ToString();
            detail.ActionType = SqlIncomeSyncLogDetailStatus.Update;
            Adapters.SqlIncomeLogDetailAdapter.Instance.Update(detail);

        }

        public override void WriteStartLog(SyncSession session)
        {
            var log = SqlIncomeSyncLog.CreateLogFromEnvironment();
            log.LogID = UuidHelper.NewUuidString();
            log.StartTime = DateTime.Now;
            log.EndTime = new DateTime(9999, 9, 9);
            log.SourceName = this.sourceName;
            log.Status = IncomeSyncStatus.Running;

            Adapters.SqlIncomeLogAdapter.Instance.Update(log);

            this.log = log;

        }

        public override void WriteEndLog(SyncSession session, bool success)
        {
            CheckLogPresent();
            this.log.Status = IncomeSyncStatus.Completed;
            if (success == false)
            {
                this.log.Status = IncomeSyncStatus.FaultError;
            }

            this.log.EndTime = DateTime.Now;
            Adapters.SqlIncomeLogAdapter.Instance.Update(this.log);
        }

        private void CheckLogPresent()
        {
            if (this.log == null)
                throw new ApplicationException("没有创建开始日志");
        }
    }
}
