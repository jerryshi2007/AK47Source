using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Protocols;
using System.Diagnostics;
using System.IO;
using System.Transactions;
using System.Xml;
using System.Web;
using System.Data.Common;

using MCS.Library.Data;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.Accredit.Configuration;

namespace MCS.Library.Accredit.SoapControl
{
	/// <summary>
	/// SoapExtension��չ����
	/// </summary>
	public class SoapTraceExtension : SoapExtension
	{
		/// <summary>
		/// ���캯��
		/// </summary>
		public SoapTraceExtension()
		{
			//if (AccreditSection.GetConfig().AccreditSettings.SoapRecord)
			soapRecorder = new SoapRecorder();
		}

		private Stream oldStream;
		private Stream newStream;
		private SoapRecorder soapRecorder = null;

		/// <summary>
		/// Soap��Ϣ�Ĳ�ͬ״̬����
		/// </summary>
		/// <param name="message">�ڲ������Soap��Ϣ������״̬��</param>
		public override void ProcessMessage(SoapMessage message)
		{
			switch (message.Stage)
			{
				case SoapMessageStage.BeforeSerialize:
					break;
				case SoapMessageStage.BeforeDeserialize:
					this.WriteInputStream(message);
					break;
				case SoapMessageStage.AfterSerialize:
					this.WriteOutputStream(message);
					break;
				case SoapMessageStage.AfterDeserialize:
					break;
				default:
					throw new Exception("Error Message Stage");
			}
		}

		private void WriteOutputStream(SoapMessage message)
		{
			this.newStream.Position = 0;
			Copy(this.newStream, this.oldStream);

			if (AccreditSection.GetConfig().AccreditSettings.SoapRecord & message is SoapServerMessage)
			{
				if (AccreditSection.GetConfig().AccreditSettings.SoapRecordOutput)
				{
					this.newStream.Position = 0;
					TextReader reader = new StreamReader(this.newStream);
					this.soapRecorder.OutputStream = reader.ReadToEnd();
				}

				this.soapRecorder.EndDate = DateTime.Now;
				if (HttpContext.Current != null)
					this.soapRecorder.HostIP = HttpContext.Current.Request.UserHostAddress;

				string sql = ORMapping.GetInsertSql<SoapRecorder>(this.soapRecorder, TSqlBuilder.Instance);
				try
				{
					OGUCommonDefine.ExecuteNonQuery(sql);
				}
				catch (DbException ex)
				{
					if (ex.Message.IndexOf("WEB_READ_LOG") > 0)
						InitDatabase(sql);
					//else
					//    throw ex;
				}
			}
		}

		private void WriteInputStream(SoapMessage message)
		{
			Copy(this.oldStream, this.newStream);

			if (AccreditSection.GetConfig().AccreditSettings.SoapRecord & message is SoapServerMessage)
			{
				this.newStream.Position = 0;
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(new StreamReader(this.newStream));

				if (AccreditSection.GetConfig().AccreditSettings.SoapRecordInput)
					this.soapRecorder.InputStream = xmlDoc.OuterXml;

				this.soapRecorder.SoapMethod += "." + xmlDoc.DocumentElement.FirstChild.FirstChild.LocalName;
			}
			
			this.newStream.Position = 0;
		}

		/// <summary>
		/// ����������
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		private void Copy(Stream from, Stream to)
		{
			TextReader reader = new StreamReader(from);
			TextWriter writer = new StreamWriter(to);
			writer.WriteLine(reader.ReadToEnd());
			writer.Flush();
		}

		/// <summary>
		/// ���ݳ�ʼ������
		/// </summary>
		/// <param name="initializer">��ʼ������</param>
		public override void Initialize(object initializer)
		{
			this.soapRecorder.SoapMethod = initializer.ToString();
		}

		/// <summary>
		/// ���Ϊÿ���������õı���SoapMessage�ļ��������ǽ������������
		/// ��SoapMessage�����浽һ����־�ļ���,����ļ�·����Ҫ��Web Service
		/// �������ļ���web.configָ��,��
		/// <appSettings>
		///  <add key="logRoot" value="c:\\serviceLog"/>
		/// </appSettings>
		/// </summary>
		/// <param name="serviceType">������������</param>
		/// <returns>���ڱ�����־��¼���ļ�·��</returns>
		public override object GetInitializer(Type serviceType)
		{
			return (string)serviceType.ToString();
		}

		/// <summary>
		/// ��Xml Web Service��һ�����е�ʱ��һ���ԵĽ�ͨ��TraceExtensionAttribute���ݽ�����
		/// ������־��Ϣ���ļ�����ʼ��
		/// </summary>
		/// <param name="methodInfo">Ӧ�� SOAP ��չ�� XML Web services �������ض�����ԭ��</param>
		/// <param name="attribute">Ӧ���� XML Web services ������ SoapExtensionAttribute</param>
		/// <returns>SOAP ��չ��������г�ʼ�������ڻ���</returns>
		public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			return ((SoapTraceExtensionAttribute)attribute).ExtensionType;
		}

		/// <summary>
		/// ������������Ӧ���浽�ڴ����У��ѱ�����
		/// </summary>
		/// <param name="stream">���� SOAP �������Ӧ���ڴ滺����</param>
		/// <returns>����ʾ�� SOAP ��չ�����޸ĵ����ڴ滺������</returns>
		public override Stream ChainStream(Stream stream)
		{
			this.oldStream = stream;
			this.newStream = new MemoryStream();

			return this.newStream;
		}
		/// <summary>
		/// ִ�����ݻ�ȡ����⡿
		/// </summary>
		/// <param name="originalSql">ԭʼ����</param>
		private void InitDatabase(string originalSql)
		{
			string sql = @"
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=N'WEB_READ_LOG')
	DROP TABLE dbo.WEB_READ_LOG
	
CREATE TABLE dbo.WEB_READ_LOG
(
	GUID uniqueidentifier NOT NULL PRIMARY KEY NONCLUSTERED,
	WEB_METHOD_NAME nvarchar(128) NOT NULL,
	INPUT_STREAM ntext NULL,
	OUTPUT_STREAM ntext NULL,
	START_DATE datetime NOT NULL,
	END_DATE datetime NOT NULL,
	HOST_IP nvarchar(16) NOT NULL
)  ON [PRIMARY]

CREATE CLUSTERED INDEX IX_WEB_READ_LOG_START_DATE ON dbo.WEB_READ_LOG (START_DATE) ON [PRIMARY]
" + originalSql;

			OGUCommonDefine.ExecuteNonQuery(sql);
		}
	}
}
