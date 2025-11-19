using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StatisticsToolDbDevOpsLibrary.Models;
using StatisticsToolDbDevOpsLibrary.DbServices;
using StatisticsToolTimerConsole.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StatisticsToolDbDevOpsLibrary.Interfaces;

namespace StatisticsToolTimerConsole.BusinessServices
{
    public class WorkItemAzureService : IWorkItemAzureService
    {
        private readonly IAzureAuthService _authService;
        private IConfiguration _configuration;
        private ILogger<WorkItemAzureService> _logger;
        private readonly IWorkItemDbService _dbService;


        public WorkItemAzureService(IAzureAuthService authService, IWorkItemDbService dbService, IConfiguration configuration, ILogger<WorkItemAzureService> logger)
        {
            _authService = authService;
            _dbService = dbService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<WorkItemDto>> GetWorkItemAzure()
        {
            try
            {
                var client = _authService.GetAuthenticatedClient();

                //Query to get work item Ids
                var workItemQuery = new
                {
                    query = @"SELECT [System.Id], [System.WorkItemType]
                              FROM WorkItems
                              WHERE [System.TeamProject] = @project
                              And[System.WorkItemType] IN('Feature', 'Bug')"
                };
                var queryResponse = await client.PostAsJsonAsync($"{_configuration["AzureWorkItem:Project"]}/_apis/wit/wiql?api-version=7.1", workItemQuery);
                queryResponse.EnsureSuccessStatusCode();

                var queryJson = JsonDocument.Parse(await queryResponse.Content.ReadAsStringAsync());
                //Console.WriteLine(queryJson);

                var workItemIds = queryJson.RootElement.GetProperty("workItems")
                    .EnumerateArray()
                    .Select(x => x.GetProperty("id").GetInt32())
                    .ToArray();

                if (!workItemIds.Any())
                {
                    return new List<WorkItemDto>();
                }

                // Batch request to get work item statistics using the retrieved Ids
                var batchQuery = new { ids = workItemIds, fields = new[] { "System.WorkItemType" } };
                var batchResponse = await client.PostAsJsonAsync($"_apis/wit/workitemsbatch?api-version=7.1", batchQuery);
                batchResponse.EnsureSuccessStatusCode();
                var batchJson = JsonDocument.Parse(await batchResponse.Content.ReadAsStringAsync());
                //Console.WriteLine(batchJson);

                var statsCount = batchJson.RootElement.GetProperty("value")
                    .EnumerateArray()
                    .Select(x => x.GetProperty("fields").GetProperty("System.WorkItemType").GetString())
                    .GroupBy(type => type)
                    .Select(g => new WorkItemDto { workItemType = g.Key, workItemCount = g.Count(), createdOn = DateTime.UtcNow })
                    .ToList();

                return statsCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error fetching work items");
                return new List<WorkItemDto>();
            }
        }

        public async Task AddWorkItem()
        {
            var stats = await GetWorkItemAzure();
            if (stats.Count == 0)
            {
                _logger.LogInformation("No work item stats retrieved.");
                return;
            }

            var response = await _dbService.AddWorkItemsDb(stats);
            if (response.statusCode != 200)
            {
                _logger.LogWarning("Failed to add work item: {Message}", response.message);
            }
            else
            {
                _logger.LogInformation("Work item added");
            }
        }

    }
}
