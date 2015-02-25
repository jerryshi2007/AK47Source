using System.Diagnostics;
using System.Linq;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Logs;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class ApplicationFacadeTest
	{
		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddApplicationTest()
		{
			Trace.CorrelationManager.ActivityId = UuidHelper.NewUuid();

			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCApplication appLoaded = (SCApplication)SchemaObjectAdapter.Instance.Load(application.ID);

			Assert.AreEqual(application.ID, appLoaded.ID);

			SCOperationLog log = SCOperationLogAdapter.Instance.LoadByResourceID(application.ID).FirstOrDefault();

			Assert.IsNotNull(log);
			Assert.AreEqual(Trace.CorrelationManager.ActivityId.ToString(), log.CorrelationID);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void DeleteApplicationTest()
		{
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCObjectOperations.Instance.DeleteApplication(application);

			SCApplication appLoaded = (SCApplication)SchemaObjectAdapter.Instance.Load(application.ID);

			Assert.AreEqual(SchemaObjectStatus.Deleted, appLoaded.Status);
		}
	}
}
