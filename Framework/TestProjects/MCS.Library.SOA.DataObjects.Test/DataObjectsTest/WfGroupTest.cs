using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
	[TestClass]
	public class WfGroupTest
	{
		[TestMethod]
		public void UpdateGroupTest()
		{
          
		}

		[TestMethod]
		public void AddGroupUserTest()
		{

		}

		[TestMethod]
		public void DeleteGroupUserTest()
		{

		}

		[TestMethod]
		public void DeleteGroupTest()
		{
		}

        private IUser GetUser()
        {
            IUser user = new OguUser(Guid.NewGuid().ToString());
           

            return user;
        }
	}
}
