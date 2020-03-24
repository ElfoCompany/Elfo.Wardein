using Elfo.Wardein.Abstractions.Configuration.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32.SafeHandles;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.Tests
{

    [TestClass]
    public class WebWatcherCallWithApiClient
    {
        private WebWatcherConfigurationModel configuration;
        private  string userNameToImpersonate;
        private  string domainToImpersonate;
        private  string userPasswordToImpersonate;

        [TestInitialize]
        public void Initialize()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

             userNameToImpersonate = configuration["Impersonate:userNameToImpersonate"];
             domainToImpersonate = configuration["Impersonate:userDomainToImpersonate"];
             userPasswordToImpersonate = configuration["Impersonate:userPasswordToImpersonate"];
        }

        async Task<bool> IsSuccessStatusCode(HttpClient client)
        {
           var response = await client.GetAsync(configuration.Url.AbsoluteUri);
            return response.IsSuccessStatusCode;
        }

        [TestMethod]
        [TestCategory("ManualTest")]
        public async Task IsHealthy()
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true, PreAuthenticate = true, AllowAutoRedirect = true });
            client.BaseAddress = new Uri(configuration.Url.AbsoluteUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = false;
            try
            {                 
                ImpersonateUser iu = new ImpersonateUser();
                iu.RunImpersonated(domainToImpersonate, userNameToImpersonate, userPasswordToImpersonate,
                      () => IsSuccessStatusCode(client).Wait());
                response =  await IsSuccessStatusCode(client);
            }
            catch(Exception ex) {
                Console.WriteLine($"There is an error: {ex}");
            }

            Assert.IsTrue(response);
        }
    }

    [TestClass]
    public class ImpersonateUser
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

        // Test harness. 
        // If you incorporate this code into a DLL, be sure to demand FullTrust.
        [TestMethod]
        [TestCategory("ManualTest")]
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void RunImpersonated(string domainName, string userName, string password, Action action)
        {
            try
            {
                const int LOGON32_PROVIDER_DEFAULT = 0;
                //This parameter causes LogonUser to create a primary token. 
                const int LOGON32_LOGON_INTERACTIVE = 2;

                // Call LogonUser to obtain a handle to an access token. 
                SafeAccessTokenHandle safeAccessTokenHandle;
                bool returnValue = LogonUser(userName, domainName, password,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    out safeAccessTokenHandle);

                if (false == returnValue)
                {
                    int ret = Marshal.GetLastWin32Error();
                    throw new System.ComponentModel.Win32Exception(ret);
                }
                 WindowsIdentity.RunImpersonated(safeAccessTokenHandle, action);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred. " + ex.Message);
            }
        }
    }
}
