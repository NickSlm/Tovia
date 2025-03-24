using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;


namespace ToDoListPlus.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Microsoft.Identity.Client;
    using Microsoft.Identity.Client.Extensions.Msal;

    namespace oauth2
    {
        public class AuthService
        {
            private static string clientId = "9c077a27-edb1-48e8-b0e8-52cbac5e502c";
            private static string Tenant = "consumers";
            private static string Instance = "https://login.microsoftonline.com/";

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

        }
    }

}
