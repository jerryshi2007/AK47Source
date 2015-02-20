using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;

namespace MCS.Library.SOA.DataObjects.Security.Logs
{
	public static class SCOperationLogExtensions
	{
		public static SCOperationLog ToOperationLog(this SCBase data, SCOperationType opType)
		{
			data.NullCheck("data");

			SCOperationLog log = SCOperationLog.CreateLogFromEnvironment();

			log.ResourceID = data.ID;
			log.SchemaType = data.SchemaType;
			log.OperationType = opType;
			log.Category = data.Schema.Category;
			log.Subject = string.Format("{0}: {1}",
				EnumItemDescriptionAttribute.GetDescription(opType), data.Name);

			log.SearchContent = data.ToFullTextString();

			return log;
		}
	}
}
