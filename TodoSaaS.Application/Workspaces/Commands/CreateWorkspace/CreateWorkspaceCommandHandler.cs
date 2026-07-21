using MediatR;
using TodoSaaS.Application.Common.Interfaces;
using TodoSaaS.Domain.Entities;
    
namespace TodoSaaS.Application.Workspaces.Commands.CreateWorkspace;
    
    public class CreateWorkspaceCommandHandler : IRequestHandler<CreateWorkspaceCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
    
        // Inyectamos la interfaz de la base de datos en el constructor
        public CreateWorkspaceCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
    
        public async Task<Guid> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
        {
            // 1. Instanciamos la entidad de dominio
            var entity = new Workspace
            {
                Name = request.Name,
                Description = request.Description
            };
    
            // 2. La agregamos al DbSet correspondiente
            _context.Workspaces.Add(entity);
    
            // 3. Guardamos los cambios de forma asíncrona
            await _context.SaveChangesAsync(cancellationToken);
    
            // 4. Retornamos el ID de la entidad creada
            return entity.Id;
        }
    }
