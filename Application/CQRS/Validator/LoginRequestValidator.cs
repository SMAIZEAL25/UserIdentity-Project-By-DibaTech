using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Validator
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.email).NotEmpty().EmailAddress()
             .WithMessage("Valid email is required");

            RuleFor(x => x.Password).NotEmpty()
                 .WithMessage("Password is required");

         }
    
    }
}


