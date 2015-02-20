#region
// -------------------------------------------------
// Assembly	：	HB.DataEntities
// FileName	：	DeltaMaterialList.cs
// Remark	：	Material操作集合
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20070724		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
    [Serializable]
    public class DeltaMaterialList : DeltaDataCollectionBase<MaterialList>
    {
        private string rootPathName = string.Empty;

        protected override DeltaDataCollectionBase CreateNewInstance()
        {
            return new DeltaMaterialList();
        }

		/// <summary>
		/// 
		/// </summary>
		public void GenerateTempPhysicalFilePath()
		{
			this.Deleted.GenerateTempPhysicalFilePath(this.rootPathName);
			this.Inserted.GenerateTempPhysicalFilePath(this.rootPathName);
			this.Updated.GenerateTempPhysicalFilePath(this.rootPathName);
		}

        /// <summary>
        /// 根据delta数据产生副本的delta数据
        /// </summary>
        /// <returns>产生的副本delta数据</returns>
        public DeltaMaterialList GenerateCopyVersion()
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(this == null, "this");

            DeltaMaterialList deltaData = new DeltaMaterialList();

            #region 新增和删除的文件　直接生成副本

            if (this.Inserted != null)
                deltaData.Inserted.Append(this.Inserted.GenerateCopyVersion());

            if (this.Deleted != null)
                deltaData.Deleted.Append(this.Deleted.GenerateCopyVersion());

            #endregion

            #region 更新的文件，检查是否已存在该环节的副本

            if (this.Updated != null && this.Updated.Count != 0)
            {
                foreach (Material material in this.Updated)
                {
                    deltaData.Updated.Add(material.GenerateCopyVersion());
                }
            }

            #endregion

            return deltaData;
        }

        /// <summary>
        /// 文件上传路径配置名
        /// </summary>
        public string RootPathName
        {
            get
            {
                return this.rootPathName;
            }
            set
            {
                this.rootPathName = value;
            }
        }

        public DeltaMaterialList()
        {
        }

        /// <summary>
        /// 转化为修改对象的比较结果
        /// </summary>
        /// <returns>比较结果</returns>
        public IModifyResult ConvertToModifyResult()
        {
            MaterialModifyResult materialModifyResult = new MaterialModifyResult();

            foreach (Material m in this.Inserted)
                materialModifyResult.Inserted.Add(m.ConvertToMaterialModifyObject());

            foreach (Material m in this.Deleted)
                materialModifyResult.Deleted.Add(m.ConvertToMaterialModifyObject());

            foreach (Material m in this.Updated)
                materialModifyResult.Updated.Add(m.ConvertToMaterialModifyObject());

            return materialModifyResult;
        }

        public override bool IsEmpty()
        {
            return this.Inserted.Count == 0 && this.Deleted.Count == 0 && this.Updated.Count == 0;
        }
    }
}