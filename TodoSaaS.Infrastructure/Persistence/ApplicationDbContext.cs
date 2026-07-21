using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoSaaS.Application.Common.Interfaces;
using TodoSaaS.Domain.Entities;


namespace TodoSaaS.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options)
        {
        }

        //cada Dbset representa una tabla en la base de datos.

        public DbSet<Workspace> Workspaces => Set<Workspace>();
        public DbSet<Board> Boards => Set<Board>();
        public DbSet<BoardList> BoardLists => Set<BoardList>();
        public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();

   protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}