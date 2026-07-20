using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoSaaS.Domain.Common;

namespace TodoSaaS.Domain.Entities
{
    public class Board : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description {get; set;} = string.Empty;

        public Guid WorkspaceId {get; set;}
        public Workspace Workspace {get; set; } = null!;

        public ICollection<BoardList> Lists {get; set;} = new List<BoardList>();
    }
}