using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.Bizlet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl.Test
{
    internal static class WfMatrixHelper
    {
        /// <summary>
        /// 创建一个只有开始结束两个环节的流程
        /// </summary>
        /// <param name="processData"></param>
        /// <returns></returns>
        public static IWfMatrixProcess CreateEmptyProcess(string processKey)
        {
            IWfMatrixProcess matrixProcess = new WfMatrixProcess();

            //流程基本信息
            matrixProcess.ApplicationName = "测试应用名称";
            matrixProcess.ProgramName = "测试程序名称";
            matrixProcess.Description = "测试动态流程描述";
            matrixProcess.Key = processKey;
            matrixProcess.Name = "测试动态流程名称";

            return matrixProcess;
        }

        /// <summary>
        /// 创建一个全属性的流程定义
        /// </summary>
        /// <param name="processKey"></param>
        /// <returns></returns>
        public static IWfMatrixProcess CreateComplexProcess(string processKey)
        {

            IWfMatrixProcess matrixProcess = WfMatrixEmptyProcessBuilder.Instance.BuildProcess(processKey);
            matrixProcess.ApplicationName = "测试应用名称";
            matrixProcess.ProgramName = "测试程序名称";
            matrixProcess.Description = "测试动态流程描述";
            matrixProcess.Name = "测试动态流程名称";
            matrixProcess.Url = "Url";

            //增加参数
            WfMatrixParameterDefinition parameter = CreateWfMatrixParameterDefinition("a1", Common.ParaType.String, "a1");
            matrixProcess.ParameterDefinitions.Add(parameter);
            parameter = CreateWfMatrixParameterDefinition("a2", Common.ParaType.Number, "1");
            matrixProcess.ParameterDefinitions.Add(parameter);
            parameter = CreateWfMatrixParameterDefinition("a3", Common.ParaType.Float, "2.1");
            matrixProcess.ParameterDefinitions.Add(parameter);
            parameter = CreateWfMatrixParameterDefinition("A1", Common.ParaType.String, "A1");
            parameter.Enabled = false;
            matrixProcess.ParameterDefinitions.Add(parameter);

            //增加节点         
            CreateWfMatrixActivity("n1", matrixProcess);
            CreateWfMatrixActivity("n2", matrixProcess);
            CreateWfMatrixActivity("n3", matrixProcess);

            return matrixProcess;
        }

        public static WfMatrixParameterDefinition CreateWfMatrixParameterDefinition(string name, Common.ParaType paratype, object defaultValue)
        {
            WfMatrixParameterDefinition parameter = new WfMatrixParameterDefinition();
            parameter.Name = name;
            parameter.DisplayName = name;
            parameter.DefaultValue = defaultValue;
            parameter.ParameterType = paratype;
            parameter.Description = Guid.NewGuid().ToString();
            parameter.Enabled = true;
            return parameter;
        }

        public static WfMatrixActivity CreateWfMatrixActivity(string code, IWfMatrixProcess process)
        {
            WfMatrixActivity activity = new WfMatrixActivity();
            activity.CodeName = code;
            activity.Name = "Name_" + code;
            activity.Url = "Url_" + code;
            activity.Description = "Description_" + code;
            SetCandidates(activity);

            SetExpression(activity, process);

            activity.ActivityType = WfMaxtrixActivityType.NormalActivity;

            return activity;
        }

        public static void SetCandidates(WfMatrixActivity activity)
        {
            WfMatrixCandidate candidate = new WfMatrixCandidate();
            candidate.ResourceType = "Role";
            candidate.Candidate = new WfMatrixParameterDefinition();
            candidate.Candidate.DefaultValue = "R1";
            activity.Candidates.Add(candidate);

            candidate = new WfMatrixCandidate();
            candidate.ResourceType = "User";
            candidate.Candidate = new WfMatrixParameterDefinition();
            candidate.Candidate.DefaultValue = "huanglan";
            activity.Candidates.Add(candidate);

            candidate = new WfMatrixCandidate();
            candidate.ResourceType = "requester";
            candidate.Candidate = new WfMatrixParameterDefinition();
            candidate.Candidate.DefaultValue = "n1";
            activity.Candidates.Add(candidate);

            candidate = new WfMatrixCandidate();
            candidate.ResourceType = "response";
            candidate.Candidate = new WfMatrixParameterDefinition();
            candidate.Candidate.DefaultValue = "n2";
            activity.Candidates.Add(candidate);
        }

        public static void SetExpression(WfMatrixActivity activity, IWfMatrixProcess matrixProcess)
        {
            activity.Expression.Relation = Common.LogicalRelation.Or;
            //分组1
            IWfMatrixConditionGroup conditionGroup = new WfMatrixConditionGroup();
            conditionGroup.Relation = Common.LogicalRelation.And;
            IWfMatrixCondition condition = new WfMatrixCondition();
            condition.Parameter = matrixProcess.ParameterDefinitions[0];
            condition.Sign = Common.ComparsionSign.Equal;
            condition.Value = "a1";
            conditionGroup.Add(condition);
            condition = new WfMatrixCondition();
            condition.Parameter = matrixProcess.ParameterDefinitions[1];
            condition.Sign = Common.ComparsionSign.Equal;
            condition.Value = "1";
            conditionGroup.Add(condition);
            activity.Expression.Add(conditionGroup);

            //分组2
            conditionGroup = new WfMatrixConditionGroup();
            conditionGroup.Relation = Common.LogicalRelation.Or;
            condition = new WfMatrixCondition();
            condition.Parameter = matrixProcess.ParameterDefinitions[2];
            condition.Sign = Common.ComparsionSign.Equal;
            condition.Value = "2.1";
            conditionGroup.Add(condition);
            activity.Expression.Add(conditionGroup);
        }


        public static void AreSame(this WfClientProcessDescriptor process, IWfMatrixProcess mprocess)
        {
            AssertStringEqual(mprocess.ApplicationName, process.ApplicationName);
            AssertStringEqual(mprocess.Key, process.Key);
            AssertStringEqual(mprocess.Name, process.Name);
            AssertStringEqual(mprocess.ProgramName, process.ProgramName);
            AssertStringEqual(mprocess.Url, process.Url);
            AssertStringEqual(mprocess.Description, process.Description);


            Assert.AreEqual(WfClientProcessType.Approval, process.ProcessType);
            Assert.AreEqual(true, process.AutoGenerateResourceUsers);

        }

        public static void AreSame(this IWfMatrixProcess expected, IWfMatrixProcess actual)
        {
            AssertStringEqual(expected.ApplicationName, actual.ApplicationName);
            AssertStringEqual(expected.Key, actual.Key);
            AssertStringEqual(expected.Name, actual.Name);
            AssertStringEqual(expected.ProgramName, actual.ProgramName);
            AssertStringEqual(expected.Url, actual.Url);
            AssertStringEqual(expected.Description, actual.Description);

            AssertStringEqual(expected.TenantCode, actual.TenantCode);

            Assert.AreEqual(expected.Activities.Count, actual.Activities.Count, "Activities");
            Assert.AreEqual(expected.ParameterDefinitions.Count, actual.ParameterDefinitions.Count, "ParameterDefinitions");
            Assert.AreEqual(expected.GlobalParameterDefinitions.Count, actual.GlobalParameterDefinitions.Count, "GlobalParameterDefinitions");

            Assert.AreEqual(expected.Properties.Count - 1, actual.Properties.Count, "Properties");
        }

        private static void AssertStringEqual(string expected, string actual)
        {
            if (!string.IsNullOrEmpty(expected) || !string.IsNullOrEmpty(expected))
                Assert.AreEqual(expected, actual);
        }

        /*
        public ActionResult BuildProcess(string processKey)
        {
            IWfMatrixProcess matrixProcess = new WfMatrixProcess();

            //流程基本信息
            matrixProcess.ApplicationName = "测试应用名称";
            matrixProcess.Description = "测试动态流程描述";
            matrixProcess.Key = processKey;
            matrixProcess.Name = "测试动态流程名称";
            matrixProcess.ProgramName = "测试程序名称";

            //添加步骤
            Guid step1Id = Guid.NewGuid();
            Guid step2Id = Guid.NewGuid();
            matrixProcess.Activities.Add(new WfMatrixActivity()
            {
                Id = step1Id,
                Code = "测试编码",
                Description = "节点描述1",
                Name = "节点1",
                Sort = 1
            });
            matrixProcess.Activities.Add(new WfMatrixActivity()
            {
                Id = step2Id,
                Code = "测试编码",
                Description = "节点描述1",
                Name = "节点1",
                Sort = 2
            });

            //设计节点的审批人
            IWfMatrixActivity activity1 = matrixProcess.Activities.GetById(step1Id);
            IWfMatrixActivity activity2 = matrixProcess.Activities.GetById(step2Id);

            activity1.Candidates.Add(new WfMatrixCandidate()
            {
                Candidate = new WfMatrixParameterDefinition()
                {
                    Description = "动态角色描述",
                    DisplayName = "直线汇报经理",
                    Enabled = true,
                    Id = Guid.NewGuid(),
                    Name = "MyManager",//动态角色,运行时刻设置人
                    ParameterType = ParaType.String
                },
                ID = Guid.NewGuid(),
                ResourceType = ResourceType.Variable
            });

            activity2.Candidates.Add(new WfMatrixCandidate()
            {
                Candidate = new WfMatrixParameterDefinition()
                {
                    Description = "审批人描述",
                    DisplayName = "审批人黄兰",
                    Enabled = true,
                    Id = Guid.NewGuid(),
                    Name = "huanglan",//直接设置的审批人
                    ParameterType = ParaType.String
                },
                ID = Guid.NewGuid(),
                ResourceType = ResourceType.User
            });

            //设置节点条件
            IWfMatrixConditionCollection conditionCollection = new WfMatrixConditionCollection();
            conditionCollection.Id = Guid.NewGuid();
            conditionCollection.Sort = 1;
            conditionCollection.Relation = LogicalRelation.And;
            conditionCollection.Add(new WfMatrixCondition()
            {
                Id = Guid.NewGuid(),
                Sign = ComparsionSign.GreaterThan,
                Sort = 1,
                Value = "100",
                Parameter = new WfMatrixParameterDefinition()
                {
                    Description = "条件变量名描述",
                    DisplayName = "金额",
                    Enabled = true,
                    Id = Guid.NewGuid(),
                    Name = "Amount",//条件中的参数名，金额
                    ParameterType = ParaType.Number
                }
            });
            conditionCollection.Add(new WfMatrixCondition()
            {
                Id = Guid.NewGuid(),
                Sign = ComparsionSign.GreaterThan,
                Sort = 1,
                Value = "100",
                Parameter = new WfMatrixParameterDefinition()
                {
                    Description = "条件变量名描述",
                    DisplayName = "金额",
                    Enabled = true,
                    Id = Guid.NewGuid(),
                    Name = "Amount",//条件中的参数名，金额
                    ParameterType = ParaType.Number
                }
            });

            IWfMatrixConditionCollection conditionCollection2 = new WfMatrixConditionCollection();
            conditionCollection2.Sort = 2;
            conditionCollection2.Id = Guid.NewGuid();
            conditionCollection2.Id = Guid.NewGuid();
            conditionCollection2.Add(new WfMatrixCondition()
            {
                Id = Guid.NewGuid(),
                Sign = ComparsionSign.GreaterThan,
                Sort = 1,
                Value = "100",
                Parameter = new WfMatrixParameterDefinition()
                {
                    Description = "条件变量名描述",
                    DisplayName = "金额",
                    Enabled = true,
                    Id = Guid.NewGuid(),
                    Name = "Amount",//条件中的参数名，金额
                    ParameterType = ParaType.Number
                }
            });

            activity1.Expression.Relation = LogicalRelation.And;
            activity1.Expression.Conditions.Add(conditionCollection);
            activity1.Expression.Conditions.Add(conditionCollection2);

            //保存流程，业务需要保存流程key，以便再次加载时使用
            IWfMatrixStorageManager storageManager = new WfMatrixStorageManager();
            storageManager.Save(matrixProcess);


            Response.Clear();

            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", processKey + ".xlsx"));
            using (Stream sr = WfClientProcessDescriptorServiceProxy.Instance.WfDynamicProcessToExcel(processKey))
            {
                sr.CopyTo(Response.OutputStream);
            }
            Response.Flush();
            Response.End();
            return Json(string.Empty);
        }
         */
    }
}
