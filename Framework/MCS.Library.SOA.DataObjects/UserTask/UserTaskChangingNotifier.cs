using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 用户消息改变的通知器
    /// </summary>
    public sealed class UserTaskChangingNotifier : IUserTaskOperation
    {
        private UserTaskChangingNotifier()
        {
        }

        private static Dictionary<string, string> TaskChangedUserIDs
        {
            get
            {
                Dictionary<string, string> result = (Dictionary<string, string>)ObjectContextCache.Instance.GetOrAddNewValue("TaskChangedUserIDs",
                    (cache, key) =>
                    {
                        Dictionary<string, string> item = new Dictionary<string, string>();

                        cache.Add(key, item);

                        return item;
                    });

                return result;
            }
        }

        private static bool TransactionEventAttached
        {
            get
            {
                bool result = false;

                SwitchContextCache.Instance.TryGetValue("TransactionEventAttached", out result);

                return result;
            }
            set
            {
                SwitchContextCache.Instance["TransactionEventAttached"] = value;
            }
        }

        #region IUserTaskOperation 成员
        public void Init(UserTaskOpEventContainer eventContainer)
        {
            eventContainer.BeforeDeleteUserTasks += new BeforeDeleteUserTaskDelegete(eventContainer_BeforeDeleteUserTasks);
            eventContainer.BeforeSendUserTasks += new BeforeSendUserTasksDelegete(eventContainer_BeforeSendUserTasks);
            eventContainer.BeforeSetUserTasksAccomplished += new BeforeSetUserTasksAccomplishedDelegete(eventContainer_BeforeSetUserTasksAccomplished);
            eventContainer.BeforeUpdateUserTask += new BeforeUpdateUserTaskDelegete(eventContainer_BeforeUpdateUserTask);

            eventContainer.DeleteUserAccomplishedTasks += new DeleteUserAccomplishedTasksDelegete(eventContainer_DeleteUserAccomplishedTasks);
            eventContainer.DeleteUserTasks += new DeleteUserTasksDelegete(eventContainer_DeleteUserTasks);
            eventContainer.SendUserTasks += new SendUserTasksDelegete(eventContainer_SendUserTasks);
            eventContainer.UpdateUserTask += new UpdateUserTaskDelegete(eventContainer_UpdateUserTask);
            eventContainer.SetUserTasksAccomplished += new SetUserTasksAccomplishedDelegete(eventContainer_SetTaskAccomplished);
        }
        #endregion

        #region Event Handler
        private void eventContainer_BeforeUpdateUserTask(UserTask task, UserTaskIDType idType, UserTaskFieldDefine fields, Dictionary<object, object> context)
        {
            if ((fields & ~UserTaskFieldDefine.ReadTime) != UserTaskFieldDefine.None)
            {
                ProcessTaskByIDType(task, idType);
            }
        }

        private void eventContainer_BeforeSetUserTasksAccomplished(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(tasks != null, "tasks");

            tasks.ForEach(task => TaskChangedUserIDs[task.SendToUserID] = task.SendToUserID);
        }

        private void eventContainer_BeforeSendUserTasks(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(tasks != null, "tasks");

            tasks.ForEach(task => TaskChangedUserIDs[task.SendToUserID] = task.SendToUserID);
        }

        private void eventContainer_BeforeDeleteUserTasks(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(tasks != null, "tasks");

            tasks.ForEach(task => TaskChangedUserIDs[task.SendToUserID] = task.SendToUserID);
        }

        private void eventContainer_SetTaskAccomplished(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            AttachTransactionEvent();
        }

        private int eventContainer_UpdateUserTask(UserTask task, UserTaskIDType idType, UserTaskFieldDefine fields, Dictionary<object, object> context)
        {
            AttachTransactionEvent();

            return 0;
        }

        private void eventContainer_SendUserTasks(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            AttachTransactionEvent();
        }

        private void eventContainer_DeleteUserTasks(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            AttachTransactionEvent();
        }

        private void eventContainer_DeleteUserAccomplishedTasks(UserTaskCollection tasks, Dictionary<object, object> context)
        {
            AttachTransactionEvent();
        }
        #endregion

        private static void ProcessTaskByIDType(UserTask task, UserTaskIDType idType)
        {
            if ((idType & UserTaskIDType.SendToUserID) != UserTaskIDType.None)
                TaskChangedUserIDs[task.SendToUserID] = task.SendToUserID;

            UserTaskIDType otherType = idType & ~UserTaskIDType.SendToUserID;

            if (otherType != UserTaskIDType.None)
            {
                UserTaskCollection tasks = GetUserTasks(task, otherType);

                foreach (UserTask oneTask in tasks)
                    TaskChangedUserIDs[oneTask.SendToUserID] = oneTask.SendToUserID;
            }
        }

        private static UserTaskCollection GetUserTasks(UserTask task, UserTaskIDType idType)
        {
            UserTaskCollection result = new UserTaskCollection();

            foreach (EnumItemDescription desp in EnumItemDescriptionAttribute.GetDescriptionList(typeof(UserTaskIDType)))
            {
                UserTaskIDType enumIdType = (UserTaskIDType)desp.EnumValue;

                if ((enumIdType & idType) != UserTaskIDType.None && enumIdType != UserTaskIDType.SendToUserID)
                {
                    UserTaskIDType targetIDType = (UserTaskIDType)desp.EnumValue | (idType & UserTaskIDType.SendToUserID);

                    UserTaskCollection tasks = UserTaskAdapter.Instance.GetUserTasks(
                                targetIDType,
                                UserTaskFieldDefine.TaskID | UserTaskFieldDefine.SendToUserID | UserTaskFieldDefine.ActivityID,
                                true,
                                DefaultUserTaskOperationImpl.GetUserTaskIDValue(task, targetIDType));

                    result.CopyFrom(tasks);
                }
            }

            return result;
        }

        private static void AttachTransactionEvent()
        {
            if (Transaction.Current != null && TransactionEventAttached == false)
            {
                TransactionScopeFactory.AttachCommittedAction(new Action<TransactionEventArgs>(Current_TransactionCompleted), false);

                TransactionEventAttached = true;
            }
        }

        private static void Current_TransactionCompleted(TransactionEventArgs e)
        {
            try
            {
                List<CacheNotifyData> dataList = new List<CacheNotifyData>();

                foreach (KeyValuePair<string, string> kp in TaskChangedUserIDs)
                {
                    CacheNotifyData notifyData = new CacheNotifyData(typeof(UserTaskChangingCache), kp.Key, CacheNotifyType.Update);

                    notifyData.CacheData = Guid.NewGuid().ToString();

                    dataList.Add(notifyData);
                }

                CacheNotifyData[] notifyArray = dataList.ToArray();

                UdpCacheNotifier.Instance.SendNotifyAsync(notifyArray);
                MmfCacheNotifier.Instance.SendNotify(notifyArray);
            }
            finally
            {
                TaskChangedUserIDs.Clear();

                TransactionEventAttached = false;
            }
        }
    }
}
