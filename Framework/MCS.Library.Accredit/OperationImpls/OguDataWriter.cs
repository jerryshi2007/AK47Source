using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Accredit.Configuration;
using System.Xml;
using System.Transactions;
using MCS.Library.Data;

namespace MCS.Library.Accredit
{
	/// <summary>
	/// 
	/// </summary>
	public static class OguDataWriter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static void UpdateObjects(XmlDocument xmlDoc)
		{
			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeUpdateObjects(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				dataOpContext.OnUpdateObjects(xmlDoc);
				scope.Complete();
			}
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDoc"></param>
        public static void InsertObjects(XmlDocument xmlDoc)
        {
            DataOpContext dataOpContext = InitEventContexts();

            dataOpContext.OnBeforeInsertObjects(xmlDoc);

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                dataOpContext.OnInsertObjects(xmlDoc);
                scope.Complete();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDoc"></param>
        public static void LogicDeleteObjects(XmlDocument xmlDoc)
        {
            DataOpContext dataOpContext = InitEventContexts();

            dataOpContext.OnBeforeLogicDeleteObjects(xmlDoc);

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                dataOpContext.OnLogicDeleteObjects(xmlDoc);
                scope.Complete();
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static void FurbishDeleteObjects(XmlDocument xmlDoc)
		{
			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeFurbishDeleteObjects(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				dataOpContext.OnFurbishDeleteObjects(xmlDoc);
				scope.Complete();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static void RealDeleteObjects(XmlDocument xmlDoc)
		{
			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeRealDeleteObjects(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				dataOpContext.OnRealDeleteObjects(xmlDoc);
				scope.Complete();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static void InitPassword(XmlDocument xmlDoc)
		{
			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeInitPassword(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				dataOpContext.OnInitPassword(xmlDoc);
				scope.Complete();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static void ResetPassword(XmlDocument xmlDoc)
		{
			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeResetPassword(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				dataOpContext.OnResetPassword(xmlDoc);
				scope.Complete();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static void MoveObjects(XmlDocument xmlDoc)
		{
			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeMoveObjects(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				dataOpContext.OnMoveObjects(xmlDoc);
				scope.Complete();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static void SortObjects(XmlDocument xmlDoc)
		{
			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeSortObjects(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				dataOpContext.OnSortObjects(xmlDoc);
				scope.Complete();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static XmlDocument GroupSortObjects(XmlDocument xmlDoc)
		{
			List<XmlDocument> xmlDocList = new List<XmlDocument>();
			XmlDocument result = null;

			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeGroupSortObjects(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				xmlDocList = dataOpContext.OnGroupSortObjects(xmlDoc);
				scope.Complete();
			}
			if (xmlDocList.Count == 1)
				result = xmlDocList[0];	

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static void SetUserMainDuty(XmlDocument xmlDoc)
		{
			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeSetUserMainDuty(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				dataOpContext.OnSetUserMainDuty(xmlDoc);
				scope.Complete();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static void DelUsersFromGroups(XmlDocument xmlDoc)
		{
			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeDelUsersFromGroups(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				dataOpContext.OnDelUsersFromGroups(xmlDoc);
				scope.Complete();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static XmlDocument AddObjectsToGroups(XmlDocument xmlDoc)
		{
			List<XmlDocument> xmlDocList = new List<XmlDocument>();
			XmlDocument result = null;

			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeAddObjectsToGroups(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				xmlDocList = dataOpContext.OnAddObjectsToGroups(xmlDoc);
				scope.Complete();
			}

			if (xmlDocList.Count == 1)
				result = xmlDocList[0];
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static XmlDocument SetSecsToLeader(XmlDocument xmlDoc)
		{
			List<XmlDocument> xmlDocList = new List<XmlDocument>();
			XmlDocument result = null;

			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeSetSecsToLeader(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				xmlDocList = dataOpContext.OnSetSecsToLeader(xmlDoc);
				scope.Complete();
			}

			if (xmlDocList.Count == 1)
				result = xmlDocList[0];
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		public static void DelSecsOfLeader(XmlDocument xmlDoc)
		{
			DataOpContext dataOpContext = InitEventContexts();

			dataOpContext.OnBeforeDelSecsOfLeader(xmlDoc);

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				dataOpContext.OnDelSecsOfLeader(xmlDoc);
				scope.Complete();
			}
		}


		private static DataOpContext InitEventContexts()
		{
			DataOpContext contexts = new DataOpContext();

			foreach (IOguDataOperation op in OguDataOperationSettings.GetConfig().Operations)
			{
				OguDataOperationEventContainer container = new OguDataOperationEventContainer();

				op.Init(container);
				contexts.EventContainers.Add(container);
			}

			return contexts;
		} 
	}
}
