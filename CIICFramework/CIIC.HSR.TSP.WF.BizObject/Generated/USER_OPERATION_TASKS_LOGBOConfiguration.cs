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
    // TSPWF_USER_OPERATION_TASKS_LOG
    public partial class USER_OPERATION_TASKS_LOGBOConfiguration : EntityTypeConfiguration<USER_OPERATION_TASKS_LOGBO>
    {
        public USER_OPERATION_TASKS_LOGBOConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".TSPWF_USER_OPERATION_TASKS_LOG");
            HasKey(x => new { x.LOG_ID, x.TASK_ID, x.SEND_TO_USER_ID });

            Property(x => x.LOG_ID).HasColumnName("LOG_ID").IsRequired();
            Property(x => x.TASK_ID).HasColumnName("TASK_ID").IsRequired().HasMaxLength(36);
            Property(x => x.SEND_TO_USER_ID).HasColumnName("SEND_TO_USER_ID").IsRequired().HasMaxLength(36);
            Property(x => x.SEND_TO_USER_NAME).HasColumnName("SEND_TO_USER_NAME").IsOptional().HasMaxLength(64);
			Ignore(x => x.ExtendProperties);
            Ignore(x => x.AdditionalFields);
            //Ignore(x => x.DynamicExtendProperties);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
