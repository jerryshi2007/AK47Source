using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Transactions;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects
{
	public class PhoneNumberAdapter : UpdatableAndLoadableAdapterBase<PhoneNumber, PhoneNumberList>
    {
		public static readonly PhoneNumberAdapter Instance = new PhoneNumberAdapter();

        /// <summary>
        /// 得到连接串的名称
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionDefine.DBConnectionName;
        }

        /// <summary>
        /// 根据CODE获取电话
        /// </summary>
        /// <param name="resourceID"></param>
        /// <returns></returns>
		public PhoneNumber LoadByCode(string code)
        {
            code.CheckStringIsNullOrEmpty("Code");
            return this.Load(p =>
            {
                p.AppendItem("Code", code);
				p.AppendItem("VersionEndTime", ConnectionDefine.MaxVersionEndTime);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 根据resourceID获取电话集合
        /// </summary>
        /// <param name="resourceID"></param>
        /// <returns></returns>
		public PhoneNumberList LoadByResourceID(string resourceID)
        {
            resourceID.CheckStringIsNullOrEmpty("resourceID");
            return this.Load(p =>
            {
                p.AppendItem("resourceID", resourceID);
				p.AppendItem("VersionEndTime", ConnectionDefine.MaxVersionEndTime);
            });
        }

        /// <summary>
        /// 根据resourceID、telephoneClass获取电话集合
        /// </summary>
        /// <param name="resourceID"></param>
        /// <returns></returns>
		public PhoneNumberList LoadByResourceIDAndClass(string resourceID, string telephoneClass)
        {
            resourceID.CheckStringIsNullOrEmpty("resourceID");
            return this.Load(p =>
            {
                p.AppendItem("resourceID", resourceID);
                p.AppendItem("Class", telephoneClass);
				p.AppendItem("VersionEndTime", ConnectionDefine.MaxVersionEndTime);
            });
        }

		public void UpdatePhoneNumber(PhoneNumber obj)
		{
			if (obj.ID.IsNullOrEmpty())
				obj.ID = Guid.NewGuid().ToString();

			if (obj.Changed)
			{
				string sql = VersionPhoneNumberUpdateSqlBuilder.Instance.ToUpdateSql(obj, GetMappingInfo(null));

				using (TransactionScope scope = TransactionScopeFactory.Create())
				{
					DateTime dt = (DateTime)DbHelper.RunSqlReturnScalar(sql, GetConnectionName());

					DBTimePointActionContext.Current.TimePoint.IsMinValue(() => DBTimePointActionContext.Current.TimePoint = dt);

					scope.Complete();

				}
			}

		}

    }
}
