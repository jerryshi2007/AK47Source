#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.OGUPermission
// FileName	��	OguImpl.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.OGUPermission.Properties;
using System.Xml.Serialization;

namespace MCS.Library.OGUPermission
{
    #region OguBaseImpl
    /// <summary>
    /// ȱʡ�Ļ�����Աʵ����Ļ���
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Name = {name}")]
    public abstract class OguBaseImpl : IOguObject, IOguPropertyAccessible
    {
        private string id = string.Empty;
        private string name = string.Empty;
        private string displayName = string.Empty;
        private string description = string.Empty;
        private string fullPath = string.Empty;
        private SchemaType objectType;
        private IOrganization parent = null;
        private string sortID = string.Empty;
        private string globalSortID = string.Empty;
        private IOrganization topOU = null;
        private int levels = -1;
        private Hashtable properties = null;

        #region Sync Objects
        private object propertiesSyncObj = new object();
        private object parentSyncObj = new object();
        private object topOUSyncObj = new object();
        #endregion

        #region IOguObject ��Ա
        /// <summary>
        /// �����ID
        /// </summary>
        public string ID
        {
            get { return this.id; }
            internal set { this.id = value; }
        }

        /// <summary>
        /// ���������
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// �������ʾ����
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (this.displayName == string.Empty)
                    this.displayName = Name;

                return this.displayName;
            }
            set { this.displayName = value; }
        }

        /// <summary>
        /// ���������
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// �����ȫ·��
        /// </summary>
        public string FullPath
        {
            get { return this.fullPath; }
            set { this.fullPath = value; }
        }

        /// <summary>
        /// ���������
        /// </summary>
        public SchemaType ObjectType
        {
            get { return this.objectType; }
            set { this.objectType = value; }
        }

        /// <summary>
        /// ������
        /// </summary>
        public IOrganization Parent
        {
            get
            {
                if (this.parent == null)
                {
                    lock (this.parentSyncObj)
                    {
                        if (this.parent == null)
                            OguPermissionSettings.GetConfig().OguObjectImpls.InitAncestorOUs(this);
                    }
                }

                return this.parent;
            }
        }

        /// <summary>
        /// ������ڲ������
        /// </summary>
        public string SortID
        {
            get { return this.sortID; }
            set { this.sortID = value; }
        }

        /// <summary>
        /// �����ȫ�������
        /// </summary>
        public string GlobalSortID
        {
            get { return this.globalSortID; }
            set { this.globalSortID = value; }
        }

        /// <summary>
        /// ����Ķ�������
        /// </summary>
        public IOrganization TopOU
        {
            get
            {
                if (this.topOU == null)
                {
                    lock (this.topOUSyncObj)
                    {
                        if (this.topOU == null)
                        {
                            string adjustedFullPath = this.fullPath;

                            if (adjustedFullPath.IsNullOrEmpty())
                            {
                                if (this.Parent != null)
                                    adjustedFullPath = this.Parent.FullPath + "\\" + this.Name;
                            }

                            if (adjustedFullPath.IsNotEmpty())
                            {
                                string mappedPath;

                                if (OguPermissionSettings.GetConfig().TopOUMapping.GetMappedPath(adjustedFullPath, out mappedPath))
                                    this.topOU = GetTopOUFromMapping(mappedPath);
                                else
                                    this.topOU = GetTopOUFromLevel(OguPermissionSettings.GetConfig().TopOUMapping.Level);
                            }
                        }
                    }
                }

                return this.topOU;
            }
        }

        /// <summary>
        /// ����Ĳ�μ���
        /// </summary>
        public int Levels
        {
            get
            {
                if (this.levels < 0)
                    this.levels = this.fullPath.Split('\\').Length;

                return this.levels;
            }
        }

        /// <summary>
        /// �Ƿ�����ĳ������
        /// </summary>
        /// <param name="parentDept"></param>
        /// <returns></returns>
        public bool IsChildrenOf(IOrganization parentDept)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(parentDept != null, "parentDept");

            return (this.FullPath.Length > parentDept.FullPath.Length &&
                this.FullPath.IndexOf(parentDept.FullPath, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// ��չ����
        /// </summary>
        public IDictionary Properties
        {
            get
            {
                if (this.properties == null)
                {
                    lock (this.propertiesSyncObj)
                    {
                        if (this.properties == null)
                            this.properties = new Hashtable();
                    }
                }

                return this.properties;
            }
        }

        #endregion

        #region �����ĳ�Ա
        /// <summary>
        /// ��ʼ������
        /// </summary>
        /// <param name="row"></param>
        public virtual void InitProperties(DataRow row)
        {
            ID = Common.GetDataRowTextValue(row, "GUID");
            this.name = Common.GetDataRowTextValue(row, "OBJ_NAME");
            this.displayName = Common.GetDataRowTextValue(row, "DISPLAY_NAME");
            this.description = Common.GetDataRowTextValue(row, "DESCRIPTION");
            this.fullPath = Common.GetDataRowTextValue(row, "ALL_PATH_NAME");

            this.globalSortID = Common.GetDataRowTextValue(row, "GLOBAL_SORT");
            this.sortID = Common.GetDataRowTextValue(row, "INNER_SORT");

            string strObjClass = Common.GetDataRowTextValue(row, "OBJECTCLASS");

            try
            {
                this.objectType = (SchemaType)Enum.Parse(this.objectType.GetType(), strObjClass, true);
            }
            catch (System.Exception)
            {
            }

            foreach (DataColumn column in row.Table.Columns)
                Properties.Add(column.ColumnName, row[column.ColumnName]);

            if (Properties.Contains("OBJECTCLASS") == false)
                Properties.Add("OBJECTCLASS", this.objectType.ToString().ToUpper());
        }

        #endregion �����ĳ�Ա

        #region ˽�еĳ�Ա
        private IOrganization GetTopOUFromLevel(int level)
        {
            IOrganization dept = null;

            if (ObjectType == SchemaType.Organizations)
                dept = (IOrganization)this;
            else
                dept = this.Parent;

            while (dept.Levels > level)
                dept = dept.Parent;

            return dept;
        }

        private IOrganization GetTopOUFromMapping(string mappedPath)
        {
            IOrganization dept = null;

            if (this.fullPath.IndexOf(mappedPath, StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (ObjectType == SchemaType.Organizations)
                    dept = (IOrganization)this;
                else
                    dept = this.Parent;

                while (dept != null)
                {
                    if (string.Compare(dept.FullPath, mappedPath, true) == 0)
                        break;

                    dept = dept.Parent;
                }
            }
            else
            {
                OguObjectCollection<IOrganization> objs = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(
                            SearchOUIDType.FullPath,
                            mappedPath);

                ExceptionHelper.FalseThrow(objs.Count > 0, Resource.CanNotFindObject, mappedPath);

                dept = objs[0];
            }

            return dept;
        }

        ///// <summary>
        ///// һ���Լ������е�����OU
        ///// </summary>
        //private void InitAncestorOUs(IOguObject currentObj)
        //{
        //    string[] allFullPath = GetAncestorsFullPath();

        //    if (allFullPath.Length > 0)
        //    {
        //        OguObjectCollection<IOrganization> organizations = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(
        //            SearchOUIDType.FullPath,
        //            allFullPath);

        //        organizations.Sort(OrderByPropertyType.FullPath, SortDirectionType.Ascending);

        //        OguBaseImpl current = (OguBaseImpl)currentObj;

        //        for (int i = organizations.Count - 1; i >= 0; i--)
        //        {
        //            IOrganization org = organizations[i];

        //            if (org.DepartmentType != DepartmentTypeDefine.XuNiJiGou)
        //            {
        //                current.parent = org;
        //                current = (OguBaseImpl)current.parent;
        //            }
        //        }
        //    }
        //}

        //private string[] GetAncestorsFullPath()
        //{
        //    ExceptionHelper.CheckStringIsNullOrEmpty(this.fullPath, "FullPath");

        //    string[] strParts = this.fullPath.Split('\\');

        //    string[] result = new string[strParts.Length - 1];

        //    for (int i = 0; i < result.Length; i++)
        //        if (i > 0)
        //            result[i] = result[i - 1] + "\\" + strParts[i];
        //        else
        //            result[i] = strParts[i];

        //    return result;
        //}
        #endregion

        #region ��ʽ�ӿ�ʵ��
        string IOguPropertyAccessible.Description
        {
            get
            {
                return this.Description;
            }
            set
            {
                this.Description = value;
            }
        }

        string IOguPropertyAccessible.DisplayName
        {
            get
            {
                return this.DisplayName;
            }
            set
            {
                this.DisplayName = value;
            }
        }

        string IOguPropertyAccessible.FullPath
        {
            get
            {
                return this.FullPath;
            }
            set
            {
                this.FullPath = value;
            }
        }

        string IOguPropertyAccessible.GlobalSortID
        {
            get
            {
                return this.GlobalSortID;
            }
            set
            {
                this.GlobalSortID = value;
            }
        }

        string IOguPropertyAccessible.ID
        {
            get { return this.ID; }
            set { this.ID = value; }
        }

        int IOguPropertyAccessible.Levels
        {
            get { return this.Levels; ; }
        }

        string IOguPropertyAccessible.Name
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
            }
        }

        SchemaType IOguPropertyAccessible.ObjectType
        {
            get
            {
                return this.ObjectType;
            }
            set
            {
                this.ObjectType = value;
            }
        }

        IOrganization IOguPropertyAccessible.Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
            }
        }

        IDictionary IOguPropertyAccessible.Properties
        {
            get { return this.Properties; }
        }

        string IOguPropertyAccessible.SortID
        {
            get
            {
                return this.SortID;
            }
            set
            {
                this.SortID = value;
            }
        }

        IOrganization IOguPropertyAccessible.TopOU
        {
            get
            {
                return this.TopOU;
            }
            set
            {
                this.topOU = value;
            }
        }

        #endregion
    }
    #endregion OguBaseImpl
}
