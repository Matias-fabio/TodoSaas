using MediatR;

namespace TodoSaaS.Application.Boards.Commands.CreateBoard;

public record CreateBoardCommand : IRequest<Guid>
{
    public string Name {get; init;} = string.Empty;
    public string Description {get; init;} = string.Empty;
    public Guid WorkspaceId {get; init;}
}