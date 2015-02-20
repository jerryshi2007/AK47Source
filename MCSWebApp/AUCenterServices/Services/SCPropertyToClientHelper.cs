using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.Core;

namespace AUCenterServices.Services
{
	/// <summary>
	/// 权限中心的属性转换成客户端的属性
	/// </summary>
	public static class SCPropertyToClientHelper
	{
		public static ClientPropertyDefine ToClientPropertyDefine(this SchemaPropertyDefine pcpd)
		{
			pcpd.NullCheck("pcpd");

			return new ClientPropertyDefine()
			{
				AllowOverride = pcpd.AllowOverride,
				Category = pcpd.Category,
				DataType = (ClientPropertyDataType)pcpd.DataType,
				DefaultValue = pcpd.DefaultValue,
				Description = pcpd.Description,
				DisplayName = pcpd.DisplayName,
				EditorKey = pcpd.EditorKey,
				EditorParams = pcpd.EditorParams,
				IsRequired = pcpd.IsRequired,
				MaxLength = pcpd.MaxLength,
				Name = pcpd.Name,
				PersisterKey = pcpd.PersisterKey,
				ReadOnly = pcpd.ReadOnly,
				SortOrder = pcpd.SortOrder,
				Visible = pcpd.Visible,
			};
		}

		public static ClientPropertyValue ToClientPropertyValue(this SchemaPropertyValue pcpv)
		{
			pcpv.NullCheck("pcpv");
			pcpv.Definition.NullCheck("pcpv.Definition");

			return new ClientPropertyValue(pcpv.Definition.Name) { DataType = (ClientPropertyDataType)pcpv.Definition.DataType, StringValue = pcpv.StringValue };
		}

		public static void CopyTo(this SchemaPropertyValueCollection pcProperties, ClientPropertyValueCollection clientProperties)
		{
			pcProperties.NullCheck("pcProperties");

			pcProperties.ForEach(pcpv => clientProperties.Add(pcpv.ToClientPropertyValue()));
		}

		public static void CopyFrom(this SchemaPropertyValueCollection pcProperties, ClientPropertyValueCollection clientProperties)
		{
			pcProperties.NullCheck("pcProperties");

			foreach (var ppt in clientProperties)
			{
				if (pcProperties.ContainsKey(ppt.Key)) // 不识别的被丢掉
					pcProperties[ppt.Key].StringValue = ppt.StringValue;
			}
		}
	}
}