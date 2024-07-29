using MyFirstService.Application;
using MyFirstService.Dto;

namespace MyFirstService.Util.Extentions
{
    public static class HttpContextAccessorExtentions
    {
        public static User? GetUser(this IHttpContextAccessor httpContextAccessor)
        {
            var userClaimGuid = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "Id");
            var userApplication = new UserApplication();
            return userApplication.GetUser(userClaimGuid.Value);
        }
    }
}
