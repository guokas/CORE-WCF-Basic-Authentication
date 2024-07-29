using System.Security.Principal;

namespace MyService.Wcf.Authentication;

[Serializable]
public class ServicePrincipal : IPrincipal
{
    public IIdentity Identity => _identity;

    public bool IsInRole(string role)
    {
        return true;
    }

    private readonly ServiceUser _identity;

    public ServicePrincipal(ServiceUser identity)
    {
        _identity = identity;
    }

    public ServicePrincipal()
    {
    }
}