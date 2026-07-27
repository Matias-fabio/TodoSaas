using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoSaaS.Application.Common.Interfaces;

namespace TodoSaaS.Application.Workspaces.Queries.GetWorkspaces
{
    public class GetWorkspacesQueryHandler : IRequestHandler<GetWorkspacesQuery, List<WorkspaceDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetWorkspacesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkspaceDto>> Handle(GetWorkspacesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Workspaces
                .AsNoTracking()
                .Select(w => new WorkspaceDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Description = w.Description
                })
                .ToListAsync(cancellationToken);
        }
    }
}