using Domain.Enums;
using System.Net;

namespace Infrastructure.Common
{
    public class ServiceResponse<T>
    {
        public ResponseStatus status { get; set; }
        public string? message { get; set; }
        public HttpStatusCode statusCode { get; set; }
        public T? data { get; set; }
        public ServiceResponse(HttpStatusCode StatusCode)
        {
            statusCode = StatusCode;
        }
        public ServiceResponse(T Data, string Message, HttpStatusCode StatusCode, ResponseStatus Status)
        {
            status = Status;
            message = Message;
            statusCode = StatusCode;
            data = Data;
        }

    }
}
