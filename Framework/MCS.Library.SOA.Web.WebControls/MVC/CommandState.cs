using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using MCS.Library.Caching;
using MCS.Library.Core;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// 命令模式的状态基类
	/// </summary>
	[Serializable]
	public abstract class CommandStateBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		protected CommandStateBase()
		{
		}

		/// <summary>
		/// 在加载ViewState时，会执行此方法，加载应用自己的数据
		/// </summary>
		protected internal virtual void LoadState()
		{
		}

		/// <summary>
		/// 该State是否需要保存到ViewState中
		/// </summary>
		internal protected virtual bool NeedSaveToViewState
		{
			get
			{
				return true;
			}
		}
	}

	/// <summary>
	/// CommandState可以实现的扩展属性
	/// </summary>
	public interface ICommandStatePersist
	{
		/// <summary>
		/// Clone内置的业务数据
		/// </summary>
		/// <returns></returns>
		object CloneBusinessObject();
	}

	/// <summary>
	/// 在Command中获取DataAdapter
	/// </summary>
	public interface ICommandStateAdapter
	{
		/// <summary>
		/// 得到DataAdapter
		/// </summary>
		/// <returns></returns>
		object GetAdapter();
	}

	/// <summary>
	/// CommandState可以实现的扩展属性
	/// </summary>
	public interface ICommandStateLoadAndPersist
	{
		/// <summary>
		/// 加载内置的业务数据
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		object LoadBusinessObject(string id);
	}

	/// <summary>
	/// 命令状态的访问器
	/// </summary>
	public static class CommandStateHelper
	{
		private const string CommandStateItemKey = "CommandState";

		/// <summary>
		/// 在HttpContext中注册状态
		/// </summary>
		/// <param name="state"></param>
		public static void RegisterState(CommandStateBase state)
		{
			Dictionary<Type, CommandStateBase> dict = GetCommandStateDictionary();

			dict[state.GetType()] = state;
		}

		/// <summary>
		/// 获取HttpContext中保存的状态
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static CommandStateBase GetCommandState(Type type)
		{
			Dictionary<Type, CommandStateBase> dict = GetCommandStateDictionary();

			CommandStateBase result = null;

			dict.TryGetValue(type, out result);

			RegisterHandlerEvents();

			return result;
		}

		/// <summary>
		/// 删除上下文
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static void RemoveCommandState(Type type)
		{
			Dictionary<Type, CommandStateBase> dict = GetCommandStateDictionary();

			if (dict.ContainsKey(type))
				dict.Remove(type);
		}

		public static T GetCommandState<T>()
		{
			T result = default(T);

			Dictionary<Type, CommandStateBase> dict = GetCommandStateDictionary();

			foreach (KeyValuePair<Type, CommandStateBase> kp in dict)
			{
				if (kp.Value is T)
				{
					object data = kp.Value;

					result = (T)(data);
					break;
				}
			}

			return result;
		}

		internal static void SaveViewState()
		{
			WebUtility.SaveViewStateToCurrentHandler(CommandStateItemKey,
				GetDictionaryToViewState());
		}

		private static Dictionary<Type, CommandStateBase> GetDictionaryToViewState()
		{
			Dictionary<Type, CommandStateBase> result = new Dictionary<Type, CommandStateBase>();

			foreach (KeyValuePair<Type, CommandStateBase> kp in GetCommandStateDictionary())
				if (kp.Value.NeedSaveToViewState)
					result.Add(kp.Key, kp.Value);

			return result;
		}

		private static void RegisterHandlerEvents()
		{
			if (ObjectContextCache.Instance.ContainsKey(HttpContext.Current.CurrentHandler) == false)
			{
				if (HttpContext.Current.CurrentHandler is Page)
				{
					Page page = (Page)HttpContext.Current.CurrentHandler;

					page.PreRenderComplete += new EventHandler(CurrentHandler_PreRenderComplete);
				}

				ObjectContextCache.Instance[HttpContext.Current.CurrentHandler] = true;
			}
		}

		private static void CurrentHandler_PreRenderComplete(object sender, EventArgs e)
		{
			SaveViewState();
		}

		private static Dictionary<Type, CommandStateBase> GetCommandStateDictionary()
		{
			object dict = null;

			if (ObjectContextCache.Instance.TryGetValue(CommandStateItemKey, out dict) == false)
			{
				if (HttpContext.Current.CurrentHandler is Page)
				{
					if (((Page)HttpContext.Current.CurrentHandler).IsPostBack)
					{
						dict = WebUtility.LoadViewStateFromCurrentHandler(CommandStateItemKey);

						if (dict != null)
						{
							foreach (KeyValuePair<Type, CommandStateBase> kp in (Dictionary<Type, CommandStateBase>)dict)
								kp.Value.LoadState();
						}
					}
				}

				if (dict == null)
					dict = new Dictionary<Type, CommandStateBase>();

				ObjectContextCache.Instance.Add(CommandStateItemKey, dict);
			}

			return (Dictionary<Type, CommandStateBase>)dict;
		}
	}
}
