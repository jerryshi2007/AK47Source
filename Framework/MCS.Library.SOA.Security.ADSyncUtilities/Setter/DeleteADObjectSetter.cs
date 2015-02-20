using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;
using MCS.Library;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security.Transfer;
using MCS.Library.SOA.Security.ADSyncUtilities.Entity;
using System.Text;
using System.Threading;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class DeleteADObjectSetter : OguAndADObjectSetterBase
	{
		public override void Convert(ObjectModifyType modifyType, IOguObject srcObject, DirectoryEntry targetObject, string context)
		{
			ADHelper adHelper = SynchronizeContext.Current.ADHelper;

			if (SynchronizeContext.Current.RecycleBinOU.IsNotEmpty())
			{
				ProcessDeleteEntry(modifyType, srcObject, targetObject, context);

				//ChangeEntryNameToFullPath(targetObject);

				using (DirectoryEntry parentEntry = adHelper.NewEntry(SynchronizeContext.Current.RecycleBinOU))
				{
					DoTheMove(targetObject, parentEntry, srcObject);
				}
			}
			else
			{
				targetObject.DeleteTree();
			}
		}

		private static void DoTheMove(DirectoryEntry targetObject, DirectoryEntry parentEntry, IOguObject oguObj)
		{
			bool fail = false;
			string ldif;
			StringBuilder builder = GetEntryLongNameBuilder(targetObject, out ldif);

			do
			{
				try
				{
					string srcDN;

					try
					{
						srcDN = targetObject.Properties["distinguishedName"].Value as string;
					}
					catch
					{
						srcDN = "无法获取起始路径";
					}

					targetObject.MoveTo(parentEntry, ldif + "=" + ADHelper.EscapeString(builder.ToString()));
					targetObject.CommitChanges();

					try
					{
						LogHelper.WriteSynchronizeDBLogDetail(SynchronizeContext.Current.SynchronizeID, "应用变更: 移到回收站 ",
										oguObj == null ? string.Empty : oguObj.ID,
										oguObj == null ? string.Empty : oguObj.Name,
										targetObject == null ? string.Empty : targetObject.NativeGuid.ToString(),
										targetObject == null ? string.Empty : targetObject.Name, "删除动作，对象的初始可分辨名称为：" + srcDN);
					}
					catch (Exception)
					{
					}


					fail = false;
				}
				catch (DirectoryServicesCOMException comEx)
				{
					switch (comEx.ErrorCode)
					{
						case -2147019886: // 重名
							MakeTimePostfixToBuilderAndTrim(builder);
							fail = true;
							Thread.Sleep(100);
							break;
						default:
							throw;
					}
				}
			} while (fail);
		}

		protected virtual void ProcessDeleteEntry(ObjectModifyType modifyType, IOguObject srcObject, DirectoryEntry targetObject, string context)
		{
		}

		/// <summary>
		/// 禁用用户账户。此方法可能有问题，应该读取原始值后，再去除ADS_UF_ACCOUNTDISABLE掩码
		/// </summary>
		/// <param stringValue="adParent"></param>
		protected static void DisableAccount(DirectoryEntry entry)
		{
			entry.Properties["userAccountControl"].Value = (int)ADS_USER_FLAG.ADS_UF_ACCOUNTDISABLE;
			entry.CommitChanges();
		}

		private static StringBuilder GetEntryLongNameBuilder(DirectoryEntry entry, out string ldif)
		{
			string dn = entry.Properties["distinguishedName"].Value.ToString();
			StringBuilder builder = new StringBuilder(64);

			var de = new RdnSequencePartEnumerator(dn).GetEnumerator();
			ldif = null;

			if (de.MoveNext())
			{
				RdnAttribute attr = RdnAttribute.Parse(de.Current);
				builder.Append(attr.StringValue);
				builder.Append('$');
				ldif = attr.Ldif;
				while (de.MoveNext())
				{
					attr = RdnAttribute.Parse(de.Current);
					builder.Append(attr.StringValue);
					builder.Append('$');
				}

				MakeTimePostfixToBuilderAndTrim(builder);

				return builder;
			}
			else
				throw new FormatException("DN格式不正确");
		}

		private static void MakeTimePostfixToBuilderAndTrim(StringBuilder builder)
		{
			char[] time = DateTime.Now.ToString("yyyyMMddHHmmssfff").ToCharArray();
			if (builder.Length + time.Length > 64)
			{
				builder.Length = 64 - time.Length;
			}

			builder.Append(time);
		}
	}
}