using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml.Linq;
using MCS.Library.Compression;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Logging;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程状态保存的实现
    /// </summary>
    internal class WfSqlProcessPersistManager : IWfProcessPersistManager
    {
        private const string BINARYDATA = "CASE WHEN BIN_DATA IS NULL THEN DATA ELSE NULL END AS DATA";

        #region IWfProcessPersist Members

        public IWfProcess LoadProcessByProcessID(string processID)
        {
            processID.CheckStringIsNullOrEmpty("processID");

            WfProcessCollection processes = LoadProcesses(builder => builder.AppendItem("INSTANCE_ID", processID));

            (processes.Count > 0).FalseThrow<WfRuntimeException>("不能找到ProcessID为{0}的流程", processID);

            return processes[0];
        }

        public IWfProcess LoadProcessByActivityID(string activityID)
        {
            activityID.CheckStringIsNullOrEmpty("activityID");

            WfProcessCollection processes = LoadProcessesByActivityID(activityID);

            (processes.Count > 0).FalseThrow<WfRuntimeException>("不能找到ActivityID为{0}的流程", activityID);

            return processes[0];
        }

        public WfProcessCollection LoadProcessByResourceID(string resourceID)
        {
            resourceID.CheckStringIsNullOrEmpty("resourceID");

            WfProcessCollection processes = LoadProcesses(builder => builder.AppendItem("RESOURCE_ID", resourceID));

            return processes;
        }

        /// <summary>
        /// 根据OwnerActivity的ID来加载流程数据
        /// </summary>
        /// <param name="activityID"></param>
        /// <param name="templateKey"></param>
        /// <returns></returns>
        public WfProcessCollection LoadProcessByOwnerActivityID(string activityID, string templateKey)
        {
            activityID.CheckStringIsNullOrEmpty("activityID");
            templateKey.CheckStringIsNullOrEmpty("templateKey");

            WfProcessCollection processes = LoadProcesses(builder =>
            {
                builder.AppendItem("OWNER_ACTIVITY_ID", activityID);
                builder.AppendItem("OWNER_TEMPLATE_KEY", templateKey);
            });

            return processes;
        }

        /// <summary>
        ///  读取子流程的信息，只包含状态信息
        /// </summary>
        /// <param name="resourceID"></param>
        /// <param name="templateKey"></param>
        /// <param name="includeAborted">是否包含已经作废的流程</param>
        /// <returns></returns>
        public WfProcessCurrentInfoCollection LoadProcessInfoOwnerActivityID(string activityID, string templateKey, bool includeAborted)
        {
            activityID.CheckStringIsNullOrEmpty("activityID");

            return WfProcessCurrentInfoAdapter.Instance.LoadByOwnerActivityID(false, activityID, templateKey, includeAborted);
        }

        public void DeleteProcessByProcessID(string processID)
        {
            processID.CheckStringIsNullOrEmpty("processID");

            DeleteProcesses(builder => builder.AppendItem("INSTANCE_ID", processID));
        }

        public void DeleteProcessByActivityID(string activityID)
        {
            activityID.CheckStringIsNullOrEmpty("activityID");

            DeleteProcesses(builder => builder.AppendItem("CURRENT_ACTIVITY_ID", activityID));
        }

        public void DeleteProcessByResourceID(string resourceID)
        {
            resourceID.CheckStringIsNullOrEmpty("resourceID");

            DeleteProcesses(builder => builder.AppendItem("RESOURCE_ID", resourceID));
        }

        public void SaveProcess(IWfProcess process)
        {
            WfProcessInstanceData instanceData = WfProcessInstanceData.FromProcess(process);

            if (instanceData.Data.IsNotEmpty())
            {
                if (WorkflowSettings.GetConfig().Compressed)
                {
                    XElement extData = GetExtData(instanceData.ExtData);

                    PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("CompressProcess", () =>
                        {
                            instanceData.BinaryData = GetCompressedStream(instanceData.Data, Encoding.GetEncoding(extData.Attribute("encoding", "utf-8")));
                            instanceData.Data = string.Empty;
                        });
                }
            }

            string sql = string.Empty;

            ORMappingItemCollection mapping = ORMapping.GetMappingInfo<WfProcessInstanceData>();

            if (process.LoadingType == DataLoadingType.External)
            {
                UpdateSqlClauseBuilder uBuilder = ORMapping.GetUpdateSqlClauseBuilder(instanceData, mapping);

                uBuilder.AppendItem("UPDATE_TAG", "UPDATE_TAG + 1", "=", true);
                uBuilder.AppendTenantCode();

                WhereSqlClauseBuilder whereBuilder = ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(instanceData, mapping);

                whereBuilder.AppendItem("UPDATE_TAG", process.UpdateTag);

                sql = string.Format("UPDATE {0} SET {1} WHERE {2}",
                    mapping.TableName, uBuilder.ToSqlString(TSqlBuilder.Instance), whereBuilder.ToSqlString(TSqlBuilder.Instance));
            }
            else
            {
                InsertSqlClauseBuilder iBuilder = ORMapping.GetInsertSqlClauseBuilder(instanceData, mapping);
                iBuilder.AppendTenantCode();

                sql = string.Format("INSERT INTO {0} {1}", mapping.TableName, iBuilder.ToSqlString(TSqlBuilder.Instance));
            }

            Dictionary<object, object> context = new Dictionary<object, object>();

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("WriteProcessToDB", () =>
                    (DbHelper.RunSql(sql, GetConnectionName()) > 0).FalseThrow<WfRuntimeException>("更新流程{0}失败，流程状态已经改变", process.ID)
                );

                PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("WriteProcessActivitiesToDB",
                    () => WfProcessCurrentActivityAdapter.Instance.UpdateProcessActivities(process));

                PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("WriteRelativeProcessInfoToDB", () =>
                    {
                        WfRelativeProcessAdapter.Instance.Delete(new WfRelativeProcess() { ProcessID = process.ID });

                        if (process.RelativeID.IsNotEmpty())
                        {
                            WfRelativeProcessAdapter.Instance.Update(new WfRelativeProcess()
                            {
                                Description = string.IsNullOrEmpty(process.Descriptor.Description) ? process.Descriptor.Name : process.Descriptor.Description,
                                ProcessID = process.ID,
                                RelativeID = process.RelativeID,
                                RelativeURL = process.RelativeURL
                            });
                        }
                    });

                if (WorkflowSettings.GetConfig().SaveRelativeData)
                {
                    PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("WriteRelativeDataToDB",
                        () => WfExtraPersistenceSettings.GetConfig().GetPersisters().SaveData(process, context));
                }

                scope.Complete();
            }
        }

        #endregion

        private void DeleteProcesses(Action<WhereSqlClauseBuilder> action)
        {
            action.NullCheck("action");

            ORMappingItemCollection mapping = ORMapping.GetMappingInfo<WfProcessInstanceData>();

            WhereSqlClauseBuilder whereBuilder = new WhereSqlClauseBuilder();

            action(whereBuilder);

            string sql = string.Format("DELETE {0} WHERE {1}", mapping.TableName, whereBuilder.ToSqlString(TSqlBuilder.Instance));
            WfProcessCurrentInfoCollection processesInfo = WfProcessCurrentInfoAdapter.Instance.Load(false, action);

            Dictionary<object, object> context = new Dictionary<object, object>();

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                DbHelper.RunSql(sql, GetConnectionName());

                WfProcessCurrentActivityAdapter.Instance.DeleteProcessActivities(processesInfo);
                WfRelativeProcessAdapter.Instance.Delete(processesInfo);

                WfExtraPersistenceSettings.GetConfig().GetPersisters().DeleteData(processesInfo, context);

                scope.Complete();
            }
        }

        private WfProcessCollection LoadProcessesByActivityID(string activityID)
        {
            activityID.CheckStringIsNullOrEmpty("activityID");

            ORMappingItemCollection mapping = ORMapping.GetMappingInfo<WfProcessInstanceData>();

            string[] fields = ORMapping.GetSelectFieldsName(mapping, "Data");

            string sql = string.Format("SELECT {0},{1} FROM {2} WHERE INSTANCE_ID IN (SELECT PROCESS_ID FROM WF.PROCESS_CURRENT_ACTIVITIES WHERE ACTIVITY_ID = {3})",
               string.Join(",", fields),
               BINARYDATA,
               mapping.TableName, TSqlBuilder.Instance.CheckQuotationMark(activityID, true));

            DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

            return DataTableToProcessCollection(table);
        }

        private WfProcessCollection LoadProcesses(Action<WhereSqlClauseBuilder> action)
        {
            action.NullCheck("action");

            var whereBuilder = new WhereSqlClauseBuilder();

            action(whereBuilder);

            var mapping = ORMapping.GetMappingInfo<WfProcessInstanceData>();

            string[] fields = ORMapping.GetSelectFieldsName(mapping, "Data");

            var sql = string.Format("SELECT {0},{1} FROM {2} WHERE {3}",
                string.Join(",", fields),
                BINARYDATA,
                mapping.TableName,
                whereBuilder.ToSqlString(TSqlBuilder.Instance));

            var table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

            WfProcessCollection result = null;

            PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("DataTableToProcessCollection",
                () => result = DataTableToProcessCollection(table)
            );

            return result;
        }

        private static WfProcessCollection DataTableToProcessCollection(DataTable table)
        {
            WfProcessCollection result = new WfProcessCollection();

            XElementFormatter formatter = new XElementFormatter();

            formatter.OutputShortType = WorkflowSettings.GetConfig().OutputShortType;

            foreach (DataRow row in table.Rows)
            {
                WfProcessInstanceData instanceData = new WfProcessInstanceData();

                ORMapping.DataRowToObject(row, instanceData);

                XElement extData = GetExtData(instanceData.ExtData);

                Encoding originalEncoding = GetEncodingFromExtData(extData);
                Encoding preferedEncoding = originalEncoding;
                byte[] decompressedData = null;

                if (null != instanceData.BinaryData)
                {
                    PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("Extra Process Data:{0}", instanceData.InstanceID),
                        () => decompressedData = CompressManager.ExtractBytes(instanceData.BinaryData));

                    PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration("EncodeProcessString", () =>
                        {
                            preferedEncoding = GetPreferedEncoding(decompressedData, originalEncoding);
                            instanceData.Data = BytesToProcessData(decompressedData, preferedEncoding);
                        }
                    );
                }
                else
                {
                    (instanceData.Data != null).FalseThrow<ArgumentException>("流程实例表Data和BinaryData都是空");
                }

                XElement root = null;

                try
                {
                    root = XElement.Parse(instanceData.Data);
                }
                catch (System.Xml.XmlException)
                {
                    if (decompressedData != null)
                    {
                        instanceData.Data = ChangeEncoding(decompressedData, preferedEncoding);
                        root = XElement.Parse(instanceData.Data);
                    }
                    else
                        throw;
                }

                extData.SetAttributeValue("encoding", originalEncoding.BodyName);

                WfProcess process = null;

                PerformanceMonitorHelper.GetDefaultMonitor().WriteExecutionDuration(string.Format("Deserialize Process:{0}", instanceData.InstanceID),
                        () => process = (WfProcess)formatter.Deserialize(root));

                process.LoadingType = DataLoadingType.External;
                process.Activities.ForEach(a =>
                {
                    ((WfActivityBase)a).LoadingType = DataLoadingType.External;
                    WfActivityBuilderBase.LoadActions(a);
                });
                process.UpdateTag = instanceData.UpdateTag;
                process.Context["SerilizationExtData"] = extData.ToString();

                result.Add(process);
            }

            return result;
        }

        private static string ChangeEncoding(byte[] decompressedData, Encoding encoding)
        {
            if (encoding.BodyName != "utf-8")
                encoding = new UTF8Encoding(true);

            return BytesToProcessData(decompressedData, encoding);
        }

        /// <summary>
        /// 得到倾向的Encoding方式。主要是判断BOM。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Encoding GetPreferedEncoding(byte[] decompressedData, Encoding defaultEncoding)
        {
            Encoding encoding = defaultEncoding;

            if (CompareEncodingBom(decompressedData, Encoding.UTF8.GetPreamble()))
                encoding = new UTF8Encoding(true);

            return encoding;
        }

        private static string BytesToProcessData(byte[] decompressedData, Encoding encoding)
        {
            using (MemoryStream ms = new MemoryStream(decompressedData))
            {
                using (StreamReader reader = new StreamReader(ms, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static byte[] GetCompressedStream(string sourceText, Encoding encoding)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(ms, encoding))
                {
                    writer.Write(sourceText);
                    writer.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    MemoryStream compressedStream = (MemoryStream)CompressManager.CompressStream(ms);

                    return compressedStream.ToArray();
                }
            }
        }

        private static Encoding GetEncodingFromExtData(XElement extData)
        {
            return Encoding.GetEncoding(extData.Attribute("encoding", "gb2312"));
        }

        /// <summary>
        /// 得到扩展描述信息的xml
        /// </summary>
        /// <param name="extData"></param>
        /// <returns></returns>
        private static XElement GetExtData(string extData)
        {
            XElement result = null;

            if (extData.IsNullOrEmpty())
                result = XElement.Parse("<ExtData />");
            else
                result = XElement.Parse(extData);

            return result;
        }

        /// <summary>
        /// 构造扩展信息的xml
        /// </summary>
        /// <param name="encodingName"></param>
        /// <returns></returns>
        private static XElement BuildExtData(string encodingName)
        {
            XElement result = XElement.Parse("<ExtData />");

            result.SetAttributeValue("encoding", encodingName);

            return result;
        }

        private static bool CompareEncodingBom(byte[] data, byte[] bom)
        {
            bool result = true;

            if (data.Length >= bom.Length)
            {
                for (int i = 0; i < bom.Length; i++)
                {
                    if (data[i] != bom[i])
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
                result = false;

            return result;
        }

        private static string GetConnectionName()
        {
            return WfRuntime.ProcessContext.SimulationContext.GetConnectionName(WorkflowSettings.GetConfig().ConnectionName);
        }
    }
}
