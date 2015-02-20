using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects.Archive
{
	public class UserAccomplishedTaskArchiveOperation : IArchiveOperation
	{
		public static readonly UserAccomplishedTaskArchiveOperation Instance = new UserAccomplishedTaskArchiveOperation();

		private UserAccomplishedTaskArchiveOperation()
		{
		}

		#region IArchiveOperation Members

		public void LoadOriginalData(ArchiveBasicInfo info)
		{
			//暂时不处理已办
			/*
			info.Context["UserAccomplishedTasks"] =
				UserTaskAdapter.Instance.GetUserAccomplishedTasks(UserTaskIDType.ResourceID, UserTaskFieldDefine.All, false, info.ResourceID);
			 */
		}

		public void SaveArchiveData(ArchiveBasicInfo info)
		{
			//暂时不处理已办
			/*
			ORMappingItemCollection mappings = ORMapping.GetMappingInfo<UserTask>().Clone();

			mappings["COMPLETED_TIME"].BindingFlags |= (ClauseBindingFlags.Update | ClauseBindingFlags.Insert);
			ORMappingContextCache.Instance[typeof(UserTask)] = mappings;

			try
			{
				info.Context.DoAction<UserTaskCollection>("UserAccomplishedTasks", uc =>
				{
					//UserTaskAdapter.Instance.SaveUserAccomplishedTasks(uc);
				});
			}
			finally
			{
				ORMappingContextCache.Instance.Remove(typeof(UserTask));
			}*/
		}

		public void DeleteOriginalData(ArchiveBasicInfo info)
		{
			//暂时不处理已办
			/*
			info.Context.DoAction<UserTaskCollection>("UserAccomplishedTasks", uc =>
			{
				UserTaskAdapter.Instance.DeleteUserAccomplishedTasks(uc);
			});
			*/
		}

		#endregion
	}
}
