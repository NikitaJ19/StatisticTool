using StatisticsToolDbDevOpsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsToolDbDevOpsLibrary.Interfaces
{
    public interface IWorkItemDbService
    {
        public Task<WorkItemResponseDto> AddWorkItemsDb(List<WorkItemDto> workItemsDto);
        public Task<List<WorkItemDto>> GetWorkItemsDb();
    }
}
