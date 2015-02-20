﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace MCS.Library.Core
{
	/// <summary>
	/// WCF服务调用的客户端代理
	/// </summary>
	public static class ServiceProxy
	{
		/// <summary>
		/// 执行一次SingleCall
		/// </summary>
		/// <typeparam name="TChannel">服务接口</typeparam>
		/// <param name="endPointName">EndPoint的名称</param>
		/// <param name="action"></param>
		public static void SingleCall<TChannel>(string endPointName, Action<TChannel> action)
		{
			ChannelFactory<TChannel> factory = new ChannelFactory<TChannel>(endPointName);

			InternalSingleCall(factory, action);
		}

		/// <summary>
		/// 默认使用BasicHttpBinding，调用address的服务
		/// </summary>
		/// <typeparam name="TChannel"></typeparam>
		/// <param name="address"></param>
		/// <param name="action"></param>
		public static void SingleCall<TChannel>(EndpointAddress address, Action<TChannel> action)
		{
			BasicHttpBinding binding = new BasicHttpBinding();

			ChannelFactory<TChannel> factory = new ChannelFactory<TChannel>(binding, address);

			InternalSingleCall(factory, action);
		}

		/// <summary>
		/// 执行一次SingleCall
		/// </summary>
		/// <typeparam name="TChannel"></typeparam>
		/// <param name="binding"></param>
		/// <param name="address"></param>
		/// <param name="action"></param>
		public static void SingleCall<TChannel>(BasicHttpBinding binding, EndpointAddress address, Action<TChannel> action)
		{
			ChannelFactory<TChannel> factory = new ChannelFactory<TChannel>(binding, address);

			InternalSingleCall(factory, action);
		}

		/// <summary>
		///  执行一次SingleCall
		/// </summary>
		/// <typeparam name="TChannel"></typeparam>
		/// <param name="binding"></param>
		/// <param name="address"></param>
		/// <param name="behavior"></param>
		/// <param name="action"></param>
		public static void SingleCall<TChannel>(BasicHttpBinding binding, EndpointAddress address, IEndpointBehavior behavior, Action<TChannel> action)
		{
			ChannelFactory<TChannel> factory = new ChannelFactory<TChannel>(binding, address);

			factory.Endpoint.Behaviors.Add(behavior);

			InternalSingleCall(factory, action);
		}

		/// <summary>
		/// 执行一次SingleCall。其中Factory已经初始化了binding和address
		/// </summary>
		/// <typeparam name="TChannel"></typeparam>
		/// <param name="factory"></param>
		/// <param name="action"></param>
		public static void SingleCall<TChannel>(ChannelFactory<TChannel> factory, Action<TChannel> action)
		{
			factory.NullCheck("factory");

			InternalSingleCall(factory, action);
		}

		private static void InternalSingleCall<TChannel>(ChannelFactory<TChannel> factory, Action<TChannel> action)
		{
			TChannel channel = factory.CreateChannel();

			try
			{
				if (action != null)
					action(channel);

				factory.Close();
			}
			catch (CommunicationException)
			{
				factory.Abort();
				throw;
			}
			catch (TimeoutException)
			{
				factory.Abort();
				throw;
			}
			catch (System.Exception)
			{
				factory.Abort();
				throw;
			}
		}
	}
}
