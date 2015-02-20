using System;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.WF.Contracts.Common.Test;
using MCS.Library.WF.Contracts.Converters.DataObjects;
using MCS.Library.WF.Contracts.Json.Converters;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class OpinionConverterTest
    {
        [TestMethod]
        public void SimpleOpinionClientToServerTest()
        {
            WfClientOpinion client = PrepareClientOpinion();

            GenericOpinion server = null;

            WfClientOpinionConverter.Instance.ClientToServer(client, ref server);

            client.AreSame(server);
        }

        [TestMethod]
        public void ClientOpinionExtraDataTest()
        {
            WfClientOpinion client = PrepareClientOpinion();

            WfClientNextStepCollection nextSteps = WfClientNextStepTest.PrepareNextSteps();

            WfClientNextStep expectedNextStep = nextSteps.GetSelectedStep();

            Dictionary<string, object> extraData = new Dictionary<string, object>();

            XElement root = new XElement("NextSteps");
            nextSteps.ToXElement(root);
            extraData["NextSteps"] = root.ToString();

            client.FillExtraDataFromDictionary(extraData);

            Console.WriteLine("意见结果：{0}", client.GetNextSteps().GetSelectedStep().GetDescription());

            WfClientNextStepTest.AreSame(expectedNextStep, client.GetNextSteps().GetSelectedStep());
        }

        [TestMethod]
        public void SimpleOpinionServerToClientTest()
        {
            GenericOpinion server = PrepareServerOpinion();

            WfClientOpinion client = null;

            WfClientOpinionConverter.Instance.ServerToClient(server, ref client);

            client.AreSame(server);
        }

        [TestMethod]
        public void SimpleOpinionSerializationTest()
        {
            WfClientJsonConverterHelper.Instance.RegisterConverters();

            WfClientOpinion client = PrepareClientOpinion();

            string data = JSONSerializerExecute.Serialize(client);

            Console.WriteLine(data);

            WfClientOpinion deserializedOpinion = JSONSerializerExecute.Deserialize<WfClientOpinion>(data);

            client.AreSame(deserializedOpinion);
        }

        private static WfClientOpinion PrepareClientOpinion()
        {
            WfClientOpinion opinion = new WfClientOpinion();

            opinion.ID = UuidHelper.NewUuidString();
            opinion.ResourceID = UuidHelper.NewUuidString();
            opinion.ActivityID = UuidHelper.NewUuidString();
            opinion.ProcessID = UuidHelper.NewUuidString();
            opinion.LevelName = "TestLevelName";
            opinion.LevelDesp = "LevelDesp";
            opinion.OpinionType = "OpinionType";
            opinion.IssueTime = new DateTime(2014, 10, 9, 0, 0, 0, DateTimeKind.Local);
            opinion.AppendTime = new DateTime(2014, 10, 10, 0, 0, 0, DateTimeKind.Local);
            opinion.Content = "Hello world";
            opinion.IssuePersonID = UuidHelper.NewUuidString();
            opinion.IssuePersonName = "Shen Zheng";
            opinion.AppendPersonID = UuidHelper.NewUuidString();
            opinion.AppendPersonName = "Shen Rong";
            opinion.ExtraData = "Extra Data";

            return opinion;
        }

        private static GenericOpinion PrepareServerOpinion()
        {
            GenericOpinion opinion = new GenericOpinion();

            opinion.ID = UuidHelper.NewUuidString();
            opinion.ResourceID = UuidHelper.NewUuidString();
            opinion.ActivityID = UuidHelper.NewUuidString();
            opinion.ProcessID = UuidHelper.NewUuidString();
            opinion.LevelName = "TestLevelName";
            opinion.LevelDesp = "LevelDesp";
            opinion.OpinionType = "OpinionType";
            opinion.IssueDatetime = DateTime.Now;
            opinion.AppendDatetime = DateTime.Now;
            opinion.Content = "Hello world";
            opinion.IssuePerson = new OguUser(UuidHelper.NewUuidString()) { DisplayName = "Shen Zheng" };
            opinion.AppendPerson = new OguUser(UuidHelper.NewUuidString()) { DisplayName = "Shen Rong" };
            opinion.RawExtData = "Extra Data";

            return opinion;
        }
    }
}
