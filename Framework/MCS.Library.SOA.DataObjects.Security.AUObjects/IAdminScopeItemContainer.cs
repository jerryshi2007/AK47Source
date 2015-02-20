using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects
{
	public interface IAdminScopeItemContainer
	{
		SchemaObjectCollection GetCurrentObjects();
	}
}
