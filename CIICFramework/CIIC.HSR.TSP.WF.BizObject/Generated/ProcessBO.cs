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
    // TSPWF_Process
    [DataContract(IsReference=true)]
    [MappedToAttribute("TSPWF_Process")]
    [BizObjectCode("TSPWF_Process")]
    public partial class ProcessBO : BizObjectBase<ProcessBO>
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
            return "TSPWF_Process";
        }
		#endregion
		
        //[DataMember(Order = 1, IsRequired = true)]
        [DataMember]
        public Guid ProcessId { get; set; } // ProcessId (Primary key)

        //[DataMember(Order = 2, IsRequired = false)]
        [DataMember]
        public string ProcessKey { get; set; } // ProcessKey

        //[DataMember(Order = 3, IsRequired = false)]
        [DataMember]
        public string ProcessName { get; set; } // ProcessName

        //[DataMember(Order = 4, IsRequired = false)]
        [DataMember]
        public string Status { get; set; } // Status

        //[DataMember(Order = 5, IsRequired = false)]
        [DataMember]
        public string CreatorId { get; set; } // CreatorId

        //[DataMember(Order = 6, IsRequired = false)]
        [DataMember]
        public string CreatorName { get; set; } // CreatorName

        //[DataMember(Order = 7, IsRequired = false)]
        [DataMember]
        public DateTime? Created { get; set; } // Created

        //[DataMember(Order = 8, IsRequired = false)]
        [DataMember]
        public string TenantCode { get; set; } // TenantCode

    }

}
