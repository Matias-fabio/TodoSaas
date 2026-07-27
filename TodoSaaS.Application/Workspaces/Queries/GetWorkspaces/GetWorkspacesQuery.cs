    using MediatR;
    
    namespace TodoSaaS.Application.Workspaces.Queries.GetWorkspaces;
    
    // Definimos que esta consulta retornará una lista de WorkspaceDto
    public record GetWorkspacesQuery : IRequest<List<WorkspaceDto>>;
