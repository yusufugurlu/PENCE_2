namespace API.TeknofestNLPApp.Dtos
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public int HttpStatus { get; set; }
        public int ValidationStatus { get; set; }

        public ServiceResult(bool success = false, string message = "", int status = 500, int validationStatus = 0, object data = null)
        {
            Success = success;
            Message = message;
            Data = data;
            HttpStatus = status;
            ValidationStatus = validationStatus;
        }


        public bool IsSuccess
        {
            get { return Success; }
        }


    }

    public static class Result
    {
        public static ServiceResult Success(string message = "", int status = 500, int validationStatus = 0, object data = null)
        {
            return new ServiceResult(true, message, status, validationStatus = 0, data);
        }
        public static ServiceResult Fail(string message = "", int status = 500, int validationStatus = 0, object data = null)
        {
            return new ServiceResult(false, message, status, validationStatus = 0, data);
        }

    }
}
