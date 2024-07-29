using MyFirstService.Dto;
using MyService.Wcf.Authentication;

namespace MyFirstService.Util.Extentions
{
    public static class OperationContextExtentions
    {
        public static User GetUser(this OperationContext context)
        {
            var principal = (ServicePrincipal)context.ServiceSecurityContext.AuthorizationContext.Properties["Principal"];
            var user = principal.Identity as ServiceUser;
            return user?.User;
        }
    }
}
