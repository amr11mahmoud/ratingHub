using System.Threading.Tasks;
using Rating.Configuration.Dto;

namespace Rating.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
