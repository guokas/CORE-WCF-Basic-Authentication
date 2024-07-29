using CoreWCF.IdentityModel.Policy;
using CoreWCF.Security;
using MyFirstService.Application.Services;
using MyService.Application.Services;
using MyService.Util;
using MyService.Wcf.Authentication;
using System.Collections.ObjectModel;
using System.Net;

var builder = WebApplication.CreateBuilder();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
//builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.Services.AddHttpContextAccessor();
builder.WebHost.ConfigureKestrel(ks =>
{
    ks.Listen(IPAddress.Any, 8080);
    ks.Listen(IPAddress.Any, 8081, option =>
    {
        option.UseHttps();
    });
}).UseUrls("http://*:8080", "https://*:8081");

var app = builder.Build();


var serviceProvider = app.Services;
app.UseServiceModel(serviceBuilder =>
{
    #region bindings
    //https
    var Transport_bassicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
    Transport_bassicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

    //https + message credential
    var messageCredential_basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
    messageCredential_basicHttpBinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

    //wshttp https + message credential
    var messageCredential_wsHttpBinding = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
    messageCredential_wsHttpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
    messageCredential_wsHttpBinding.Security.Message.EstablishSecurityContext = false;
    messageCredential_wsHttpBinding.MaxReceivedMessageSize = int.MaxValue;
    messageCredential_wsHttpBinding.MaxBufferPoolSize = int.MaxValue;

    /* or with CustomBinding
    var wsHttpBinding = new CustomBinding();
    var securityElement = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
    securityElement.IncludeTimestamp = false;
    wsHttpBinding.Elements.Add(securityElement);
    wsHttpBinding.Elements.Add(new TextMessageEncodingBindingElement());
    var transportElement = new HttpsTransportBindingElement
    {
        MaxReceivedMessageSize = int.MaxValue,
        MaxBufferSize = int.MaxValue
    };
    wsHttpBinding.Elements.Add(transportElement);
    */
    #endregion

    #region add services
    // add services
    serviceBuilder.AddService<AnimalService>(options => { options.BaseAddresses.Add(new Uri("https://localhost:8081")); });
    serviceBuilder.AddServiceEndpoint<AnimalService, IAnimalService>(Transport_bassicHttpBinding, "/AnimalService.svc");

    serviceBuilder.AddService<CarService>(options => { options.BaseAddresses.Add(new Uri("https://localhost:8081")); });
    serviceBuilder.AddServiceEndpoint<CarService, ICarService>(messageCredential_basicHttpBinding, "/CarService.svc");

    serviceBuilder.AddService<WeatherService>(options => { options.BaseAddresses.Add(new Uri("https://localhost:8081")); });
    serviceBuilder.AddServiceEndpoint<WeatherService, IWeatherService>(messageCredential_wsHttpBinding, "/WeatherService.svc");

    var serviceMetadataBehavior = serviceProvider.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpsGetEnabled = true;
    #endregion

    #region Authentication

    //Add authentication
    serviceBuilder.ConfigureAllServiceHostBase(serviceHostBase =>
    {
        if (!ShouldExemptAuth(serviceHostBase))
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            serviceHostBase.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            serviceHostBase.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new WcfUserValidator(httpContextAccessor);
            serviceHostBase.Authorization.ServiceAuthorizationManager = new WcfAuthorizationManager(httpContextAccessor);
            serviceHostBase.Authorization.ExternalAuthorizationPolicies = new ReadOnlyCollection<IAuthorizationPolicy>(new List<IAuthorizationPolicy> { new WcfUserValidator(httpContextAccessor) });
        }
    });

    static bool ShouldExemptAuth(ServiceHostBase serviceHostBase)
    {
        foreach (var endpoints in serviceHostBase.Description.Endpoints)
        {
            //Basic Http Binding with non ClientCredentialType should exempt from authentication.
            //if one endpoint in this service should exempt from auth, then should exempt this entire service. as binding is based on service.
            if (endpoints.Binding is BasicHttpBinding basicHttpBinding)
            {
                if (BasicHttpSecurityMode.Transport == basicHttpBinding.Security.Mode &&
                    HttpClientCredentialType.None == basicHttpBinding.Security.Transport.ClientCredentialType)
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

});

ServiceProviderHelper.ServiceProvider = serviceProvider;

app.Run();
