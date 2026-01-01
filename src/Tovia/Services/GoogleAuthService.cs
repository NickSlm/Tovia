using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Tovia.Models;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Google.Apis.Util.Store;
using System.Windows.Media.Imaging;

namespace Tovia.Services
{
    public class GoogleAuthService
    {

        private readonly IConfiguration _config;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string[] _scopes;
        private IDataStore _dataStore;
        public GoogleAuthService(IConfiguration config)
        {
            _config = config;
            var googleAuth = _config.GetRequiredSection("GoogleAuth").Get<GoogleAuth>();
            _clientId = googleAuth.ClientId;
            _clientSecret = googleAuth.ClientSecret;
            _scopes = googleAuth.Scopes;
        }

        public UserProfile? User { get; private set; }
        public UserCredential? UserCredentials { get; private set; }
        public async Task SignInAsync()
        {

            try
            {
                UserCredentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                },
                _scopes,
                "user",
                CancellationToken.None,
                dataStore: _dataStore
                );

                await LoadProfileAsync();
  
            }
            catch (InvalidJwtException ex)
            {
                MessageBox.Show("Error authorizing: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }

        }
        public async Task SignOutAsync()
        {
            _dataStore.ClearAsync();
            UserCredentials = null;
            User = null;

        }
        private async Task LoadProfileAsync()
        {
            var authService = new Oauth2Service(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = UserCredentials
                });

            var userInfo = await authService.Userinfo.V2.Me.Get().ExecuteAsync();
            var pfp = new BitmapImage(new Uri(userInfo.Picture));

            User = new UserProfile
            {
                Id = userInfo.Id,
                FirstName = userInfo.GivenName,
                LastName = userInfo.FamilyName,
                Email = userInfo.Email,
                Pfp = pfp
            };

        }

    }
}
