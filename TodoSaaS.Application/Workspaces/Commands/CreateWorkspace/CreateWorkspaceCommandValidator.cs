using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace TodoSaaS.Application.Workspaces.Commands.CreateWorkspace
{
    public class CreateWorkspaceCommandValidator : AbstractValidator<CreateWorkspaceCommand>
    {
        public CreateWorkspaceCommandValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("El nombre del espacio de trabajo es requerido.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres");

            RuleFor(v => v.Description)
                .MaximumLength(500).WithMessage("La descripcion no puede superar los 500 caracteres.");
        }
    }
}