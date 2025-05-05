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
        private static string _clientId;
        private static string _tenant;
        private static string _instance;
        private static string[] _scopes;

        private string _accessToken = string.Empty;
        private string _accountUsername = string.Empty;
        private string _accountTaskListId = string.Empty;



        public static string ClientId => _clientId;
        public static string Tenant => _tenant;
        public static string Instance => _instance;
        public static string[] Scopes => _scopes;
        public string AccessToken
        {
            get { return _accessToken; }
        }
        public string AccountUsername
        {
            get { return _accountUsername; }
        }
        public string AccountTaskListId
        {
            get { return _accountTaskListId; }
        }

        private static IPublicClientApplication _clientApp;
        public static IPublicClientApplication ClientApp { get { return _clientApp; } }

        public AuthService(AuthConfig AuthConfig)
        {
            _clientId = AuthConfig.ClientId;
            _tenant = AuthConfig.Tenant;
            _instance = AuthConfig.Instance;
            _scopes = AuthConfig.Scopes;

            CreateApplication();
        }

        public static void CreateApplication()
        {
            _clientApp = PublicClientApplicationBuilder.Create(ClientId).WithAuthority($"{Instance}{Tenant}").WithDefaultRedirectUri().Build();

            MsalCacheHelper cacheHelper = CreateCacheHelperAsync().GetAwaiter().GetResult();
            cacheHelper.RegisterCache(_clientApp.UserTokenCache);
        }
        private static async Task<MsalCacheHelper> CreateCacheHelperAsync()
        {
            var storageProperties = new StorageCreationPropertiesBuilder(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".msalcache.bin", MsalCacheHelper.UserRootDirectory).Build();
            MsalCacheHelper cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties, new TraceSource("MSAL.CacheTrace")).ConfigureAwait(false);

            return cacheHelper;

        }
        public async Task<string> GetAccessTokenAsync()
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

            _accessToken = authResult.AccessToken;
            _accountUsername = authResult.Account.Username;
            _accountTaskListId = await GetDefaultTaskListIdAsync();


            return $"Authorization Succeded";
        }
        public async Task<string> GetDefaultTaskListIdAsync()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

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
                    _accessToken = string.Empty;
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
