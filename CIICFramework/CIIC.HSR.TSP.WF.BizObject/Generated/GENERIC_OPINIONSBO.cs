// ReSharper disable RedundantUsingDirective
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable RedundantNameQualifier
using CIIC.HSR.TSP.DataAccess.EF;
using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.Models;
using CIIC.HSR.TSP.IoC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;
//using DatabaseGeneratedOption = System.ComponentModel.DataAnnotations.DatabaseGeneratedOption;

namespace CIIC.HSR.TSP.WF.BizObject
{
    // TSPWF_GENERIC_OPINIONS
    [DataContract(IsReference=true)]
    [MappedToAttribute("TSPWF_GENERIC_OPINIONS")]
    [BizObjectCode("TSPWF_GENERIC_OPINIONS")]
    public partial class GENERIC_OPINIONSBO : BizObjectBase<GENERIC_OPINIONSBO>
    {
		#region Impl abstract
		
		private AdditionalFields _additionalFields;
		
        [DataMember]
        public override AdditionalFields AdditionalFields
        {
            get { return _additionalFields; }
            set { _additionalFields = value; }
        }

        public override string GetBusinessName()
        {
            return "TSPWF_GENERIC_OPINIONS";
        }
		#endregion
		
        //[DataMember(Order = 1, IsRequired = true)]
        [DataMember]
        public string ID { get; set; } // ID

        //[DataMember(Order = 2, IsRequired = false)]
        [DataMember]
        public string RESOURCE_ID { get; set; } // RESOURCE_ID

        //[DataMember(Order = 3, IsRequired = false)]
        [DataMember]
        public string CONTENT { get; set; } // CONTENT

        //[DataMember(Order = 4, IsRequired = false)]
        [DataMember]
        public string ISSUE_PERSON_ID { get; set; } // ISSUE_PERSON_ID

        //[DataMember(Order = 5, IsRequired = false)]
        [DataMember]
        public string ISSUE_PERSON_NAME { get; set; } // ISSUE_PERSON_NAME

        //[DataMember(Order = 6, IsRequired = false)]
        [DataMember]
        public string ISSUE_PERSON_LEVEL { get; set; } // ISSUE_PERSON_LEVEL

        //[DataMember(Order = 7, IsRequired = false)]
        [DataMember]
        public string APPEND_PERSON_ID { get; set; } // APPEND_PERSON_ID

        //[DataMember(Order = 8, IsRequired = false)]
        [DataMember]
        public string APPEND_PERSON_NAME { get; set; } // APPEND_PERSON_NAME

        //[DataMember(Order = 9, IsRequired = false)]
        [DataMember]
        public DateTime? ISSUE_DATETIME { get; set; } // ISSUE_DATETIME

        //[DataMember(Order = 10, IsRequired = false)]
        [DataMember]
        public DateTime? APPEND_DATETIME { get; set; } // APPEND_DATETIME

        //[DataMember(Order = 11, IsRequired = false)]
        [DataMember]
        public string PROCESS_ID { get; set; } // PROCESS_ID

        //[DataMember(Order = 12, IsRequired = false)]
        [DataMember]
        public string ACTIVITY_ID { get; set; } // ACTIVITY_ID

        //[DataMember(Order = 13, IsRequired = false)]
        [DataMember]
        public string LEVEL_NAME { get; set; } // LEVEL_NAME

        //[DataMember(Order = 14, IsRequired = false)]
        [DataMember]
        public string LEVEL_DESP { get; set; } // LEVEL_DESP

        //[DataMember(Order = 15, IsRequired = false)]
        [DataMember]
        public string OPINION_TYPE { get; set; } // OPINION_TYPE

        //[DataMember(Order = 16, IsRequired = false)]
        [DataMember]
        public int? EVALUE { get; set; } // EVALUE

        //[DataMember(Order = 17, IsRequired = false)]
        [DataMember]
        public int? RESULT { get; set; } // RESULT

        //[DataMember(Order = 18, IsRequired = false)]
        [DataMember]
        public string EXT_DATA { get; set; } // EXT_DATA


        public GENERIC_OPINIONSBO():this(null)
        {
            ISSUE_DATETIME = System.DateTime.Now;
            APPEND_DATETIME = System.DateTime.Now;
            InitializePartial();
        }

		public GENERIC_OPINIONSBO(BusinessObjectView metadataView):base(metadataView)
		{
		
		}
        partial void InitializePartial();
    }

}
