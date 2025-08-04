using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;

namespace TaleTrail.API.Validations
{
    public static class ReviewValidation
    {
        public static void ValidateReview(ReviewDto review)
        {
            var errors = new Dictionary<string, List<string>>();

            if (review.BookId == Guid.Empty)
                errors.AddError("BookId", "Book ID is required");

            if (review.Rating < 1 || review.Rating > 5)
                errors.AddError("Rating", "Rating must be between 1 and 5");

            if (!string.IsNullOrEmpty(review.Comment) && review.Comment.Length > 500)
                errors.AddError("Comment", "Comment cannot exceed 500 characters");

            if (errors.Any())
                throw new ValidationException(errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()));
        }
    }
}