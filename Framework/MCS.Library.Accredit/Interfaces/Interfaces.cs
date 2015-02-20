using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Accredit
{
	/// <summary>
	/// 机构人员数据操作接口定义
	/// </summary>
    public interface IOguDataOperation
    {
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="eventContainer"></param>
        void Init(OguDataOperationEventContainer eventContainer);
	}

	#region 委托定义
	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeUpdateObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void UpdateObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xmlDoc"></param>
    /// <param name="context"></param>
    public delegate void BeforeInsertObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xmlDoc"></param>
    /// <param name="context"></param>
    public delegate void InsertObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xmlDoc"></param>
    /// <param name="context"></param>
    public delegate void BeforeLogicDeleteObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xmlDoc"></param>
    /// <param name="context"></param>
    public delegate void LogicDeleteObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeFurbishDeleteObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void FurbishDeleteObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeRealDeleteObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void RealDeleteObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeInitPasswordHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void InitPasswordHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeResetPasswordHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void ResetPasswordHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeMoveObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void MoveObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeSortObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void SortObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeGroupSortObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate XmlDocument GroupSortObjectsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeSetUserMainDutyHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void SetUserMainDutyHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeDelUsersFromGroupsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void DelUsersFromGroupsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeAddObjectsToGroupsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate XmlDocument AddObjectsToGroupsHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeSetSecsToLeaderHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate XmlDocument SetSecsToLeaderHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void BeforeDelSecsOfLeaderHandler(XmlDocument xmlDoc, Dictionary<object, object> context);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="xmlDoc"></param>
	/// <param name="context"></param>
	public delegate void DelSecsOfLeaderHandler(XmlDocument xmlDoc, Dictionary<object, object> context);


	#endregion 委托定义

	/// <summary>
	/// 操作的事件容器
	/// </summary>
    public class OguDataOperationEventContainer
    {
		/// <summary>
		/// 
		/// </summary>
		public event BeforeUpdateObjectsHandler BeforeUpdateObjects;
		
		/// <summary>
		/// 
		/// </summary>
		public event UpdateObjectsHandler UpdateObjects;

        /// <summary>
        /// 
        /// </summary>
        public event BeforeInsertObjectsHandler BeforeInsertObjects;

        /// <summary>
        /// 
        /// </summary>
        public event InsertObjectsHandler InsertObjects;

        /// <summary>
        /// 
        /// </summary>
        public event BeforeLogicDeleteObjectsHandler BeforeLogicDeleteObjects;

        /// <summary>
        /// 
        /// </summary>
        public event LogicDeleteObjectsHandler LogicDeleteObjects;

		/// <summary>
		/// 
		/// </summary>
		public event BeforeFurbishDeleteObjectsHandler BeforeFurbishDeleteObjects;

		/// <summary>
		/// 
		/// </summary>
		public event FurbishDeleteObjectsHandler FurbishDeleteObjects;

		/// <summary>
		/// 
		/// </summary>
		public event BeforeRealDeleteObjectsHandler BeforeRealDeleteObjects;

		/// <summary>
		/// 
		/// </summary>
		public event RealDeleteObjectsHandler RealDeleteObjects;

		/// <summary>
		/// 
		/// </summary>
		public event BeforeInitPasswordHandler BeforeInitPassword;

		/// <summary>
		/// 
		/// </summary>
		public event InitPasswordHandler InitPassword;

		/// <summary>
		/// 
		/// </summary>
		public event BeforeResetPasswordHandler BeforeResetPassword;

		/// <summary>
		/// 
		/// </summary>
		public event ResetPasswordHandler ResetPassword;

		/// <summary>
		/// 
		/// </summary>
		public event BeforeMoveObjectsHandler BeforeMoveObjects;

		/// <summary>
		/// 
		/// </summary>
		public event MoveObjectsHandler MoveObjects;

		/// <summary>
		/// 
		/// </summary>
		public event BeforeSortObjectsHandler BeforeSortObjects;

		/// <summary>
		/// 
		/// </summary>
		public event SortObjectsHandler SortObjects;

		/// <summary>
		/// 
		/// </summary>
		public event BeforeGroupSortObjectsHandler BeforeGroupSortObjects;

		/// <summary>
		/// 
		/// </summary>
		public event GroupSortObjectsHandler GroupSortObjects;

		/// <summary>
		/// 
		/// </summary>
		public event BeforeSetUserMainDutyHandler BeforeSetUserMainDuty;

		/// <summary>
		/// 
		/// </summary>
		public event SetUserMainDutyHandler SetUserMainDuty;

		/// <summary>
		/// 
		/// </summary>
		public event BeforeDelUsersFromGroupsHandler BeforeDelUsersFromGroups;

		/// <summary>
		/// 
		/// </summary>
		public event DelUsersFromGroupsHandler DelUsersFromGroups;

		/// <summary>
		/// 
		/// </summary>
		public event BeforeAddObjectsToGroupsHandler BeforeAddObjectsToGroups;

		/// <summary>
		/// 
		/// </summary>
		public event AddObjectsToGroupsHandler AddObjectsToGroups;

		/// <summary>
		/// 
		/// </summary>
		public event BeforeSetSecsToLeaderHandler BeforeSetSecsToLeader;

		/// <summary>
		/// 
		/// </summary>
		public event SetSecsToLeaderHandler SetSecsToLeader;

		/// <summary>
		/// 
		/// </summary>
		public event BeforeDelSecsOfLeaderHandler BeforeDelSecsOfLeader;

		/// <summary>
		/// 
		/// </summary>
		public event DelSecsOfLeaderHandler DelSecsOfLeader;



		internal void OnBeforeUpdateObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeUpdateObjects != null)
				BeforeUpdateObjects(xmlDoc, context);
		}

		internal void OnUpdateObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (UpdateObjects != null)
				UpdateObjects(xmlDoc, context);
		}

        internal void OnBeforeInsertObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
        {
            if (BeforeInsertObjects != null)
                BeforeInsertObjects(xmlDoc, context);
        }

        internal void OnInsertObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
        {
            if (InsertObjects != null)
                InsertObjects(xmlDoc, context);
        }

        internal void OnBeforeLogicDeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
        {
            if (BeforeLogicDeleteObjects != null)
                BeforeLogicDeleteObjects(xmlDoc, context);
        }

        internal void OnLogicDeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
        {
            if (LogicDeleteObjects != null)
                LogicDeleteObjects(xmlDoc, context);
        }

		internal void OnBeforeFurbishDeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeFurbishDeleteObjects != null)
				BeforeFurbishDeleteObjects(xmlDoc, context);
		}

		internal void OnFurbishDeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (FurbishDeleteObjects != null)
				FurbishDeleteObjects(xmlDoc, context);
		}

		internal void OnBeforeRealDeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeRealDeleteObjects != null)
				BeforeRealDeleteObjects(xmlDoc, context);
		}

		internal void OnRealDeleteObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (RealDeleteObjects != null)
				RealDeleteObjects(xmlDoc, context);
		}

		internal void OnBeforeInitPassword(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeInitPassword != null)
				BeforeInitPassword(xmlDoc, context);
		}

		internal void OnInitPassword(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (InitPassword != null)
				InitPassword(xmlDoc, context);
		}

		internal void OnBeforeResetPassword(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeResetPassword != null)
				BeforeResetPassword(xmlDoc, context);
		}

		internal void OnResetPassword(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (ResetPassword != null)
				ResetPassword(xmlDoc, context);
		}

		internal void OnBeforeMoveObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeMoveObjects != null)
				BeforeMoveObjects(xmlDoc, context);
		}

		internal void OnMoveObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (MoveObjects != null)
				MoveObjects(xmlDoc, context);
		}

		internal void OnBeforeSortObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeSortObjects != null)
				BeforeSortObjects(xmlDoc, context);
		}

		internal void OnSortObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (SortObjects != null)
				SortObjects(xmlDoc, context);
		}

		internal void OnBeforeGroupSortObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeGroupSortObjects != null)
				BeforeGroupSortObjects(xmlDoc, context);
		}

		internal XmlDocument OnGroupSortObjects(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlDocument result = null;
			if (GroupSortObjects != null)
				result = GroupSortObjects(xmlDoc, context);
			return result;
		}

		internal void OnBeforeSetUserMainDuty(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeSetUserMainDuty != null)
				BeforeSetUserMainDuty(xmlDoc, context);
		}

		internal void OnSetUserMainDuty(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (SetUserMainDuty != null)
				SetUserMainDuty(xmlDoc, context);
		}

		internal void OnBeforeDelUsersFromGroups(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeDelUsersFromGroups != null)
				BeforeDelUsersFromGroups(xmlDoc, context);
		}

		internal void OnDelUsersFromGroups(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (DelUsersFromGroups != null)
				DelUsersFromGroups(xmlDoc, context);
		}

		internal void OnBeforeAddObjectsToGroups(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeAddObjectsToGroups != null)
				BeforeAddObjectsToGroups(xmlDoc, context);
		}

		internal XmlDocument OnAddObjectsToGroups(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlDocument result = null;
			if (AddObjectsToGroups != null)
				result = AddObjectsToGroups(xmlDoc, context);
			return result;
		}

		internal void OnBeforeSetSecsToLeader(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeSetSecsToLeader != null)
				BeforeSetSecsToLeader(xmlDoc, context);
		}

		internal XmlDocument OnSetSecsToLeader(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlDocument result = null;
			if (SetSecsToLeader != null)
				result = SetSecsToLeader(xmlDoc, context);
			return result;
		}

		internal void OnBeforeDelSecsOfLeader(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (BeforeDelSecsOfLeader != null)
				BeforeDelSecsOfLeader(xmlDoc, context);
		}

		internal void OnDelSecsOfLeader(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			if (DelSecsOfLeader != null)
				DelSecsOfLeader(xmlDoc, context);
		}

    }

	#region DataOpContext
	internal class DataOpContext
	{
		public readonly List<OguDataOperationEventContainer> EventContainers = new List<OguDataOperationEventContainer>();
		public readonly Dictionary<object, object> Context = new Dictionary<object, object>();

		public void OnBeforeUpdateObjects(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeUpdateObjects(xmlDoc, Context);
		}

		public void OnUpdateObjects(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnUpdateObjects(xmlDoc, Context);
		}

        public void OnBeforeInsertObjects(XmlDocument xmlDoc)
        {
            foreach (OguDataOperationEventContainer container in EventContainers)
                container.OnBeforeInsertObjects(xmlDoc, Context);
        }

        public void OnInsertObjects(XmlDocument xmlDoc)
        {
            foreach (OguDataOperationEventContainer container in EventContainers)
                container.OnInsertObjects(xmlDoc, Context);
        }

        public void OnBeforeLogicDeleteObjects(XmlDocument xmlDoc)
        {
            foreach (OguDataOperationEventContainer container in EventContainers)
                container.OnBeforeLogicDeleteObjects(xmlDoc, Context);
        }

        public void OnLogicDeleteObjects(XmlDocument xmlDoc)
        {
            foreach (OguDataOperationEventContainer container in EventContainers)
                container.OnLogicDeleteObjects(xmlDoc, Context);
        }

		public void OnBeforeFurbishDeleteObjects(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeFurbishDeleteObjects(xmlDoc, Context);
		}

		public void OnFurbishDeleteObjects(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnFurbishDeleteObjects(xmlDoc, Context);
		}

		public void OnBeforeRealDeleteObjects(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeRealDeleteObjects(xmlDoc, Context);
		}

		public void OnRealDeleteObjects(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnRealDeleteObjects(xmlDoc, Context);
		}

		public void OnBeforeInitPassword(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeInitPassword(xmlDoc, Context);
		}

		public void OnInitPassword(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnInitPassword(xmlDoc, Context);
		}

		public void OnBeforeResetPassword(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeResetPassword(xmlDoc, Context);
		}

		public void OnResetPassword(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnResetPassword(xmlDoc, Context);
		}

		public void OnBeforeMoveObjects(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeMoveObjects(xmlDoc, Context);
		}

		public void OnMoveObjects(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnMoveObjects(xmlDoc, Context);
		}

		public void OnBeforeSortObjects(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeSortObjects(xmlDoc, Context);
		}

		public void OnSortObjects(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnSortObjects(xmlDoc, Context);
		}

		public void OnBeforeGroupSortObjects(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeGroupSortObjects(xmlDoc, Context);
		}

		public List<XmlDocument> OnGroupSortObjects(XmlDocument xmlDoc)
		{
			List<XmlDocument> xmlDocList = new List<XmlDocument>();
			foreach (OguDataOperationEventContainer container in EventContainers)
				xmlDocList.Add(container.OnGroupSortObjects(xmlDoc, Context));
			return xmlDocList;
		}
		
		public void OnBeforeSetUserMainDuty(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeSetUserMainDuty(xmlDoc, Context);
		}

		public void OnSetUserMainDuty(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnSetUserMainDuty(xmlDoc, Context);
		}

		public void OnBeforeDelUsersFromGroups(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeDelUsersFromGroups(xmlDoc, Context);
		}

		public void OnDelUsersFromGroups(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnDelUsersFromGroups(xmlDoc, Context);
		}

		public void OnBeforeAddObjectsToGroups(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeAddObjectsToGroups(xmlDoc, Context);
		}

		public List<XmlDocument> OnAddObjectsToGroups(XmlDocument xmlDoc)
		{
			List<XmlDocument> xmlDocList = new List<XmlDocument>();
			foreach (OguDataOperationEventContainer container in EventContainers)
				xmlDocList.Add(container.OnAddObjectsToGroups(xmlDoc, Context));
			return xmlDocList;
		}

		public void OnBeforeSetSecsToLeader(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeSetSecsToLeader(xmlDoc, Context);
		}

		public List<XmlDocument> OnSetSecsToLeader(XmlDocument xmlDoc)
		{
			List<XmlDocument> xmlDocList = new List<XmlDocument>();
			foreach (OguDataOperationEventContainer container in EventContainers)
				xmlDocList.Add(container.OnSetSecsToLeader(xmlDoc, Context));
			return xmlDocList;
		}

		public void OnBeforeDelSecsOfLeader(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnBeforeDelSecsOfLeader(xmlDoc, Context);
		}

		public void OnDelSecsOfLeader(XmlDocument xmlDoc)
		{
			foreach (OguDataOperationEventContainer container in EventContainers)
				container.OnDelSecsOfLeader(xmlDoc, Context);
		}

	}
	#endregion
}
