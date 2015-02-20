using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	public class GenericFormDataAdapter : GenericFormDataAdapterBase<GenericFormData, GenericFormDataCollection>
	{
		public static readonly GenericFormDataAdapter Instance = new GenericFormDataAdapter();

		private GenericFormDataAdapter()
		{
		}
	}
}
