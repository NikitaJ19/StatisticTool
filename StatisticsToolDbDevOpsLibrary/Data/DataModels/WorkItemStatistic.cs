using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StatisticsToolDbDevOpsLibrary.Data.DataModels;

public partial class WorkItemStatistic
{
    [Key]
    public int WorkItemStatisticsId { get; set; }

    [StringLength(255)]
    public string Type { get; set; } = null!;

    public int Count { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedOn { get; set; }
}
