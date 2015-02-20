using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Logging;
using MCS.Library.SOA.Security.ADSyncUtilities.Adapters;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class LogHelper
	{
		private static readonly MCS.Library.Logging.Logger _Logger = LoggerFactory.Create("PermissionCenterToADSynchronize");
		private static readonly int _EventID = 9999;

		public static void WriteEventLog(string title, string message, LogPriority logPriority, TraceEventType traceEventType)
		{
			_Logger.Write(message, logPriority, _EventID, traceEventType, title);
		}

		public static void WriteSynchronizeDBLog(string operatorID, string operatorName)
		{
			SynchronizeContext context = SynchronizeContext.Current;

			if (context.SynchronizeResult == ADSynchronizeResult.Correct && context.ExceptionCount > 0)
			{
				context.SynchronizeResult = ADSynchronizeResult.HasError;
			}

			if (context.LogEntity == null)
			{
				context.LogEntity = new ADSynchronizeLog()
				{
					LogID = Guid.NewGuid().ToString(),
					SynchronizeID = context.SynchronizeID,
					StartTime = DateTime.Now,
					OperatorID = operatorID,
					OperatorName = operatorName,
					ExceptionCount = context.ExceptionCount,
					SynchronizeResult = context.SynchronizeResult,
					AddingItemCount = context.AddingItemCount,
					DeletingItemCount = context.DeletingItemCount,
					ModifyingItemCount = context.ModifyingItemCount,
					AddedItemCount = context.AddedItemCount,
					DeletedItemCount = context.DeletedItemCount,
					ModifiedItemCount = context.ModifiedItemCount
				};
			}
			else
			{
				context.LogEntity.EndTime = DateTime.Now;
				context.LogEntity.ExceptionCount = context.ExceptionCount;
				context.LogEntity.SynchronizeResult = context.SynchronizeResult;
				context.LogEntity.AddingItemCount = context.AddingItemCount;
				context.LogEntity.DeletingItemCount = context.DeletingItemCount;
				context.LogEntity.ModifyingItemCount = context.ModifyingItemCount;
				context.LogEntity.AddedItemCount = context.AddedItemCount;
				context.LogEntity.DeletedItemCount = context.DeletedItemCount;
				context.LogEntity.ModifiedItemCount = context.ModifiedItemCount;
			}

			ADSynchronizeLogAdapter.Instance.Update(context.LogEntity);
		}

		public static void WriteSynchronizeDBLogDetail(string synchronizeID, string actionName, string scObjectID, string scObjectName, string adObjectID, string adObjectName, string detail)
		{
			ADSynchronizeLogDetail log = new ADSynchronizeLogDetail()
			{
				LogDetailID = Guid.NewGuid().ToString(),
				SynchronizeID = synchronizeID,
				SCObjectID = scObjectID,
				SCObjectName = scObjectName,
				ADObjectID = adObjectID,
				ADObjectName = adObjectName,
				ActionName = actionName,
				Detail = detail
			};

			ADSynchronizeLogDetailAdapter.Instance.Update(log);
		}

		public static void WriteReverseSynchronizeDBLogDetail(string synchronizeID, string scObjectID, string adObjectID, string adObjectName, string summary, string detail)
		{
			ADReverseSynchronizeLogDetail log = new ADReverseSynchronizeLogDetail()
			{
				LogDetailID = Guid.NewGuid().ToString(),
				LogID = synchronizeID,
				SCObjectID = scObjectID,
				ADObjectID = adObjectID,
				ADObjectName = adObjectName,
				CreateTime = DateTime.Now,
				Summary = summary,
				Detail = detail,
			};

			ADReverseSynchronizeLogDetailAdapter.Instance.Update(log);
		}

		public static void WriteSynchronizeExceptionDBLogDetail(string synchronizeID, string actionName, string scObjectID, string scObjectName, string adObjectID, string adObjectName, Exception ex)
		{
			WriteSynchronizeDBLogDetail(synchronizeID, actionName, scObjectID, scObjectName, adObjectID, adObjectName, ex.ToString());
		}
	}
}