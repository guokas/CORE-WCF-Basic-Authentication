using System.Security.Claims;

namespace MyService.Wcf.Authentication;

public class WcfAuthorizationManager : ServiceAuthorizationManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WcfAuthorizationManager()
    {
    }
    
    public WcfAuthorizationManager(IHttpContextAccessor accessor)
    {
        _httpContextAccessor = accessor;
    }
    
    protected override async ValueTask<bool> CheckAccessCoreAsync(OperationContext operationContext)
    {
        await base.CheckAccessCoreAsync(operationContext);
        var servicePrincipal = GetCurrentXmlPrincipal(operationContext);
        if (servicePrincipal is { Identity.IsAuthenticated: true })
        {
            var user = servicePrincipal.Identity as ServiceUser;
            var claims = user.GetClaimsIdentity();
            var identity = new ClaimsIdentity(servicePrincipal.Identity, claims);
            _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(identity);
            
            return true;
        }

        return false;
    }

    private ServicePrincipal GetCurrentXmlPrincipal(OperationContext operationContext)
    {
        return (ServicePrincipal)operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"];
    }
}