using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;

namespace MCS.Web.Library.Script
{
	/// <summary>
	/// ProcessProgress类的JSON序列化器
	/// </summary>
	public class ProcessProgressConverter : JavaScriptConverter
	{
		/// <summary>
		/// 反序列化
		/// </summary>
		/// <param name="dictionary"></param>
		/// <param name="type"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			ProcessProgress progress = (ProcessProgress)TypeCreator.CreateInstance(typeof(ProcessProgress));

			progress.MinStep = dictionary.GetValue("MinStep", progress.MinStep);
			progress.MaxStep = dictionary.GetValue("MaxStep", progress.MaxStep);
			progress.CurrentStep = dictionary.GetValue("CurrentStep", progress.CurrentStep);
			progress.StatusText = dictionary.GetValue("StatusText", progress.StatusText);

			return progress;
		}

		/// <summary>
		/// 序列化
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="serializer"></param>
		/// <returns></returns>
		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			ProcessProgress progress = (ProcessProgress)obj;

			dictionary.Add("MinStep", progress.MinStep);
			dictionary.Add("MaxStep", progress.MaxStep);
			dictionary.Add("CurrentStep", progress.CurrentStep);
			dictionary.Add("StatusText", progress.StatusText);

			return dictionary;
		}

		/// <summary>
		/// 所支持的类型
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(ProcessProgress) };
			}
		}
	}
}
