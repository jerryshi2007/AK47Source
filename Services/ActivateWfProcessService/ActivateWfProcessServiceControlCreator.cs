using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Services;

namespace ActivateWfProcessService
{
	class ActivateWfProcessServiceControlCreator : IFunctionTestControlCreator
	{
		public System.Windows.Forms.Control CreateControl(params string[] args)
		{
			return new ActivateWfProcessServiceControl();
		}
	}
}
