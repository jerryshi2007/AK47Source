using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCS.Library.Services;

namespace MCS.Services.Messages
{
	public class SendMailControlCreator : IFunctionTestControlCreator
	{
		public Control CreateControl(params string[] args)
		{
			return new SendMailControl();
		}
	}
}
