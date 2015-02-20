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
	/// SoapExtension扩展定义
	/// </summary>
	public class SoapTraceExtension : SoapExtension
	{
		/// <summary>
		/// 构造函数
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
		/// Soap消息的不同状态处理
		/// </summary>
		/// <param name="message">内部传输的Soap消息【带有状态】</param>
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
		/// 拷贝流到流
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
		/// 数据初始化处理
		/// </summary>
		/// <param name="initializer">初始化数据</param>
		public override void Initialize(object initializer)
		{
			this.soapRecorder.SoapMethod = initializer.ToString();
		}

		/// <summary>
		/// 替代为每个方法配置的保存SoapMessage文件名，而是将整个网络服务
		/// 的SoapMessage都保存到一个日志文件中,这个文件路径需要在Web Service
		/// 的配置文件中web.config指出,如
		/// <appSettings>
		///  <add key="logRoot" value="c:\\serviceLog"/>
		/// </appSettings>
		/// </summary>
		/// <param name="serviceType">网络服务的类型</param>
		/// <returns>用于保存日志记录的文件路径</returns>
		public override object GetInitializer(Type serviceType)
		{
			return (string)serviceType.ToString();
		}

		/// <summary>
		/// 在Xml Web Service第一次运行的时候，一次性的将通过TraceExtensionAttribute传递进来的
		/// 保存日志信息的文件名初始化
		/// </summary>
		/// <param name="methodInfo">应用 SOAP 扩展的 XML Web services 方法的特定函数原型</param>
		/// <param name="attribute">应用于 XML Web services 方法的 SoapExtensionAttribute</param>
		/// <returns>SOAP 扩展将对其进行初始化以用于缓存</returns>
		public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			return ((SoapTraceExtensionAttribute)attribute).ExtensionType;
		}

		/// <summary>
		/// 将请求流和响应流存到内存流中，已被调用
		/// </summary>
		/// <param name="stream">包含 SOAP 请求或响应的内存缓冲区</param>
		/// <returns>它表示此 SOAP 扩展可以修改的新内存缓冲区。</returns>
		public override Stream ChainStream(Stream stream)
		{
			this.oldStream = stream;
			this.newStream = new MemoryStream();

			return this.newStream;
		}
		/// <summary>
		/// 执行数据获取【入库】
		/// </summary>
		/// <param name="originalSql">原始数据</param>
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
