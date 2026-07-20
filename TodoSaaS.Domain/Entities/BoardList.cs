using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoSaaS.Domain.Common;

namespace TodoSaaS.Domain.Entities
{
    public class BoardList : BaseEntity
    {
        public string Title {get; set; } = string.Empty;
        public int Position {get; set;}
        public Guid BoardId {get; set;}
        public Board Board {get; set; } = null!;

        public ICollection<ProjectTask> Tasks {get; set; } = new List<ProjectTask>();
    }
}