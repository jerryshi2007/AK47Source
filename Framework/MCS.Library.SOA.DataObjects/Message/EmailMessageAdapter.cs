using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Workflow;
using System.Data;
using System.Transactions;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects
{
	public class EmailMessageAdapter
	{
		public static readonly EmailMessageAdapter Instance = new EmailMessageAdapter();

		private EmailMessageAdapter()
		{
		}

		public void Send(EmailMessage message, SmtpParameters sp)
		{
			EmailMessageCollection messages = new EmailMessageCollection();

			messages.Add(message);

			Send(messages, sp);
		}

		public void Send(IEnumerable<EmailMessage> messages, SmtpParameters sp)
		{
			messages.NullCheck("messages");
			sp.NullCheck("sp");

			sp.CheckParameters();

			using (SmtpClient client = sp.ToSmtpClient())
			{
				foreach (EmailMessage message in messages)
				{
					EmailMessage clonedMessage = message.Clone();

					if (clonedMessage.From == null)
						clonedMessage.From = sp.DefaultSender;

					try
					{
						clonedMessage.Attachments.ForEach(ea => ea.PrepareContent());
						client.Send(clonedMessage.ToMailMessage());
						clonedMessage.Status = EmailMessageStatus.Sent;
					}
					catch (System.Exception ex)
					{
						clonedMessage.Status = EmailMessageStatus.Fail;
						clonedMessage.StatusText = ex.ToString();
					}

					if (sp.AfterSentOP == EmailMessageAfterSentOP.MoveToSentTable ||
						(sp.AfterSentOP == EmailMessageAfterSentOP.OnlyPersistErrorMessages && clonedMessage.Status == EmailMessageStatus.Fail))
					{
						InsertSentMessage(clonedMessage);
					}
				}
			}
		}

		/// <summary>
		/// 发送候选的邮件
		/// </summary>
		/// <param name="batchCount"></param>
		public void SendCandidateMessages(int batchCount)
		{
			SmtpParameters sp = EmailMessageSettings.GetConfig().ToSmtpParameters();

			SendCandidateMessages(batchCount, sp);
		}

		/// <summary>
		/// 发送候选的邮件
		/// </summary>
		/// <param name="batchCount"></param>
		/// <param name="sp"></param>
		public void SendCandidateMessages(int batchCount, SmtpParameters sp)
		{
			sp.NullCheck("sp");

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				EmailMessageCollection messages = LoadCandidateMessages(batchCount);

				Send(messages, sp);

				DeleteEmailMessagesMainData(messages);

				scope.Complete();
			}
		}

		public EmailMessageCollection LoadCandidateMessages(int batchCount)
		{
			string topDesp = batchCount >= 0 ? string.Format(" TOP {0} ", batchCount) : string.Empty;

			string sql = string.Format("SELECT {0}* FROM MSG.EMAIL_MESSAGES WITH (UPDLOCK READPAST) ORDER BY SORT_ID", topDesp);

			EmailMessageCollection result = new EmailMessageCollection();

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			foreach (DataRow row in table.Rows)
			{
				EmailMessage message = new EmailMessage();

				DataRowToEmailMessage(row, message);

				result.Add(message);
			}

			return result;
		}

		/// <summary>
		/// 删除EmailMessage
		/// </summary>
		/// <param name="action"></param>
		public void Delete(Action<WhereSqlClauseBuilder> action)
		{
			action.NullCheck("action");

			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			action(builder);

			StringBuilder strB = new StringBuilder();

			using (StringWriter writer = new StringWriter(strB))
			{
				WriteDeleteEmailAddressesSql(writer, builder);
				WriteDeleteEmailAttachmentsSql(writer, builder);
				writer.WriteLine("DELETE MSG.EMAIL_MESSAGES WHERE {0}",
					builder.ToSqlString(TSqlBuilder.Instance));
			}

			DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName());
		}

		internal EmailAttachmentCollection LoadAttachments(string messageID)
		{
			string sql = string.Format("SELECT * FROM MSG.EMAIL_ATTACHMENTS WHERE MESSAGE_ID = {0} ORDER BY SORT_ID",
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(messageID));

			EmailAttachmentCollection result = new EmailAttachmentCollection();

			ORMapping.DataViewToCollection(result,
				DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0].DefaultView);

			return result;
		}

		private static void WriteDeleteEmailAddressesSql(TextWriter writer, WhereSqlClauseBuilder builder)
		{
			writer.WriteLine("DELETE MSG.EMAIL_ADDRESSES FROM MSG.EMAIL_MESSAGES M INNER JOIN MSG.EMAIL_ADDRESSES A ON M.MESSAGE_ID = A.MESSAGE_ID WHERE {0}",
				builder.ToSqlString(TSqlBuilder.Instance));
		}

		private static void WriteDeleteEmailAttachmentsSql(TextWriter writer, WhereSqlClauseBuilder builder)
		{
			writer.WriteLine("DELETE MSG.EMAIL_ATTACHMENTS FROM MSG.EMAIL_MESSAGES M INNER JOIN MSG.EMAIL_ATTACHMENTS A ON M.MESSAGE_ID = A.MESSAGE_ID WHERE {0}",
				builder.ToSqlString(TSqlBuilder.Instance));
		}

		private void InsertSentMessage(EmailMessage message)
		{
			StringBuilder strB = new StringBuilder(2048);

			using (StringWriter writer = new StringWriter(strB))
			{
				WriteEmailMessageBodySql(writer, message, "MSG.SENT_EMAIL_MESSAGES", true);
			}

			DbHelper.RunSql(strB.ToString(), GetConnectionName());
		}

		public void Insert(EmailMessage message)
		{
			message.NullCheck("message");

			message.Attachments.ForEach(ea => ea.PrepareContent());

			StringBuilder strB = new StringBuilder(2048);

			using (StringWriter writer = new StringWriter(strB))
			{
				WriteInsertMessageSql(writer, message);
			}

			DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName());
		}

		public void Insert(IEnumerable<EmailMessage> messages)
		{
			messages.NullCheck("messages");

			messages.ForEach(m => m.Attachments.ForEach(ea => ea.PrepareContent()));

			StringBuilder strB = new StringBuilder(2048);

			using (StringWriter writer = new StringWriter(strB))
			{
				messages.ForEach(m => WriteInsertMessageSql(writer, m));
			}

			DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName());
		}

		public EmailMessage Load(string messageID)
		{
			return LoadEmailMessage(messageID, ORMapping.GetMappingInfo<EmailMessage>().TableName);
		}

		public EmailMessage LoadSentMessage(string messageID)
		{
			return LoadEmailMessage(messageID, "MSG.SENT_EMAIL_MESSAGES");
		}

		internal void EnsureChildPropertiesLoaded(EmailMessage message)
		{
			if (message.Loaded)
			{
				if (message.ChildrenPropertiesLoaded == false)
				{
					DataTable addressTable = LoadAddresses(message.ID);

					message._Bcc = GetAddressesFromTable(addressTable, "Bcc");
					message._CC = GetAddressesFromTable(addressTable, "CC");
					message._ReplyToList = GetAddressesFromTable(addressTable, "ReplyToList");
					message._To = GetAddressesFromTable(addressTable, "To");

					message._From = GetAddressFromTable(addressTable, "From");
					message._Sender = GetAddressFromTable(addressTable, "Sender");
				}

				message.ChildrenPropertiesLoaded = true;
			}
			else
			{
				message._Bcc = GetExistOrNewAddressColleciton(message._Bcc);
				message._CC = GetExistOrNewAddressColleciton(message._CC);
				message._ReplyToList = GetExistOrNewAddressColleciton(message._ReplyToList);
				message._To = GetExistOrNewAddressColleciton(message._To);
			}
		}

		private void DeleteEmailMessagesMainData(IEnumerable<EmailMessage> messages)
		{
			InSqlClauseBuilder builder = new InSqlClauseBuilder("MESSAGE_ID");

			messages.ForEach(m => builder.AppendItem(m.ID));

			if (builder.Count > 0)
			{
				string sql = string.Format("DELETE MSG.EMAIL_MESSAGES WHERE {0}", builder.ToSqlString(TSqlBuilder.Instance));
				DbHelper.RunSql(sql, GetConnectionName());
			}
		}

		#region DataToObject
		private static EmailMessage LoadEmailMessage(string messageID, string tableName)
		{
			messageID.CheckStringIsNullOrEmpty("messageID");

			string sql = string.Format("SELECT * FROM {0} WHERE MESSAGE_ID = {1}",
				tableName,
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(messageID));

			DataTable table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];

			(table.Rows.Count > 0).FalseThrow("不能找到MESSAGE_ID为{0}的记录", messageID);

			EmailMessage message = new EmailMessage();

			DataRowToEmailMessage(table.Rows[0], message);

			return message;
		}

		private static void DataRowToEmailMessage(DataRow row, EmailMessage message)
		{
			ORMapping.DataRowToObject(row, message);

			message.BodyEncoding = message.BodyEncoding.FromDescription(row["BODY_ENCODING"].ToString());
			message.HeadersEncoding = message.HeadersEncoding.FromDescription(row["HEADERS_ENCODING"].ToString());
			message.SubjectEncoding = message.SubjectEncoding.FromDescription(row["SUBJECT_ENCODING"].ToString());

			message.Loaded = true;
		}

		private static EmailAddressCollection GetExistOrNewAddressColleciton(EmailAddressCollection originalAddresses)
		{
			EmailAddressCollection result = originalAddresses;

			if (result == null)
				result = new EmailAddressCollection();

			return result;
		}

		private static EmailAddress GetAddressFromTable(DataTable addressTable, string className)
		{
			DataView view = GetAddressViewByClassName(addressTable, className);

			EmailAddress address = null;

			if (view.Count > 0)
			{
				address = new EmailAddress();
				ORMapping.DataRowToObject(view[0].Row, address);
			}

			return address;
		}

		private static EmailAddressCollection GetAddressesFromTable(DataTable addressTable, string className)
		{
			EmailAddressCollection addresses = new EmailAddressCollection();

			DataView view = GetAddressViewByClassName(addressTable, className);

			foreach (DataRowView drv in view)
			{
				EmailAddress address = new EmailAddress();

				ORMapping.DataRowToObject(drv.Row, address);

				addresses.Add(address);
			}

			return addresses;
		}

		private static DataView GetAddressViewByClassName(DataTable addressTable, string className)
		{
			DataView view = new DataView(addressTable);

			view.RowFilter = "CLASS = " + TSqlBuilder.Instance.CheckQuotationMark(className, true);
			view.Sort = "SORT_ID";

			return view;
		}

		private static DataTable LoadAddresses(string messageID)
		{
			string sql = string.Format("SELECT * FROM MSG.EMAIL_ADDRESSES WHERE MESSAGE_ID = {0}",
				TSqlBuilder.Instance.CheckUnicodeQuotationMark(messageID));

			return DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];
		}

		#endregion

		#region WriteSQL
		private static void WriteInsertMessageSql(TextWriter writer, EmailMessage message)
		{
			ORMappingItemCollection mappingInfo = GetMappingInfo();
			WriteEmailMessageBodySql(writer, message, mappingInfo.TableName, false);

			WriteInsertAddressSql(writer, message.From, message.ID, "From", 0);
			WriteInsertAddressSql(writer, message.Sender, message.ID, "Sender", 0);
			WriteInsertAddressCollection(writer, message.Bcc, message.ID, "Bcc");
			WriteInsertAddressCollection(writer, message.CC, message.ID, "CC");
			WriteInsertAddressCollection(writer, message.ReplyToList, message.ID, "ReplyToList");
			WriteInsertAddressCollection(writer, message.To, message.ID, "To");

			WriteInsertAttachmentCollection(writer, message.Attachments, message.ID);
		}

		private static void WriteEmailMessageBodySql(TextWriter writer, EmailMessage message, string tableName, bool generateSentTime)
		{
			ORMappingItemCollection mappingInfo = GetMappingInfo();
			InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(message, mappingInfo);

			builder.AppendItem("BODY_ENCODING", message.BodyEncoding.ToDescription());
			builder.AppendItem("HEADERS_ENCODING", message.HeadersEncoding.ToDescription());
			builder.AppendItem("SUBJECT_ENCODING", message.SubjectEncoding.ToDescription());

			if (generateSentTime)
			{
				builder.AppendItem("SENT_TIME", "GETDATE()", "=", true);
				builder.AppendItem("SORT_ID", message.SortID);
			}

			string sql = string.Format("INSERT INTO {0}{1}",
				tableName, builder.ToSqlString(TSqlBuilder.Instance));

			writer.WriteLine(sql);
		}

		private static void WriteInsertAttachmentCollection(TextWriter writer, EmailAttachmentCollection attachments, string messageID)
		{
			for (int i = 0; i < attachments.Count; i++)
				WriteInsertAttachmentSql(writer, attachments[i], messageID, i);
		}

		private static void WriteInsertAttachmentSql(TextWriter writer, EmailAttachment attachment, string messageID, int index)
		{
			InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(attachment);

			builder.AppendItem("MESSAGE_ID", messageID);
			builder.AppendItem("SORT_ID", index);

			string sql = string.Format("INSERT INTO MSG.EMAIL_ATTACHMENTS{0}",
				builder.ToSqlString(TSqlBuilder.Instance));

			writer.WriteLine(sql);
		}

		private static void WriteInsertAddressCollection(TextWriter writer, EmailAddressCollection addresses, string messageID, string className)
		{
			for (int i = 0; i < addresses.Count; i++)
				WriteInsertAddressSql(writer, addresses[i], messageID, className, i);
		}

		private static void WriteInsertAddressSql(TextWriter writer, EmailAddress address, string messageID, string className, int index)
		{
			if (address != null)
			{
				InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(address);

				builder.AppendItem("MESSAGE_ID", messageID);
				builder.AppendItem("SORT_ID", index);
				builder.AppendItem("CLASS", className);

				string sql = string.Format("INSERT INTO MSG.EMAIL_ADDRESSES{0}",
					builder.ToSqlString(TSqlBuilder.Instance));

				writer.WriteLine(sql);
			}
		}
		#endregion WriteSQL

		private static string GetConnectionName()
		{
			string result = EmailMessageSettings.GetConfig().ConnectionName;

			if (result.IsNullOrEmpty())
				result = WorkflowSettings.GetConfig().ConnectionName;

			return result;
		}

		private static ORMappingItemCollection GetMappingInfo()
		{
			return ORMapping.GetMappingInfo<EmailMessage>();
		}
	}
}
