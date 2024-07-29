using MyService.Util;

namespace MyService.Application.Services
{
    [ServiceContract]
    public interface IWeatherService
    {
        [OperationContract]
        string Forcast(DayInWeek dayInWeek);

    }

    public class WeatherService : IWeatherService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeatherService()
        {
            _httpContextAccessor = ServiceProviderHelper.ServiceProvider.GetService<IHttpContextAccessor>()!;
        }

        public string Forcast(DayInWeek dayInWeek)
        {
            if (_httpContextAccessor?.HttpContext?.User.Identity?.IsAuthenticated ?? false)
            {
                return dayInWeek switch
                {
                    DayInWeek.Mon => "Monday is raining",
                    DayInWeek.Tue => "Tuesday is windy",
                    DayInWeek.Wed => "Wednesday is sunny",
                    DayInWeek.Thu => "Thursday is tornado",
                    DayInWeek.Fri => "Friday is cloudy",
                    DayInWeek.Sat => "Saturday is snowy",
                    DayInWeek.Sun => "Sunday is scorcher",
                    _ => throw new ArgumentException("Argument exception"),
                };
            }
            throw new Exception("not authenticated");
        }
    }

    [DataContract]
    public enum DayInWeek
    {
        [EnumMember] Mon,
        [EnumMember] Tue,
        [EnumMember] Wed,
        [EnumMember] Thu,
        [EnumMember] Fri,
        [EnumMember] Sat,
        [EnumMember] Sun
    }
}
