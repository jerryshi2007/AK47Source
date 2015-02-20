using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;

namespace SyncMaterialFileToDocCenterService
{
	public static class MaterialHelper
	{
		public static string ShowUrlKey = "ShowUrl";

		public static Material LoadMaterial(string materialId, string connectionName)
		{
			materialId.CheckStringIsNullOrEmpty("materialId");
			connectionName.CheckStringIsNullOrEmpty("connectionName");

			Material result = null;

			DbConnectionMappingContext.DoMappingAction(
				MaterialContentAdapter.Instance.ConnectionName,
				connectionName,
				() =>
				{
					result = MaterialAdapter.Instance.LoadMaterialByMaterialID(materialId).FirstOrDefault();
				});

			return result;
		}

		public static MaterialContent LoadMaterialContent(string materialId, string connectionName)
		{
			materialId.CheckStringIsNullOrEmpty("materialId");
			connectionName.CheckStringIsNullOrEmpty("connectionName");

			MaterialContent result = null;

			DbConnectionMappingContext.DoMappingAction(
				MaterialContentAdapter.Instance.ConnectionName,
				connectionName,
				() =>
				{
					result = MaterialContentAdapter.Instance.Load(p => p.AppendItem("CONTENT_ID", materialId)).FirstOrDefault();
				});

			return result;
		}
	}
}