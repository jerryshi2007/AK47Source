﻿using MCS.Library.Core;
using MCS.Library.OGUPermission;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.DataObjects
{
    [TestClass]
    public class TaskAssigneeTest
    {
        [TestMethod]
        [TestCategory("TaskAssignee")]
        public void UpdataTaskAssignee()
        {
            TaskAssignee ta = new TaskAssignee();

            ta.ID = UuidHelper.NewUuidString();
            ta.ResourceID = UuidHelper.NewUuidString();
            ta.Type = "Test";
            ta.InnerID = 0;

            ta.Assignee = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

            TaskAssigneeAdapter.Instance.Update(ta);

            TaskAssigneeCollection tasLoaded = TaskAssigneeAdapter.Instance.Load(ta.ResourceID);

            Assert.IsTrue(tasLoaded.Count > 0);

            Assert.AreEqual(ta.ID, tasLoaded[0].ID);
            Assert.AreEqual(ta.ResourceID, tasLoaded[0].ResourceID);
            Assert.AreEqual(ta.Type, tasLoaded[0].Type);
            Assert.AreEqual(ta.Assignee.ID, tasLoaded[0].Assignee.ID);
            Assert.AreEqual(ta.Assignee.DisplayName, tasLoaded[0].Assignee.DisplayName);
        }

        [TestMethod]
        [TestCategory("TaskAssignee")]
        public void UpdataTaskAssignedObject()
        {
            TaskAssignedObject ta = new TaskAssignedObject();

            ta.ID = UuidHelper.NewUuidString();
            ta.ResourceID = UuidHelper.NewUuidString();
            ta.Type = "Test";
            ta.InnerID = 0;

            ta.Assignee = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

            TaskAssignedObjectAdapter.Instance.Update(ta);

            TaskAssignedObjectCollection tasLoaded = TaskAssignedObjectAdapter.Instance.Load(ta.ResourceID);

            Assert.IsTrue(tasLoaded.Count > 0);

            Assert.AreEqual(ta.ID, tasLoaded[0].ID);
            Assert.AreEqual(ta.ResourceID, tasLoaded[0].ResourceID);
            Assert.AreEqual(ta.Type, tasLoaded[0].Type);
            Assert.IsTrue(ta.Assignee is IUser);
            Assert.AreEqual(ta.Assignee.ID, tasLoaded[0].Assignee.ID);
            Assert.AreEqual(ta.Assignee.DisplayName, tasLoaded[0].Assignee.DisplayName);
        }
    }
}
