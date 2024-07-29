using MyService.Util;

namespace MyService.Application.Services
{
    [ServiceContract]
    public interface ICarService
    {
        [OperationContract]
        string GetData(string carName);
    }

    public class CarService : ICarService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CarService()
        {
            _httpContextAccessor = ServiceProviderHelper.ServiceProvider.GetService<IHttpContextAccessor>()!;
        }
        public string GetData(string carName)
        {
            if (_httpContextAccessor?.HttpContext?.User.Identity?.IsAuthenticated ?? false)
            {
                var user = CurrentUserHelper.GetCurrentUser(OperationContext.Current, _httpContextAccessor)!;
                return string.Format("{0}'s car: {1}", user.Name, carName);
            }

            throw new Exception("not authenticated");
        }
    }
}
