using System.ComponentModel.DataAnnotations;
using TaleTrail.API.DTOs.Auth.Signup;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Validations
{
    public static class AuthValidation
    {
        public static void ValidateSignup(SignupDTO signup)
        {
            var errors = new Dictionary<string, List<string>>();

            if (string.IsNullOrWhiteSpace(signup.Email))
                errors.AddError("Email", "Email is required");
            else if (!ValidationHelper.IsValidEmail(signup.Email))
                errors.AddError("Email", "Invalid email format");

            if (string.IsNullOrWhiteSpace(signup.Password))
                errors.AddError("Password", "Password is required");
            else if (signup.Password.Length < 6)
                errors.AddError("Password", "Password must be at least 6 characters");

            if (string.IsNullOrWhiteSpace(signup.FullName))
                errors.AddError("FullName", "Full name is required");

            if (errors.Any())
            {
                var errorMessage = string.Join("; ", errors.SelectMany(kvp => kvp.Value.Select(msg => $"{kvp.Key}: {msg}")));
                throw new ValidationException(errorMessage);
            }
        }

        public static void ValidateLogin(LoginDTO login)
        {
            var errors = new Dictionary<string, List<string>>();

            if (string.IsNullOrWhiteSpace(login.Email))
                errors.AddError("Email", "Email is required");

            if (string.IsNullOrWhiteSpace(login.Password))
                errors.AddError("Password", "Password is required");

            if (errors.Any())
            {
                var errorMessage = string.Join("; ", errors.SelectMany(kvp => kvp.Value.Select(msg => $"{kvp.Key}: {msg}")));
                throw new ValidationException(errorMessage);
            }
        }
    }
}