using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;

namespace MCS.Services.Messages
{
	[Serializable]
	internal class SendMailParameters
	{
		public SmtpParameters SmtpParams = null;
		public string DefaultEmailSubject = string.Empty;
		public EmailAddress ToAddress = null;
	}
}
