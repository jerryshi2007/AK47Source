using MCS.Library.WF.Contracts.Workflow.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace MCS.Library.WF.Contracts.Converters.Test
{
    [TestClass]
    public class WfClientNextStepTest
    {
        [TestMethod]
        public void WfClientNextStepToXElementTest()
        {
            WfClientNextStepCollection expected = PrepareNextSteps();

            XElement root = new XElement("NextSteps");

            expected.ToXElement(root);

            Console.WriteLine(root.ToString());

            WfClientNextStepCollection actual = new WfClientNextStepCollection(root);

            AreSame(expected, actual);

            Assert.AreEqual(expected.SelectedKey, actual.GetSelectedStep().TransitionKey);
        }

        internal static WfClientNextStepCollection PrepareNextSteps()
        {
            WfClientNextStepCollection result = new WfClientNextStepCollection();

            WfClientNextStep step1 = new WfClientNextStep();

            step1.TransitionKey = "L1";
            step1.TransitionName = "线1";
            step1.TransitionDescription = "线描述1";

            step1.ActivityKey = "N1";
            step1.ActivityName = "活动1";
            step1.ActivityDescription = "活动描述1";

            result.Add(step1);

            WfClientNextStep step2 = new WfClientNextStep();

            step2.TransitionKey = "L2";
            step2.TransitionName = "线2";
            step2.TransitionDescription = "线描述2";

            step2.ActivityKey = "N2";
            step2.ActivityName = "活动2";
            step2.ActivityDescription = "活动描述2";

            result.Add(step2);

            result.SelectedKey = "L2";

            return result;
        }

        internal static void AreSame(WfClientNextStepCollection expected, WfClientNextStepCollection actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            Assert.AreEqual(expected.SelectedKey, actual.SelectedKey);

            for (int i = 0; i < expected.Count; i++)
                AreSame(expected[i], actual[i]);
        }

        internal static void AreSame(WfClientNextStep expected, WfClientNextStep actual)
        {
            Assert.AreEqual(expected.TransitionKey, actual.TransitionKey);
            Assert.AreEqual(expected.TransitionName, actual.TransitionName);
            Assert.AreEqual(expected.TransitionDescription, actual.TransitionDescription);

            Assert.AreEqual(expected.ActivityKey, actual.ActivityKey);
            Assert.AreEqual(expected.ActivityName, actual.ActivityName);
            Assert.AreEqual(expected.TransitionDescription, actual.TransitionDescription);
        }
    }
}
