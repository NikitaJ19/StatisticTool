using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StatisticsToolDbDevOpsLibrary.Data.Data;
using StatisticsToolDbDevOpsLibrary.Data.DataModels;
using StatisticsToolDbDevOpsLibrary.Interfaces;
using StatisticsToolDbDevOpsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsToolDbDevOpsLibrary.DbServices
{
    public class WorkItemDbService : IWorkItemDbService
    {
        private readonly StatsDbContext _dbContext;
        private readonly ILogger<WorkItemDbService> _logger;

        public WorkItemDbService(StatsDbContext dbContext, ILogger<WorkItemDbService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<WorkItemResponseDto> AddWorkItemsDb(List<WorkItemDto> workItemsDto)
        {
            if (workItemsDto == null || !workItemsDto.Any())
            {
                return new WorkItemResponseDto
                {
                    statusCode = 400,
                    message = "Work item list is empty"
                };
            }

            try
            {
                var workItemStatistic = workItemsDto.Select(dto => new WorkItemStatistic
                {
                    Type = dto.workItemType,
                    Count = dto.workItemCount,
                    CreatedOn = dto.createdOn
                }).ToList();
                await _dbContext.WorkItemStatistics.AddRangeAsync(workItemStatistic);
                await _dbContext.SaveChangesAsync();

                return new WorkItemResponseDto
                {
                    statusCode = 200,
                    message = "Work items added"
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding work items");
                return new WorkItemResponseDto
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }

       public async Task<List<WorkItemDto>> GetWorkItemsDb()
        {
            try
            {
                var workItemStats = await _dbContext.WorkItemStatistics
                    .OrderByDescending(x => x.CreatedOn)
                    .ToListAsync();

                var workItemDto = workItemStats.Select(s => new WorkItemDto
                {
                    workItemType = s.Type,
                    workItemCount = s.Count,
                    createdOn = s.CreatedOn
                }).ToList();

                return workItemDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching work items from database");
                return new List<WorkItemDto>();
            }
        }
    }
}
