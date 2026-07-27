using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoSaaS.Application.Workspaces.Commands.CreateWorkspace;
using TodoSaaS.Application.Workspaces.Queries;
using TodoSaaS.Application.Workspaces.Queries.GetWorkspaces;

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

        [HttpGet]
        public async Task<ActionResult<List<WorkspaceDto>>> Get()
        {
            var Workspaces = await Mediator.Send(new GetWorkspacesQuery());
            return Ok(Workspaces);
        }
        
    }
}