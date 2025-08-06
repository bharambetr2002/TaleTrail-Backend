namespace TaleTrail.API.Constants
{
    public static class SupabaseTables
    {
        // Core Tables
        public const string Users = "users";
        public const string Books = "books";
        public const string Authors = "authors";
        public const string Publishers = "publishers";
        public const string Reviews = "reviews";
        public const string Blogs = "blogs";
        public const string BlogLikes = "blog_likes";

        // This is the new name for the user's reading list table
        public const string UserBooks = "user_books";

        // Junction Tables
        public const string BookAuthors = "book_authors";
    }
}