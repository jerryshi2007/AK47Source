using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
	public class LockConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			Lock lockObj = new Lock();

			lockObj.LockID = DictionaryHelper.GetValue(dictionary, "lockID", string.Empty);

			lockObj.LockTime = DateTime.Parse((string)dictionary["lockTime"]);
			lockObj.PersonID = DictionaryHelper.GetValue(dictionary, "personID", string.Empty);
			lockObj.ResourceID = DictionaryHelper.GetValue(dictionary, "resourceID", string.Empty);
			lockObj.LockType = DictionaryHelper.GetValue(dictionary, "lockType", LockType.FormLock);
			lockObj.EffectiveTime = TimeSpan.FromSeconds(DictionaryHelper.GetValue(dictionary, "effectiveTime", 0));

			return lockObj;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			Lock lockObj = (Lock)obj;

			DictionaryHelper.AddNonDefaultValue(dictionary, "lockID", lockObj.LockID);
			DictionaryHelper.AddNonDefaultValue(dictionary, "lockTime", lockObj.LockTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff"));
			DictionaryHelper.AddNonDefaultValue(dictionary, "personID", lockObj.PersonID);
			DictionaryHelper.AddNonDefaultValue(dictionary, "resourceID", lockObj.ResourceID);
			DictionaryHelper.AddNonDefaultValue(dictionary, "lockType", lockObj.LockType);
			DictionaryHelper.AddNonDefaultValue(dictionary, "effectiveTime", lockObj.EffectiveTime.TotalSeconds);

			return dictionary;
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				List<Type> types = new List<Type>();
				types.Add(typeof(Lock));

				return types;
			}
		}
	}
}
