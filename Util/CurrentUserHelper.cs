using MyFirstService.Dto;
using MyFirstService.Util.Extentions;

namespace MyService.Util
{
    public static class CurrentUserHelper
    {
        public static User? GetCurrentUser(OperationContext operationContext, IHttpContextAccessor httpContextAccessor)
        {
            return operationContext.GetUser() ?? httpContextAccessor.GetUser();
        }
    }
}
