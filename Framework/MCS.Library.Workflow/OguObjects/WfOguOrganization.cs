using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Engine;

namespace MCS.Library.Workflow.OguObjects
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class WfOguOrganization : WfOguObject, IOrganization
    {
		internal WfOguOrganization() : base(SchemaType.Organizations)
		{
		}

        /// <summary>
        /// 
        /// </summary>
        public WfOguOrganization(string id)
            : base(id, SchemaType.Organizations)
        {
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="org"></param>
		public WfOguOrganization(IOrganization org)
			: base(org, SchemaType.Organizations)
		{
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WfOguOrganization(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #region IOrganization 成员

        /// <summary>
        /// 
        /// </summary>
        public OguObjectCollection<IOguObject> Children
        {
            get
            {
                return BaseOrganizationObject.Children;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CustomsCode
        {
            get
            {
                return BaseOrganizationObject.CustomsCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DepartmentClassType DepartmentClass
        {
            get
            {
                return BaseOrganizationObject.DepartmentClass;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DepartmentTypeDefine DepartmentType
        {
            get
            {
                return BaseOrganizationObject.DepartmentType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeSideLine"></param>
        /// <returns></returns>
        public OguObjectCollection<T> GetAllChildren<T>(bool includeSideLine) where T : IOguObject
        {
            return BaseOrganizationObject.GetAllChildren<T>(includeSideLine);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsTopOU
        {
            get
            {
                return BaseOrganizationObject.IsTopOU;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DepartmentRankType Rank
        {
            get
            {
                return BaseOrganizationObject.Rank;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="matchString"></param>
		/// <param name="includeSideLine"></param>
		/// <param name="level"></param>
		/// <param name="returnCount">返回行数</param>
		/// <returns></returns>
		public OguObjectCollection<T> QueryChildren<T>(string matchString, bool includeSideLine, SearchLevel level, int returnCount) where T : IOguObject
		{
			return BaseOrganizationObject.QueryChildren<T>(matchString, includeSideLine, level, returnCount);
		}

        #endregion

        private IOrganization BaseOrganizationObject
        {
            get
            {
                return (IOrganization)base.BaseObject;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WfOguOrganizationCollection : WfKeyedCollectionBase<string, WfOguOrganization>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="organization"></param>
        public void Add(WfOguOrganization organization)
        {
            this.InnerAdd(organization);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public WfOguOrganization this[int index]
        {
            get
            {
				return this.InnerGet(index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deptID"></param>
        /// <returns></returns>
        public WfOguOrganization this[string deptID]
        {
            get
            {
                return this.InnerGet(deptID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        protected override string GetKeyFromItem(WfOguOrganization organization)
        {
            return organization.ID;
        }
    }
}
