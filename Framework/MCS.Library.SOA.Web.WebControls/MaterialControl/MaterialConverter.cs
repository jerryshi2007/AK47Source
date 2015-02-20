#region
// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	MaterialConverter.cs
// Remark	��	
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			����		20070731		����
// -------------------------------------------------
#endregion
using System;
using System.IO;
using System.Collections;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.Script.Serialization;

using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// Material�������л�
	/// </summary>
	internal class MaterialConverter : JavaScriptConverter
	{
		private const string DisplayName = "DisplayName";

		/// <summary>
		///�����л�material
		/// </summary>
		/// <param name="dictionary">��������</param>
		/// <param name="type">��������</param>
		/// <param name="serializer">JS���л���</param>
		/// <returns>�����л����Ķ���</returns>
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			Material material = new Material();

			object id = string.Empty;
			if (dictionary.TryGetValue("id", out id))
				material.ID = id.ToString();

			//object oguOrganization;

			//if (dictionary.TryGetValue("department", out oguOrganization))
			//    material.Department = JSONSerializerExecute.Deserialize<OguOrganization>(oguOrganization);

			object resourceID = string.Empty;
			if (dictionary.TryGetValue("resourceID", out resourceID))
				material.ResourceID = (resourceID == null ? string.Empty : resourceID.ToString());

			object sortID = 0;
			if (dictionary.TryGetValue("sortID", out sortID))
				material.SortID = int.Parse(sortID.ToString());

			object materialClass = string.Empty;
			if (dictionary.TryGetValue("materialClass", out materialClass))
				material.MaterialClass = (materialClass == null ? string.Empty : materialClass.ToString());

			object title = string.Empty;
			if (dictionary.TryGetValue("title", out title))
				material.Title = title.ToString();

			object pageQuantity = string.Empty;
			if (dictionary.TryGetValue("pageQuantity", out pageQuantity))
				material.PageQuantity = int.Parse(pageQuantity.ToString());

			object relativeFilePath = string.Empty;
			if (dictionary.TryGetValue("relativeFilePath", out relativeFilePath))
				material.RelativeFilePath = relativeFilePath.ToString();

			object originalName = string.Empty;
			if (dictionary.TryGetValue("originalName", out originalName))
				material.OriginalName = originalName.ToString();

			//object oguUser;

			//if (dictionary.TryGetValue("creator", out oguUser))
			//    material.Creator = JSONSerializerExecute.Deserialize<OguUser>(oguUser);

			object lastUploadTag = string.Empty;
			if (dictionary.TryGetValue("lastUploadTag", out lastUploadTag))
				material.LastUploadTag = lastUploadTag != null ? lastUploadTag.ToString() : string.Empty;

			object createDateTime;
			if (dictionary.TryGetValue("createDateTime", out createDateTime))
				material.CreateDateTime = createDateTime == null ? DateTime.Now : DateTime.Parse(createDateTime.ToString());

			object modifyTime;
			if (dictionary.TryGetValue("modifyTime", out modifyTime))
				material.ModifyTime = (DateTime)modifyTime;
			//material.ModifyTime = modifyTime == null ? DateTime.Now.ToLocalTime() : DateTime.Parse(modifyTime.ToString()).ToLocalTime();

			object wfProcessID = string.Empty;
			if (dictionary.TryGetValue("wfProcessID", out wfProcessID))
				material.WfProcessID = (wfProcessID == null ? string.Empty : wfProcessID.ToString());

			object wfActivityID = string.Empty;
			if (dictionary.TryGetValue("wfActivityID", out wfActivityID))
				material.WfActivityID = (wfActivityID == null ? string.Empty : wfActivityID.ToString());

			object wfActivityName = string.Empty;
			if (dictionary.TryGetValue("wfActivityName", out wfActivityName))
				material.WfActivityName = (wfActivityName == null ? string.Empty : wfActivityName.ToString());

			object parentID = string.Empty;
			if (dictionary.TryGetValue("parentID", out parentID))
				material.ParentID = (parentID == null ? string.Empty : parentID.ToString());

			object versionType = string.Empty;
			if (dictionary.TryGetValue("versionType", out versionType))
				material.VersionType = (MaterialVersionType)Convert.ToInt32(versionType.ToString());

			object extraData = null;
			if (dictionary.TryGetValue("extraData", out extraData))
			{
				if (extraData is Dictionary<string, Object>)
				{
					material.ExtraDataDictionary.Clear();
					foreach (var item in (Dictionary<string, Object>)extraData)
					{
						material.ExtraDataDictionary.Add(item.Key, item.Value.ToString());
					}
				}
			}

			object showFileUrl = string.Empty;
			if (dictionary.TryGetValue("showFileUrl", out showFileUrl))
				material.ShowFileUrl = showFileUrl.ToString();

			return material;
		}

		//private  IOrganization JSONSerializerExecute(OguOrganization OguOrganization)
		//{
		//    throw new Exception("The method or operation is not implemented.");
		//}

		/// <summary>
		/// ���л�material
		/// </summary>
		/// <param name="obj">material����</param>
		/// <param name="serializer">���л���</param>
		/// <returns>���Լ���</returns>
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			Material material = (Material)obj;

			dictionary.Add("id", material.ID);
			//dictionary.Add("department", JSONSerializerExecute.Serialize(material.Department));
			dictionary.Add("resourceID", material.ResourceID);
			dictionary.Add("sortID", material.SortID);
			dictionary.Add("materialClass", material.MaterialClass);
			dictionary.Add("title", material.Title);
			dictionary.Add("pageQuantity", material.PageQuantity);
			dictionary.Add("relativeFilePath", material.RelativeFilePath);
			dictionary.Add("originalName", material.OriginalName);

			//OguUser creator = (OguUser)material.Creator;
			//creator.DisplayName = DisplayName;

			//dictionary.Add("creator", JSONSerializerExecute.Serialize(creator));
			dictionary.Add("lastUploadTag", material.LastUploadTag);
			dictionary.Add("createDateTime", material.CreateDateTime);
			dictionary.Add("modifyTime", material.ModifyTime);
			dictionary.Add("wfProcessID", material.WfProcessID);
			dictionary.Add("wfActivityID", material.WfActivityID);
			dictionary.Add("wfActivityName", material.WfActivityName);
			dictionary.Add("parentID", material.ParentID);
			dictionary.Add("sourceMaterial", material.SourceMaterial);
			dictionary.Add("versionType", material.VersionType);
			dictionary.Add("extraData", material.ExtraDataDictionary);
			dictionary.Add("fileIconPath", FileConfigHelper.GetFileIconPath(material.OriginalName));
			dictionary.Add("showFileUrl", material.ShowFileUrl);
			dictionary.Add("selected", false);

			return dictionary;
		}

		/// <summary>
		/// ��ȡ��Converter֧�ֵ����
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				List<Type> types = new List<Type>();
				types.Add(typeof(Material));

				return types;
			}
		}
	}

	/// <summary>
	/// DeltaMaterial�������л�
	/// </summary>
	internal class DeltaMaterialConverter : JavaScriptConverter
	{
		/// <summary>
		///�����л�DeltaMaterial
		/// </summary>
		/// <param name="dictionary">��������</param>
		/// <param name="type">��������</param>
		/// <param name="serializer">JS���л���</param>
		/// <returns>�����л����Ķ���</returns>
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			DeltaMaterialList deltaMaterialList = new DeltaMaterialList();

			object insertedMaterials;

			if (dictionary.TryGetValue("insertedMaterials", out insertedMaterials))
			{
				ArrayList materials = (ArrayList)insertedMaterials;
				for (int i = 0; i < materials.Count; i++)
					deltaMaterialList.Inserted.Add(JSONSerializerExecute.Deserialize<Material>(materials[i]));
			}

			object updatedMaterials;

			if (dictionary.TryGetValue("updatedMaterials", out updatedMaterials))
			{
				ArrayList materials = (ArrayList)updatedMaterials;
				for (int i = 0; i < materials.Count; i++)
					deltaMaterialList.Updated.Add(JSONSerializerExecute.Deserialize<Material>(materials[i]));
			}

			object deletedMaterials;

			if (dictionary.TryGetValue("deletedMaterials", out deletedMaterials))
			{
				ArrayList materials = (ArrayList)deletedMaterials;
				for (int i = 0; i < materials.Count; i++)
					deltaMaterialList.Deleted.Add(JSONSerializerExecute.Deserialize<Material>(materials[i]));
			}

			deltaMaterialList.RootPathName = (string)dictionary["rootPathName"];

			return deltaMaterialList;
		}

		/// <summary>
		/// ���л�DeltaMaterial
		/// </summary>
		/// <param name="obj">DeltaMaterial����</param>
		/// <param name="serializer">���л���</param>
		/// <returns>���Լ���</returns>
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			DeltaMaterialList deltaMaterialList = (DeltaMaterialList)obj;

			Material[] insertedMaterials = new Material[deltaMaterialList.Inserted.Count];

			for (int i = 0; i < insertedMaterials.Length; i++)
				insertedMaterials[i] = deltaMaterialList.Inserted[i];

			dictionary.Add("insertedMaterials", insertedMaterials);

			Material[] updatedMaterials = new Material[deltaMaterialList.Updated.Count];

			for (int i = 0; i < updatedMaterials.Length; i++)
				updatedMaterials[i] = deltaMaterialList.Updated[i];

			dictionary.Add("updatedMaterials", updatedMaterials);

			Material[] deletedMaterials = new Material[deltaMaterialList.Deleted.Count];

			for (int i = 0; i < deletedMaterials.Length; i++)
				deletedMaterials[i] = deltaMaterialList.Deleted[i];

			dictionary.Add("deletedMaterials", deletedMaterials);

			dictionary.Add("rootPathName", deltaMaterialList.RootPathName);

			return dictionary;
		}

		/// <summary>
		/// ��ȡ��Converter֧�ֵ����
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				List<Type> types = new List<Type>();
				types.Add(typeof(DeltaMaterialList));

				return types;
			}
		}
	}

	/// <summary>
	/// ����MaterialList��DeltaMaterialList�ļ���
	/// </summary>
	internal class MultiMaterialConverter : JavaScriptConverter
	{
		/// <summary>
		///�����л�MultiMaterialList
		/// </summary>
		/// <param name="dictionary">��������</param>
		/// <param name="type">��������</param>
		/// <param name="serializer">JS���л���</param>
		/// <returns>�����л����Ķ���</returns>
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			MultiMaterialList multiMaterialList = new MultiMaterialList();

			object materialList;

			if (dictionary.TryGetValue("materials", out materialList))
			{
				ArrayList materials = (ArrayList)materialList;

				for (int i = 0; i < materials.Count; i++)
					multiMaterialList.Materials.Add(JSONSerializerExecute.Deserialize<Material>(materials[i]));
			}

			object deltaMaterialList;

			if (dictionary.TryGetValue("deltaMaterials", out deltaMaterialList))
				multiMaterialList.DeltaMaterials = JSONSerializerExecute.Deserialize<DeltaMaterialList>(deltaMaterialList);

			return multiMaterialList;
		}

		/// <summary>
		/// ���л�MultiMaterial
		/// </summary>
		/// <param name="obj">MultiMaterial����</param>
		/// <param name="serializer">���л���</param>
		/// <returns>���Լ���</returns>
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			MultiMaterialList multiMaterialList = (MultiMaterialList)obj;

			Material[] materials = new Material[multiMaterialList.Materials.Count];

			for (int i = 0; i < materials.Length; i++)
				materials[i] = multiMaterialList.Materials[i];

			dictionary.Add("materials", materials);

			dictionary.Add("deltaMaterials", multiMaterialList.DeltaMaterials);

			return dictionary;
		}

		/// <summary>
		/// ��ȡ��Converter֧�ֵ����
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				List<Type> types = new List<Type>();
				types.Add(typeof(MultiMaterialList));

				return types;
			}
		}
	}

	/// <summary>
	/// ����DeltaMaterialList��MaterialList�ļ���
	/// </summary>
	internal class MultiMaterialList
	{
		private DeltaMaterialList deltaMaterials = new DeltaMaterialList();
		private MaterialList materials = new MaterialList();

		/// <summary>
		/// �仯��
		/// </summary>
		public DeltaMaterialList DeltaMaterials
		{
			get
			{
				return this.deltaMaterials;
			}
			set
			{
				this.deltaMaterials = value;
			}
		}

		/// <summary>
		/// ��������
		/// </summary>
		public MaterialList Materials
		{
			get
			{
				return this.materials;
			}
			set
			{
				this.materials = value;
			}
		}

	}

	/// <summary>
	/// OguUser�������л������л�
	/// </summary>
	internal class MaterialOguUserConverter : JavaScriptConverter
	{
		/// <summary>
		///�����л�OguUser
		/// </summary>
		/// <param name="dictionary">��������</param>
		/// <param name="type">��������</param>
		/// <param name="serializer">JS���л���</param>
		/// <returns>�����л����Ķ���</returns>
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			object id, logOnName, displayName;
			OguUser oguUser = null;

			if (dictionary.TryGetValue("id", out id))
			{
				oguUser = new OguUser((string)dictionary["id"]);

				if (dictionary.TryGetValue("logOnName", out logOnName))
					oguUser.LogOnName = (string)logOnName;

				if (dictionary.TryGetValue("displayName", out displayName))
					oguUser.DisplayName = (string)displayName;
			}

			return oguUser;
		}

		/// <summary>
		/// ���л�OguOrganization
		/// </summary>
		/// <param name="obj">material����</param>
		/// <param name="serializer">���л���</param>
		/// <returns>���Լ���</returns>
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			OguUser oguUser = (OguUser)obj;

			if (string.IsNullOrEmpty(oguUser.ID) == false)
			{
				dictionary.Add("id", oguUser.ID);
				dictionary.Add("logOnName", oguUser.LogOnName);
				dictionary.Add("displayName", oguUser.DisplayName);
			}

			return dictionary;
		}

		/// <summary>
		/// ��ȡ��Converter֧�ֵ����
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				List<Type> types = new List<Type>();
				types.Add(typeof(OguUser));

				return types;
			}
		}
	}

	/// <summary>
	/// OguOrganization�������л������л�
	/// </summary>
	internal class MaterialOguOrganizationConverter : JavaScriptConverter
	{
		/// <summary>
		///�����л�OguOrganization
		/// </summary>
		/// <param name="dictionary">��������</param>
		/// <param name="type">��������</param>
		/// <param name="serializer">JS���л���</param>
		/// <returns>�����л����Ķ���</returns>
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			OguOrganization oguOrganization = null;
			object id;

			if (dictionary.TryGetValue("id", out id))
			{
				oguOrganization = new OguOrganization((string)dictionary["id"]);
				oguOrganization.Name = (string)dictionary["name"];
				oguOrganization.GlobalSortID = (string)dictionary["globalSortID"];
			}

			return oguOrganization;
		}

		/// <summary>
		/// ���л�OguOrganization
		/// </summary>
		/// <param name="obj">material����</param>
		/// <param name="serializer">���л���</param>
		/// <returns>���Լ���</returns>
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			OguOrganization oguOrganization = (OguOrganization)obj;

			if (string.IsNullOrEmpty(oguOrganization.ID) == false)
			{
				dictionary.Add("id", oguOrganization.ID);
				dictionary.Add("name", oguOrganization.Name);
				dictionary.Add("globalSortID", oguOrganization.GlobalSortID);
			}

			return dictionary;
		}

		/// <summary>
		/// ��ȡ��Converter֧�ֵ����
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				List<Type> types = new List<Type>();
				types.Add(typeof(OguOrganization));

				return types;
			}
		}
	}

}
