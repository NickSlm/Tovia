using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;
using Tovia.Models;
using Tovia.Converters;


namespace Tovia.Services
{
    public class AuthService
    {

        private readonly IConfiguration _config;
        private readonly string ClientId;
        private readonly string Tenant;
        private readonly string Instance;
        private readonly string[] Scopes;
        private string _oId;
        private string _accountUsername;
        private string _accountTaskListId;
        private BitmapImage _accountProfilePic;
        private IPublicClientApplication _clientApp;
        private readonly HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("https://graph.microsoft.com")
        };

        public AuthService(IConfiguration config)
        {
            _config = config;

            var azureAd = _config.GetSection("AzureAd").Get<AzureAd>();

            ClientId = azureAd.ClientId;
            Tenant = azureAd.Tenant;
            Instance = azureAd.Instance;
            Scopes = azureAd.Scopes;

            CreateApplication();
        }

        public string AccountUsername
        {
            get => _accountUsername; 
        }
        public string OID
        {
            get => _oId;
        }
        public string AccountTaskListId
        {
            get => _accountTaskListId;
        }
        public BitmapImage AccountProfilePic
        {
            get => _accountProfilePic;
        }
        public IPublicClientApplication ClientApp 
        { 
            get => _clientApp;
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
            _oId = authResult.Account.HomeAccountId.ObjectId;
            _accountTaskListId = await GetDefaultTaskListIdAsync(authResult.AccessToken);
            _accountUsername = await GetProfileDisplayNameAsync(authResult.AccessToken);
            _accountProfilePic = await GetProfilePictureAsync(authResult.AccessToken);
        }
        public async Task<string> GetAccessToken()
        {
            IAccount? firstAccount = (await ClientApp.GetAccountsAsync()).FirstOrDefault();
            var authResult = await ClientApp.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();
            return authResult.AccessToken;
        }
        public async Task<BitmapImage> GetProfilePictureAsync(string accessToken)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, $"v1.0/me/photo/$value");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);


            if (!response.IsSuccessStatusCode)
            {
                var bmp = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/ProfilePlaceHolder.png", UriKind.Absolute));
                return bmp;
            }

            var result = await response.Content.ReadAsByteArrayAsync();
            BitmapImage profilePic = ConvertBytesToBitmapImage.ConvertToImage(result);
            return profilePic;
        }
        public async Task<string> GetProfileDisplayNameAsync(string accessToken)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, $"v1.0/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(json);
            return result.RootElement.GetProperty("displayName").ToString();
        }
        public async Task<string> GetDefaultTaskListIdAsync(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"v1.0/me/todo/lists");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

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
