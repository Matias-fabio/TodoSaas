using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoSaaS.Application.Workspaces.Commands.CreateWorkspace;

namespace TodoSaaS.WebApi.Controllers
{
    public class WorkspacesController : ApiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateWorkspaceCommand command)
        {
            var workspaceId = await Mediator.Send(command);
            return Ok(workspaceId);
        }
        
    }
}