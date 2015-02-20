using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class DataObjectDefine
	{
		public string Name { get; set; }
		public string Description { get; set; }

		private DataFieldDefineCollection _Fields = null;

		public DataFieldDefineCollection Fields
		{
			get 
			{
				if (this._Fields == null)
					this._Fields = new DataFieldDefineCollection();

				return this._Fields; 
			}
		}
	}
}
