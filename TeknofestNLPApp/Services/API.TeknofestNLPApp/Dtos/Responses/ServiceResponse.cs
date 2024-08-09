namespace API.TeknofestNLPApp.Dtos.Responses
{
    public class ServiceResponse
    {
        public object Data { get; set; } // Veriler
        public bool Success { get; set; } // İşlem başarılı mı?
        public string Message { get; set; } // İşlemle ilgili mesaj
        public List<string> Errors { get; set; } // Hata mesajları
        public int Status { get; set; }

        public ServiceResponse()
        {
            Success = true; // Varsayılan olarak başarılı işlem
        }

        public ServiceResponse(Object data)
        {
            Success = true;
            Data = data;
        }

        public ServiceResponse(string message)
        {
            Success = true;
            Message = message;
        }

        public ServiceResponse(List<string> errors)
        {
            Success = false;
            Errors = errors;
        }
    }
}
