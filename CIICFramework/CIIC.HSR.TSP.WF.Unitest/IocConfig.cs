using CIIC.HSR.TSP.Cache;
using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.DataAccess.EF;
using CIIC.HSR.TSP.IoC;
using CIIC.HSR.TSP.Logging;
using CIIC.HSR.TSP.Models;
using CIIC.HSR.TSP.TA.Bizlet.Contract;
using CIIC.HSR.TSP.TA.BizObject;
using CIIC.HSR.TSP.TA.Persistence.Contract;
using CIIC.HSR.TSP.TA.Persistence.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.Bizlet.Impl;
using CIIC.HSR.TSP.WF.Unitest;
//using CIIC.HSR.SSP.Tenants;
using CIIC.HSR.TSP.DataAccess.IsolateTypes;
namespace CIIC.HSR.TSP.TA.Bizlet.Impl.Test
{

	public class IoCConfig
	{
		public static void Start()
		{

			Containers.Global.AddConfigures(WebSiteIoCConfigure.Create
			 (
				 sl =>
				 {
					 sl.Register<ILogger>(LogCategory.LOG_CATEGORY_EFSQL, new NLogger(LogCategory.LOG_CATEGORY_EFSQL));
					 sl.Register<ILogger>(new NLogger());

					 #region Default IUOW
					 var dbconf = SimpleDbModelConfig.Create(
							dbModelBuilder =>
							{
								dbModelBuilder.Configurations.Add(new AADomainBOConfiguration());
								dbModelBuilder.Configurations.Add(new AADomainRoleBOConfiguration());
								dbModelBuilder.Configurations.Add(new AADomainRoleResourceBOConfiguration());
								dbModelBuilder.Configurations.Add(new AADomainRoleUserBOConfiguration());
								dbModelBuilder.Configurations.Add(new AADomainUserBOConfiguration());
								dbModelBuilder.Configurations.Add(new AADomainTypeBOConfiguration());
								dbModelBuilder.Configurations.Add(new AAResourceBOConfiguration());
								dbModelBuilder.Configurations.Add(new AAResourceMenuBOConfiguration());
								dbModelBuilder.Configurations.Add(new AAResourcePageBOConfiguration());
								dbModelBuilder.Configurations.Add(new AARoleBOConfiguration());
								dbModelBuilder.Configurations.Add(new AARoleResourceBOConfiguration());
								dbModelBuilder.Configurations.Add(new AARoleUserBOConfiguration());
								dbModelBuilder.Configurations.Add(new AAUserAuditLogBOConfiguration());
								dbModelBuilder.Configurations.Add(new AAUserBOConfiguration());
								dbModelBuilder.Configurations.Add(new AAUserPwdHiBOConfiguration());
								dbModelBuilder.Configurations.Add(new AAUserVerifyBOConfiguration());
								dbModelBuilder.Configurations.Add(new BizCodeBOConfiguration());
								dbModelBuilder.Configurations.Add(new BizCodeExCludeBOConfiguration());
								dbModelBuilder.Configurations.Add(new BizCodeSectionBOConfiguration());
								dbModelBuilder.Configurations.Add(new BizCodeSectionCurrentValueBOConfiguration());
								dbModelBuilder.Configurations.Add(new ProfilePasswordPolicyBOConfiguration());
								dbModelBuilder.Configurations.Add(new ProfileSettingBOConfiguration());
								dbModelBuilder.Configurations.Add(new CIIC.HSR.TSP.WF.BizObject.USER_TASKBOConfiguration());

							}
						);
					 sl.Register<IDbModelConfig>(
						IoCKeys.DefaultUnitOfWorkKey, dbconf
					  );
					 sl.Register<IDbModelConfig>(
					  IoCKeys.WorkflowUnitOfWorkKey, dbconf
					);

					 sl.RegisterFactory<IUnitOfWork>(IoCKeys.DefaultUnitOfWorkKey,
						 p =>
						 {
							 var conf = sl.Resolve<IDbModelConfig>(IoCKeys.DefaultUnitOfWorkKey);
							 return new EFUnitOfWork(new CommonDbContext<Default>("name=" + ConnectionStringKeys.DefaultBusinessConnectionStringKey, IoCKeys.DefaultUnitOfWorkKey));
						 });
					 sl.RegisterFactory<IUnitOfWork>(IoCKeys.WorkflowUnitOfWorkKey,
						p =>
						{
							var conf = sl.Resolve<IDbModelConfig>(IoCKeys.WorkflowUnitOfWorkKey);
							return new EFUnitOfWork(new CommonDbContext<Workflow>("name=" + ConnectionStringKeys.WorkflowUnitOfWorkKey, IoCKeys.WorkflowUnitOfWorkKey));
						});

					 #endregion

					 //配置各个Instance数据库
					 #region TenantIUOW

					 sl.Register<IDbModelConfig>(IoCKeys.TenantUnitOfWorkKey,
						 SimpleDbModelConfig.Create(
							modelBuilder =>
							{

							}
						 ));

					 sl.Register<ITenantUnitOfWork, EFTenantUnitOfWork>(IoCKeys.TenantUnitOfWorkKey, true);

					 #endregion

					 sl.Register<ICaching>(ResourcePermissionBizlet.AAUserMenuResourceCacheKey,
						 new AppFabricCache(ConfigurationManager.AppSettings["UserMenuResourceCacheName"]));
					 sl.Register<ICaching>(new MemoryCache());

					 sl.Register(MetadataKeys.MetadataConnectionStringIOCKey, ConfigurationManager.ConnectionStrings[ConnectionStringKeys.DefaultMetadataConnectionStringKey].ConnectionString);
					 sl.Register<ICachedDataProvider<MetadataSet, double>, CachedMetadataProvider>("", false);
					 //sl.Register<ICachedDataProvider<TenantCacheData, DateTime>, TenantCacheDataProvider>(IoCKeys.TenantUnitOfWorkKey, true);
					 sl.Register<ICachedMetadataService, CachedMetadataService>();
					 //sl.Register<ITenantCacheService, TenantCacheService>();
					 //sl.Register<string>(TenantsUtilities.TenantConnectionStringResolveKey, ConfigurationManager.ConnectionStrings[ConnectionStringKeys.DefaultTenantConnectionStringKey].ConnectionString);
					 //sl.Register<ITenantContext, TenantContext>(IoCKeys.TenantUnitOfWorkKey, true);


					 sl.Register<IAADomainRepository, AADomainRepository>();
					 sl.Register<IAAUserRepository, AAUserRepository>();
					 sl.Register<IBizCodeRepository, BizCodeRepository>();
					 sl.Register<IBizCodeSectionCurrentValueRepository, BizCodeSectionCurrentValueRepository>();
					 sl.Register<IBizCodeExCludeRepository, BizCodeExCludeRepository>();
					 sl.Register<IAADomainRoleResourceRepository, AADomainRoleResourceRepository>();
					 sl.Register<IAADomainRoleUserRepository, AADomainRoleUserRepository>();
					 sl.Register<IAAResourceMenuRepository, AAResourceMenuRepository>();
					 sl.Register<IAAResourceRepository, AAResourceRepository>();
					 sl.Register<IAADomainUserRepository, AADomainUserRepository>();
					 sl.Register<IAARoleRepository, AARoleRepository>();
					 sl.Register<IAARoleResourceRepository, AARoleResourceRepository>();
					 sl.Register<IAARoleUserRepository, AARoleUserRepository>();
					 sl.Register<IAADomainRoleRepository, AADomainRoleRepository>();
					 sl.Register<CIIC.HSR.TSP.WF.Persistence.Contract.IUSER_TASKRepository, CIIC.HSR.TSP.WF.Persistence.Impl.USER_TASKRepository>();

					 sl.Register<IAAUserBizlet, AAUserBizlet>();
					 sl.Register<IResourcePermissionBizlet, ResourcePermissionBizlet>();
					 sl.Register<IAADomainRoleBizlet, AADomainRoleBizlet>();
					 sl.Register<IAADomainBizlet, AADomainBizlet>();
					 sl.Register<IBizCodeBizlet, BizCodeBizlet>();
					 sl.Register<IAADomainRoleUserBizlet, AADomainRoleUserBizlet>();
					 sl.Register<IAARoleUserBizlet, AARoleUserBizlet>();
					 sl.Register<IAAResourceBizlet, AAResourceBizlet>();
					 sl.Register<IAADomainUserBizlet, AADomainUserBizlet>();
					 sl.Register<IAARoleBizlet, AARoleBizlet>();
					 sl.Register<IAARoleResourceBizlet, AARoleResourceBizlet>();
					 sl.Register<IAADomainRoleBizlet, AADomainRoleBizlet>();
					 sl.Register<IAADomainRoleResourceBizlet, AADomainRoleResourceBizlet>();
					 sl.Register<IProfilePasswordPolicyBizlet, ProfilePasswordPolicyBizlet>();
					 sl.Register<CIIC.HSR.TSP.WF.Bizlet.Contract.ITaskQuery, CIIC.HSR.TSP.WF.Bizlet.Impl.TaskQuery>(alwaysNew: true);
					 sl.Register<CIIC.HSR.TSP.WF.Bizlet.Contract.ITaskOperator, CIIC.HSR.TSP.WF.Bizlet.Impl.TaskOperator>(alwaysNew: true);
					 sl.Register<IServiceFactory, ServiceFactory>(alwaysNew: true);
					 sl.Register<IWfTenantUnitWork, DefaultWFTenantUOWFactory>(alwaysNew: true, name: "OMPTaskUnitWork");
					 sl.Register<IWfTenantUnitWork, SSPWFTenantUOWFactory>(alwaysNew: true, name: "SSPTaskUnitWork");
				 }
			 ));

		}
	}


	public class WebSiteIoCConfigure : IIoCConfigure
	{
		public static WebSiteIoCConfigure Create(Action<IIoCContainer> configureAction)
		{
			var x = new WebSiteIoCConfigure { _configureAction = configureAction };
			return x;
		}

		Action<IIoCContainer> _configureAction;
		public void Configure(IIoCContainer container)
		{
			if (_configureAction != null)
			{
				_configureAction(container);
			}
		}
	}
}
