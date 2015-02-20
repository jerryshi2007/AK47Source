using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.SOA.DataObjects;

namespace MCS.Web.WebControls
{
	internal static class OpinionListControlHelper
	{
		/// <summary>
		/// 用户签名信息的字典
		/// </summary>
		public static Dictionary<string, string> UserSignatures
		{
			get
			{
				return (Dictionary<string, string>)ObjectContextCache.Instance.GetOrAddNewValue("UserSignatures", (cache, key) =>
				{
					Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

					cache.Add("UserSignatures", dict);

					return dict;
				});
			}
		}

		/// <summary>
		/// 准备用户的签名信息
		/// </summary>
		/// <param name="opinions"></param>
		public static void PrepareSignImages(GenericOpinionCollection opinions)
		{
			if (opinions != null && opinions.Count > 0)
			{
				HashSet<string> opinionUserIDs = new HashSet<string>();

				foreach (GenericOpinion opinion in opinions)
				{
					if (UserSignatures.ContainsKey(opinion.IssuePerson.ID) == false && opinionUserIDs.Contains(opinion.IssuePerson.ID) == false)
						opinionUserIDs.Add(opinion.IssuePerson.ID);
				}

				UserInfoExtendCollection opinionUserInfo =
					UserOUControlSettings.GetConfig().UserOUControlQuery.QueryUsersExtendedInfo(opinionUserIDs.ToArray());

				foreach (UserInfoExtendDataObject item in opinionUserInfo)
				{
					string path = item.GetSignImagePath();

					if (path.IsNotEmpty())
						UserSignatures[item.ID] = path;
				}
			}
		}
	}
}
