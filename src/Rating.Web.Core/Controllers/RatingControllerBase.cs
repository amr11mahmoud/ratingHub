using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Rating.Controllers
{
    public abstract class RatingControllerBase: AbpController
    {
        protected RatingControllerBase()
        {
            LocalizationSourceName = RatingConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
