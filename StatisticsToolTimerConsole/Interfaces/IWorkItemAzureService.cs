using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StatisticsToolDbDevOpsLibrary.Models;

namespace StatisticsToolTimerConsole.Interfaces
{
    public interface IWorkItemAzureService
    {
        public Task<List<WorkItemDto>> GetWorkItemAzure();
        public Task AddWorkItem();
    }
}
