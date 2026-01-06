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
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;
using Tovia.interfaces;


namespace Tovia.Services
{
    public class MicrosoftAuthService: IAuthProvider
    {
        public string? AccessToken { get; set; }
        private readonly IConfiguration _config;
        private readonly string ClientId;
        private readonly string Tenant;
        private readonly string Instance;
        private readonly string[] Scopes;
        private IPublicClientApplication _clientApp;
        private readonly HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("https://graph.microsoft.com")
        };

        public MicrosoftAuthService(IConfiguration config)
        {
            _config = config;

            var azureAd = _config.GetSection("AzureAd").Get<AzureAd>();

            ClientId = azureAd.ClientId;
            Tenant = azureAd.Tenant;
            Instance = azureAd.Instance;
            Scopes = azureAd.Scopes;

            CreateApplication();
        }

        public UserProfile? User { get; private set; }
        public AuthenticationResult? AuthResult { get; private set; }

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
        public async Task SignInAsync()
        {
            IAccount? firstAccount = (await ClientApp.GetAccountsAsync()).FirstOrDefault();

            if (firstAccount == null)
            {
                firstAccount = PublicClientApplication.OperatingSystemAccount;
            }
            try
            {
                AuthResult = await ClientApp.AcquireTokenSilent(Scopes, firstAccount).ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");
                try
                {
                    AuthResult = await ClientApp.AcquireTokenInteractive(Scopes)
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
            AccessToken = AuthResult.AccessToken;
            await LoadProfileAsync();
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
        private async Task LoadProfileAsync()
        {

            var firstName = await GetProfileDisplayNameAsync();
            var taskListId = await GetDefaultTaskListIdAsync();
            var pfp = await GetProfilePictureAsync();

            User = new UserProfile
            {
                Id = AuthResult.Account.HomeAccountId.ObjectId,
                FirstName = firstName,
                Pfp = pfp,
                TaskListId = taskListId
            };

        }
        private async Task<BitmapImage> GetProfilePictureAsync()
        {
            var accessToken = AuthResult.AccessToken;

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
        private async Task<string> GetProfileDisplayNameAsync()
        {
            var accessToken = AuthResult.AccessToken;

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
        private async Task<string> GetDefaultTaskListIdAsync()
        {
            var accessToken = AuthResult.AccessToken;

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
    }
}
