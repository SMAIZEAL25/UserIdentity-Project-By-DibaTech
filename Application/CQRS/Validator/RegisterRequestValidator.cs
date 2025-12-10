
using Application.CQRS.Command;
using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Validator
{
    public class RegisterRequestValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches(@"[A-Z]").WithMessage("Password must contain uppercase")
                .Matches(@"[a-z]").WithMessage("Password must contain lowercase")
                .Matches(@"[0-9]").WithMessage("Password must contain a number")
                .Matches(@"[\W_]").WithMessage("Password must contain a special character");

            RuleFor(x => x.ConfrimedPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match");
        }
    }
}
