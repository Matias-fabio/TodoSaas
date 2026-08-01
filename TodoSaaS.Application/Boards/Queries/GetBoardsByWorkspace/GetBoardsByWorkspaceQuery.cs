
using MediatR;
using TodoSaaS.Domain.Entities;

namespace TodoSaaS.Application.Boards.Queries.GetBoardsByWorkspace
{
    public record GetBoardsByWorkspaceQuery(Guid WorkspaceId) : IRequest<List<BoardDto>>;
  
}