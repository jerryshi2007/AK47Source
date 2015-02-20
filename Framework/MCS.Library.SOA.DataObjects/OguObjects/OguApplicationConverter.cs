using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	public class OguApplicationConverter : OguPermissionObjectConverter<OguApplication>
	{
		protected override OguApplication CreateObject()
		{
			return new OguApplication();
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				List<Type> types = new List<Type>();

				types.Add(typeof(OguApplication));
				types.Add(typeof(IApplication));

				return types;
			}
		}
	}
}
