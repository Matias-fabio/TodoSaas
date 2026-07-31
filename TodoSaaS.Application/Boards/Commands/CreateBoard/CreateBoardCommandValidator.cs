using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace TodoSaaS.Application.Boards.Commands.CreateBoard
{
    public class CreateBoardCommandValidator : AbstractValidator<CreateBoardCommand>
    {
        public CreateBoardCommandValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("El nombre del tablero es requerido")
                .MaximumLength(100).WithMessage("El nombre del tablero no pueda superar los 100 caracteres");

            RuleFor(v => v.Description)
                .MaximumLength(500).WithMessage("La descripcion no puede superar los 100 caracteres");
            
            RuleFor(v => v.WorkspaceId)
                .NotEmpty().WithMessage("El identificador del espacio de trabajo (WorkSpaceId) es requerido.");
        }
    }
}