using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Abstractions.WebWatcher;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32.SafeHandles;
using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Elfo.Wardein.Abstractions.Configuration.Models.WebWatcherConfigurationModel;

namespace Elfo.Wardein.Core.ServiceManager
{
    public class HttpClientUrlResponseManager : IAmUrlResponseManager
    {
        private string userNameToImpersonate;
        private string domainToImpersonate;
        private string userPasswordToImpersonate;

        protected static ILogger log = LogManager.GetCurrentClassLogger();

        public HttpClientUrlResponseManager()
        {
            var configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .Build();

            userNameToImpersonate = configuration["Impersonate:userNameToImpersonate"];
            domainToImpersonate = configuration["Impersonate:userDomainToImpersonate"];
            userPasswordToImpersonate = configuration["Impersonate:userPasswordToImpersonate"];
        }

 

        public async Task<bool> IsHealthy(WebWatcherConfigurationModel configuration, HttpCallApiMethod method)
        {
            HttpResponseMessage response = null;
            var apiClient = InitializeApiClient(configuration);
            ImpersonateUser iu = new ImpersonateUser();
            try
            {
                iu.RunImpersonated(domainToImpersonate, userNameToImpersonate, userPasswordToImpersonate,
                 async () => response = await apiClient.GetAsync(configuration.Url.AbsoluteUri));
            }
            catch (UnauthorizedAccessException ex)
            {
                log.Error($"Exception got while waiting response from {configuration.Url.AbsoluteUri} - {ex}");
                throw;
            }
            if (!configuration.AssertWithStatusCode)
            {
                if (!string.IsNullOrWhiteSpace(configuration.AssertWithRegex))
                {
                    return await CheckIsMatch(configuration.AssertWithRegex, response);
                }
                else
                {
                    return await Task.FromResult(true);
                }
            }
            else
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return await Task.FromResult(false);
                }
                else
                {
                    return await CheckIsMatch(configuration.AssertWithRegex, response);
                }
            }
        }

        private async Task<bool> CheckIsMatch(string assertionRegex, HttpResponseMessage response)
        {
            if (!string.IsNullOrWhiteSpace(assertionRegex))
            {
                var htmlResponse = await response.Content.ReadAsStringAsync();
                var isMatch = Regex.IsMatch(htmlResponse, assertionRegex);
                if (isMatch)
                {
                    return await Task.FromResult(false);
                }
                else
                {
                    return await Task.FromResult(true);
                }
            }
            else
            {
                return await Task.FromResult(true);
            }
        }

        public async Task RestartPool(string poolName)
        {
            await new IISPoolManager(poolName).Restart();
        }

        HttpClient InitializeApiClient(WebWatcherConfigurationModel configuration)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true, PreAuthenticate = true });
            client.BaseAddress = new Uri(configuration.Url.AbsoluteUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public class ImpersonateUser
        {
            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
                int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);

            // Test harness. 
            // If you incorporate this code into a DLL, be sure to demand FullTrust.

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
}
