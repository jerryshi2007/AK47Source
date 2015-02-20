using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Security.Locks
{
	[Serializable]
	public class SCCheckLockException : Exception
	{
		public SCCheckLockException() :
			base()
		{
		}

		public SCCheckLockException(string message) :
			base(message)
		{
		}

		public SCCheckLockException(string message, System.Exception innerException) :
			base(message, innerException)
		{
		}

		public static string CheckLockResultToMessage(SCCheckLockResult checkResult)
		{
			checkResult.NullCheck("checkResult");

			StringBuilder strB = new StringBuilder();

			strB.AppendFormat("申请{0}失败。", EnumItemDescriptionAttribute.GetDescription(checkResult.Lock.LockType));

			if (OguBase.IsNotNullOrEmpty(checkResult.Lock.LockPerson))
				strB.AppendFormat("正在由\"{0}\"执行\"{1}\"。", checkResult.Lock.LockPerson.DisplayName, checkResult.Lock.Description);
			else
				strB.AppendFormat("正在执行\"{0}\"", checkResult.Lock.Description);

			strB.Append("请稍后再尝试。");

			return strB.ToString();
		}
	}
}
