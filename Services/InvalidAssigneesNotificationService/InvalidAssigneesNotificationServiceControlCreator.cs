using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Services;

namespace InvalidAssigneesNotificationService
{
	public class InvalidAssigneesNotificationServiceControlCreator : IFunctionTestControlCreator
	{
		public System.Windows.Forms.Control CreateControl(params string[] args)
		{
			return new InvalidAssigneesNotificationServiceControl();
		}
	}
}
