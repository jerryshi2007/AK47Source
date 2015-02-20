using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace MCS.Library.SOA.DataObjects.Security.Debugger
{
	[DebuggerDisplay("{value}", Name = "{key}")]
	internal class ListKeyAndValue
	{
		private IList list;
		private object key;
		private object value;

		public ListKeyAndValue(IList list, object key, object value)
		{
			this.value = value;
			this.key = key;
			this.list = list;
		}
	}
}
