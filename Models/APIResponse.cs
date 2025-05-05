namespace BackendAPI.Models
{
    public class APIResponse<T>
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public APIResponse()
        {
                
        }
        public APIResponse(string status, string message, T data)
        {
            Status = status;
            Message = message;
            Data = data;
        }
    }
}
