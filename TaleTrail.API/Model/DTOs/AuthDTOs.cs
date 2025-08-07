using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.Model.DTOs;

public class SignupRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required")]
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    public string Username { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(1, ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public User User { get; set; } = new();
}