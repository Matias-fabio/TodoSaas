using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoSaaS.Domain.Common;
using TodoSaaS.Domain.Enums;

namespace TodoSaaS.Domain.Entities
{
    public class ProjectTask : BaseEntity
    {
        public string Title{get; set;} = string.Empty;
        public string Description {get; set; } = string.Empty;
        public int Position {get; set;}

        public TaskPriority Priority {get; set;} = TaskPriority.Medium;
        public DateTime? DueTime {get; set;}
        public Guid BoardListId {get; set;}
        public BoardList BoardList {get; set;} = null!;
    }
}