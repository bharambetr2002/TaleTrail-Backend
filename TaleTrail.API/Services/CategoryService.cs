using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Services
{
    public class CategoryService
    {
        private readonly SupabaseService _supabase;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(SupabaseService supabase, ILogger<CategoryService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            try
            {
                var response = await _supabase.Client.From<Category>()
                    .Order("name", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                return response.Models ?? new List<Category>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all categories");
                throw new AppException($"Failed to get categories: {ex.Message}", ex);
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                var response = await _supabase.Client.From<Category>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, id.ToString())
                    .Get();

                return response.Models?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get category {CategoryId}", id);
                throw new AppException($"Failed to get category: {ex.Message}", ex);
            }
        }

        public async Task<Category> CreateCategoryAsync(CategoryDto categoryDto)
        {
            ValidationHelper.ValidateModel(categoryDto);

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categoryDto.Name
            };

            try
            {
                var response = await _supabase.Client.From<Category>().Insert(category);
                var createdCategory = response.Models?.FirstOrDefault();

                if (createdCategory == null)
                    throw new AppException("Failed to create category - no data returned");

                _logger.LogInformation("Category created successfully with ID {CategoryId}", createdCategory.Id);
                return createdCategory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create category with name {Name}", categoryDto.Name);
                throw;
            }
        }
    }
}
