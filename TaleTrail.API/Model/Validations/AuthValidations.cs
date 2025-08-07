using System.ComponentModel.DataAnnotations;

namespace TaleTrail.API.Model.Validations;

/// <summary>
/// Validation model for user signup
/// </summary>
public class SignupValidation
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required")]
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    public string Username { get; set; } = string.Empty;
}

/// <summary>
/// Validation model for user login
/// </summary>
public class LoginValidation
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Validation model for user profile updates
/// </summary>
public class UpdateProfileValidation
{
    [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Full name can only contain letters and spaces")]
    public string? FullName { get; set; }

    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
    [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    public string? Username { get; set; }

    [MaxLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
    public string? Bio { get; set; }

    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? AvatarUrl { get; set; }
}