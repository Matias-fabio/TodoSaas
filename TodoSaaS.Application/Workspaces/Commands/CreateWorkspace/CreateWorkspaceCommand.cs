using MediatR;

namespace TodoSaaS.Application.Workspaces.Commands.CreateWorkspace;

public record CreateWorkspaceCommand : IRequest<Guid>
{
    public string Name {get; init; } = string.Empty;
        public string Description {get; init; } = string.Empty;
        
}