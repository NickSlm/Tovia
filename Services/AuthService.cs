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


namespace ToDoListPlus.Services
{
    public class AuthService
    {
        private static readonly string clientId = "9c077a27-edb1-48e8-b0e8-52cbac5e502c";
        private static readonly string Tenant = "consumers";
        private static readonly string Instance = "https://login.microsoftonline.com/";
        private static readonly string[] scopes = new string[] { "user.read", "Calendars.ReadWrite" };

        private string _accessToken = string.Empty;
        private string _accountUsername = string.Empty;
        public string AccessToken
        {
            get { return _accessToken; }
        }
        public string AccountUsername
        {
            get { return _accountUsername; }
        }

        private static IPublicClientApplication _clientApp;
        public static IPublicClientApplication ClientApp { get { return _clientApp; } }

        public AuthService()
        {
            CreateApplication();
        }

        public static void CreateApplication()
        {
            _clientApp = PublicClientApplicationBuilder.Create(clientId).WithAuthority($"{Instance}{Tenant}").WithDefaultRedirectUri().Build();

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
                authResult = await ClientApp.AcquireTokenSilent(scopes, firstAccount).ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");
                try
                {
                    authResult = await ClientApp.AcquireTokenInteractive(scopes)
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
            return $"Authorization Succeded";
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

        public async Task<string> PostTaskAsync(string title, string? description, DateTime? date)
        {
            var httpClient = new HttpClient();
            string url = "https://graph.microsoft.com/v1.0/me/events";
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);

            var eventData = new
            {
                subject = title,
                isReminderOn = true,
                reminderMinutesBeforeStart = 720,
                body = new
                {
                    contentType = "HTML",
                    content = description
                },
                start = new
                {
                    dateTime = date,
                    timeZone = "Israel Standard Time"
                },
                end = new
                {
                    dateTime = date,
                    timeZone = "Israel Standard Time"
                }
            };

            var json = JsonSerializer.Serialize(eventData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseBody);
                string eventId = doc.RootElement.GetProperty("id").GetString() ?? string.Empty;
                return eventId;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"Failed to create event: {response.StatusCode} - {error}";
            }
        }

        public async Task<string> DeleteTaskAsync(string eventId)
        {
            var httpClient = new HttpClient();
            string url = $"https://graph.microsoft.com/v1.0/me/events/{eventId}";
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            var response = await httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return "Event deleted successfully";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"Failed to delete event: {response.StatusCode} - {error}";
            }
        }
    }
}
