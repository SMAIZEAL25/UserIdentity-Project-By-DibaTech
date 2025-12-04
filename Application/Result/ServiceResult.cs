using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Result
{
   
        public sealed class ServiceResult<T>
        {
            public bool IsSuccessful { get; init; }
            public string Message { get; init; } = string.Empty;
            public int StatusCode { get; init; }
            public IReadOnlyCollection<string> Errors { get; init; } = Array.Empty<string>();
            public T? Data { get; init; }

            private ServiceResult() { }

            // -------------------- SUCCESS FACTORIES --------------------

            public static ServiceResult<T> Success(
                T data,
                string message = "Operation successful",
                int statusCode = 200)
            {
                return new ServiceResult<T>
                {
                    IsSuccessful = true,
                    Data = data,
                    Message = message,
                    StatusCode = statusCode
                };
            }

            public static ServiceResult<T> Created(T data, string message = "Resource created")
                => Success(data, message, 201);

            public static ServiceResult<T> NoContent(string message = "No content")
                => new ServiceResult<T>
                {
                    IsSuccessful = true,
                    Message = message,
                    StatusCode = 204
                };

            // -------------------- ERROR FACTORIES --------------------

            public static ServiceResult<T> Failure(
                string message,
                int statusCode = 400,
                IEnumerable<string>? errors = null)
            {
                return new ServiceResult<T>
                {
                    IsSuccessful = false,
                    Message = message,
                    StatusCode = statusCode,
                    Errors = errors != null
                        ? errors.ToList().AsReadOnly()
                        : Array.Empty<string>()
                };
            }

            public static ServiceResult<T> BadRequest(string message, IEnumerable<string>? errors = null)
                => Failure(message, 400, errors);

            public static ServiceResult<T> Unauthorized(string message = "Unauthorized")
                => Failure(message, 401);

            public static ServiceResult<T> Forbidden(string message = "Forbidden")
                => Failure(message, 403);

            public static ServiceResult<T> NotFound(string message = "Resource not found")
                => Failure(message, 404);

            public static ServiceResult<T> Conflict(string message, IEnumerable<string>? errors = null)
                => Failure(message, 409, errors);
        }
    }

