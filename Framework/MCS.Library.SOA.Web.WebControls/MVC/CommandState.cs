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
	/// ����ģʽ��״̬����
	/// </summary>
	[Serializable]
	public abstract class CommandStateBase
	{
		/// <summary>
		/// ���췽��
		/// </summary>
		protected CommandStateBase()
		{
		}

		/// <summary>
		/// �ڼ���ViewStateʱ����ִ�д˷���������Ӧ���Լ�������
		/// </summary>
		protected internal virtual void LoadState()
		{
		}

		/// <summary>
		/// ��State�Ƿ���Ҫ���浽ViewState��
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
	/// CommandState����ʵ�ֵ���չ����
	/// </summary>
	public interface ICommandStatePersist
	{
		/// <summary>
		/// Clone���õ�ҵ������
		/// </summary>
		/// <returns></returns>
		object CloneBusinessObject();
	}

	/// <summary>
	/// ��Command�л�ȡDataAdapter
	/// </summary>
	public interface ICommandStateAdapter
	{
		/// <summary>
		/// �õ�DataAdapter
		/// </summary>
		/// <returns></returns>
		object GetAdapter();
	}

	/// <summary>
	/// CommandState����ʵ�ֵ���չ����
	/// </summary>
	public interface ICommandStateLoadAndPersist
	{
		/// <summary>
		/// �������õ�ҵ������
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		object LoadBusinessObject(string id);
	}

	/// <summary>
	/// ����״̬�ķ�����
	/// </summary>
	public static class CommandStateHelper
	{
		private const string CommandStateItemKey = "CommandState";

		/// <summary>
		/// ��HttpContext��ע��״̬
		/// </summary>
		/// <param name="state"></param>
		public static void RegisterState(CommandStateBase state)
		{
			Dictionary<Type, CommandStateBase> dict = GetCommandStateDictionary();

			dict[state.GetType()] = state;
		}

		/// <summary>
		/// ��ȡHttpContext�б����״̬
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
		/// ɾ��������
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
