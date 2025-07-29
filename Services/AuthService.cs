using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows;
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

            ApplySettings();
            CreateApplication();
        }

        private void ApplySettings()
        {
            var appSettings = _settingsService.appSettings;

            _clientId = appSettings.AzureAd.ClientId;
            _tenant = appSettings.AzureAd.Tenant;
            _instance = appSettings.AzureAd.Instance;
            _scopes = appSettings.AzureAd.Scopes;
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
        public async Task Authorize()
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
                Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");
                try
                {
                    authResult = await ClientApp.AcquireTokenInteractive(Scopes)
                        .WithAccount(firstAccount)
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    Debug.WriteLine($"Failed to Acquire Token Error:{msalex.Message}");
                }

            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
            }

            _accountUsername = authResult.Account.Username;
            _accountTaskListId = await GetDefaultTaskListIdAsync(authResult.AccessToken);

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
        public async Task SignOutAsync()
        {
            try
            {
                IAccount? firstAccount = (await ClientApp.GetAccountsAsync()).FirstOrDefault();
                if (firstAccount != null)
                {
                    await ClientApp.RemoveAsync(firstAccount);
                }
            }
            catch (MsalException msalex)
            {
                Debug.WriteLine($"Error occurred while trying to sign out {msalex}");
            }
        }
    }
}
