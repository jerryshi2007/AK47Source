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
    public partial class ProcessBOConfiguration : EntityTypeConfiguration<ProcessBO>
    {
        public ProcessBOConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".TSPWF_Process");
            HasKey(x => x.ProcessId);

            Property(x => x.ProcessId).HasColumnName("ProcessId").IsRequired();
            Property(x => x.ProcessKey).HasColumnName("ProcessKey").IsOptional().HasMaxLength(100);
            Property(x => x.ProcessName).HasColumnName("ProcessName").IsOptional().HasMaxLength(250);
            Property(x => x.Status).HasColumnName("Status").IsOptional().HasMaxLength(10);
            Property(x => x.CreatorId).HasColumnName("CreatorId").IsOptional().HasMaxLength(50);
            Property(x => x.CreatorName).HasColumnName("CreatorName").IsOptional().HasMaxLength(50);
            Property(x => x.Created).HasColumnName("Created").IsOptional();
            Property(x => x.TenantCode).HasColumnName("TenantCode").IsOptional().HasMaxLength(36);
			Ignore(x => x.ExtendProperties);
            Ignore(x => x.AdditionalFields);
            //Ignore(x => x.DynamicExtendProperties);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
