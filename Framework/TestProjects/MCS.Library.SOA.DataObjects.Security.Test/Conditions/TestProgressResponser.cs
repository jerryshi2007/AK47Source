using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Test.Conditions
{
	public class TestProgressResponser : IProcessProgressResponser
	{
		public static readonly TestProgressResponser Instance = new TestProgressResponser();

		private TestProgressResponser()
		{
		}

		#region IProcessProgressResponser Members

		public void Register(ProcessProgress progress)
		{
		}

		public void Response(ProcessProgress progress)
		{
			Console.WriteLine("Current Step: {0}, Text: {1}", progress.CurrentStep, progress.StatusText);
		}

		#endregion
	}
}
