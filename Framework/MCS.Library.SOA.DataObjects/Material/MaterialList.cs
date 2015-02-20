#region
// -------------------------------------------------
// Assembly	��	HB.DataEntities
// FileName	��	MaterialList.cs
// Remark	��	Material������
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			����		20070724		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Properties;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// Material������
	/// </summary>
	[Serializable]
	public class MaterialList : EditableDataObjectCollectionBase<Material>
	{
		/// <summary>
		/// ���ɸ���
		/// </summary>
		/// <returns>���ɵĸ���</returns>
		internal MaterialList GenerateCopyVersion()
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(this == null, "this");

			MaterialList materialList = new MaterialList();

			foreach (Material material in this)
				materialList.Add(material.GenerateCopyVersion());

			return materialList;
		}

		/// <summary>
		/// �����ļ��汾
		/// </summary>
		/// <param name="resourceID">�±�ID</param>
		/// <param name="wfProcessID">�¹�����ID</param>
		/// <param name="wfActivityID">�¹������ڵ�ID</param>
		/// <param name="wfActivityName">�¹������ڵ�����</param>
		/// <param name="department">�����ĸ������ڵĲ���</param>
		/// <returns>���ɵĸ���</returns>
		public MaterialList GenerateOtherVersion(string resourceID, string wfProcessID, string wfActivityID, string wfActivityName, IOrganization department)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(this == null, "this");
			ExceptionHelper.CheckStringIsNullOrEmpty(resourceID, "resourceID");
			ExceptionHelper.CheckStringIsNullOrEmpty(wfProcessID, "wfProcessID");
			ExceptionHelper.CheckStringIsNullOrEmpty(wfActivityID, "wfActivityID");

			MaterialList materialList = new MaterialList();

			foreach (Material material in this)
				materialList.Add(material.GenerateOtherVersion(resourceID, wfProcessID, wfActivityID, wfActivityName, department));

			return materialList;
		}

		/// <summary>
		/// �����ͷ���
		/// </summary>
		/// <returns></returns>
		public MaterialList GetMaterialsByClass(string className)
		{
			MaterialList materials = new MaterialList();

			foreach (Material m in this)
			{
				if (m.MaterialClass == className)
					materials.Add(m);
			}

			return materials;
		}

		/// <summary>
		/// �����µ�list
		/// </summary>
		/// <param name="materials">Ҫ���ӵ�MaterialList</param>
		public void Append(MaterialList materials)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(materials == null, "materials");

			if (materials.Count != 0)
			{
				foreach (Material material in materials)
					this.Add(material);
			}
		}

		/// <summary>
		/// ���仯�ĵĸ����ϲ���MaterialList��
		/// </summary>
		/// <param name="delta"></param>
		public void Merge(DeltaMaterialList delta)
		{
			delta.NullCheck("delta");

			MaterialList copiedMaterialed = new MaterialList();

			copiedMaterialed.CopyFrom(this);

			copiedMaterialed.RemoveExistsMaterials(delta.Inserted);
			copiedMaterialed.RemoveExistsMaterials(delta.Updated);
			copiedMaterialed.RemoveExistsMaterials(delta.Deleted);
			copiedMaterialed.CopyFrom(delta.Inserted);
			copiedMaterialed.CopyFrom(delta.Updated);

			copiedMaterialed.Sort((m1, m2) => m1.SortID - m2.SortID);

			this.Clear();
			this.CopyFrom(copiedMaterialed);
		}

		#region Merge���˽�з���
		/// <summary>
		/// ɾ����compareList���Ѿ����ڵ�Material
		/// </summary>
		/// <param name="compareList"></param>
		private void RemoveExistsMaterials(MaterialList compareList)
		{
			foreach (Material cm in compareList)
			{
				this.Remove(m => string.Compare(m.ID, cm.ID, true) == 0);
			}
		}

		#endregion Merge���˽�з���

		/// <summary>
		/// ��ʼ��������׼��MaterialContent
		/// </summary>
		public void EnsureMaterialContent()
		{
			this.ForEach(m => m.EnsureMaterialContent());
		}

		/// <summary>
		/// ����һ�������б�
		/// </summary>
		/// <returns></returns>
		public MaterialList Clone()
		{
			MaterialList result = new MaterialList();

			this.ForEach(m => result.Add(m.Clone()));

			return result;
		}

		/// <summary>
		/// �õ��ļ�������·��
		/// </summary>
		/// <param name="rootPathName"></param>
		public void GenerateTempPhysicalFilePath(string rootPathName)
		{
			this.ForEach(m => m.GenerateTempPhysicalFilePath(rootPathName));
		}

		#region ���캯��

		public MaterialList()
		{

		}

		#endregion

		#region ���ϲ���

		public void Insert(int index, Material material)
		{
			List.Insert(index, material);
		}

        //public void Remove(Material material)
        //{
        //    List.Remove(material);
        //}

		#endregion

		#region ���ػ���ķ���

		protected override void OnValidate(object value)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");
			ExceptionHelper.FalseThrow(
				value.GetType() == typeof(Material),
				string.Format(Resource.CollectionValidateError, typeof(Material)));

			ExceptionHelper.CheckStringIsNullOrEmpty(((Material)value).ID, "ID");
			ExceptionHelper.CheckStringIsNullOrEmpty(((Material)value).Title, "Title");
			ExceptionHelper.CheckStringIsNullOrEmpty(((Material)value).RelativeFilePath, "RelativeFilePath");
		}

		#endregion

	}

}