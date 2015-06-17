using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data;

namespace MCS.Library.SOA.DataObjects
{
	public static class Counter
	{
		/// <summary>
		/// 获取下一个计数值，并记入计数器。
		/// </summary>
		/// <param name="counterID">计数器ID</param>
		/// <returns>新计数值</returns>
		public static int NewCountValue(string counterID)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(counterID, "counterID");

			using (DbContext dbi = DbHelper.GetDBContext())
			{
				Database db = DatabaseFactory.Create(dbi);
                return (int)db.ExecuteScalar("WF.NewCountValue", PrepareParameters(counterID));
			}
		}

		/// <summary>
		/// 窥视下一个可用计数值
		/// </summary>
		/// <param name="counterID">计数器ID</param>
		/// <returns>新计数值</returns>
		public static int PeekCountValue(string counterID)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(counterID, "counterID");

			using (DbContext dbi = DbHelper.GetDBContext())
			{
				Database db = DatabaseFactory.Create(dbi);
                return (int)db.ExecuteScalar("WF.PeekCountValue", PrepareParameters(counterID));
			}
		}

		/// <summary>
		/// 设置计数器的计数值
		/// </summary>
		/// <param name="counterID">计数器ID</param>
		/// <param name="countValue">计数值</param>
		/// 在并发情况下设置计数器值可能会造成冲突
		public static void SetCountValue(string counterID, int countValue)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(counterID, "counterID");
			ExceptionHelper.TrueThrow(countValue < 0, "计数值不能小于零");

			using (DbContext dbi = DbHelper.GetDBContext())
			{
				Database db = DatabaseFactory.Create(dbi);
                db.ExecuteScalar("WF.SetCountValue", PrepareParameters(counterID, countValue));
			}
		}

        private static object[] PrepareParameters(params object[] parameters)
        {
            List<object> result = new List<object>(parameters);

            if (TenantContext.Current.Enabled)
                result.Add(TenantContext.Current.TenantCode);

            return result.ToArray();
        }
	}
}
