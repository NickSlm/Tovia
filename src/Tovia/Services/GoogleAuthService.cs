using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Tovia.Models;

namespace Tovia.Services
{
    public class GoogleAuthService
    {

        private readonly IConfiguration _config;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string[] _scopes;

        public GoogleAuthService(IConfiguration config)
        {
            _config = config;
            var googleAuth = _config.GetRequiredSection("GoogleAuth").Get<GoogleAuth>();
            _clientId = googleAuth.ClientId;
            _clientSecret = googleAuth.ClientSecret;
            _scopes = googleAuth.Scopes;
        }


        public async Task Authorize()
        {
            try
            {
                var credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                },
                _scopes,
                "user",
                CancellationToken.None,
                dataStore: null
                );

                var payload = await GoogleJsonWebSignature.ValidateAsync(credentials.Token.IdToken);

                MessageBox.Show(payload.Subject + "\n" +
                    payload.Email + "\n" +
                    payload.Name + "\n" +
                    payload.GivenName + "\n" +
                    payload.FamilyName + "\n" +
                    payload.Picture + "\n");
                    

            }
            catch ( Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        }

    }
}
