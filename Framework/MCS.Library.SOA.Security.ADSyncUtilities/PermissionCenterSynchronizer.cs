using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using MCS.Library;
using MCS.Library.Configuration;
using MCS.Library.Core;
using MCS.Library.Logging;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;


namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class PermissionCenterSynchronizer
	{
		public void Start(string startPath)
		{
			Start(startPath, UuidHelper.NewUuidString());
		}

		public void Start(string startPath, string taskID)
		{
			SynchronizeContext.Clear();
#if NOLOCK
#else
			SynchronizeContext.Current.AddLock();
#endif
			try
			{
				if (!MCS.Library.SOA.DataObjects.Security.Adapters.SCSnapshotHelper.Instance.ValidateGroups(DateTime.MinValue))
					throw new InvalidOperationException("检查到群组数据有误，一个群组不能属于2个组织。请联系DBA。");

				taskID = string.IsNullOrEmpty(taskID) ? UuidHelper.NewUuidString() : taskID;
				ServiceBrokerContext.Current.SaveContextStates();

				try
				{
					//禁用权限中心的缓存
					ServiceBrokerContext.Current.UseServerCache = false;
					ServiceBrokerContext.Current.UseLocalCache = false;

					//初始化上下文和权限中心的初始组织
					IOrganization startOrg = PrepareStartOrganization(startPath, taskID);

					//开始对比权限中心和AD的数据差异
					StartRecursiveProcess(startOrg);

					//保存差异的数据和ID映射表
					PersistSynchornizeInfo();

					SynchronizeContext.Current.SynchronizeResult = ADSynchronizeResult.Correct;
				}
				catch (Exception ex)
				{
					SynchronizeContext.Current.SynchronizeResult = ADSynchronizeResult.Interrupted;
					LogHelper.WriteEventLog("未识别或致命异常", ex.GetRealException().ToString(), LogPriority.Highest, TraceEventType.Error);
				}
				finally
				{
					ServiceBrokerContext.Current.RestoreSavedStates();
					LogHelper.WriteSynchronizeDBLog("PermissionCenterServices", "PermissionCenterServices");
					LogHelper.WriteEventLog("同步结束", "同步结束", LogPriority.Lowest, TraceEventType.Start);
				}
			}
			finally
			{
#if NOLOCK
#else
				SynchronizeContext.Current.DeleteLock();
#endif
#if UNITTEST
#else
				SynchronizeContext.Clear();
#endif

			}
		}

		//从配置信息中初始化同步上下文
		private void InitContext(string startPath)
		{
			SynchronizeContext context = SynchronizeContext.Current;
			context.SynchronizeResult = ADSynchronizeResult.Correct;

			//初始化域控制器的配置信息
			ServerInfoConfigureElement serverSetting = ServerInfoConfigSettings.GetConfig().ServerInfos["dc"];
			ServerInfo serverInfo = serverSetting == null ? null : serverSetting.ToServerInfo();

			PermissionCenterToADSynchronizeSettings config = PermissionCenterToADSynchronizeSettings.GetConfig();

			context.SourceRootPath = config.SourceRoot;
			//初始化（权限中心）开始同步的路径和回收站路径
			string path = startPath.IsNullOrEmpty() ? config.DefaultStartPath : startPath;

			if (string.IsNullOrEmpty(path))
				path = context.SourceRootPath;

			if (path != context.SourceRootPath && path.StartsWith(context.SourceRootPath + "\\", StringComparison.Ordinal) == false)
				throw new System.Configuration.ConfigurationErrorsException("开始同步路径必须位于同步根范围内");

			context.StartPath = path;
			context.RecycleBinOU = config.RecycleBinOU;

			//默认用户口令
			context.DefaultPassword = config.DefaultPassword;

			context.TargetRootOU = config.TargetRootOU;
			context.ADHelper = ADHelper.GetInstance(serverInfo);

			//初始化权限中心和AD的ID映射关系
			context.ExtendLockTime();
			context.IDMapper.Initialize();

			context.ClearIncludeMappings();

			foreach (ObjectMappingElement elem in config.ObjectMappings)
			{
				context.AddIncludeMapping(elem.SCObjectName, elem.ADObjectName);
			}
		}

		/// <summary>
		/// 开始递归处理
		/// </summary>
		/// <param stringValue="startOrg"></param>
		private void StartRecursiveProcess(IOrganization startOrg)
		{
			SynchronizeContext.Current.WriteExceptionDBLogIfError("递归处理", () =>
			{
				RecursiveProcess(startOrg);
			});
		}

		/// <summary>
		/// 保存需要变更的数据
		/// </summary>
		private void PersistSynchornizeInfo()
		{
			SynchronizeContext.Current.NormalizeModifiedItems();
			ADObjectModifier.ApplyModify();

			SynchronizeContext.Current.CurrentOguObject = null;
			SynchronizeContext.Current.WriteExceptionDBLogIfError("更新IDMapping", () => SynchronizeContext.Current.IDMapper.UpdateIDMapping());
		}

		#region 预处理和初始化
		/// <summary>
		/// 准备权限中心的开始组织
		/// </summary>
		/// <param stringValue="startPath"></param>
		/// <returns></returns>
		private IOrganization PrepareStartOrganization(string startPath, string taskID)
		{
			IOrganization startOrg = null;

			SynchronizeContext.Current.WriteExceptionDBLogIfError("初始化", () =>
			{
				SynchronizeContext.Current.SynchronizeResult = ADSynchronizeResult.Running;
				SynchronizeContext.Current.SynchronizeID = taskID;

				LogHelper.WriteEventLog("同步开始", "同步开始", LogPriority.Lowest, TraceEventType.Start);
				LogHelper.WriteSynchronizeDBLog("PermissionCenterServices", "PermissionCenterServices");

				SynchronizeContext.Current.ExtendLockTime();

				InitContext(startPath);

				SynchronizeContext.Current.ExtendLockTime();
				EnsureRecycleBin(); // 检查并创建回收站

				EnsureADTarget(); // 确保AD中有对应节点

				SynchronizeContext.Current.ExtendLockTime();
				startOrg = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.FullPath, SynchronizeContext.Current.StartPath).FirstOrDefault();

				EnsureRootOU(startOrg); //确保开始路径对应的机构的父节点都存在，如果不存在，则首先添加到变更列表里。
			});

			return startOrg;
		}

		/// <summary>
		/// 保证在AD中，回收站已经存在
		/// </summary>
		private void EnsureRecycleBin()
		{
			if (string.IsNullOrEmpty(SynchronizeContext.Current.RecycleBinOU))
				throw new ApplicationException("回收站配置不得为空");

			string dn = SynchronizeContext.Current.RecycleBinOU;

			EnsureOU(dn, SynchronizeContext.Current.ADHelper);
		}

		private static void EnsureOU(string dn, ADHelper adHelper)
		{
			if (string.IsNullOrEmpty(dn) == false)
			{
				string[] rdns = new RdnSequencePartEnumerator(dn).ToArray();

				for (int i = rdns.Length - 1; i >= 0; i--)
				{
					if (rdns[i].StartsWith("OU=", StringComparison.Ordinal) == false)
						throw new FormatException("DN格式错误。正确的格式为OU=ou1[,OU=ouBase][,...]");
				}

				string lastRoot = null;
				for (int i = rdns.Length - 1; i >= 0; i--)
				{
					string root = rdns[i] + (lastRoot == null ? null : "," + lastRoot);
					if (adHelper.EntryExists(root) == false)
					{
						using (DirectoryEntry entry = string.IsNullOrEmpty(lastRoot) ? adHelper.GetRootEntry() : adHelper.NewEntry(lastRoot))
						{
							using (DirectoryEntry here = entry.Children.Add(rdns[i], "organizationalUnit"))
							{
								here.CommitChanges();
							}
						}
					}

					lastRoot = root;
				}
			}
		}

		private void EnsureRootOU(IOrganization startOrg)
		{
			if (SynchronizeContext.Current.IsIncluded(startOrg) == false)
				throw new InvalidOperationException("同步根不在同步范围内");

			List<string> parentPaths = SynchronizeHelper.GetAllParentPaths(startOrg.FullPath);

			if (parentPaths.Count > 0)
			{
				foreach (string path in parentPaths)
				{
					if (path == SynchronizeContext.Current.SourceRootPath)
						continue; // 根节点不应该被处理

					var curOrg = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.FullPath, path).FirstOrDefault();
					if (SynchronizeContext.Current.IsIncluded(curOrg))
					{
						ADObjectWrapper adObject = ADObjectFinder.Find(curOrg, true);

						if (adObject == null) //这里判断找不到的情况,则加到变更列表
						{
							SynchronizeContext.Current.PrepareAndAddModifiedItem(curOrg, adObject, ObjectModifyType.Add);
						}
						else//否则需要比较一下路径对不对，有可能按ID找到了，但路径变了
						{
							if (!ObjectComparerHelper.ArePathEqaul(curOrg, adObject))
							{
								SynchronizeContext.Current.PrepareAndAddModifiedItem(curOrg, adObject, ObjectModifyType.PropertyModified);
							}
						}
					}
				}
			}
		}

		private static void EnsureADTarget()
		{
			//确定AD中，同步根根节点必须存在，不存在也不会新建
			if (string.IsNullOrEmpty(SynchronizeContext.Current.TargetRootOU) == false)
				if (SynchronizeContext.Current.ADHelper.EntryExists(SynchronizeContext.Current.TargetRootOU) == false)
					throw new DirectoryEntryNotFoundException(SynchronizeContext.Current.TargetRootOU);

			string rootPostfix = SynchronizeContext.Current.TargetRootOU.IsNullOrEmpty() ? string.Empty : "," + SynchronizeContext.Current.TargetRootOU;
			foreach (string item in SynchronizeContext.Current.AdTargets)
			{
				string dn = item + rootPostfix;
				if (SynchronizeContext.Current.ADHelper.EntryExists(dn) == false)
					throw new DirectoryEntryNotFoundException(dn);
			}
		}

		#endregion 预处理和初始化

		#region 同步信息处理
		private void RecursiveProcess(IOguObject oguObject)
		{
			if (oguObject != null && SynchronizeContext.Current.IsIncluded(oguObject))
			{
				SynchronizeContext.Current.ExtendLockTime();

				SynchronizeContext.Current.CurrentOguObject = oguObject;

				Debug.WriteLine("递归：OGU对象：" + oguObject.FullPath);

				//AD中是否找到对应的权限中心的对象
				ADObjectWrapper adObject = ADObjectFinder.Find(oguObject);

				if (oguObject.ObjectType == SchemaType.Groups)
				{
					SynchronizeContext.Current.PrepareGroupToTakeCare(oguObject); //需注意的群组
				}

				if (adObject == null) //没找到,新增
				{
					if (SynchronizeContext.Current.IsRealObject(oguObject))
					{
						Trace.WriteLine("AD中不存在决定新增" + oguObject.FullPath);
						SynchronizeContext.Current.PrepareAndAddModifiedItem(oguObject, adObject, ObjectModifyType.Add);
					}
					else
					{
						Trace.WriteLine("不要新增AD中实际不存在的对象" + oguObject.FullPath);
					}
				}
				else
				{
					if (SynchronizeContext.Current.IsRealObject(oguObject))
					{
						//比较已经找到的两个对象的差异
						Trace.Write("AD中存在,比较差异……");
						//比较差异，也要考虑时间戳是否变化
						ObjectModifyType compareResult = ObjectComparerHelper.Compare(oguObject, adObject);

						Trace.WriteLine(compareResult);

						if (compareResult != ObjectModifyType.None) // 修改
							SynchronizeContext.Current.PrepareAndAddModifiedItem(oguObject, adObject, compareResult);
					}
					else
					{
						Trace.WriteLine("实际不存在，该删掉的对象：" + oguObject.FullPath);
					}
				}

				if (oguObject.ObjectType == SchemaType.Organizations)
				{
					//组织要检查是否有删除项，然后递归
					ProcessOrganization((IOrganization)oguObject, adObject);
				}
			}
		}

		private void ProcessOrganization(IOrganization organization, ADObjectWrapper adObject)
		{
			SynchronizeContext.Current.ExtendLockTime();
			IList<IOrganization> allOrganizationChildren = GetObjectDetailInfo<IOrganization>(organization.Children);

			SynchronizeContext.Current.ExtendLockTime();
			IList<IUser> allUserChildren = GetUserDetailInfo(organization.Children);

			SynchronizeContext.Current.ExtendLockTime();
			IList<IGroup> allGroupChildren = GetObjectDetailInfo<IGroup>(organization.Children);

			//如果AD中有对应的组织
			if (adObject != null)
			{
				//在AD中找对应的子对象
				List<ADObjectWrapper> allADChildren = SynchronizeHelper.SearchChildren(adObject);

				Trace.WriteLine("检查AD中的子项，共" + allADChildren.Count + "项");

				foreach (ADObjectWrapper child in allADChildren)
				{
					SynchronizeContext.Current.ExtendLockTime();

					if (child.Properties.Contains("displayNamePrintable"))
					{
						//肯定是群组了
						if (SynchronizeHelper.PermissionCenterInvolved.Equals(child.Properties["displayNamePrintable"]))
						{
							IOguObject subObj = OguObjectFinder.Find(child, organization.Children);

							if (subObj == null)
							{
								//在权限中心的组织下没有找到则删除项
								SynchronizeContext.Current.PrepareAndAddDeletedItem(subObj, child, ObjectModifyType.Delete);

								Trace.WriteLineIf(subObj == null, "无对应AD对象 " + child.DN + "的OGU对象，加入删除列表");
							}
						}
					}
					else
					{
						IOguObject subObj = OguObjectFinder.Find(child, organization.Children);

						if (subObj == null)
						{
							//在权限中心的组织下没有找到则删除项
							SynchronizeContext.Current.PrepareAndAddDeletedItem(subObj, child, ObjectModifyType.Delete);

							Trace.WriteLineIf(subObj == null, "无对应AD对象 " + child.DN + "的OGU对象，加入删除列表");
						}
					}
				}
			}

			allUserChildren.ForEach(child => RecursiveProcess(child));
			allGroupChildren.ForEach(child => RecursiveProcess(child));
			allOrganizationChildren.ForEach(child => RecursiveProcess(child));
		}

		/// <summary>
		/// 得到某一级组织下组织和群组的详细信息。
		/// 权限中心的Organization的Children属性，仅仅返回子对象的基本属性，这个方法是通过再查询一次权限中心，继而得到完整的属性
		/// </summary>
		/// <typeparam stringValue="T"></typeparam>
		/// <param stringValue="allOguChildren"></param>
		/// <returns></returns>
		private static IList<T> GetObjectDetailInfo<T>(OguObjectCollection<IOguObject> allOguChildren) where T : IOguObject
		{
			List<T> result = new List<T>();
			List<string> childrenIDs = new List<string>();

			foreach (IOguObject child in allOguChildren)
			{
				if (child is T)
					childrenIDs.Add(child.ID);
			}

			if (childrenIDs.Count > 0)
			{
				OguObjectCollection<T> detailList = OguMechanismFactory.GetMechanism().GetObjects<T>(SearchOUIDType.Guid, childrenIDs.ToArray());

				detailList.ForEach(obj => result.Add(obj));
			}

			return result;
		}

		/// <summary>
		/// 得到某一级组织下人员（带兼职）的详细信息。
		/// 权限中心的Organization的Children属性，仅仅返回子对象的基本属性，这个方法是通过再查询一次权限中心，继而得到完整的属性
		/// </summary>
		/// <param stringValue="allOguChildren"></param>
		/// <returns></returns>
		private static IList<IUser> GetUserDetailInfo(OguObjectCollection<IOguObject> allOguChildren)
		{
			List<string> childrenIDs = new List<string>();

			foreach (IOguObject child in allOguChildren)
			{
				if (child is IUser)
				{
					if (SynchronizeContext.Current.IsRealObject(child))
						childrenIDs.Add(child.ID);
				}
			}

			return OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, childrenIDs.ToArray()).ToList();
		}
		#endregion 同步信息处理
	}
}