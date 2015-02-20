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
    // TSPWF_USER_OPERATION_LOG
    public partial class USER_OPERATION_LOGBOConfiguration : EntityTypeConfiguration<USER_OPERATION_LOGBO>
    {
        public USER_OPERATION_LOGBOConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".TSPWF_USER_OPERATION_LOG");
            HasKey(x => new { x.ID, x.OPERATE_DATETIME });

            Property(x => x.ID).HasColumnName("ID").IsRequired();
            Property(x => x.RESOURCE_ID).HasColumnName("RESOURCE_ID").IsOptional().HasMaxLength(64);
            Property(x => x.SUBJECT).HasColumnName("SUBJECT").IsOptional().HasMaxLength(255);
            Property(x => x.APPLICATION_NAME).HasColumnName("APPLICATION_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.PROGRAM_NAME).HasColumnName("PROGRAM_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.PROCESS_ID).HasColumnName("PROCESS_ID").IsOptional().HasMaxLength(36);
            Property(x => x.ACTIVITY_ID).HasColumnName("ACTIVITY_ID").IsOptional().HasMaxLength(36);
            Property(x => x.ACTIVITY_NAME).HasColumnName("ACTIVITY_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.OPERATOR_ID).HasColumnName("OPERATOR_ID").IsOptional().HasMaxLength(36);
            Property(x => x.OPERATOR_NAME).HasColumnName("OPERATOR_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.TOP_DEPT_ID).HasColumnName("TOP_DEPT_ID").IsOptional().HasMaxLength(36);
            Property(x => x.TOP_DEPT_NAME).HasColumnName("TOP_DEPT_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.REAL_USER_ID).HasColumnName("REAL_USER_ID").IsOptional().HasMaxLength(36);
            Property(x => x.REAL_USER_NAME).HasColumnName("REAL_USER_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.OPERATE_DATETIME).HasColumnName("OPERATE_DATETIME").IsRequired();
            Property(x => x.OPERATE_NAME).HasColumnName("OPERATE_NAME").IsOptional().HasMaxLength(64);
            Property(x => x.OPERATE_TYPE).HasColumnName("OPERATE_TYPE").IsOptional().HasMaxLength(1);
            Property(x => x.OPERATE_DESCRIPTION).HasColumnName("OPERATE_DESCRIPTION").IsOptional().HasMaxLength(1024);
            Property(x => x.HTTP_CONTEXT).HasColumnName("HTTP_CONTEXT").IsOptional().HasMaxLength(1073741823);
            Property(x => x.CORRELATION_ID).HasColumnName("CORRELATION_ID").IsOptional().HasMaxLength(36);
			Ignore(x => x.ExtendProperties);
            Ignore(x => x.AdditionalFields);
            //Ignore(x => x.DynamicExtendProperties);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
