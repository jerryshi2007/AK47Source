using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.Passport.Social.Mechanism
{
	/// <summary>
	/// QQ登录返回的错误信息
	/// </summary>
	public class QQLoginConnectionException : System.Exception
	{
		public QQLoginConnectionException()
		{
		}

		public QQLoginConnectionException(string message)
			: base(message)
		{
		}

		public QQLoginConnectionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public string ErrorCode
		{
			get;
			private set;
		}

		public string ErrorDescription
		{
			get;
			private set;
		}

		/// <summary>
		/// 检查返回的文本，如果是异常信息，则抛出
		/// </summary>
		/// <param name="responseText"></param>
		public static void CheckResponseText(string responseText)
		{
			QQLoginConnectionException exception = QQLoginConnectionException.FromResponseText(responseText);

			if (exception != null)
				throw exception;
		}

		/// <summary>
		/// 从响应结果中返回异常类。如果不是错误信息，则返回NULL
		/// </summary>
		/// <param name="responseText"></param>
		/// <returns></returns>
		public static QQLoginConnectionException FromResponseText(string responseText)
		{
			string json = QQLoginConnectionManager.GetResponseJsonString(responseText);

			QQLoginConnectionException result = null;

			if (json.IsNotEmpty())
			{
				string error = null;
				string description = null;
				string message = null;

				Dictionary<string, object> data = JSONSerializerExecute.Deserialize<Dictionary<string, object>>(json);

				if (data.ContainsKey("error"))
				{
					error = data.GetValue("error", string.Empty);
					description = data.GetValue("error_description", string.Empty);

					message = string.Format("Error: {0}, Description: {1}", error, description);
				}
				else
				{
					error = data.GetValue("ret", 0).ToString();

					if (error != "0")
					{
						error = data.GetValue("ret", 0).ToString();
						description = data.GetValue("msg", string.Empty);

						message = string.Format("Error: {0}, Description: {1}", error, description);
					}
				}

				if (message.IsNotEmpty())
				{
					result = new QQLoginConnectionException(message);

					result.ErrorCode = error;
					result.ErrorDescription = description;
				}
			}

			return result;
		}
	}
}
