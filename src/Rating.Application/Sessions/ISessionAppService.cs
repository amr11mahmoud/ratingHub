using System.Threading.Tasks;
using Abp.Application.Services;
using Rating.Sessions.Dto;

namespace Rating.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
