namespace AKG_Task_Managment_System.DTOs.Common
{
    public class ResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public T? Data { get; set; }

        public static ResponseDto<T> SuccessResponse(T data, string message = "Success")
            => new() { Success = true, StatusCode = 200, Message = message, Data = data };

        public static ResponseDto<T> ErrorResponse(string message, int code = 400)
            => new() { Success = false, StatusCode = code, Message = message };
    }
}
