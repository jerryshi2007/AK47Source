using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
	public sealed class ComplexFormDataAdapter : GenericFormDataAdapterBase<ComplexFormData, GenericFormRelativeDataCollection>
	{
		public static readonly ComplexFormDataAdapter Instance = new ComplexFormDataAdapter();

		private ComplexFormDataAdapter()
		{
		}

		protected override void AfterInnerUpdate(ComplexFormData data, Dictionary<string, object> context)
		{
			base.AfterInnerUpdate(data, context);

			base.UpdateRelativeData(data.ID, "SubDataA", data.SubDataA);
			base.UpdateRelativeData(data.ID, "SubDataB", data.SubDataB);
		}
	}
}
