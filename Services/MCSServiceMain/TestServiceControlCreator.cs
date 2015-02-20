using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Services
{
	public class TestServiceControlCreator : IFunctionTestControlCreator
	{
		public System.Windows.Forms.Control CreateControl(params string[] args)
		{
			return new TestServiceControl();
		}
	}
}
