using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.DataObjects;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Workflow.Runtime;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    /// <summary>
    /// 查询流程的条件测试
    /// </summary>
    [TestClass]
    public class QueryProcessConditionTest
    {
        [TestMethod]
        public void ClientProcessQueryConditionSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientProcessQueryCondition condition = PrepareQueryCondition();

            string data = JSONSerializerExecute.Serialize(condition);

            Console.WriteLine(data);

            WfClientProcessQueryCondition deserialized = JSONSerializerExecute.Deserialize<WfClientProcessQueryCondition>(data);

            condition.AreSame(deserialized);
        }

        private static WfClientProcessQueryCondition PrepareQueryCondition()
        {
            WfClientProcessQueryCondition condition = new WfClientProcessQueryCondition();

            condition.ApplicationName = "My Applicaiton";
            condition.ProcessName = "My Process";
            condition.AssigneesUserName = "樊海云";
            condition.AssigneesSelectType = WfClientAssigneesFilterType.AllActivities;
            condition.AssigneeExceptionFilterType = WfClientAssigneeExceptionFilterType.All;
            condition.BeginStartTime = DateTime.Now;
            condition.EndStartTime = DateTime.Now.AddDays(1);
            condition.DepartmentName = "流程管理部";
            condition.ProcessStatus = WfClientProcessStatus.Running.ToString();

            condition.CurrentAssignees.Add(Consts.Users["Approver1"]);

            return condition;
        }
    }
}
