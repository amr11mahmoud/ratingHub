using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Rating.EntityFrameworkCore;
using Rating.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Rating.Web.Tests
{
    [DependsOn(
        typeof(RatingWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class RatingWebTestModule : AbpModule
    {
        public RatingWebTestModule(RatingEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(RatingWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(RatingWebMvcModule).Assembly);
        }
    }
}