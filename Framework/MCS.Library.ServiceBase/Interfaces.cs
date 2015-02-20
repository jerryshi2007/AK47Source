using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using MCS.Library.OGUPermission;

namespace MCS.Library.Services
{    
    /// <summary>
    /// 服务程序的入口类型
    /// </summary>
    public enum ServiceEntryType
    {
        /// <summary>
        /// 服务
        /// </summary>
        Service = 1,

        /// <summary>
        /// 应用程序
        /// </summary>
        Application = 2
    }

    public enum ServiceStatusType
    {
        /// <summary>
        /// 运行中
        /// </summary>
        Running = 1,

        /// <summary>
        /// 已停止
        /// </summary>
        Stopped = 2
    }
    
    public interface IThreadParam
    {
        string Name { get; }

        string Description { get; }

        ServiceLog Log { get;}

        ServiceEntryType EntryType { get; }

        TimeSpan ActivateDuration { get; }

        TimeSpan DisposeDuration { get; }

        bool CanForceStop { get;}

        int BatchCount { get; }

        bool UseDefaultLogger { get; }

		[ScriptIgnore]
		string OwnerServiceName { get; }
    }

    public interface IServiceThread
    {
        IThreadParam Params { get;}

        System.Threading.ThreadState Status { get;}

        System.Threading.Thread CurrentThread { get;}

        DateTime LastPollTime { get;}

        string LastExceptionMessage { get;}

        string LastMessage { get; }
    }

    public delegate void CreateThreadDelegete(ThreadParam tp);

    public interface IStatusService
    {
        ServiceThreadCollection GetThreadStatus();
    }

    /// <summary>
    /// 用于调试主程序，加载各测试控件的接口
    /// </summary>
    public interface IFunctionTestControlCreator
    {
        Control CreateControl(params string[] args);
    }

	/// <summary>
	/// 用于调试主程序，各测试控件实现的接口
	/// </summary>
	public interface IFunctionTestControl
	{
		/// <summary>
		/// 主程序关闭时
		/// </summary>
		/// <param name="e"></param>
		void OnClosing(EventArgs e);
	}

	/*
    public interface INotifySender : IDisposable
    {
        void Initialize();
        void Send(EmailNotify notify, IUser receiver);
    }
	*/

    /// <summary>
    /// 刷新人员角色缓存
    /// </summary>
    public interface IRefreshUserAppRoles
    {
        /// <summary>
        /// 刷新人员角色缓存的方法
        /// </summary>
        /// <param name="user">待刷新的人员</param>
        /// <param name="codeNames">待刷新的应用的CodeName数组</param>
        void RefreshUserAppRole(IUser user, params string[] codeNames);
    }
}
