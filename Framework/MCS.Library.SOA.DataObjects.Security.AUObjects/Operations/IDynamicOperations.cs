using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Operations
{
	public interface IDynamicOperations
	{
		SchemaObjectBase DoOperation(SCObjectOperationMode opMode, SchemaObjectBase data, SchemaObjectBase parent, bool deletedByContainer = false);
	}
}
