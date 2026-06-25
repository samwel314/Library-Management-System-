namespace LibraryManagement.Api.Shared
{
    public class ResultT<T> : Result
    {
        public T? Data { get; }

        private ResultT(bool isSuccess,T? data, string? message,ErrorType errorType): base(isSuccess, message, errorType)
        {
            Data = data;
        }

        public static ResultT<T> Success(
            T data,
            string? message = null)
            => new(true, data, message, ErrorType.Success);

        public static new ResultT<T> Failure(
            string message,
            ErrorType errorType)
            => new(false, default, message, errorType);

    }
}
