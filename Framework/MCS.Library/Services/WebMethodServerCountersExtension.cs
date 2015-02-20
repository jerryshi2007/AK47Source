using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace MCS.Library.Services
{
	/// <summary>
	/// Web Service服务器端性能指针的扩展
	/// </summary>
	public class WebMethodServerCountersExtension : SoapExtension
	{
		private string _Initializer;
		private string _InstanceName;
		private Stopwatch _ExecutionWatch = null;

		private WebMethodServerCounters _GlobalInstance = null;
		private WebMethodServerCounters _ActionInstance = null;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		public override object GetInitializer(Type serviceType)
		{
			return serviceType.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="methodInfo"></param>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="initializer"></param>
		public override void Initialize(object initializer)
		{
			this._Initializer = initializer.ToString();
		}

		/// <summary>
		/// 处理消息。
		/// BeforeDeserialize和AfterDeserialize是发生在服务内代码执行之前。
		/// BeforeSerialize和AfterSerialize是发生在服务内代码执行之后。
		/// 整个执行时序是：ChainStream，BeforeDeserialize，AfterDeserialize，BeforeSerialize，AfterSerialize
		/// </summary>
		/// <param name="message"></param>
		public override void ProcessMessage(SoapMessage message)
		{
			//仅处理Server端
			if (message is SoapServerMessage)
			{
				switch (message.Stage)
				{
					case SoapMessageStage.BeforeDeserialize:
						break;
					case SoapMessageStage.AfterDeserialize:
						//进入
						InitParameters(message);
						InitializeCounters(this._InstanceName);
						BeforeExecuteMethodCounter(message);
						break;
					case SoapMessageStage.BeforeSerialize:
						break;
					case SoapMessageStage.AfterSerialize:
						//离开
						AfterExecuteMethodCounter(message);
						break;
					default:
						throw new Exception("Error Message Stage");
				}
			}
		}

		private void BeforeExecuteMethodCounter(SoapMessage message)
		{
			this._GlobalInstance.RequestCount.Increment();
			this._ActionInstance.RequestCount.Increment();
		}

		private void AfterExecuteMethodCounter(SoapMessage message)
		{
			this._ExecutionWatch.Stop();
			SetAfterExecuteMethodCounterValues(this._GlobalInstance, message);
			SetAfterExecuteMethodCounterValues(this._ActionInstance, message);
		}

		private void SetAfterExecuteMethodCounterValues(WebMethodServerCounters instance, SoapMessage message)
		{
			instance.RequestAverageDurationBase.Increment();
			instance.RequestAverageDuration.IncrementBy(this._ExecutionWatch.ElapsedMilliseconds / 100);

			if (message.Exception != null)
				instance.RequestFailCount.Increment();
			else
				instance.RequestSuccessCount.Increment();

			instance.RequestsPerSecond.Increment();
		}

		private void InitializeCounters(string instanceName)
		{
			if (this._GlobalInstance == null)
				this._GlobalInstance = new WebMethodServerCounters("_Total_");

			if (this._ActionInstance == null)
				this._ActionInstance = new WebMethodServerCounters(instanceName);
		}

		private void InitParameters(SoapMessage message)
		{
			if (this._InstanceName == null)
			{
				this._InstanceName = message.GetMethodName() + " of " + this._Initializer;
				this._InstanceName = this._InstanceName.Replace('/', '-');
				this._InstanceName = this._InstanceName.Replace('\\', '-');
			}

			if (this._ExecutionWatch == null)
			{
				this._ExecutionWatch = new Stopwatch();
				this._ExecutionWatch.Start();
			}
		}
	}
}
