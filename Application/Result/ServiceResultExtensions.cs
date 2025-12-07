using Microsoft.AspNetCore.Mvc;


namespace Application.Result
{

    public static class ServiceResultExtensions
        {
            public static IActionResult ToActionResult<T>(this ServiceResult<T> result)
            {
                var response = new
                {
                    success = result.IsSuccessful,
                    message = result.Message,
                    data = result.IsSuccessful ? result.Data : default,
                    errors = result.IsSuccessful ? null : result.Errors
                };

                return new ObjectResult(response)
                {
                    StatusCode = result.StatusCode
                };
            }
        }
    }

