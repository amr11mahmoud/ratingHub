using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Rating.Authorization;

namespace Rating
{
    [DependsOn(
        typeof(RatingCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class RatingApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<RatingAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(RatingApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
