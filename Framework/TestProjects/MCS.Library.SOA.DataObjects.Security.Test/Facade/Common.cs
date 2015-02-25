using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class Common
	{
		[ClassInitialize]
		public static void Cleanup()
		{
			MCS.Library.SOA.DataObjects.Security.Adapters.SchemaObjectAdapter.Instance.ClearAllData();
		}
	}
}
