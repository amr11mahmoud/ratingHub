using Abp.Authorization;
using Rating.Authorization.Roles;
using Rating.Authorization.Users;

namespace Rating.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
