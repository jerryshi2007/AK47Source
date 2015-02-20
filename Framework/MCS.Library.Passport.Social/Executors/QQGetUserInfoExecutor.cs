using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Services;
using MCS.Library.Passport.Social.DataObjects;
using System.Net;
using MCS.Library.Passport.Social.Mechanism;
using MCS.Web.Library.Script;

namespace MCS.Library.Passport.Social.Executors
{
	public class QQGetUserInfoExecutor : HttpWebRequestExecutorBase
	{
		public QQGetUserInfoExecutor()
		{
		}

		public QQGetUserInfoExecutor(QQGetUserInfoRequestParams requestParams)
		{
			requestParams.NullCheck("requestParams");

			this.RequestParams = requestParams;
		}

		public QQGetUserInfoRequestParams RequestParams
		{
			get;
			set;
		}

		public QQGetUserInfoResponseParams ResponseParams
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

			string json = QQLoginConnectionManager.GetResponseJsonString(responseText);

			if (json.IsNullOrEmpty())
				throw new ApplicationException(responseText);

			Dictionary<string, object> data = JSONSerializerExecute.Deserialize<Dictionary<string, object>>(json);

			QQGetUserInfoResponseParams response = new QQGetUserInfoResponseParams(this.RequestParams.OpenID);

			response.FromDictionary(data);

			this.ResponseParams = response;
		}
	}
}
