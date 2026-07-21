using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoSaaS.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace TodoSaaS.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Workspace> Workspaces {get;}
        DbSet<Board> Boards {get;}
        DbSet<BoardList> BoardLists {get;}
        DbSet<ProjectTask> ProjectTasks {get;}

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}