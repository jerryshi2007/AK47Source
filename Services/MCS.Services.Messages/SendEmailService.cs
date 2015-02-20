using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Services;
using MCS.Library.SOA.DataObjects;

namespace MCS.Services.Messages
{
	/// <summary>
	/// 发送邮件的服务
	/// </summary>
	public class SendEmailService : ThreadTaskBase
	{
		public override void OnThreadTaskStart()
		{
			EmailMessageAdapter.Instance.SendCandidateMessages(this.Params.BatchCount);
		}
	}
}
