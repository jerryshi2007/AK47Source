using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Validation;

namespace WfFormTemplate.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class TemplateData : WorkflowObjectBase
	{
		[StringLengthValidator(1, 255, MessageTemplate = "请填写标题，且长度必须小于255个字符")]
		public override string Subject
		{
			get
			{
				return base.Subject;
			}
			set
			{
				base.Subject = value;
			}
		}

		public string AdministrativeUnit
		{
			get;
			set;
		}

		public string CostCenter
		{
			get;
			set;
		}

		public int Amount
		{
			get;
			set;
		}
	}
}