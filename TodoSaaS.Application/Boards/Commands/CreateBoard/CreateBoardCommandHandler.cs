using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoSaaS.Application.Common.Interfaces;
using TodoSaaS.Application.Common.Exceptions;
using TodoSaaS.Domain.Entities;

namespace TodoSaaS.Application.Boards.Commands.CreateBoard;

public class CreateBoardCommandHandler : IRequestHandler<CreateBoardCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateBoardCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateBoardCommand request, CancellationToken cancellationToken)
    {
        var WorkSpaceExists = await _context.Workspaces
            .AnyAsync(w => w.Id == request.WorkspaceId, cancellationToken);

        if (!WorkSpaceExists)
        {
            throw new NotFoundException(nameof(Workspace), request.WorkspaceId);
        }

        var entity = new Board
        {
            Name = request.Name,
            Description = request.Description,
            WorkspaceId = request.WorkspaceId
        };

        _context.Boards.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);


        return entity.Id;
    }
}