using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;

namespace TaleTrail.API.Validations
{
    public static class BookValidation
    {
        public static void ValidateBook(BookDto book)
        {
            var errors = new Dictionary<string, List<string>>();

            if (string.IsNullOrWhiteSpace(book.Title))
                errors.AddError("Title", "Title is required");
            else if (book.Title.Length > 255)
                errors.AddError("Title", "Title cannot exceed 255 characters");

            if (!string.IsNullOrEmpty(book.Description) && book.Description.Length > 1000)
                errors.AddError("Description", "Description cannot exceed 1000 characters");

            if (book.PublicationYear.HasValue &&
                (book.PublicationYear < 1000 || book.PublicationYear > DateTime.Now.Year + 1))
                errors.AddError("PublicationYear", $"Publication year must be between 1000 and {DateTime.Now.Year + 1}");

            if (errors.Any())
                throw new ValidationException(errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
        }
    }
}