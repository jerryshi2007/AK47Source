using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects
{
	
	[DataContract(IsReference=true)]
	public class ClientPropertyDefine
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public string Category { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string DefaultValue { get; set; }

		[DataMember]
		public bool ReadOnly { get; set; }

		[DataMember]
		public string EditorKey { get; set; }
		
		private ClientPropertyDataType _DataType = ClientPropertyDataType.String;

		private bool _Visible = true;
		private bool _AllowOverride = true;

		public ClientPropertyDefine()
		{
		}

		[DataMember]
		public bool Visible 
		{
			get
			{
				return this._Visible;
			}
			set
			{
				this._Visible = value;
			}
		}

		/// <summary>
		/// 属性合并的时候是否允许覆盖之前的属性
		/// </summary>
		[DataMember]
		public bool AllowOverride
		{
			get
			{
				return this._AllowOverride;
			}
			set
			{
				this._AllowOverride = value;
			}
		}

		[DataMember]
		public ClientPropertyDataType DataType
		{
			get { return _DataType; }
			set { _DataType = value; }
		}
	}
}
