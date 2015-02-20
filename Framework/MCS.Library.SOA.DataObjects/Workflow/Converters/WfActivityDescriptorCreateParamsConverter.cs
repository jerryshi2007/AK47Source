using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.OGUPermission;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	public class WfActivityDescriptorCreateParamsConverter : JavaScriptConverter
	{
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			WfActivityDescriptorCreateParams result = new WfActivityDescriptorCreateParams();

			result.AllAgreeWhenConsign = DictionaryHelper.GetValue(dictionary, "allAgreeWhenConsign", false);
			result.Operation = DictionaryHelper.GetValue(dictionary, "operation", string.Empty);
			result.CurrentActivityKey = DictionaryHelper.GetValue(dictionary, "currentActivityKey", string.Empty);
			result.Name = DictionaryHelper.GetValue(dictionary, "name", string.Empty);
			if (dictionary.ContainsKey("users"))
			{
				result.Users = DeserializeUsers((IList)dictionary["users"]);
			}
			if (dictionary.ContainsKey("circulateUsers"))
			{
				result.CirculateUsers = DeserializeUsers((IList)dictionary["circulateUsers"]);
			}
			if (dictionary.ContainsKey("variables"))
			{
				result.Variables =
					JSONSerializerExecute.Deserialize<WfVariableDescriptor[]>(dictionary["variables"]);
			}

			return result;
		}

		public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new System.Type[] { typeof(WfActivityDescriptorCreateParams) };
			}
		}

		private static OguDataCollection<IUser> DeserializeUsers(IList objs)
		{
			Dictionary<string, IUser> userDict = new Dictionary<string, IUser>();

			for (int i = 0; i < objs.Count; i++)
			{
				IOguObject obj = (IOguObject)objs[i];

				if (obj is IGroup)
				{
					foreach (IUser user in ((IGroup)obj).Members)
					{
						userDict[user.ID] = user;
					}
				}
				else
					if (obj is IUser)
					{
						IUser user = (IUser)obj;
						userDict[user.ID] = user;
					}
			}

			OguDataCollection<IUser> users = new OguDataCollection<IUser>();

			foreach (KeyValuePair<string, IUser> kp in userDict)
			{
				users.Add(kp.Value);
			}

			return users;
		}
	}
}
