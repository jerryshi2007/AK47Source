using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	public enum EmailMessageStatus
	{
		NotSend = 0,
		Sent = 1,
		Fail = 2
	}

	[Serializable]
	[ORTableMapping("MSG.EMAIL_MESSAGES")]
    [TenantRelativeObject]
	public class EmailMessage
	{
		public EmailMessage()
		{
			this.ID = UuidHelper.NewUuidString();
		}

		public EmailMessage(EmailAddress sendToAddress, string subject, string body)
		{
			this.ID = UuidHelper.NewUuidString();

			if (sendToAddress != null)
				this.To.Add(sendToAddress);

			this.Subject = subject;
			this.Body = body;
		}

		public EmailMessage(string sendToAddress, string subject, string body)
		{
			this.ID = UuidHelper.NewUuidString();

			if (sendToAddress.IsNotEmpty())
			{
				this.To.Add(EmailAddress.FromDescription(sendToAddress));
			}
	
			this.Subject = subject;
			this.Body = body;
		}

		[ORFieldMapping("MESSAGE_ID", PrimaryKey = true)]
		public string ID
		{
			get;
			set;
		}

		[ORFieldMapping("SORT_ID", IsIdentity = true)]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public int SortID
		{
			get;
			set;
		}

		internal EmailAddressCollection _Bcc = null;

		/// <summary>
		/// 邮件的Bcc人员
		/// </summary>
		[NoMapping]
		public EmailAddressCollection Bcc
		{
			get
			{
				EmailMessageAdapter.Instance.EnsureChildPropertiesLoaded(this);

				return this._Bcc;
			}
		}

		/// <summary>
		/// 邮件体
		/// </summary>
		[ORFieldMapping("BODY")]
		public string Body
		{
			get;
			set;
		}

		private Encoding _BodyEncoding = Encoding.UTF8;

		/// <summary>
		/// 邮件体的编码方式
		/// </summary>
		[NoMapping]
		public Encoding BodyEncoding
		{
			get
			{
				return this._BodyEncoding;
			}
			set
			{
				this._BodyEncoding = value;
			}
		}

		internal EmailAddressCollection _CC = null;

		/// <summary>
		/// 邮件的抄送人
		/// </summary>
		[NoMapping]
		public EmailAddressCollection CC
		{
			get
			{
				EmailMessageAdapter.Instance.EnsureChildPropertiesLoaded(this);

				return this._CC;
			}
		}

		internal EmailAddress _From = null;

		/// <summary>
		/// 发件人
		/// </summary>
		[NoMapping]
		public EmailAddress From
		{
			get
			{
				EmailMessageAdapter.Instance.EnsureChildPropertiesLoaded(this);

				return this._From;
			}
			set
			{
				this._From = value;
			}
		}

		private Encoding _HeadersEncoding = Encoding.UTF8;

		/// <summary>
		/// Header的Encoding方式
		/// </summary>
		[NoMapping]
		public Encoding HeadersEncoding
		{
			get
			{
				return this._HeadersEncoding;
			}
			set
			{
				this._HeadersEncoding = value;
			}
		}

		private bool _IsBodyHtml = false;

		/// <summary>
		/// 是否是Html的邮件体
		/// </summary>
		[ORFieldMapping("IS_BODY_HTML")]
		public bool IsBodyHtml
		{
			get
			{
				return this._IsBodyHtml;
			}
			set
			{
				this._IsBodyHtml = value;
			}
		}

		private MailPriority _Priority = MailPriority.Normal;

		/// <summary>
		/// 优先级
		/// </summary>
		[ORFieldMapping("PRIORITY")]
		public MailPriority Priority
		{
			get
			{
				return this._Priority;
			}
			set
			{
				this._Priority = value;
			}
		}

		internal EmailAddressCollection _ReplyToList = null;

		/// <summary>
		/// 邮件回复地址
		/// </summary>
		[NoMapping]
		public EmailAddressCollection ReplyToList
		{
			get
			{
				EmailMessageAdapter.Instance.EnsureChildPropertiesLoaded(this);

				return this._ReplyToList;
			}
		}

		internal EmailAddress _Sender = null;

		/// <summary>
		/// 邮件的发件人
		/// </summary>
		[NoMapping]
		public EmailAddress Sender
		{
			get
			{
				EmailMessageAdapter.Instance.EnsureChildPropertiesLoaded(this);

				return this._Sender;
			}
			set
			{
				this._Sender = value;
			}
		}

		/// <summary>
		/// 邮件的标题
		/// </summary>
		[ORFieldMapping("SUBJECT")]
		public string Subject
		{
			get;
			set;
		}

		private Encoding _SubjectEncoding = Encoding.UTF8;

		/// <summary>
		/// 标题的编码方式
		/// </summary>
		[NoMapping]
		public Encoding SubjectEncoding
		{
			get
			{
				return this._SubjectEncoding;
			}
			set
			{
				this._SubjectEncoding = value;
			}
		}

		internal EmailAddressCollection _To = null;

		/// <summary>
		/// 邮件的发送人
		/// </summary>
		[NoMapping]
		public EmailAddressCollection To
		{
			get
			{
				EmailMessageAdapter.Instance.EnsureChildPropertiesLoaded(this);

				return this._To;
			}
		}

		private EmailAttachmentCollection _Attachments = null;

		[NoMapping]
		public EmailAttachmentCollection Attachments
		{
			get
			{
				if (this._Attachments == null)
				{
					if (this.Loaded)
						this._Attachments = EmailMessageAdapter.Instance.LoadAttachments(this.ID);
					else
						this._Attachments = new EmailAttachmentCollection();
				}

				return this._Attachments;
			}
		}

		[ORFieldMapping("STATUS")]
		public EmailMessageStatus Status
		{
			get;
			set;
		}

		[ORFieldMapping("STATUS_TEXT")]
		public string StatusText
		{
			get;
			set;
		}

		[ORFieldMapping("SENT_TIME")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public DateTime SentTime
		{
			get;
			set;
		}

		[NoMapping]
		internal bool Loaded
		{
			get;
			set;
		}

		[NoMapping]
		internal bool ChildrenPropertiesLoaded
		{
			get;
			set;
		}

		/// <summary>
		/// 转换为标准SMTP的邮件消息
		/// </summary>
		/// <returns></returns>
		public MailMessage ToMailMessage()
		{
			MailMessage result = new MailMessage();

			this.Bcc.CopyTo(result.Bcc);
			result.Body = this.Body;
			result.BodyEncoding = this.BodyEncoding;
			this.CC.CopyTo(result.CC);
			result.From = this.From.ToMailAddress();
			result.IsBodyHtml = this.IsBodyHtml;
			result.Priority = this.Priority;
			this.ReplyToList.CopyTo(result.ReplyToList);

			result.Sender = this.Sender.ToMailAddress();
			result.Subject = this.Subject;
			result.SubjectEncoding = this.SubjectEncoding;
			this.To.CopyTo(result.To);
			this.Attachments.CopyTo(result.Attachments);

			return result;
		}

		public EmailMessage Clone()
		{
			EmailMessage result = new EmailMessage();

			result.ID = this.ID;
			result.SortID = this.SortID;
			result._Attachments = this._Attachments;
			result._Bcc = this._Bcc;
			result.Body = this.Body;
			result._BodyEncoding = this._BodyEncoding;
			result._CC = this._CC;
			result._From = this._From;
			result._HeadersEncoding = this._HeadersEncoding;
			result._IsBodyHtml = this._IsBodyHtml;
			result._Priority = this._Priority;
			result._ReplyToList = this._ReplyToList;
			result._Sender = this._Sender;
			result.Subject = this.Subject;
			result._SubjectEncoding = this._SubjectEncoding;
			result._To = this._To;
			result.Loaded = this.Loaded;
			result.ChildrenPropertiesLoaded = this.ChildrenPropertiesLoaded;

			return result;
		}
	}

	[Serializable]
	public class EmailMessageCollection : EditableDataObjectCollectionBase<EmailMessage>
	{
	}
}
