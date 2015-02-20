using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Passport.Social.DataObjects;
using MCS.Library.Passport.Social.Mechanism;
using MCS.Library.Services;
using MCS.Web.Library.Script;

namespace MCS.Library.Passport.Social.Executors
{
	public class QQGetOpenIDExecutor : HttpWebRequestExecutorBase
	{
		public QQGetOpenIDExecutor()
		{
		}

		public QQGetOpenIDExecutor(QQGetOpenIDRequestParams requestParams)
		{
			requestParams.NullCheck("requestParams");

			this.RequestParams = requestParams;
		}

		public QQGetOpenIDRequestParams RequestParams
		{
			get;
			set;
		}

		public QQGetOpenIDResponseParams ResponseParams
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

			QQGetOpenIDResponseParams result = new QQGetOpenIDResponseParams();

			result.ClientID = data.GetValue("client_id", string.Empty);
			result.OpenID = data.GetValue("openid", string.Empty);

			this.ResponseParams = result;
		}
	}
}
