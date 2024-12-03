using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Domain.Entities;

namespace ToDo.Application.Validators
{
    public class ToDoValidator : AbstractValidator<Domain.Entities.ToDo>
    {
        public ToDoValidator(IHttpContextAccessor httpContextAccessor) //Validation of todo
        {
            var httpContext = httpContextAccessor.HttpContext;
            When(_ => httpContext?.Request.Method == HttpMethods.Post, () =>
            {
                RuleFor(x => x.Id)
                    .Empty().WithMessage("Id should not be provided during creation.");
            });
            When(_ => httpContext?.Request.Method == HttpMethods.Put, () =>
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0).WithMessage("Id must be greater than 0 for updates.");
            });
            RuleFor(todo => todo.ExpiryDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.");
            RuleFor(todo => todo.PercentComplete)
                .InclusiveBetween(0, 100).WithMessage("Completion percentage must be between 0 and 100.");
            RuleFor(todo => todo.IsCompleted)
                .Equal(false).When(todo => todo.PercentComplete < 100)
                .WithMessage("Todo cannot be marked as completed unless completion percentage is 100.");
        }
    }
}
