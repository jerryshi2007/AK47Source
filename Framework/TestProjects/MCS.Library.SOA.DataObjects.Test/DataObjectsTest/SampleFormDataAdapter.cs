using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
	public sealed class SampleFormDataAdapter : GenericFormDataAdapterBase<SampleFormData, SampleFormDataCollection>
	{
		public static readonly SampleFormDataAdapter Instance = new SampleFormDataAdapter();

		private SampleFormDataAdapter()
		{
		}
	}
}
