using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security.Transfer
{
	[Serializable]
	public class SetterContext : Dictionary<string, object>
	{
		private bool _PropertyChanged = false;

		public bool PropertyChanged
		{
			get
			{
				return this._PropertyChanged;
			}
			set
			{
				this._PropertyChanged = value;
			}
		}
	}
}
