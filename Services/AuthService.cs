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


namespace ToDoListPlus.Services
{
    public class AuthService
    {
        private static readonly string clientId = "9c077a27-edb1-48e8-b0e8-52cbac5e502c";
        private static readonly string Tenant = "consumers";
        private static readonly string Instance = "https://login.microsoftonline.com/";
        private static readonly string[] scopes = new string[] { "user.read", "Calendars.ReadWrite" };

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

        public async Task<string> GetAccessTokenAsync(Window parentWindow)
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
                        .WithParentActivityOrWindow(new WindowInteropHelper(parentWindow).Handle)
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

            return authResult.Account.Username;
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
