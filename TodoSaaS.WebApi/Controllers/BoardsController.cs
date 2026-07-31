using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoSaaS.Application.Boards.Commands.CreateBoard;

namespace TodoSaaS.WebApi.Controllers
{
    public class BoardsController : ApiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateBoardCommand command)
        {
            var boardId = await Mediator.Send(command);
            return Ok(boardId) ;  
        }
    }
}