using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using MCS.Library.Core;

namespace MCS.Library.Services
{
    using System.Threading;

    [Serializable]
    public class ServiceThread : IServiceThread, ISerializable
    {
        private ThreadParam param = null;
        private Thread currentThread = null;
        private System.Threading.ThreadState status = System.Threading.ThreadState.Unstarted;
        private CreateThreadDelegete callBack;
        private DateTime lastPollTime = DateTime.MinValue;
        private string lastExceptionMessage = string.Empty;
        private string lastMessage = string.Empty;

        private ReaderWriterLock pollTimeRWLock = new ReaderWriterLock();

        private ServiceThread(ThreadParam tp)
        {
            this.param = tp;
        }

        #region IServiceThread 成员

        public IThreadParam Params
        {
            get 
            {
                return this.param;
            }
        }

        public System.Threading.ThreadState Status
        {
            get 
            {
                if (this.currentThread != null)
                    this.status = this.currentThread.ThreadState;
                
                return this.status;
            }
        }

        public Thread CurrentThread
        {
            get
            {
                return this.currentThread;
            }
        }

        public DateTime LastPollTime
        {
            get
            {
                this.pollTimeRWLock.AcquireReaderLock(1000);
                try
                {
                    return this.lastPollTime;
                }
                finally
                {
                    this.pollTimeRWLock.ReleaseReaderLock();
                }
            }
        }

        public string LastExceptionMessage
        {
            get
            {
                return this.lastExceptionMessage;
            }
        }

        public string LastMessage
        {
            get
            {
                return this.lastMessage;
            }
        }

        #endregion

        #region ISerializable 成员

        private ServiceThread(SerializationInfo info, StreamingContext context)
        {
            this.status = (System.Threading.ThreadState)info.GetValue("Status", typeof(System.Threading.ThreadState));
            this.param = (ThreadParam)info.GetValue("Param", typeof(ThreadParam));
            this.lastPollTime = info.GetDateTime("LastPollTime");
            this.lastMessage = info.GetString("LastMessage");
            this.lastExceptionMessage = info.GetString("LastExceptionMessage");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Param", this.Params);
            info.AddValue("Status", this.Status);
            info.AddValue("LastPollTime", this.LastPollTime);
            info.AddValue("LastMessage", GetServiceLastMessage());
            info.AddValue("LastExceptionMessage", GetServiceLastExceptionMessage());
        }

        #endregion

        public static ServiceThread CreateThread(ThreadParam tp, CreateThreadDelegete callBack)
        {
            ServiceThread serviceThread = new ServiceThread(tp);

            serviceThread.currentThread = new Thread(new ThreadStart(serviceThread.Execute));
            serviceThread.currentThread.Name = tp.Name;

            serviceThread.callBack = callBack;

            return serviceThread;
        }

        public void Execute()
        {
            try
            {
                ThreadParam tp = this.param;

                if (this.callBack != null)
                    callBack(tp);

                ManualResetEvent mre = tp.ExitEvent;

				if (tp.Enabled)
				{
					tp.ThreadTask.Initialize();
					//tp.Log.Write("初始化线程：" + tp.Name);

					DateTime startTime = DateTime.Now;

					while (true)
					{
						if (mre.WaitOne(tp.ActivateDuration, false))
						{
							//tp.Log.Write("Exit");                    
							break;
						}

						try
						{
							if (tp.Enabled)
							{
								UpdateLastPollTime();

								if (tp.Schedule.Enabled)
								{
									if (tp.Schedule.IsCheckPointNeared())
										tp.ThreadTask.OnThreadTaskStart();
								}
								else
									tp.ThreadTask.OnThreadTaskStart();

								TimeSpan ts = DateTime.Now - startTime;

								if (ts >= tp.DisposeDuration)
								{
									startTime = DateTime.Now;
									tp.ThreadTask.Dispose();
								}
							}
						}
						catch (System.Threading.ThreadAbortException)
						{
							throw;
						}
						catch (System.Exception ex)
						{
							tp.Log.Write(ex, ServiceLogEventID.SERVICEBASE_THREADEXECUTE);
						}
					}
				}
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                Debug.WriteLine("正常：" + ex.Message);

                this.param.Log.Write("任务线程被中止", ex.Message, ServiceLogEventID.SERVICEBASE_THREADABORTED);
            }
            catch (Exception ex)
            {
                this.param.Log.Write(ex, ServiceLogEventID.SERVICEBASE_THREADEXECUTE);
            }
        }

        private void UpdateLastPollTime()
        {
            this.pollTimeRWLock.AcquireWriterLock(1000);

            try
            {
                this.lastPollTime = DateTime.Now;
            }
            finally
            {
                this.pollTimeRWLock.ReleaseWriterLock();
            }
        }

        private string GetServiceLastExceptionMessage()
        {
            string message = string.Empty;

            try
            {
                message = ((ThreadParam)this.Params).Log.LastExceptionMessage;
            }
            catch (Exception)
            {
            }

            return message;
        }

        private string GetServiceLastMessage()
        {
            string message = string.Empty;

            try
            {
                message = ((ThreadParam)this.Params).Log.LastMessage;
            }
            catch (Exception)
            {
            }

            return message;
        }
    }

    [Serializable]
    public class ServiceThreadCollection : System.Collections.CollectionBase
    {
        public void Add(ServiceThread thread)
        {
            InnerList.Add(thread);
        }

        public IServiceThread this[int i]
        {
            get
            {
                return (ServiceThread)InnerList[i];
            }
        }

        /// <summary>
        /// 启动所有线程
        /// </summary>
        public void StartAllThreads()
        {
            foreach (ServiceThread serviceThread in InnerList)
            {
                serviceThread.CurrentThread.Start();
            }
        }

        /// <summary>
        /// 终止所有线程
        /// </summary>
        public void AbortAllThreads()
        {
            foreach (ServiceThread serviceThread in InnerList)
            {
                if (serviceThread.Params.CanForceStop)
                    serviceThread.CurrentThread.Abort();

                serviceThread.CurrentThread.Join();
            }
        }

        public System.Threading.ThreadState[] GetAllThreadStates()
        {
            System.Threading.ThreadState[] states = new System.Threading.ThreadState[this.Count];

            for(int i = 0; i < this.Count; i++)
            {
                states[i] = this[i].CurrentThread.ThreadState;
            }

            return states;
        }

        /// <summary>
        /// 是否所有线程都已终止
        /// </summary>
        public bool IsAllStopped
        {
            get
            {
                bool result = true;

                foreach (ThreadState state in GetAllThreadStates())
                {
                    if ((state & (ThreadState.Unstarted | ThreadState.Stopped | ThreadState.Aborted)) == 0)
                    {
                        result = false;
                        break;
                    }
                }

                return result;
            }
        }

        public new void Clear()
        {
            ExceptionHelper.FalseThrow(IsAllStopped, "所有线程都停止以后，才可以清除线程");

            InnerList.Clear();
        }
    }
}
