using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace MCS.Library.Accredit.SoapControl
{
	/// <summary>
	/// SoapLog对象
	/// </summary>
	[ORTableMapping("WEB_READ_LOG")]
	public class SoapRecorder
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public SoapRecorder()
		{
			this.id = Guid.NewGuid();
			this.startDate = DateTime.Now;
			//this.hostIP = "1.1.1.1";// HttpContext.Current.Request.UserHostAddress;
		}
		private Guid id;
		/// <summary>
		/// 记录ID值
		/// </summary>
		[ORFieldMapping("GUID", IsIdentity = false, IsNullable = false, PrimaryKey = true)]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Insert | ClauseBindingFlags.Where, DefaultExpression = "NEWID()")]
		public Guid ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		private string hostIP;
		/// <summary>
		/// 客户端IP地址
		/// </summary>
		[ORFieldMapping("HOST_IP", IsIdentity = false, IsNullable = false)]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Insert, DefaultExpression = "")]
		public string HostIP
		{
			get
			{
				return this.hostIP;
			}
			set
			{
				this.hostIP = value;
			}
		}

		private string soapMethod;
		/// <summary>
		/// 请求服务的名称
		/// </summary>
		[ORFieldMapping("WEB_METHOD_NAME", IsIdentity = false, IsNullable = false)]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Insert, DefaultExpression = "")]
		public string SoapMethod
		{
			get
			{
				return this.soapMethod;
			}
			set
			{
				this.soapMethod = value;
			}
		}

		private string inputStream;
		/// <summary>
		/// 请求数据流对象
		/// </summary>
		[ORFieldMapping("INPUT_STREAM", IsIdentity = false, IsNullable = false)]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Insert, DefaultExpression = "")]
		public string InputStream
		{
			get
			{
				return this.inputStream;
			}
			set
			{
				//if (value.Length > 4000)
				//    this.inputStream = value.Substring(0, 4000);
				//else
				this.inputStream = value;
			}
		}

		private string outputStream;
		/// <summary>
		/// 反馈数据流对象
		/// </summary>
		[ORFieldMapping("OUTPUT_STREAM", IsIdentity = false, IsNullable = false)]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Insert, DefaultExpression = "")]
		public string OutputStream
		{
			get
			{
				return this.outputStream;
			}
			set
			{
				//if (value.Length > 4000)
				//    this.outputStream = value.Substring(0, 4000);
				//else
				this.outputStream = value;
			}
		}

		private DateTime startDate;
		/// <summary>
		/// 开始时间
		/// </summary>
		[ORFieldMapping("START_DATE", IsIdentity = false, IsNullable = false)]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Insert, DefaultExpression = "GETDATE()")]
		public DateTime StartDate
		{
			get
			{
				return this.startDate;
			}
			set
			{
				this.startDate = value;
			}
		}

		private DateTime endDate;
		/// <summary>
		/// 结束时间
		/// </summary>
		[ORFieldMapping("END_DATE", IsIdentity = false, IsNullable = false)]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Insert, DefaultExpression = "GETDATE()")]
		public DateTime EndDate
		{
			get
			{
				return this.endDate;
			}
			set
			{
				this.endDate = value;
			}
		}
	}
}
