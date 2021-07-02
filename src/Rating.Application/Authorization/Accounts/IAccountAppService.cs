using System.Threading.Tasks;
using Abp.Application.Services;
using Rating.Authorization.Accounts.Dto;

namespace Rating.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
