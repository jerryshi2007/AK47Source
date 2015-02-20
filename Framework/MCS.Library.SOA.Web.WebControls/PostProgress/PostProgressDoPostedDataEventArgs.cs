using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.WebControls
{
	public delegate void PostProgressDoPostedDataEventHandler(object sender, PostProgressDoPostedDataEventArgs eventArgs);

	public class PostProgressDoPostedDataEventArgs : System.EventArgs
	{
		private UploadProgressResult _Result = null;
		private IList _Steps = null;
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

		public IList Steps
		{
			get
			{
				return this._Steps;
			}
			internal set
			{
				this._Steps = value;
			}
		}

		public UploadProgressResult Result
		{
			get
			{
				return this._Result;
			}
			internal set
			{
				this._Result = value;
			}
		}
	}
}
