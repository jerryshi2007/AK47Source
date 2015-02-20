using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Services.Protocols;
using MCS.Library.Caching;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 人员和授权访问客户端服务代理的基类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public abstract class ServiceBrokerContextBase<T> where T : ServiceBrokerContextBase<T>, new()
	{
		private string url = string.Empty;
		private TimeSpan timeout = TimeSpan.FromSeconds(90);
		private bool useLocalCache = true;
		private bool useServerCache = true;
		private DateTime timePoint = DateTime.MinValue;
		private Dictionary<string, string> connectionMappings = new Dictionary<string, string>();
		private Dictionary<string, object> context = new Dictionary<string, object>();

		[NonSerialized]
		private byte[] savedStates = null;

		/// <summary>
		/// 得到上下文
		/// </summary>
		public static T Current
		{
			get
			{
				return (T)ObjectContextCache.Instance.GetOrAddNewValue(GetAndCheckContextKey(),
						new ContextCacheQueueBase<object, object>.ContextCacheItemNotExistsAction(AddNewContext));
			}
		}

		#region Properties
		/// <summary>
		/// 是否使用本地缓存
		/// </summary>
		public bool UseLocalCache
		{
			get { return this.useLocalCache; }
			set { this.useLocalCache = value; }
		}

		/// <summary>
		/// 是否使用服务器端缓存。这需要服务方支持。此属性会通过ServiceBrokerSoapHeader放置在SoapHeader中
		/// </summary>
		public bool UseServerCache
		{
			get { return this.useServerCache; }
			set { this.useServerCache = value; }
		}

		/// <summary>
		/// 穿梭时间点
		/// </summary>
		public DateTime TimePoint
		{
			get { return this.timePoint; }
			set { this.timePoint = value; }
		}

		/// <summary>
		/// 超时时间
		/// </summary>
		public TimeSpan Timeout
		{
			get { return this.timeout; }
			set { this.timeout = value; }
		}

		/// <summary>
		/// 连接串映射
		/// </summary>
		public Dictionary<string, string> ConnectionMappings
		{
			get { return this.connectionMappings; }
		}

		/// <summary>
		/// 上下文信息
		/// </summary>
		public Dictionary<string, object> Context
		{
			get { return this.context; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// 保存状态
		/// </summary>
		public void SaveContextStates()
		{
			BinaryFormatter formatter = new BinaryFormatter();

			using (MemoryStream stream = new MemoryStream(4096))
			{
				formatter.Serialize(stream, this);
				this.savedStates = stream.ToArray();
			}
		}

		/// <summary>
		/// 恢复状态
		/// </summary>
		public void RestoreSavedStates()
		{
			ExceptionHelper.FalseThrow(this.savedStates != null,
					"没有执行SaveContextStates，不能恢复上下文状态改变");

			BinaryFormatter formatter = new BinaryFormatter();

			using (MemoryStream stream = new MemoryStream(this.savedStates))
			{
				stream.Position = 0;
				ObjectContextCache.Instance[GetAndCheckContextKey()] = (ServiceBrokerContext)formatter.Deserialize(stream);
			}
			this.savedStates = null;
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="protocol"></param>
		public void InitWebClientProtocol(WebClientProtocol protocol)
		{
			protocol.Timeout = (int)this.Timeout.TotalMilliseconds;
		}

		/// <summary>
		/// 初始化刚创建的代理类的属性
		/// </summary>
		protected abstract void InitProperties();

		private static object AddNewContext(ContextCacheQueueBase<object, object> cache, object key)
		{
			T context = new T();

			context.InitProperties();
			cache.Add(key, context);

			return context;
		}

		private static string GetAndCheckContextKey()
		{
			ActionContextDescriptionAttribute attr = AttributeHelper.GetCustomAttribute<ActionContextDescriptionAttribute>(typeof(T));

			(attr != null).FalseThrow("不能在类{0}上找到ActionContextDescriptionAttribute的定义", typeof(T).AssemblyQualifiedName);

			(attr.Key.IsNotEmpty()).FalseThrow("类{0}上ActionContextDescriptionAttribute的Key属性不能为空", typeof(T).AssemblyQualifiedName);

			return attr.Key;
		}
	}
}
