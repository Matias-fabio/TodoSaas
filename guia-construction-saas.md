# Guía de Construcción: TodoSaaS (.NET 8 + Postgres + React)

Esta guía documenta detalladamente todos los pasos realizados hasta ahora en la creación del proyecto SaaS tipo Trello/ClickUp. Sirve como registro de aprendizaje y para reanudar el contexto en cualquier otra máquina.

---

## FASE 1: Estructuración y Arquitectura Limpia

Configuramos una solución dividida en 4 capas para aislar la lógica del negocio de los detalles técnicos.

### 1. Inicialización del Repositorio y Git
```bash
git init
dotnet new gitignore
git branch -m main
git config --global init.defaultBranch main
```

### 2. Creación de la Solución y Capas (.csproj)
```bash
# Crear contenedor principal (.sln)
dotnet new sln -n TodoSaaS

# Crear bibliotecas de clases para la lógica
dotnet new classlib -o TodoSaaS.Domain
dotnet new classlib -o TodoSaaS.Application
dotnet new classlib -o TodoSaaS.Infrastructure

# Crear proyecto web para la API
dotnet new webapi -o TodoSaaS.WebApi

# Asociar proyectos a la solución
dotnet sln add TodoSaaS.Domain/TodoSaaS.Domain.csproj
dotnet sln add TodoSaaS.Application/TodoSaaS.Application.csproj
dotnet sln add TodoSaaS.Infrastructure/TodoSaaS.Infrastructure.csproj
dotnet sln add TodoSaaS.WebApi/TodoSaaS.WebApi.csproj
```

### 3. Configuración de Referencias (Límites de la Arquitectura)
Para asegurar que todo apunte hacia el Dominio y no al revés:
```bash
# Application depende únicamente de Domain
dotnet add TodoSaaS.Application/TodoSaaS.Application.csproj reference TodoSaaS.Domain/TodoSaaS.Domain.csproj

# Infrastructure depende de Application
dotnet add TodoSaaS.Infrastructure/TodoSaaS.Infrastructure.csproj reference TodoSaaS.Application/TodoSaaS.Application.csproj

# WebApi depende de Application y de Infrastructure
dotnet add TodoSaaS.WebApi/TodoSaaS.WebApi.csproj reference TodoSaaS.Application/TodoSaaS.Application.csproj
dotnet add TodoSaaS.WebApi/TodoSaaS.WebApi.csproj reference TodoSaaS.Infrastructure/TodoSaaS.Infrastructure.csproj
```

---

## FASE 2: Modelado del Dominio (El Corazón del Negocio)

Diseñamos las entidades y reglas de negocio sin ninguna dependencia externa (sin base de datos, sin frameworks).

### 1. Clase Base (`TodoSaaS.Domain/Common/BaseEntity.cs`)
Define el identificador único `Guid` y campos de auditoría comunes:
```csharp
namespace TodoSaaS.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }
}
```

### 2. Enum de Prioridades (`TodoSaaS.Domain/Enums/TaskPriority.cs`)
```csharp
namespace TodoSaaS.Domain.Enums;

public enum TaskPriority
{
    Low,
    Medium,
    High
}
```

### 3. Entidades Principales (`TodoSaaS.Domain/Entities/`)

* **`Workspace.cs`**:
```csharp
using TodoSaaS.Domain.Common;

namespace TodoSaaS.Domain.Entities;

public class Workspace : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Board> Boards { get; set; } = new List<Board>();
}
```

* **`Board.cs`**:
```csharp
using TodoSaaS.Domain.Common;

namespace TodoSaaS.Domain.Entities;

public class Board : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;
    public ICollection<BoardList> Lists { get; set; } = new List<BoardList>();
}
```

* **`BoardList.cs`**:
```csharp
using TodoSaaS.Domain.Common;

namespace TodoSaaS.Domain.Entities;

public class BoardList : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public int Position { get; set; }
    public Guid BoardId { get; set; }
    public Board Board { get; set; } = null!;
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}
```

* **`ProjectTask.cs`**:
```csharp
using TodoSaaS.Domain.Common;
using TodoSaaS.Domain.Enums;

namespace TodoSaaS.Domain.Entities;

public class ProjectTask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Position { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueTime { get; set; }
    public Guid BoardListId { get; set; }
    public BoardList BoardList { get; set; } = null!;
}
```

---

## FASE 3: Conexión a Base de Datos (Infraestructura)

Configuramos Entity Framework Core y PostgreSQL para persistir nuestras entidades.

### 1. Instalación de Paquetes Compatibles con .NET 8
```bash
# Npgsql para Postgres en la capa de Infraestructura
dotnet add TodoSaaS.Infrastructure/TodoSaaS.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL -v 8.0.8

# Herramientas de Diseño en el proyecto WebApi para Migraciones
dotnet add TodoSaaS.WebApi/TodoSaaS.WebApi.csproj package Microsoft.EntityFrameworkCore.Design -v 8.0.8
```

### 2. Contexto de Base de Datos (`TodoSaaS.Infrastructure/Persistence/ApplicationDbContext.cs`)
```csharp
using Microsoft.EntityFrameworkCore;
using TodoSaaS.Domain.Entities;

namespace TodoSaaS.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<BoardList> BoardLists => Set<BoardList>();
    public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
}
```

### 3. Registro de Servicios y Conexión (`appsettings.json` y `Program.cs`)

* **En `TodoSaaS.WebApi/appsettings.json`**:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=TodoSaaS;Username=postgres;Password=admin123"
}
```

* **En `TodoSaaS.WebApi/Program.cs`**:
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### 4. Generación y Aplicación de Migraciones
Instalamos y configuramos la herramienta CLI de EF Core, y creamos la base de datos física:
```bash
# Instalar herramienta global
dotnet tool install --global dotnet-ef --version 8.0.8

# Configurar variables de entorno si usamos snap dotnet
export PATH="$PATH:$HOME/.dotnet/tools"
export DOTNET_ROOT=/snap/dotnet-sdk/current
export PATH=$PATH:$DOTNET_ROOT

# Crear migración inicial
dotnet ef migrations add InitialCreate --project TodoSaaS.Infrastructure --startup-project TodoSaaS.WebApi

# Aplicar migración (Crear BD y Tablas en Postgres)
dotnet ef database update --project TodoSaaS.Infrastructure --startup-project TodoSaaS.WebApi
```

---

## FASE 4: Capa de Aplicación y API (MediatR, CQRS y Controladores)

Configuramos la lógica de casos de uso (Capa de Aplicación) utilizando el patrón **CQRS** (Command Query Responsibility Segregation) con la librería **MediatR**, y expusimos los endpoints usando controladores clásicos en la Web API.

### 1. Instalación de MediatR en Aplicación y WebApi
```bash
# Instalar MediatR en la capa de Aplicación
dotnet add TodoSaaS.Application/TodoSaaS.Application.csproj package MediatR -v 12.4.1

# Instalar MediatR en la capa WebApi para registro de servicios
dotnet add TodoSaaS.WebApi/TodoSaaS.WebApi.csproj package MediatR -v 12.4.1

# Instalar EntityFrameworkCore en la capa de Aplicación (necesario para usar DbSet en la interfaz)
dotnet add TodoSaaS.Application/TodoSaaS.Application.csproj package Microsoft.EntityFrameworkCore -v 8.0.8
```

### 2. Inversión de Dependencias (DIP)
Para que la capa de Aplicación no dependa directamente de la infraestructura de la base de datos, creamos una interfaz intermedia.

* **`TodoSaaS.Application/Common/Interfaces/IApplicationDbContext.cs`**:
```csharp
using Microsoft.EntityFrameworkCore;
using TodoSaaS.Domain.Entities;

namespace TodoSaaS.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Workspace> Workspaces { get; }
    DbSet<Board> Boards { get; }
    DbSet<BoardList> BoardLists { get; }
    DbSet<ProjectTask> ProjectTasks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
```

* **`TodoSaaS.Infrastructure/Persistence/ApplicationDbContext.cs`** (Implementación):
```csharp
// Modificamos la definición de la clase para implementar la interfaz
public class ApplicationDbContext : DbContext, IApplicationDbContext
```

### 3. Caso de Uso: Crear un Workspace (`CreateWorkspace`)
Dividimos la lógica en un **Command** (petición de datos inmutable) y un **Handler** (procesador de la lógica).

* **`TodoSaaS.Application/Workspaces/Commands/CreateWorkspace/CreateWorkspaceCommand.cs`**:
```csharp
using MediatR;

namespace TodoSaaS.Application.Workspaces.Commands.CreateWorkspace;

public record CreateWorkspaceCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
```

* **`TodoSaaS.Application/Workspaces/Commands/CreateWorkspace/CreateWorkspaceCommandHandler.cs`**:
```csharp
using MediatR;
using TodoSaaS.Application.Common.Interfaces;
using TodoSaaS.Domain.Entities;

namespace TodoSaaS.Application.Workspaces.Commands.CreateWorkspace;

public class CreateWorkspaceCommandHandler : IRequestHandler<CreateWorkspaceCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateWorkspaceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var entity = new Workspace
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Workspaces.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
```

### 4. Configuración de API y Controladores en WebApi

* **Registro en `TodoSaaS.WebApi/Program.cs`**:
```csharp
// 1. Habilitar controladores y MediatR (antes de builder.Build())
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(TodoSaaS.Application.Common.Interfaces.IApplicationDbContext).Assembly);
});

// 2. Mapear rutas de controladores (antes de app.Run())
app.MapControllers();
```

* **Controlador Base (`TodoSaaS.WebApi/Controllers/ApiControllerBase.cs`)**:
Implementa Lazy Loading para inyectar MediatR (`ISender`) de forma limpia en todos los controladores sin usar constructores manuales.
```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TodoSaaS.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    // Uso de expresión corporal y operador de asignación coalescente nula (??=)
    // Si _mediator es nulo, busca el servicio en el contenedor y lo asigna; si no, lo retorna.
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
```

* **Controlador Concreto (`TodoSaaS.WebApi/Controllers/WorkspacesController.cs`)**:
```csharp
using Microsoft.AspNetCore.Mvc;
using TodoSaaS.Application.Workspaces.Commands.CreateWorkspace;

namespace TodoSaaS.WebApi.Controllers;

public class WorkspacesController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateWorkspaceCommand command)
    {
        var workspaceId = await Mediator.Send(command);
        return Ok(workspaceId);
    }
}
```

### 5. Resolución de Errores Comunes de Inyección de Dependencias
Durante la integración de la API con MediatR y el contexto de base de datos, es común enfrentarse a errores de resolución de tipos en el arranque de la aplicación.

* **El Error:**
  `Unable to resolve service for type 'TodoSaaS.Application.Common.Interfaces.IApplicationDbContext' while attempting to activate 'TodoSaaS.Application.Workspaces.Commands.CreateWorkspace.CreateWorkspaceCommandHandler'`
* **La Causa:**
  Registramos `ApplicationDbContext` en el contenedor de dependencias, pero el manejador (Handler) de MediatR solicita en su constructor la interfaz `IApplicationDbContext`. El contenedor de dependencias no sabía de manera automática que al pedir esa interfaz debía entregar la implementación del DbContext.
* **La Solución:**
  Mapear la interfaz con la clase concreta en `Program.cs` usando un ciclo de vida `Scoped` (el mismo que el del DbContext):
  ```csharp
  using TodoSaaS.Application.Common.Interfaces; // namespace necesario
  
  builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
  ```
  *(Se usa `GetRequiredService` para reutilizar la misma instancia y configuración de conexión que ya maneja EF Core).*

---

## FASE 5: Ejecución y Pruebas de Funcionamiento

Validamos todo el flujo limpio levantando el servidor de desarrollo y realizando una petición HTTP POST real.

### 1. Iniciar el Servidor de la Web API
```bash
dotnet run --project TodoSaaS.WebApi/TodoSaaS.WebApi.csproj
```
El servidor arrancará e indicará las URLs en las que está escuchando (por ejemplo, `http://localhost:5003`).

### 2. Pruebas Interactivas con Swagger
1. Abrir el navegador e ingresar a `http://localhost:5003/swagger/index.html`.
2. Ubicar el endpoint `POST /api/Workspaces`.
3. Presionar **"Try it out"** y enviar el JSON de prueba:
   ```json
   {
     "name": "Mi Primer Workspace",
     "description": "Contenedor de tableros"
   }
   ```
4. Presionar **"Execute"**. La respuesta correcta es un código `200 OK` acompañado de un identificador `Guid` único autogenerado en la base de datos PostgreSQL.

---

## FASE 6: Validaciones con FluentValidation y Pipeline Behaviors

Para asegurar que los datos ingresados al sistema cumplan con las reglas de negocio antes de tocar la base de datos, implementamos validaciones automáticas mediante **Pipeline Behaviors** (Middlewares internos de MediatR) y **FluentValidation**.

### 1. Instalación de Paquetes
```bash
# Agregar FluentValidation al proyecto de Aplicación
dotnet add TodoSaaS.Application/TodoSaaS.Application.csproj package FluentValidation.DependencyInjectionExtensions -v 11.9.2
```

### 2. Clase Excepción Personalizada (`TodoSaaS.Application/Common/Exceptions/ValidationException.cs`)
Agrupa todos los errores de validación por campo para retornar un formato estandarizado (RFC 7807) en lugar de una excepción genérica:
```csharp
using FluentValidation.Results;

namespace TodoSaaS.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("Se han producido uno o más errores de validación.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}
```

### 3. Comportamiento del Pipeline (`TodoSaaS.Application/Common/Behaviors/ValidationBehavior.cs`)
Intercepta cualquier petición (Command/Query) antes de llegar a su Handler para ejecutar los validadores correspondientes.
```csharp
using MediatR;
using FluentValidation;
using ValidationException = TodoSaaS.Application.Common.Exceptions.ValidationException;

namespace TodoSaaS.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        return await next();
    }
}
```

### 4. Validador Concreto (`TodoSaaS.Application/Workspaces/Commands/CreateWorkspace/CreateWorkspaceCommandValidator.cs`)
```csharp
using FluentValidation;

namespace TodoSaaS.Application.Workspaces.Commands.CreateWorkspace;

public class CreateWorkspaceCommandValidator : AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("El nombre del espacio de trabajo es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");
    }
}
```

### 5. Encapsulado del Registro (`TodoSaaS.Application/DependencyInjection.cs`)
Clase estática para que la capa de aplicación registre sus propios servicios de manera autónoma, respetando la separación de conceptos:
```csharp
using System.Reflection;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TodoSaaS.Application.Common.Behaviors;

namespace TodoSaaS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
```

---

## Explicación Detallada de la Fase 6

### ¿Cómo funciona el flujo de validación automática?
Cuando el controlador llama a `Mediator.Send(command)`, la petición no viaja directamente al `Handler`. En su lugar, pasa a través de una tubería (pipeline) de comportamientos.

```
Petición HTTP -> Controlador -> [ Mediator.Send ]
                                       |
                                       v
                             [ ValidationBehavior ]
                                       |
                       (¿Hay fallos de validación?)
                              /          \
                            SÍ            NO
                            /              \
           [ lanza ValidationException ]   [ CreateWorkspaceCommandHandler ]
                                                    |
                                                    v
                                            [ Guarda en DB ]
```

1. **`ValidationBehavior`**: Actúa como un interceptor genérico. Recibe una lista de todos los validadores (`IValidator<TRequest>`) que coincidan con el tipo de comando enviado.
2. **`Task.WhenAll`**: Ejecuta todas las reglas de validación en paralelo para optimizar la velocidad de respuesta.
3. **`ValidationException`**: Si algún validador detecta un fallo, el comportamiento detiene la ejecución del flujo de inmediato y lanza esta excepción personalizada. Esto previene que se intente procesar un comando inválido o se guarde información corrupta en la base de datos.
4. **`DependencyInjection` de Aplicación**: En lugar de ensuciar el `Program.cs` del proyecto web registrando manualmente cada validador o comportamiento, creamos un método de extensión `AddApplicationServices`. Esto encapsula los detalles de configuración dentro de la propia capa de Aplicación.



```