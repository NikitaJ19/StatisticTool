using Microsoft.AspNetCore.Mvc;
using StatisticsToolDbDevOpsLibrary.Interfaces;
using StatisticsToolDbDevOpsLibrary.Models;
using StatisticsToolWebUi.Models;

namespace StatisticsToolWebUi.Controllers
{
    public class WorkItemStatisticsController : Controller
    {
        private readonly IWorkItemDbService _dbService;
        public WorkItemStatisticsController(IWorkItemDbService dbService) 
        { 
             _dbService = dbService;
        }
        public async Task<IActionResult> Analytics()
        {
            try
            {
                var workItemStats = await _dbService.GetWorkItemsDb();
                return View(workItemStats);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading work item statistics.";
                return View(new List<WorkItemDto>());
            }
        }

        public async Task<IActionResult> Chart()
        {
            try
            {
                 var workItemStats = await _dbService.GetWorkItemsDb();
                 var timeGroups = workItemStats.OrderBy(x => x.createdOn) 
                                 .GroupBy(x => x.createdOn.ToString("yyyy-MM-dd HH:mm"))
                                 .ToList();

                var time = timeGroups.Select(g => g.Key).ToList();

                var featureData = timeGroups.Select(g =>
                {
                    var feature = g.Where(x => x.workItemType == "Feature").SingleOrDefault();
                    return feature != null ? feature.workItemCount : 0;
                }).ToList();

                var bugData = timeGroups.Select(g =>
                {
                    var bug = g.Where(x => x.workItemType == "Bug").SingleOrDefault();
                    return bug != null ? bug.workItemCount : 0;
                }).ToList();

                var chartModel = new ChartModel
                {
                    time = time,
                    featureData = featureData,
                    bugData = bugData
                };

                return View(chartModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading work item statistics.";
                return View(new List<WorkItemDto>());
            }
        }



    }
}
