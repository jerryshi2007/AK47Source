using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using System.Xml.Serialization;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects.Security.AUClient
{
	public class ClientNamedObject : ClientGenericObject
	{
		public ClientNamedObject()
			: base()
		{
		}

		public ClientNamedObject(string schemaType)
			: base(schemaType)
		{
		}

		/// <summary>
		/// 获取或设置名称
		/// </summary>
		[XmlIgnore]
		[ScriptIgnore]
		public string Name
		{
			get
			{
				return this.Properties.GetValue("Name", string.Empty);
			}

			set
			{
				this.Properties.AddOrSetValue("Name", ClientPropertyDataType.String, value ?? string.Empty);
			}
		}

		/// <summary>
		/// 获取或设置显示名称
		/// </summary>
		[XmlIgnore]
		[ScriptIgnore]
		public string DisplayName
		{
			get
			{
				return this.Properties.GetValue("DisplayName", string.Empty);
			}

			set
			{
				this.Properties.AddOrSetValue("DisplayName", ClientPropertyDataType.String, value ?? string.Empty);
			}
		}

		/// <summary>
		/// 获取或设置代码名称
		/// </summary>
		[XmlIgnore]
		[ScriptIgnore]
		public string CodeName
		{
			get
			{
				return this.Properties.GetValue("CodeName", string.Empty); ;
			}

			set
			{
				this.Properties.AddOrSetValue("CodeName", ClientPropertyDataType.String, value ?? string.Empty);
			}
		}
	}
}
