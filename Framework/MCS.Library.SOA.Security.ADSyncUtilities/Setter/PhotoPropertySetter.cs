using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class PhotoPropertySetter : SimplePropertySetter
	{
		protected override void SetPropertyValue(OGUPermission.IOguObject srcOguObject, string srcPropertyName, System.DirectoryServices.DirectoryEntry entry, string targetPropertyName, string context, DataObjects.Security.Transfer.SetterContext setterContext)
		{
			// 借用Pager属性，属性值格式： 
			// 时间的内部格式十六进制|MaterialContent的ID，如果为空白，则表示无图片
			string srcPropertyValue = GetNormalizeddSourceValue(srcOguObject, srcPropertyName, context);
			string targetPropertyValue = GetNormalizeddTargetValue(entry, targetPropertyName, context);

			if (srcPropertyValue != targetPropertyValue)
			{
				entry.Properties[targetPropertyName].Value = srcPropertyValue;

				int ind = string.IsNullOrEmpty(srcPropertyValue) ? -1 : srcPropertyValue.IndexOf('|');
				if (ind > 0)
				{
					string hexPart = srcPropertyValue.Substring(0, ind);
					string keyPart = srcPropertyValue.Substring(ind + 1);
					var imageData = MCS.Library.SOA.DataObjects.MaterialContentAdapter.Instance.Load(w => w.AppendItem("CONTENT_ID", keyPart));
					if (imageData.Count != 1)
						throw new InvalidDataException(string.Format("未能根据ID{0}找到图像数据", keyPart));

					entry.Properties["thumbnailPhoto"].Value = imageData[0].ContentData;
				}
				else
				{
					entry.Properties["thumbnailPhoto"].Value = null;
				}
			}
		}
	}
}
