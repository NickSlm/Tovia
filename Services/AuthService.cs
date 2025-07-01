using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Windows.Interop;
using System.Net.Http;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Security.Policy;
using ToDoListPlus.Models;


namespace ToDoListPlus.Services
{
    public class AuthService
    {
        private string _clientId;
        private string _tenant;
        private string _instance;
        private string[] _scopes;

        private readonly SettingsService _settingsService;
        private string _accountUsername = string.Empty;
        private string _accountTaskListId = string.Empty;

        public string ClientId => _clientId;
        public string Tenant => _tenant;
        public string Instance => _instance;
        public string[] Scopes => _scopes;
        public string AccountUsername
        {
            get { return _accountUsername; }
        }
        public string AccountTaskListId
        {
            get { return _accountTaskListId; }
        }

        private IPublicClientApplication _clientApp;
        public IPublicClientApplication ClientApp { get { return _clientApp; } }


        public AuthService(SettingsService settingsService)
        {
            _settingsService = settingsService;
            var settings = _settingsService.Load();

            _clientId = settings.AzureAd.ClientId;
            _tenant = settings.AzureAd.Tenant;
            _instance = settings.AzureAd.Instance;
            _scopes = settings.AzureAd.Scopes;

            CreateApplication();
        }

        private void CreateApplication()
        {
            _clientApp = PublicClientApplicationBuilder.Create(ClientId).WithAuthority($"{Instance}{Tenant}").WithDefaultRedirectUri().Build();

            MsalCacheHelper cacheHelper = CreateCacheHelperAsync().GetAwaiter().GetResult();
            cacheHelper.RegisterCache(_clientApp.UserTokenCache);
        }
        private async Task<MsalCacheHelper> CreateCacheHelperAsync()
        {
            var storageProperties = new StorageCreationPropertiesBuilder(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".msalcache.bin", MsalCacheHelper.UserRootDirectory).Build();
            MsalCacheHelper cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties, new TraceSource("MSAL.CacheTrace")).ConfigureAwait(false);

            return cacheHelper;

        }
        public async Task<string> Authorize()
        {
            AuthenticationResult? authResult = null;
            IAccount? firstAccount = (await ClientApp.GetAccountsAsync()).FirstOrDefault();

            if (firstAccount == null)
            {
                firstAccount = PublicClientApplication.OperatingSystemAccount;
            }
            try
            {
                authResult = await ClientApp.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");
                try
                {
                    authResult = await ClientApp.AcquireTokenInteractive(Scopes)
                        .WithAccount(firstAccount)
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    return $"Error {msalex.Message}";
                }

            }
            catch(Exception ex)
            {
                return $"Error {ex.Message}";
            }

            _accountUsername = authResult.Account.Username;
            _accountTaskListId = await GetDefaultTaskListIdAsync(authResult.AccessToken);

            return $"Authorization Succeded";
        }
        public async Task<string> GetAccessToken()
        {
            IAccount? firstAccount = (await ClientApp.GetAccountsAsync()).FirstOrDefault();
            var authResult = await ClientApp.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();
            return authResult.AccessToken;
        }
        public async Task<string> GetDefaultTaskListIdAsync(string accessToken)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync("https://graph.microsoft.com/v1.0/me/todo/lists");
            var json = await response.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(json);
            var tasksList = doc.RootElement.GetProperty("value")
                .EnumerateArray()
                .FirstOrDefault();

            var taskListId = tasksList.GetProperty("id").ToString();

            return taskListId;
        }
        public async Task<string> SignOutAsync()
        {
            try
            {
                IAccount? firstAccount = (await ClientApp.GetAccountsAsync()).FirstOrDefault();

                if (firstAccount != null)
                {
                    await ClientApp.RemoveAsync(firstAccount);
                    return "Sign-out successful";
                }
                else
                {
                    return "No user is signed in";
                }
            }
            catch (MsalException msalex)
            {
                //Log The Error msalex
                return $"An Error occured while signing out. {msalex}";
            }
        }
    }
}
