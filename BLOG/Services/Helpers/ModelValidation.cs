using Azure;
using BLOG.Services.IServices;
using System.ComponentModel.DataAnnotations;

namespace BLOG.Services.Helpers
{
    public class ModelValidation
    {
        public static List<string?> ModelValidationResponse<T>(T instance)
        {
            ResponseType<T> response = new();
            if (instance == null)
            {
                response.isSuccess = false;
                response.Message = "Models are null";
                response.Errors.Add("Models Cannot be Null");
                return response.Errors ?? new List<string>() {"error"};
            }
            List<ValidationResult> validationResults = new ();

            var validationContext = new ValidationContext(instance);
            Validator.TryValidateObject(instance, validationContext, validationResults, true);
            return validationResults.Select(x => x.ErrorMessage).ToList();

        }
    }
}
