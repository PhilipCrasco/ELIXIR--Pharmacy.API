namespace ELIXIR.API.ERRORS
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "You have made a bad reqeust!",
                401 => "Your are not Authorized!",
                404 => "Resouce not found!",
                500 => "Please contact your Administrator!",
                _ => null
            };
        }
    }
}
