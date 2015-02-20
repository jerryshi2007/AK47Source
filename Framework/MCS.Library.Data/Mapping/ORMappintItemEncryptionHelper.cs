using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Security;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// 得到加密器
	/// </summary>
	internal static class ORMappingItemEncryptionHelper
	{
		/// <summary>
		/// 得到加密器
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static ISymmetricEncryption GetEncryptor(string name)
		{
			ISymmetricEncryption result = null;

			using (ORMappingContext context = ORMappingContext.GetContext())
			{
				if (name.IsNullOrEmpty())
					name = "DefaultPropertyEncryptor";

				result = context.ItemEncryptors[name];

				if (result == null)
				{
					result = new ORMappintItemEncryption(name);
					context.ItemEncryptors.Add((ORMappintItemEncryption)result);
				}
			}

			return result;
		}
	}
}
