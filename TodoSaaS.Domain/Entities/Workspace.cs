using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoSaaS.Domain.Common;

namespace TodoSaaS.Domain.Entities
{
    public class Workspace : BaseEntity
    {
        public string Name {get; set;} = string.Empty;
        public string Description {get; set;} = string.Empty;

        public ICollection<Board> Boards {get; set;} = new List<Board>();
    }
}