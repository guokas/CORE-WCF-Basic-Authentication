using System.Security.Principal;
using System.Xml.Linq;
using MyFirstService.Application;
using MyFirstService.Dto;
using MyFirstService.Util;

namespace MyService.Wcf.Authentication
{
    public class ServiceUser : IIdentity
    {
        public string PasswordHash { get; private set; }

        #region IIdentity Members
        public string Login { get; private set; }
        public string Name { get; private set; }
        public string Id { get; private set; }
        public string Country { get; private set; }
        public string Email { get; private set; }
        public string LocaleID { get; private set; }

        public string AuthenticationType{ get; private set; }

        public bool IsAuthenticated{ get; private set; }

        public User User { get; private set; }
        
        #endregion

        public ServiceUser()
        {

        }

        public ServiceUser(User objUser)
        {
            PasswordHash = objUser.PasswordHash;
            Login = objUser.Name;
            Name = objUser.Id.ToString();
            Id = objUser.Id.ToString();
            Country = objUser.country;
            Email = objUser.Email;
            LocaleID = objUser.localeID;
            AuthenticationType = "Integration";
            IsAuthenticated = !string.IsNullOrEmpty(Login);
            User = objUser;
        }
        
        public ServiceUser ValidateUser(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("username must be informed");
            }
            else if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("password must be informed");
            }

            User? objUser = new UserApplication().GetUser(userName);

            if (objUser == null)
            {
                throw new Exception("invalid login");
            }

            //check password 
            if (objUser.PasswordHash != CryptographyHelper.GetHashString(password)) {
                throw new Exception("invalid password");
            }

            //If you are use Single-Sign On application, then call its API to validate the username and password
            //such as: keycloak password grant login
            //var token = keycloackClient.GetToken(
            //    "https://keycloakAddress/protocol/openid-connect/token",
            //    {
            //        new("grant_type", "password"),
            //        new("client_id", clientId),
            //        new("client_secret", clientSecret),
            //        new("username", username),
            //        new("password", password),
            //    }
            //)
            //if(token.access_token == null)
            //   throw new Exception("invalid password");


            return new ServiceUser(objUser);
        }

        public List<System.Security.Claims.Claim> GetClaimsIdentity()
        {
            return new List<System.Security.Claims.Claim>
            {
                new ("Login", Login),
                new ("Name", Name),
                new ("Id", Id ),
                new ("Country", Country ),
                new ("Email", Email),
                new ("LocaleID", LocaleID)
            };
        }
    }
    
}
