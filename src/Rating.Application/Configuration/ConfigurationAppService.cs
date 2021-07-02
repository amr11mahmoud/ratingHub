using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Rating.Configuration.Dto;

namespace Rating.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : RatingAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
