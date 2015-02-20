using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Data.DataObjects;

namespace MCS.Web.Responsive.WebControls
{
	public class ControlPropertyDefineWrapper
	{
		public string ControlID
		{
			get;
			set;
		}

		public IEnumerable<ControlPropertyDefine> ControlPropertyDefineList
		{
			get;
			set;
		}

		public bool UseTemplate
		{
			get;
			set;
		}

		public string ExtendedSettings
		{
			get;
			set;
		}
	}

	public class ControlPropertyDefineKeyedCollection : EditableKeyedDataObjectCollectionBase<string, ControlPropertyDefineWrapper>
	{
		protected override string GetKeyForItem(ControlPropertyDefineWrapper item)
		{
			return item.ControlID;
		}
	}
}
