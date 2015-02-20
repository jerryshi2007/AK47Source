using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Services;
using System.Net;
using MCS.Library.Passport.Social.DataObjects;
using MCS.Library.Passport.Social.Mechanism;

namespace MCS.Library.Passport.Social.Executors
{
	/// <summary>
	/// 处理获取AccessToken请求的执行器
	/// </summary>
	public class QQGetAccessTokenExecutor : HttpWebRequestExecutorBase
	{
		public QQGetAccessTokenExecutor()
		{
		}

		public QQGetAccessTokenExecutor(QQAccessTokenRequestParams requestParams)
		{
			requestParams.NullCheck("requestParams");

			this.RequestParams = requestParams;
		}

		public QQAccessTokenRequestParams RequestParams
		{
			get;
			set;
		}

		public QQAccessTokenResponseParams ResponseParams
		{
			get;
			set;
		}

		protected override HttpWebRequest PrepareWebRequest()
		{
			this.RequestParams.NullCheck("RequestParams");

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.RequestParams.ToUrl());

			request.Method = "GET";

			return request;
		}

		protected override void ProcessResponseText(string responseText)
		{
			QQLoginConnectionException.CheckResponseText(responseText);

			this.ResponseParams = new QQAccessTokenResponseParams(responseText);
		}
	}
}
