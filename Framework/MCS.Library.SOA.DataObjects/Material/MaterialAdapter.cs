#region
// -------------------------------------------------
// Assembly	：	HB.DataEntities
// FileName	：	MaterialAdapter.cs
// Remark	：	
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20070724		创建
// -------------------------------------------------
#endregion

using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 附件操作类
    /// </summary>
    public class MaterialAdapter
    {
        internal const string DefaultUploadPathName = "UploadRootPath";

        private static MaterialAdapter instance = new MaterialAdapter();

        private MaterialAdapter()
        {

        }

        public static MaterialAdapter Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 文件操作的委托
        /// </summary>
        /// <param name="m">附件对象</param>
        /// <param name="fileOPList">文件操作的集合</param>
        /// <returns>形成的SQL语句</returns>
        private delegate string GetMaterialSqlOPDelegate(Material m, List<MaterialFileOeprationInfo> fileOPList);

        #region 查询

        /// <summary>
        /// 获得指定附件的副本
        /// </summary>
        /// <param name="materials">附件集合</param>
        /// <returns>指定附件对应的副本集合</returns>
        internal MaterialList LoadCopyVersionMaterial(MaterialList materials)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(materials == null, "materials");

            MaterialList copyVersionMaterials = new MaterialList();

            if (materials.Count != 0)
            {
                ConnectiveSqlClauseCollection orClause = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or);

                foreach (Material material in materials)
                {
                    WhereSqlClauseBuilder whereSqlClause = new WhereSqlClauseBuilder();

                    whereSqlClause.AppendItem("PARENT_ID", material.ID);
                    whereSqlClause.AppendItem("WF_ACTIVITY_ID", material.WfActivityID);
                    whereSqlClause.AppendItem("VERSION_TYPE", (int)MaterialVersionType.CopyVersion);

                    orClause.Add(whereSqlClause);
                }

                string sql = string.Format("SELECT * FROM WF.MATERIAL WHERE {0}",
                    orClause.AppendTenantCodeSqlClause(typeof(Material)).ToSqlString(TSqlBuilder.Instance));

                using (DbContext dbi = DbHelper.GetDBContext(GetConnectionName()))
                {
                    using (IDataReader dr = DbHelper.RunSqlReturnDR(sql, GetConnectionName()))
                    {
                        while (dr.Read())
                        {
                            Material material = new Material();

                            ORMapping.DataReaderToObject(dr, material);

                            copyVersionMaterials.Add(material);
                        }
                    }
                }

                DecorateMaterials(copyVersionMaterials);
            }

            return copyVersionMaterials;
        }

        public MaterialList LoadVersionMaterialsBySceneKey(string resourceID, string wfActivityName)
        {
            MaterialList copyVersionMaterials = new MaterialList();

            WhereSqlClauseBuilder whereSqlClause = new WhereSqlClauseBuilder();

            whereSqlClause.AppendItem("RESOURCE_ID", resourceID);
            whereSqlClause.AppendItem("VERSION_TYPE", Convert.ToInt16(MaterialVersionType.CopyVersion).ToString());
            whereSqlClause.AppendItem("WF_ACTIVITY_NAME", wfActivityName);
            whereSqlClause.AppendTenantCode(typeof(Material));

            string sql = string.Format("SELECT * FROM WF.MATERIAL WHERE {0}",
                                    whereSqlClause.ToSqlString(TSqlBuilder.Instance));

            using (DbContext dbi = DbHelper.GetDBContext(GetConnectionName()))
            {
                using (IDataReader dr = DbHelper.RunSqlReturnDR(sql, GetConnectionName()))
                {
                    while (dr.Read())
                    {
                        Material material = new Material();

                        ORMapping.DataReaderToObject(dr, material);

                        copyVersionMaterials.Add(material);
                    }
                }
            }

            DecorateMaterials(copyVersionMaterials);

            return copyVersionMaterials;
        }

        /// <summary>
        /// 由资源ID查询
        /// </summary>
        /// <param name="resourceIDs">资源ID</param>
        /// <returns>查询结果</returns>
        public MaterialList LoadMaterialsByResourceID(params string[] resourceIDs)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(resourceIDs == null, "resourceIDs");

            MaterialList result = new MaterialList();

            if (resourceIDs.Length != 0)
            {
                InSqlClauseBuilder inBuilder = new InSqlClauseBuilder("RESOURCE_ID");

                inBuilder.AppendItem(resourceIDs);

                OrderBySqlClauseBuilder orderClause = new OrderBySqlClauseBuilder();

                orderClause.AppendItem("SORT_ID", FieldSortDirection.Ascending);

                string sql = string.Format("SELECT * FROM WF.MATERIAL WHERE {0} AND VERSION_TYPE = '{1}' ORDER BY {2}",
                    inBuilder.AppendTenantCodeSqlClause(typeof(Material)).ToSqlString(TSqlBuilder.Instance),
                    Convert.ToInt32(MaterialVersionType.Normal).ToString(),
                    orderClause.ToSqlString(TSqlBuilder.Instance));

                using (DbContext dbi = DbHelper.GetDBContext(GetConnectionName()))
                {
                    using (IDataReader dr = DbHelper.RunSqlReturnDR(sql, GetConnectionName()))
                    {
                        while (dr.Read())
                        {
                            Material material = new Material();

                            ORMapping.DataReaderToObject(dr, material);

                            result.Add(material);
                        }
                    }
                }

                DecorateMaterials(result);
            }

            return result;
        }

        /// <summary>
        /// 由资源ID查询，返回根据MaterialClass分组的MaterialGroupCollection对象
        /// </summary>
        /// <param name="resourceIDs">资源ID</param>
        /// <returns>查询结果</returns>
        public MaterialGroupCollection LoadMaterialClassGroupByResourceID(params string[] resourceIDs)
        {
            MaterialList materials = LoadMaterialsByResourceID(resourceIDs);

            MaterialGroupCollection materialGroups = new MaterialGroupCollection();

            materialGroups.FillGroupByMaterialClass(materials);

            return materialGroups;
        }

        /// <summary>
        /// 获得materialID对应的附件对象的所有版本和本身形成的树
        /// 对应的存储过程GetMaterialVersions不存在
        /// </summary>
        /// <param name="mainMaterialID">主版本ID</param>
        /// <returns>以主版本为跟节点各版本为子节点的树</returns>
        //[Obsolete("对应的存储过程GetMaterialVersions不存在")]
        public MaterialTreeNode LoadMaterialVersionsByMaterialID(string mainMaterialID)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(mainMaterialID, "mainMaterialID");

            MaterialList materials = new MaterialList();

            using (DbContext dbi = DbHelper.GetDBContext(GetConnectionName()))
            {
                Database db = DatabaseFactory.Create(dbi);

                using (SqlCommand dbc = new SqlCommand())
                {
                    SqlParameter sqlParameter = new SqlParameter();
                    sqlParameter.Value = mainMaterialID;
                    sqlParameter.SqlDbType = SqlDbType.NVarChar;
                    sqlParameter.Size = 36;
                    sqlParameter.ParameterName = "@mainMaterialID";
                    sqlParameter.SourceColumn = "@mainMaterialID";
                    sqlParameter.Direction = ParameterDirection.InputOutput;

                    dbc.CommandType = CommandType.StoredProcedure;
                    dbc.CommandText = "GetMaterialVersions";
                    dbc.Parameters.Add(sqlParameter);

                    using (IDataReader dr = db.ExecuteReader(dbc))
                    {
                        while (dr.Read())
                        {
                            Material material = new Material();

                            ORMapping.DataReaderToObject(dr, material);

                            materials.Add(material);
                        }
                    }

                    mainMaterialID = sqlParameter.Value.ToString();
                }
            }

            return GenerateMaterialVersionTree(materials, mainMaterialID);
        }

        /// <summary>
        /// 获得指定ID的Material
        /// </summary>
        /// <param name="materialID">指定ID</param>
        /// <returns>MaterialList</returns>
        public MaterialList LoadMaterialByMaterialID(string materialID)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(materialID, "materialID");

            MaterialList materials = new MaterialList();

            WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

            wBuilder.AppendItem("ID", materialID);
            wBuilder.AppendTenantCode(typeof(Material));

            string sql = "SELECT * FROM WF.MATERIAL WHERE " + wBuilder.ToSqlString(TSqlBuilder.Instance);

            using (DbContext dbi = DbHelper.GetDBContext(GetConnectionName()))
            {
                using (IDataReader dr = DbHelper.RunSqlReturnDR(sql, GetConnectionName()))
                {
                    while (dr.Read())
                    {
                        Material material = new Material();

                        ORMapping.DataReaderToObject(dr, material);

                        materials.Add(material);
                    }
                }
            }

            DecorateMaterials(materials);

            return materials;
        }

        private static void DecorateMaterials(MaterialList materials)
        {
            MaterialFileOperationSettings.GetConfig().Operations.ForEach(op => op.DecorateMaterialListAfterLoad(materials));
        }

        /// <summary>
        ///由MaterialList形成以主版本为跟节点各版本为子节点的树
        /// </summary>
        /// <param name="materials">包含主版本和其他版本的集合</param>
        /// <param name="mainMaterialID">重版本ID</param>
        /// <returns>形成的树</returns>
        private MaterialTreeNode GenerateMaterialVersionTree(MaterialList materials, string mainMaterialID)
        {
            MaterialTreeNode rootNode = null;
            MaterialTreeNodeCollection materialTreeNodeCollection = new MaterialTreeNodeCollection();

            foreach (Material m in materials)
            {
                if (string.Compare(m.ID, mainMaterialID, true) == 0)
                    rootNode = new MaterialTreeNode(m);
                else if (m.ParentID == mainMaterialID)
                {
                    MaterialTreeNode node = GenerateMaterialVersionTree(materials, m.ID);
                    if (node != null)
                        materialTreeNodeCollection.Add(node);
                }
            }

            if (rootNode != null)
            {
                foreach (MaterialTreeNode node in materialTreeNodeCollection)
                {
                    rootNode.Children.Add(node);
                }
            }

            return rootNode;
        }

        /// <summary>
        /// 获得指定ID的副本 和 它之前的一切副本 按照时间正排序 
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>MaterialList</returns>
        public MaterialList GetPreMaterialsCopyVersion(string id)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(id, "id");

            WhereSqlClauseBuilder whereSqlClause = new WhereSqlClauseBuilder();

            whereSqlClause.AppendItem("PARENT_ID",
                string.Format("(SELECT PARENT_ID FROM WF.MATERIAL WHERE ID = {0})", TSqlBuilder.Instance.CheckQuotationMark(id, true)), "=", true);

            whereSqlClause.AppendItem("MODIFY_TIME",
                string.Format("(SELECT MODIFY_TIME FROM WF.MATERIAL WHERE ID = {0})", TSqlBuilder.Instance.CheckQuotationMark(id, true)), "<=", true);

            whereSqlClause.AppendTenantCode(typeof(Material));

            OrderBySqlClauseBuilder orderBySqlClause = new OrderBySqlClauseBuilder();

            orderBySqlClause.AppendItem("MODIFY_TIME", FieldSortDirection.Ascending);

            string sql = string.Format(@"SELECT * FROM WF.MATERIAL 
				WHERE {0} AND VERSION_TYPE = {1} ORDER BY {2}",
                whereSqlClause.ToSqlString(TSqlBuilder.Instance),
                Convert.ToInt16(MaterialVersionType.CopyVersion),
                orderBySqlClause.ToSqlString(TSqlBuilder.Instance));

            MaterialList materials = new MaterialList();

            using (DbContext dbi = DbHelper.GetDBContext(GetConnectionName()))
            {
                using (IDataReader dr = DbHelper.RunSqlReturnDR(sql, GetConnectionName()))
                {
                    while (dr.Read())
                    {
                        Material material = new Material();

                        ORMapping.DataReaderToObject(dr, material);

                        materials.Add(material);
                    }
                }
            }

            return materials;
        }

        #endregion

        #region 保存 DeltaMaterials

        /// <summary>
        /// 保存CommonDeltaMaterials
        /// </summary>
        /// <param name="deltaMaterials">DeltaMaterialList</param>
        public void SaveCommonDeltaMaterials(CommonDeltaMaterialsContainer commonContainer)
        {
            IList<DeltaMaterialList> deltaMaterialsList = commonContainer.ToList();

            foreach (var deltaMaterials in deltaMaterialsList)
            {
                this.SaveDeltaMaterials(deltaMaterials);
            }
        }

        /// <summary>
        /// 保存deltaMaterials
        /// </summary>
        /// <param name="deltaMaterials">DeltaMaterialList</param>
        public void SaveDeltaMaterials(DeltaMaterialList deltaMaterials)
        {
            SaveDeltaMaterials(deltaMaterials, true);

            deltaMaterials.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaMaterials"></param>
        /// <param name="doFileOperations"></param>
        public void SaveDeltaMaterials(DeltaMaterialList deltaMaterials, bool doFileOperations)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(deltaMaterials == null, "deltaMaterials");

            string rootPathName = deltaMaterials.RootPathName;

            if (string.IsNullOrEmpty(rootPathName))
                rootPathName = MaterialAdapter.DefaultUploadPathName;

            StringBuilder strB = new StringBuilder(1024);

            List<MaterialFileOeprationInfo> fileOPList = new List<MaterialFileOeprationInfo>();

            string sql = FillMaterialSqlAndFileOpList(deltaMaterials, fileOPList);

            if (string.IsNullOrEmpty(sql) == false)
            {
                using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                {
                    DbHelper.RunSql(sql, GetConnectionName());

                    if (doFileOperations)
                        DoFileOperations(rootPathName, fileOPList);

                    scope.Complete();
                }
            }
        }

        private void DoFileOperation(string rootPathName, MaterialFileOeprationInfo fileOp, MaterialContent content)
        {
            MaterialFileOperationSettings.GetConfig().Operations.ForEach(op =>
                op.DoModifyFileOperations(rootPathName, fileOp, content));
        }

        /// <summary>
        /// 文件操作
        /// </summary>
        /// <param name="dirRootName">文件保存路径的配置名称</param>
        /// <param name="fileOPList">文件操作集合</param>
        private void DoFileOperations(string rootPathName, IEnumerable<MaterialFileOeprationInfo> fileOPList)
        {
            foreach (MaterialFileOeprationInfo fileOp in fileOPList)
            {
                MaterialFileOperationSettings.GetConfig().Operations.ForEach(op =>
                    op.DoModifyFileOperations(rootPathName, fileOp, null));
            }
        }

        /// <summary>
        /// 生成SQL，形成文件操作集合
        /// </summary>
        /// <param name="deltaMaterials">Material操作集合</param>
        /// <param name="fileOPList">文件操作集合</param>
        /// <returns>形成的SQL语句</returns>
        private string FillMaterialSqlAndFileOpList(DeltaMaterialList deltaMaterials, List<MaterialFileOeprationInfo> fileOPList)
        {
            StringBuilder strB = new StringBuilder(1024);

            string sql = GetMaterialsOperationSql(deltaMaterials.Inserted, fileOPList, GetInsertMaterialSql);

            if (string.IsNullOrEmpty(sql) == false)
                strB.Append(sql);

            sql = GetMaterialsOperationSql(deltaMaterials.Updated, fileOPList, GetUpdateMaterialSql);

            if (string.IsNullOrEmpty(sql) == false)
            {
                if (strB.Length > 0)
                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                strB.Append(sql);
            }

            sql = GetMaterialsOperationSql(deltaMaterials.Deleted, fileOPList, GetDeleteMaterialSql);

            if (string.IsNullOrEmpty(sql) == false)
            {
                if (strB.Length > 0)
                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                strB.Append(sql);
            }

            return strB.ToString();
        }

        /// <summary>
        /// 获得插入操作的SQL语句
        /// </summary>
        /// <param name="material">要处理的material</param>
        /// <param name="fileOPList">文件操作</param>
        /// <returns>形成的SQL语句</returns>
        private string GetInsertMaterialSql(Material material, List<MaterialFileOeprationInfo> fileOPList)
        {
            BeforeInnerUpdate(material);
            fileOPList.Add(new MaterialFileOeprationInfo(material, FileOperation.Add));
            return ORMapping.GetInsertSql<Material>(material, TSqlBuilder.Instance);
        }

        /// <summary>
        /// 获得修改操作的SQL语句
        /// </summary>
        /// <param name="material">要处理的material</param>
        /// <param name="fileOPList">文件操作</param>
        /// <returns>形成的SQL语句</returns>
        private string GetUpdateMaterialSql(Material material, List<MaterialFileOeprationInfo> fileOPList)
        {
            BeforeInnerUpdate(material);
            fileOPList.Add(new MaterialFileOeprationInfo(material, FileOperation.Update));

            if (material.Department == null || material.Creator == null)
            {
                return ORMapping.GetUpdateSql<Material>(material, TSqlBuilder.Instance,
                                                        new string[] { "Creator", "Department" });
            }
            else
            {
                return ORMapping.GetUpdateSql<Material>(material, TSqlBuilder.Instance);
            }
        }

        /// <summary>
        /// 获得删除操作的SQL语句
        /// </summary>
        /// <param name="material">要处理的material</param>
        /// <param name="fileOPList">文件操作</param>
        /// <returns>形成的SQL语句</returns>
        private string GetDeleteMaterialSql(Material material, List<MaterialFileOeprationInfo> fileOPList)
        {
            fileOPList.Add(new MaterialFileOeprationInfo(material, FileOperation.Delete));

            return string.Format("DELETE WF.MATERIAL WHERE {0}",
                ORMapping.GetWhereSqlClauseBuilderByPrimaryKey<Material>(material).ToSqlString(TSqlBuilder.Instance));
        }

        /// <summary>
        /// 获得指定操作的SQL语句
        /// </summary>
        /// <param name="materials">material集合</param>
        /// <param name="fileOPList">文件操作集合</param>
        /// <param name="sqlDelegate">形成相应操作SQL语句的方法</param>
        /// <returns>形成的SQL语句</returns>
        private string GetMaterialsOperationSql(MaterialList materials, List<MaterialFileOeprationInfo> fileOPList, GetMaterialSqlOPDelegate sqlDelegate)
        {
            StringBuilder strB = new StringBuilder(256);

            for (int i = 0; i < materials.Count; i++)
            {
                if (strB.Length > 0)
                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                strB.Append(sqlDelegate(materials[i], fileOPList));
            }

            return strB.ToString();
        }

        #endregion

        #region 删除指定ResourceID的附件

        /// <summary>
        /// 删除指定ResourceID的附件数据，不删除对应文件
        /// </summary>
        /// <param name="resourceID">资源ID</param>
        public void DeleteMaterialsByResourceID(string resourceID)
        {
            InnerDeleteMaterialsByResourceID(string.Empty, resourceID);
        }

        /// <summary>
        /// 删除指定ResourceID的附件
        /// </summary>
        /// <param name="rootPathName">根目录配置名称</param>
        /// <param name="resourceID">资源ID</param>
        public void DeleteMaterialsByResourceID(string rootPathName, string resourceID)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(rootPathName, "rootPathName");

            InnerDeleteMaterialsByResourceID(rootPathName, resourceID);
        }

        /// <summary>
        /// 删除指定ResourceID的附件
        /// </summary>
        /// <param name="rootPathName">根目录配置名称</param>
        /// <param name="resourceID">资源ID</param>
        private void InnerDeleteMaterialsByResourceID(string rootPathName, string resourceID)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(resourceID, "resourceID");

            MaterialList materialList = this.LoadMaterialsByResourceID(resourceID);

            DeltaMaterialList deltaMaterialList = new DeltaMaterialList();

            deltaMaterialList.RootPathName = rootPathName;

            foreach (Material material in materialList)
            {
                deltaMaterialList.Deleted.Add(material);
            }

            this.SaveDeltaMaterials(deltaMaterialList, string.IsNullOrEmpty(rootPathName) == false);
        }

        #endregion

        #region 保存文件版本
        /// <summary>
        /// 保存文件的版本
        /// </summary>
        /// <param name="materials">已经做好的文件版本信息</param>
        /// <param name="rootPathName">根文件夹路径配置名称</param>
        public void SaveOtherVersion(MaterialList materials, string rootPathName)
        {
            StringBuilder strB = new StringBuilder(256);

            foreach (Material material in materials)
            {
                if (strB.Length > 0)
                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                strB.Append(ORMapping.GetInsertSql<Material>(material, TSqlBuilder.Instance));
            }

            if (strB.Length != 0)
            {
                using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                {
                    DbHelper.RunSql(strB.ToString(), GetConnectionName());
                    DoFilesCopy(materials, rootPathName, rootPathName, false);
                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// 得到连接串的名称
        /// </summary>
        /// <returns></returns>
        public string GetConnectionName()
        {
            return ConnectionDefine.DBConnectionName;
        }
        #endregion

        #region 保存文件副本
        public void InsertWithContent(MaterialList materials)
        {
            materials.NullCheck("materials");

            StringBuilder strB = new StringBuilder();

            ORMappingItemCollection mappings = ORMapping.GetMappingInfo(typeof(Material));

            foreach (Material m in materials)
            {
                if (strB.Length > 0)
                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                InsertSqlClauseBuilder builder = ORMapping.GetInsertSqlClauseBuilder(m, mappings, "CreateDateTime");

                builder.AppendItem("CREATE_DATETIME", m.CreateDateTime);

                strB.AppendFormat("INSERT INTO {0}{1}", mappings.TableName, builder.ToSqlString(TSqlBuilder.Instance));
            }

            if (strB.Length > 0)
            {
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    DbHelper.RunSql(strB.ToString(), GetConnectionName());

                    foreach (Material m in materials)
                    {
                        m.Content.RelativeID = m.ResourceID;

                        MaterialFileOeprationInfo fileOp = new MaterialFileOeprationInfo(m, FileOperation.Update);

                        DoFileOperation(MaterialAdapter.DefaultUploadPathName, fileOp, m.Content);
                    }

                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// 保存副本
        /// </summary>
        /// <param name="deltaMaterials">已经复制完成的副本的delta对象</param>
        /// <param name="sourceRootPathName">源文件的主路径的配置节点名称</param>
        /// <param name="destRootPathName">目标文件的主路径的配置节点名称</param>
        public void SaveCopyVersion(DeltaMaterialList deltaMaterials, string destRootPathName)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(deltaMaterials == null, "deltaMaterials");

            if (string.IsNullOrEmpty(deltaMaterials.RootPathName))
                deltaMaterials.RootPathName = MaterialAdapter.DefaultUploadPathName;

            ExceptionHelper.CheckStringIsNullOrEmpty(destRootPathName, "destRootPathName");

            string sql = this.GetCopyFilesSql(deltaMaterials);

            if (string.IsNullOrEmpty(sql) == false)
            {
                using (TransactionScope scope = TransactionScopeFactory.Create(TransactionScopeOption.Required))
                {
                    DbHelper.RunSql(sql, GetConnectionName());

                    DoFilesCopy(deltaMaterials.Inserted, deltaMaterials.RootPathName, destRootPathName, true);
                    DoFilesCopy(deltaMaterials.Updated, deltaMaterials.RootPathName, destRootPathName, true);
                    DoFilesCopy(deltaMaterials.Deleted, deltaMaterials.RootPathName, destRootPathName, false);

                    scope.Complete();
                }
            }
        }

        private static IMaterialContentPersistManager GetMaterialContentPersistManager(string rootPathName, MaterialFileOeprationInfo fileOp)
        {
            string uploadRootPath = AppPathConfigSettings.GetConfig().Paths[rootPathName].Dir;

            FileInfo sourceFile = new FileInfo(uploadRootPath + @"Temp\" + Path.GetFileName(fileOp.Material.RelativeFilePath));
            FileInfo destFile = new FileInfo(uploadRootPath + fileOp.Material.RelativeFilePath);

            IMaterialContentPersistManager persistManager = MaterialContentSettings.GetConfig().PersistManager;

            persistManager.SourceFileInfo = sourceFile;
            persistManager.DestFileInfo = destFile;

            if (fileOp.Operation == FileOperation.Update)
                persistManager.CheckSourceFileExists = false;

            return persistManager;
        }

        /// <summary>
        /// 形成更新副本的SQL
        /// </summary>
        /// <param name="deltaMaterials">要更新的DeltaMaterialList对象</param>
        /// <returns>形成的SQL语句</returns>
        private string GetCopyFilesSql(DeltaMaterialList deltaMaterials)
        {
            ExceptionHelper.TrueThrow<ArgumentNullException>(deltaMaterials == null, "deltaMaterials");

            StringBuilder strB = new StringBuilder(256);

            //新增
            foreach (Material material in deltaMaterials.Inserted)
            {
                if (strB.Length > 0)
                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                strB.Append(ORMapping.GetInsertSql<Material>(material, TSqlBuilder.Instance));
            }

            //更新
            foreach (Material material in deltaMaterials.Updated)
            {
                if (strB.Length > 0)
                    strB.Append(TSqlBuilder.Instance.DBStatementSeperator);

                WhereSqlClauseBuilder wBuilder = new WhereSqlClauseBuilder();

                wBuilder.AppendItem("PARENT_ID", material.ParentID);
                wBuilder.AppendItem("WF_ACTIVITY_ID", material.WfActivityID);
                wBuilder.AppendItem("VERSION_TYPE", (int)MaterialVersionType.CopyVersion);

                strB.Append(string.Format(
                        @"IF EXISTS(SELECT * FROM WF.MATERIAL WHERE {0})
							{1}
						  ELSE
							{2}",
                        wBuilder.ToSqlString(TSqlBuilder.Instance),
                        ORMapping.GetUpdateSql<Material>(material, TSqlBuilder.Instance),
                        ORMapping.GetInsertSql<Material>(material, TSqlBuilder.Instance)));
            }

            return strB.ToString();
        }

        public static void DoFilesCopy(MaterialList materials, string sourceRootPathName, string destRootPathName)
        {
            string sourceRootPath = AppPathConfigSettings.GetConfig().Paths[sourceRootPathName].Dir;
            string destRootPath = AppPathConfigSettings.GetConfig().Paths[destRootPathName].Dir;

            ExceptionHelper.CheckStringIsNullOrEmpty(sourceRootPath, "sourceRootPath");
            ExceptionHelper.CheckStringIsNullOrEmpty(destRootPath, "destRootPath");

            foreach (Material material in materials)
            {
                FileInfo sourceFile = new FileInfo(sourceRootPath + material.RelativeFilePath);

                if (sourceFile.Exists == false)
                    sourceFile = new FileInfo(sourceRootPath + @"Temp\" + Path.GetFileName(material.RelativeFilePath));

                if (sourceFile.Exists)
                {
                    FileInfo destFile = new FileInfo(Path.Combine(destRootPath, Path.GetFileName(material.RelativeFilePath)));

                    DoFileCopy(sourceFile, destFile);
                }
            }
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="materials">文件集合</param>
        /// <param name="sourceRootPathName">源文件的主路径的配置节点名称</param>
        /// <param name="destRootPathName">目标文件的主路径的配置节点名称</param>	
        /// <param name="copyFromTempFolder">是否从临时文件夹拷贝文件</param>	
        private static void DoFilesCopy(MaterialList materials, string sourceRootPathName, string destRootPathName, bool copyFromTempFolder)
        {
            string sourceRootPath = AppPathConfigSettings.GetConfig().Paths[sourceRootPathName].Dir;
            string destRootPath = AppPathConfigSettings.GetConfig().Paths[destRootPathName].Dir;

            ExceptionHelper.CheckStringIsNullOrEmpty(sourceRootPath, "sourceRootPath");
            ExceptionHelper.CheckStringIsNullOrEmpty(destRootPath, "destRootPath");

            foreach (Material material in materials)
            {
                ExceptionHelper.TrueThrow<ArgumentNullException>(material.SourceMaterial == null, "material.SourceMaterial");

                FileInfo sourceFile;

                if (copyFromTempFolder)
                    sourceFile = new FileInfo(sourceRootPath + @"Temp\" + Path.GetFileName(material.SourceMaterial.RelativeFilePath));
                else
                    sourceFile = new FileInfo(sourceRootPath + material.SourceMaterial.RelativeFilePath);

                if (sourceFile.Exists)
                {
                    FileInfo destFile = new FileInfo(destRootPath + material.RelativeFilePath);

                    DoFileCopy(sourceFile, destFile);
                }
            }
        }

        /// <summary>
        /// 复制一个文件
        /// </summary>
        /// <param name="sourceFile">源文件</param>
        /// <param name="destFile">目标文件</param>
        private static void DoFileCopy(FileInfo sourceFile, FileInfo destFile)
        {
            ExceptionHelper.FalseThrow(sourceFile.Exists, string.Format(Resource.FileNotFound, sourceFile.Name));

            if (destFile.Directory.Exists == false)
                destFile.Directory.Create();

            sourceFile.CopyTo(destFile.FullName, true);
        }

        #endregion

        protected void BeforeInnerUpdate(Material data)
        {
            data.FillExtraData(data.ExtraDataDictionary);
        }
    }
}