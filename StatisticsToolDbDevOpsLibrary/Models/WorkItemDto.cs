using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsToolDbDevOpsLibrary.Models
{
    public class WorkItemDto
    {
        public string workItemType { get; set; }
        public int workItemCount { get; set; }
        public DateTime createdOn    { get; set; }
    }

    public class WorkItemStatisticsDto
    {
        public string workItemType { get; set; }
        public int workItemCount { get; set; }
    }

    public class WorkItemResponseDto
    {
        public int statusCode { get; set; }
        public string? message { get; set; }
    }
}
