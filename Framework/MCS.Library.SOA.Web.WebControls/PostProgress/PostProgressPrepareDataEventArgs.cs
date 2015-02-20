using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.WebControls
{
	public delegate void PostProgressPrepareDataHandler(object sender, PostProgressPrepareDataEventArgs eventArgs);

	public class PostProgressPrepareDataEventArgs : System.EventArgs
	{
		private string _SerializedData = string.Empty;
		private IList _DeserializedData = null;
		private string _ClientExtraPostedData = string.Empty;

		public string ClientExtraPostedData
		{
			get
			{
				return this._ClientExtraPostedData;
			}
			internal set
			{
				this._ClientExtraPostedData = value;
			}
		}

		public string SerializedData
		{
			get
			{
				return _SerializedData;
			}
			internal set
			{
				this._SerializedData = value;
			}
		}

		public IList DeserializedData
		{
			get
			{
				return this._DeserializedData;
			}
			internal set
			{
				this._DeserializedData = value;
			}
		}
	}
}
