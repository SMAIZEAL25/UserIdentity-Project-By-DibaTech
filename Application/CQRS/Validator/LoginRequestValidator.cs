
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
    public class LoginRequestValidator : AbstractValidator<LoginCommand>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress()
             .WithMessage("Valid email is required");

            RuleFor(x => x.Password).NotEmpty()
                 .WithMessage("Password is required");

         }
    
    }
}


