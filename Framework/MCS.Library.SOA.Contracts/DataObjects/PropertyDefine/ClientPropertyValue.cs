using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.Contracts;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects
{
    [Serializable]
    [DataContract(IsReference = true,Name="CPV",Namespace="WF")]
	public class ClientPropertyValue
	{
		public ClientPropertyValue()
		{
		}

		public ClientPropertyValue(ClientPropertyDefine def)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(def != null, "def");

			this._Definition = def;
		}

		private string _StringValue = null;

		[DataMember]
		public string StringValue
		{
			get
			{
				string result = this._StringValue;

				if (result == null)
					result = this.Definition.DefaultValue;

				return result;
			}
			set
			{
				this._StringValue = value;
			}
		}

		private ClientPropertyDefine _Definition = null;

		[DataMember]
		public ClientPropertyDefine Definition
		{
			get { return _Definition; }
			set { _Definition = value; }
		}

		public ClientPropertyValue Clone()
		{
			ClientPropertyValue newValue = new ClientPropertyValue(this._Definition);

			newValue._StringValue = this._StringValue;

			return newValue;
		}
	}
}
