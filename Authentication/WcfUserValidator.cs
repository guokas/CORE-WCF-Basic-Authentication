using CoreWCF.IdentityModel.Claims;
using CoreWCF.IdentityModel.Policy;
using CoreWCF.IdentityModel.Selectors;
using MyFirstService.Application;
using System.Security.Claims;
using System.Security.Principal;

namespace MyService.Wcf.Authentication;

public class WcfUserValidator : UserNamePasswordValidator, IAuthorizationPolicy
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public string Id => Guid.NewGuid().ToString();

    public ClaimSet Issuer => ClaimSet.System;

    public WcfUserValidator(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public bool Evaluate(EvaluationContext evaluationContext, ref object state)
    {
        var identity = GetIdentityFromClient(evaluationContext);

        if (identity == null) return false;


        var userApplication = new UserApplication();
        var objUser = userApplication.GetUser(identity.Name!);
        if (objUser == null) return false;


        var serviceUser = new ServiceUser(objUser);
        evaluationContext.Properties["Principal"] = new ServicePrincipal(serviceUser);

        return true;
    }

    public override async ValueTask ValidateAsync(string userName, string password)
    {
        var objServiceUser = new ServiceUser().ValidateUser(userName, password);

        var claims = objServiceUser.GetClaimsIdentity();

        _httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(objServiceUser, claims));
        await ValueTask.CompletedTask;
    }

    private IIdentity GetIdentityFromClient(EvaluationContext evaluationContext)
    {
        if (!evaluationContext.Properties.TryGetValue("Identities", out object propertyIdentities))
        {
            throw new Exception("No identities found in evaluation context");
        }

        if (propertyIdentities is IList<IIdentity> { Count: > 0 } identities)
        {
            return identities[0];
        }

        return null;
    }
}