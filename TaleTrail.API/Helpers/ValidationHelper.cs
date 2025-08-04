using System.ComponentModel.DataAnnotations;
using TaleTrail.API.Exceptions;

namespace TaleTrail.API.Helpers
{
    public static class ValidationHelper
    {
        public static void ValidateModel<T>(T model) where T : class
        {
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, results, true))
            {
                var errors = results
                    .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "General")
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(r => r.ErrorMessage ?? "Validation error").ToArray()
                    );

                var errorMessage = string.Join("; ", errors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}"));
                throw new System.ComponentModel.DataAnnotations.ValidationException(errorMessage);
            }
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidGuid(string guidString)
        {
            return Guid.TryParse(guidString, out _);
        }
    }
}