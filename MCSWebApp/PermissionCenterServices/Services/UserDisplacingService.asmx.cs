using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Workflow;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using System.Text;
using System.Transactions;

namespace PermissionCenter.Services
{
	/// <summary>
	/// 人员替换的服务
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
	[System.Web.Script.Services.ScriptService]
	public class UserDisplacingService : System.Web.Services.WebService
	{

		/// <summary>
		/// 替换角色矩阵中的固定人员
		/// </summary>
		/// <param name="roleID">角色矩阵ID</param>
		/// <param name="userID">被替换的人员ID</param>
		/// <param name="roleIDArray">一个字符串数组，每个表示一个人员ID，表示用于取代被替换人员的人员。如果此数组为空数组，则仅移除人员。</param>
		/// <returns>1表示执行了替换，0表示未执行</returns>
		/// <remarks>如果人员ID或者角色矩阵ID无效，则不会进行任何替换，直接返回0。</remarks>
		[WebMethod(Description = "替换角色中固定人员<br />roleID:指定的角色的ID，在此角色中进行人员替换。如果为空或空字符串，则替换所有角色<br />userID:被替换的人员ID，必选。<br/>displacingUserIDArray：一个数组，其中包含角色中将出现的替换人员的ID。如果为空数组，则仅移除人员。<br/>返回值1表示替换成功，0表示未进行替换，可能传入的参数有误。")]
		public int DisplaceUserInRole(string roleID, string userID, string[] displacingUserIDArray)
		{
			return Adapters.QueryByUserAdapter.Instance.DisplaceUserInRole(roleID, userID, displacingUserIDArray);
		}

		[WebMethod(Description = "替换流程定义中人员<br />processDespKey:指定的流程定义的ID，在此流程定义中进行人员替换。<br />userID:被替换的人员ID，必选。<br/>displacingUserIDArray：一个数组，其中包含流程定义中将出现的替换人员的ID。如果为空数组，则仅移除人员。<br/>返回值1表示替换成功，0表示未进行替换，可能传入的参数有误。")]
		public int DisplaceUserInProcessDescriptor(string processDespKey, string userID, string[] displacingUserIDArray)
		{
			IWfProcessDescriptor processDesp = WfProcessDescriptorManager.LoadDescriptor(processDespKey);

			IUser originalUser = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userID).FirstOrDefault();
			OguObjectCollection<IUser> displacingUsers = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, displacingUserIDArray);

			List<IUser> distinctUsers = new List<IUser>();

			foreach (IUser user in displacingUsers)
			{
				if (distinctUsers.Exists(u => u.ID == user.ID) == false)
					distinctUsers.Add(user);
			}

			int result = processDesp.ReplaceAllUserResourceDescriptors(originalUser, distinctUsers);

			WfProcessDescriptorManager.SaveDescriptor(processDesp);

			return result;
		}

		[WebMethod(Description = "替换流程实例中人员<br />processID:指定的流程实例的ID，在此流程实例中进行人员替换。<br />userID:被替换的人员ID，必选。<br/>displacingUserIDArray：一个数组，其中包含流程实例中将出现的替换人员的ID。如果为空数组，则仅移除人员。<br/>返回值1表示替换成功，0表示未进行替换，可能传入的参数有误。")]
		public int DisplaceUserInProcess(string processID, string userID, string[] displacingUserIDArray)
		{
			IUser originalUser = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userID).FirstOrDefault();

			OguObjectCollection<IUser> displacingUsers = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, displacingUserIDArray);

			List<IUser> distinctUsers = new List<IUser>();

			foreach (IUser user in displacingUsers)
			{
				if (distinctUsers.Exists(u => u.ID == user.ID) == false)
					distinctUsers.Add(user);
			}

			IWfProcess process = WfRuntime.GetProcessByProcessID(processID);

			int result = 0;

			foreach (IWfActivity activity in process.Activities)
			{
				bool needReplace = false;

				if (activity.Status != WfActivityStatus.Completed && activity.Status != WfActivityStatus.Aborted)
				{
					if (activity.Status == WfActivityStatus.Running || activity.Status == WfActivityStatus.Pending)
						needReplace = activity.Assignees.Contains(originalUser.ID);
					else
						needReplace = activity.Candidates.Contains(originalUser.ID);

					if (needReplace)
					{
						WfReplaceAssigneesExecutor executor = new WfReplaceAssigneesExecutor(null, activity, originalUser, distinctUsers);

						executor.ExecuteNotPersist();
						result++;
					}
				}
			}

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				WfReplaceAssigneesExecutor.DoUserTaskOperations();
				WfRuntime.PersistWorkflows();

				scope.Complete();
			}

			return result;
		}

		[WebMethod(Description = "替换角色矩阵中人员<br />roleID:指定的角色矩阵的ID，在此角色矩阵中进行人员替换。<br />userID:被替换的人员ID，必选。<br/>displacingUserIDArray：一个数组，其中包含角色矩阵中将出现的替换人员的ID。如果为空数组，则仅移除人员。<br/>返回值1表示替换成功，0表示未进行替换，可能传入的参数有误。")]
		public int DisplaceUserInRoleMatrix(string roleID, string userID, string[] displacingUserIDArray)
		{
			IUser originalUser = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userID).FirstOrDefault();
			OguObjectCollection<IUser> displacingUsers = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, displacingUserIDArray);

			string originalUserCodeName = originalUser != null ? originalUser.LogOnName : string.Empty;

			List<string> desplacingUserCodeNames = new List<string>();

			foreach (IUser user in displacingUsers)
			{
				if (desplacingUserCodeNames.Contains(user.LogOnName) == false)
					desplacingUserCodeNames.Add(user.LogOnName);
			}

			SOARolePropertyRowCollection rows = SOARolePropertiesAdapter.Instance.LoadByRoleID(roleID, null);

			int result = rows.ReplaceOperators(SOARoleOperatorType.User, originalUserCodeName, desplacingUserCodeNames.ToArray());

			SOARolePropertiesAdapter.Instance.Update(roleID, rows);

			return result;
		}

		[WebMethod(Description = "替换权限矩阵中人员<br />processDespKey:指定的权限矩阵对应流程定义的ID，在此权限矩阵中进行人员替换。<br />userID:被替换的人员ID，必选。<br/>displacingUserIDArray：一个数组，其中包含权限矩阵中将出现的替换人员的ID。如果为空数组，则仅移除人员。<br/>返回值1表示替换成功，0表示未进行替换，可能传入的参数有误。")]
		public int DisplaceUserInProcessMatrix(string processDespKey, string userID, string[] displacingUserIDArray)
		{
			WfMatrix matrix = WfMatrixAdapter.Instance.LoadByProcessKey(processDespKey, true);

			IUser originalUser = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userID).FirstOrDefault();
			OguObjectCollection<IUser> displacingUsers = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, displacingUserIDArray);

			string originalUserCodeName = originalUser != null ? originalUser.LogOnName : string.Empty;

			List<string> desplacingUserCodeNames = new List<string>();

			foreach (IUser user in displacingUsers)
			{
				if (desplacingUserCodeNames.Contains(user.LogOnName) == false)
					desplacingUserCodeNames.Add(user.LogOnName);
			}

			int result = matrix.Rows.ReplcaeOperators(WfMatrixOperatorType.Person, originalUserCodeName, desplacingUserCodeNames.ToArray());

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				WfMatrixAdapter.Instance.Update(matrix);

				scope.Complete();
			}

			return result;
		}
	}
}
