using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[ORTableMapping("MSG.EMAIL_ATTACHMENTS")]
	public class EmailAttachment
	{
		public EmailAttachment()
		{
		}

		public EmailAttachment(string candidateFilePath)
		{
			this.CandidateFilePath = candidateFilePath;
		}

		[ORFieldMapping("FILE_NAME")]
		public string FileName
		{
			get;
			set;
		}

		[ORFieldMapping("CONTENT_DATA")]
		public byte[] ContentData
		{
			get;
			set;
		}

		/// <summary>
		/// 候选的文件路径，用于生成ContentData
		/// </summary>
		[NoMapping]
		public string CandidateFilePath
		{
			get;
			set;
		}

		public Attachment ToMailAttachment()
		{
			MemoryStream ms = new MemoryStream(this.ContentData);

			return new Attachment(ms, this.FileName);
		}

		/// <summary>
		/// 根据CandidateFilePath的数据准备Content
		/// </summary>
		internal void PrepareContent()
		{
			if (this.CandidateFilePath.IsNotEmpty())
			{
				using (FileStream fs = new FileStream(this.CandidateFilePath, FileMode.Open, FileAccess.Read))
				{
					using (MemoryStream ms = new MemoryStream())
					{
						fs.CopyTo(ms);

						ContentData = ms.ToArray();
					}

					this.FileName = Path.GetFileName(this.CandidateFilePath);
				}
			}
		}
	}

	[Serializable]
	public class EmailAttachmentCollection : EditableDataObjectCollectionBase<EmailAttachment>
	{
		public void Add(string candidateFilePath)
		{
			EmailAttachment ea = new EmailAttachment(candidateFilePath);

			this.Add(ea);
		}

		public void CopyTo(AttachmentCollection attachments)
		{
			this.ForEach(ea => attachments.Add(ea.ToMailAttachment()));
		}
	}
}
