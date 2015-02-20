using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.DataObjects.Test.MessageTest
{
	[TestClass]
	public class EmailTest
	{
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		[TestMethod]
		[TestCategory("Message Test")]
		public void GenerateEmailMessageTest()
		{
			EmailMessage message = PrepareMailMessage();

			EmailMessageAdapter.Instance.Insert(message);

			try
			{
				EmailMessage messageLoaded = EmailMessageAdapter.Instance.Load(message.ID);

				Assert.AreEqual(message.ID, messageLoaded.ID);
				Assert.AreEqual(message.From.Address, messageLoaded.From.Address);
				Assert.AreEqual(message.To.Count, messageLoaded.To.Count);
			}
			finally
			{
				DeleteAllTestMessages();
			}
		}

		[TestMethod]
		[TestCategory("Message Test")]
		public void GenerateEmailMessageWithAttachmentsTest()
		{
			EmailMessage message = PrepareMailMessageWithAttachments();

			EmailMessageAdapter.Instance.Insert(message);

			try
			{
				EmailMessage messageLoaded = EmailMessageAdapter.Instance.Load(message.ID);

				Assert.AreEqual(message.ID, messageLoaded.ID);
				Assert.AreEqual(message.From.Address, messageLoaded.From.Address);
				Assert.AreEqual(message.To.Count, messageLoaded.To.Count);
				Assert.AreEqual(message.Attachments.Count, messageLoaded.Attachments.Count);
			}
			finally
			{
				DeleteAllTestMessages();
			}
		}

		[TestMethod]
		[TestCategory("Message Test")]
		public void SendEmailMessageTest()
		{
			EmailMessage message = PrepareMailMessage();

			SmtpParameters sp = PrepareSmtpParameters();

			EmailMessageCollection messages = new EmailMessageCollection();

			messages.Add(message);

			EmailMessageAdapter.Instance.Send(messages, sp);

			CheckSentMessage(message);
		}

		[TestMethod]
		[TestCategory("Message Test")]
		public void SendEmailMessageWithAttachmentTest()
		{
			EmailMessage message = PrepareMailMessageWithAttachments();

			SmtpParameters sp = PrepareSmtpParameters();

			EmailMessageCollection messages = new EmailMessageCollection();

			messages.Add(message);

			EmailMessageAdapter.Instance.Send(messages, sp);
		}

		[TestMethod]
		[TestCategory("Message Test")]
		public void SendCandidateEmailMessageTest()
		{
			DeleteAllTestMessages();

			EmailMessage message1 = PrepareMailMessage();
			EmailMessage message2 = PrepareMailMessage();

			EmailMessageCollection messages = new EmailMessageCollection();

			messages.Add(message1);
			messages.Add(message2);

			EmailMessageAdapter.Instance.Insert(messages);

			try
			{
				SmtpParameters sp = PrepareSmtpParameters();

				EmailMessageAdapter.Instance.SendCandidateMessages(10, sp);

				CheckSentMessage(message1);
				CheckSentMessage(message2);
			}
			finally
			{
				DeleteAllTestMessages();
			}
		}

		[TestMethod]
		[TestCategory("Message Test")]
		public void SendCandidateEmailMessageWithAttachmentsTest()
		{
			DeleteAllTestMessages();

			EmailMessage message1 = PrepareMailMessageWithAttachments();
			EmailMessage message2 = PrepareMailMessageWithAttachments();

			EmailMessageCollection messages = new EmailMessageCollection();

			messages.Add(message1);
			messages.Add(message2);

			EmailMessageAdapter.Instance.Insert(messages);

			try
			{
				SmtpParameters sp = PrepareSmtpParameters();

				EmailMessageAdapter.Instance.SendCandidateMessages(10, sp);

				CheckSentMessage(message1);
				CheckSentMessage(message2);
			}
			finally
			{
				DeleteAllTestMessages();
			}
		}

		private static void CheckSentMessage(EmailMessage sourceMessage)
		{
			EmailMessage sentMessage = EmailMessageAdapter.Instance.LoadSentMessage(sourceMessage.ID);

			Console.WriteLine(sentMessage.SentTime);

			Assert.AreEqual(sourceMessage.ID, sentMessage.ID);
			Assert.IsTrue(sentMessage.SentTime != DateTime.MinValue);
		}

		[TestMethod]
		[TestCategory("Message Test")]
		public void EmailAddressParsingTest()
		{
			EmailAddress address1 = EmailAddress.FromDescription("Tooling<Tooling@microsoft.com>");

			Assert.AreEqual("Tooling", address1.DisplayName);
			Assert.AreEqual("Tooling@microsoft.com", address1.Address);

			Console.WriteLine(address1);

			EmailAddress address2 = EmailAddress.FromDescription("Tooling@microsoft.com");

			Assert.AreEqual("Tooling@microsoft.com", address1.Address);

			Console.WriteLine(address2);
		}

		private static void DeleteAllTestMessages()
		{
			EmailMessageAdapter.Instance.Delete(b => b.AppendItem("SUBJECT", "Hello 神奇的邮件%", "LIKE"));
		}

		private static SmtpParameters PrepareSmtpParameters()
		{
			return EmailMessageSettings.GetConfig().ToSmtpParameters();
		}

		private static EmailMessage PrepareMailMessage()
		{
			EmailMessage message = new EmailMessage();

			message.ID = UuidHelper.NewUuidString();
			message.Subject = string.Format("Hello 神奇的邮件: {0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
			message.Body = "<B>神奇的邮件</B>";
			message.IsBodyHtml = true;
			message.From = new EmailAddress() { Address = "soaadmin@sinooceanland.com", DisplayName = "SOA管理员" };
			message.To.Add(new EmailAddress() { Address = "zhshen@microsoft.com", DisplayName = "沈峥" });
			message.To.Add(new EmailAddress() { Address = "sz@sinooceanland.com", DisplayName = "沈峥(微软)" });

			return message;
		}

		private EmailMessage PrepareMailMessageWithAttachments()
		{
			EmailMessage message = PrepareMailMessage();

			message.Attachments.Add(Path.Combine(TestContext.TestDeploymentDir, "9 persons.JPG"));
			message.Attachments.Add(Path.Combine(TestContext.TestDeploymentDir, "SilverLight - Intro ScottGu.pdf"));

			return message;
		}
	}
}
