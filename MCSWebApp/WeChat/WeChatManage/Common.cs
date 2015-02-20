using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Expression;
using MCS.Web.Apps.WeChat.Adapters;
using MCS.Web.Apps.WeChat.DataObjects;

namespace WeChatManage
{
    public class Common
    {
        public static IList<Member> GetCalculatedGroupMembers(string condition)
        {
            var allMembers = MemberAdapter.Instance.LoadAll();
            var result = allMembers.FindAll(p => { return (bool)ExpressionParser.Calculate(condition, new CalculateUserFunction(CalulateFunction), p); });
            return result;
        }

        private static object CalulateFunction(string funcName, ParamObjectCollection arrParams, object callerContext)
        {
            object result = null;

            var member = (Member)callerContext;
            switch (funcName)
            {
                case "Gender":
                    result = member.Gender;
                    break;
                case "Age":
                    result = member.Age;
                    break;
                case "AnnualHouseholdIncome":
                    result = member.AnnualHouseholdIncome;
                    break;
                case "FamilyComposition":
                    result = member.FamilyComposition;
                    break;
                case "HousePaymentPrice":
                    result = member.HousePaymentPrice;
                    break;
                case "NativePlace":
                    result = member.NativePlace;
                    break;
                case "RegisteredPermanentResidence":
                    result = member.RegisteredPermanentResidence;
                    break;
            }

            return result;
        }
    }
}