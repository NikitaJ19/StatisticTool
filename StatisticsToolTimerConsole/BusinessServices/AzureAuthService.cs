using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StatisticsToolTimerConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsToolTimerConsole.BusinessServices
{
    public class AzureAuthService : IAzureAuthService
    {
        private IConfiguration _configuration;
        private ILogger<AzureAuthService> _logger;

        public AzureAuthService(IConfiguration configuration, ILogger<AzureAuthService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public HttpClient GetAuthenticatedClient()
        {
            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri($"https://dev.azure.com/{_configuration["AzureWorkItem:Organization"]}/")
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($":{_configuration["AzureWorkItem:PAT"]}"))
                );

                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in authentication = " + ex );
                _logger.LogError(ex, "Error creating authenticated client");
                throw;
            }
        }

    }
}
