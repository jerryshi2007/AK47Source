using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;

namespace MCS.Library.SOA.DataObjects.Security.Test.SchemaObject
{
	[TestClass]
	public class PersistSchemaInfo
	{
		[TestMethod]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void SchemaDefinePersistTest()
		{
			SchemaDefineCollection schemas = SchemaExtensions.CreateSchemasDefineFromConfiguration();

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				schemas.ForEach(schema => SchemaDefineAdapter.Instance.Update(schema));

				scope.Complete();
			}
		}
	}
}
