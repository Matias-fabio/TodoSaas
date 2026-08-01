    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using TodoSaaS.Application.Common.Interfaces;
    using TodoSaaS.Application.Common.Exceptions;
    using TodoSaaS.Domain.Entities;
    
    namespace TodoSaaS.Application.Boards.Queries.GetBoardsByWorkspace;
    
    public class GetBoardsByWorkspaceQueryHandler : IRequestHandler<GetBoardsByWorkspaceQuery, List<BoardDto>>
    {
        private readonly IApplicationDbContext _context;
    
        public GetBoardsByWorkspaceQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
    
        public async Task<List<BoardDto>> Handle(GetBoardsByWorkspaceQuery request, CancellationToken cancellationToken)
        {
            // 1. Validar que el Workspace exista
            var workspaceExists = await _context.Workspaces
                .AnyAsync(w => w.Id == request.WorkspaceId, cancellationToken);
    
            if (!workspaceExists)
            {
                throw new NotFoundException(nameof(Workspace), request.WorkspaceId);
            }
    
            // 2. Retornar los tableros mapeados a DTOs
            return await _context.Boards
                .AsNoTracking()
                .Where(b => b.WorkspaceId == request.WorkspaceId)
                .Select(b => new BoardDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    WorkspaceId = b.WorkspaceId
                })
                .ToListAsync(cancellationToken);
        }
    }
