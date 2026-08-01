using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoSaaS.Application.Boards.Queries
{
    public class BoardDto
    {
        public Guid Id {get; set;}
        public string Name {get; set;} = string.Empty;
        public string Description {get; set;} = string.Empty;
        public Guid WorkspaceId{get; set;}
    }
}