using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace MCS.Web.Apps.WeChat.DataObjects
{
    [Serializable]
    [ORTableMapping("Biz.Members")]
    public class Member
    {
        [ORFieldMapping("MemberID",PrimaryKey = true)]
        public string MemberID
        {
            get;
            set;
        }

        [ORFieldMapping("MemberName")]
        public string MemberName
        {
            get;
            set;
        }

        [NoMapping]
        public string GroupName
        {
            get; set;
        }

        [ORFieldMapping("Age")]
        public int Age
        {
            get;
            set;
        }

        [ORFieldMapping("Gender")]
        public string Gender
        {
            get;
            set;
        }

        [ORFieldMapping("AnnualHouseholdIncome")]
        public string AnnualHouseholdIncome 
        {
            get;
            set;
        }

        [ORFieldMapping("NativePlace")]
        public string NativePlace
        {
            get;
            set;
        }

        [ORFieldMapping("RegisteredPermanentResidence")]
        public string RegisteredPermanentResidence
        {
            get;
            set;
        }

        [ORFieldMapping("HousePaymentPrice")]
        public string HousePaymentPrice
        {
            get;
            set;
        }

        [ORFieldMapping("FamilyComposition")]
        public string FamilyComposition 
        {
            get;
            set;
        }
    }

    [Serializable]
    public class MemberCollection : EditableDataObjectCollectionBase<Member>
    {
    }
}
