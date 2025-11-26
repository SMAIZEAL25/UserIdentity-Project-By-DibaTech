using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Result
{
    public class ServiceResult<T>
    {
        public bool IsSucessful { get; set; }
        public string? Message { get; init; }
        public int StatusCode { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public T Data { get; set; }


        public ServiceResult()
        {
            Errors = new List<string>();
        }

        public static ServiceResult<T> Success(T data, string message, int statucode = 400, IEnumerable<string> error = null)
        {
            return new ServiceResult<T>
            {
                IsSucessful = true,
                Message = message,
                StatusCode = statucode,
                Errors = new List<string>(),
                Data = data,
            };
        }

        public static ServiceResult<T> Failure(string message, int statusCode = 400, IEnumerable<string> errors = null)
        {
            return new ServiceResult<T>
            {
                IsSucessful = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors ?? new List<string>()
            };
        }

        //public static ServiceResult<T> Success(T data, string message = "", int statusCode = 200)
        //{
        //    return new ServiceResult<T>
        //    {
        //        IsSucessful = true,
        //        Data = data,
        //        StatusCode = statusCode,
        //        Message = message,
        //        Errors = new List<string>()

        //    };
        //}

    }
}
