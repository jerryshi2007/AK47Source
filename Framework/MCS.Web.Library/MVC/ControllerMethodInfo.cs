using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MCS.Library.Core;

namespace MCS.Web.Library.MVC
{
	internal class ControllerMethodInfo
	{
		private SortedSet<string> forceIgnoreParameters = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

		public ControllerMethodInfo(MethodInfo mi, string delimitedFIP)
		{
			mi.NullCheck("mi");

			this.ControllerMethod = mi;
			FillForceIgnoreParameters(delimitedFIP);
		}

		public MethodInfo ControllerMethod
		{
			get;
			private set;
		}

		public ICollection<string> ForceIgnoreParameters
		{
			get
			{
				return this.forceIgnoreParameters;
			}
		}

		private void FillForceIgnoreParameters(string delimitedFIP)
		{
			if (delimitedFIP.IsNotEmpty())
			{
				string[] parts = delimitedFIP.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

				for (int i = 0; i < parts.Length; i++)
				{
					string part = parts[i].Trim();

					if (part.IsNotEmpty())
						this.forceIgnoreParameters.Add(part);
				}
			}
		}
	}
}
