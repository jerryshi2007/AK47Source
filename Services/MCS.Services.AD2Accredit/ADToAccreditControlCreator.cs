using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCS.Library.Services;

namespace MCS.Services.AD2Accredit
{
	public class ADToAccreditControlCreator : IFunctionTestControlCreator
	{
		public Control CreateControl(params string[] args)
		{
			return new ADToAccreditControl();
		}
	}
}
