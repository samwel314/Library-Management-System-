using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace LibraryManagement.Api.Shared
{

    public class Result
    {
        public bool IsSuccess { get; }
        public string? Message { get; }
        [JsonIgnore]
        public ErrorType ErrorType { get; }

        protected Result(bool isSuccess, string? message, ErrorType errorType)
        {
            IsSuccess = isSuccess;
            Message = message;
            ErrorType = errorType;
        }

        public static Result Success(string? message = null)
            => new(true, message, ErrorType.Success);

        public static Result Failure(string message, ErrorType errorType)
            => new(false, message, errorType);

    }
}
