using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;
using System.IO;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Archive
{
	public class MaterialArchiveOperation : IArchiveOperation
	{
		public static readonly MaterialArchiveOperation Instance = new MaterialArchiveOperation();

		private MaterialArchiveOperation()
		{
		}

		#region IArchiveOperation Members

		public void LoadOriginalData(ArchiveBasicInfo info)
		{
			MaterialList materials = MaterialAdapter.Instance.LoadMaterialsByResourceID(info.ResourceID);

			info.Context["Materials"] = materials;

			materials.ForEach(m => m.EnsureMaterialContent());
		}

		public void SaveArchiveData(ArchiveBasicInfo info)
		{
			ArchiveSettings settings = ArchiveSettings.GetConfig();

			ORMappingItemCollection mappings = ORMapping.GetMappingInfo<Material>().Clone();

			mappings["CREATE_DATETIME"].BindingFlags |= (ClauseBindingFlags.Update | ClauseBindingFlags.Insert);
			ORMappingContextCache.Instance[typeof(Material)] = mappings;

			try
			{
				MaterialAdapter.Instance.DeleteMaterialsByResourceID(info.ResourceID);
				MaterialContentAdapter.Instance.Delete(b => b.AppendItem("RELATIVE_ID", info.ResourceID));

				info.Context.DoAction<MaterialList>("Materials", materials =>
				{
					DeltaMaterialList dml = new DeltaMaterialList();

					materials.ForEach(m =>
					{
						dml.Inserted.Add(m);
					});

					MaterialAdapter.Instance.SaveDeltaMaterials(dml, false);

					dml.Inserted.ForEach(m =>
					{
						if (m.Content != null)
							MaterialContentAdapter.Instance.Update(m.Content);
					});
				});
			}
			finally
			{
				ORMappingContextCache.Instance.Remove(typeof(Material));
			}
		}

		public void DeleteOriginalData(ArchiveBasicInfo info)
		{
			ArchiveSettings settings = ArchiveSettings.GetConfig();

			info.Context.DoAction<MaterialList>("Materials", materials =>
			{
				MaterialAdapter.Instance.DeleteMaterialsByResourceID(info.ResourceID);
				MaterialContentAdapter.Instance.Delete(b => b.AppendItem("RELATIVE_ID", info.ResourceID));
			});
		}
		#endregion
	}
}
