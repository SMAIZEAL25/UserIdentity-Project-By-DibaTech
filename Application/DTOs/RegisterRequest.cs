using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record RegisterRequest(
     string FirstName,
     string LastName,
     string Email,
     string Password,
     string ConfirmPassword);
}
