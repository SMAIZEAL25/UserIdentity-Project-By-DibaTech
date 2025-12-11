using Application.DTOs;
using Application.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.CQRS.Command
{
    public partial record RegisterCommand(

        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber,
        string Password,
        string ConfrimedPassword) : IRequest<ServiceResult<LoginResponse>>
    {

    };
    
    



   

};
    

    

