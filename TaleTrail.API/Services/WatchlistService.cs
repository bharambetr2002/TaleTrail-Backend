using TaleTrail.API.Models;
using TaleTrail.API.DTOs;
using TaleTrail.API.Exceptions;
using TaleTrail.API.Helpers;

namespace TaleTrail.API.Services
{
    public class WatchlistService
    {
        private readonly SupabaseService _supabase;

        public WatchlistService(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        public async Task<List<Watchlist>> GetUserWatchlistAsync(Guid userId)
        {
            var response = await _supabase.Client.From<Watchlist>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Get();
            return response.Models;
        }

        public async Task<Watchlist> AddToWatchlistAsync(WatchlistDto watchlistDto, Guid userId)
        {
            ValidationHelper.ValidateModel(watchlistDto);

            // Check if already exists
            var existing = await _supabase.Client.From<Watchlist>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Filter("book_id", Supabase.Postgrest.Constants.Operator.Equals, watchlistDto.BookId.ToString())
                .Get();

            if (existing.Models.Any())
                throw new AppException("Book already in watchlist");

            var watchlist = new Watchlist
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = watchlistDto.BookId,
                Status = watchlistDto.Status,
                AddedAt = DateTime.UtcNow
            };

            var response = await _supabase.Client.From<Watchlist>().Insert(watchlist);
            return response.Models.First();
        }
    }
}