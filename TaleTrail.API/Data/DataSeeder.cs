using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TaleTrail.API.DAO;
using TaleTrail.API.Models;

namespace TaleTrail.API.Data
{
    public static class DataSeeder
    {
        public static async Task SeedData(IServiceProvider services)
        {
            var authorDao = services.GetRequiredService<AuthorDao>();
            var publisherDao = services.GetRequiredService<PublisherDao>();
            var bookDao = services.GetRequiredService<BookDao>();
            var bookAuthorDao = services.GetRequiredService<BookAuthorDao>();

            if ((await authorDao.GetAllAsync()).Any())
            {
                Console.WriteLine("Database already contains data. Skipping seed process.");
                return;
            }

            Console.WriteLine("Database is empty. Seeding initial static data...");

            var authorMartin = await authorDao.AddAsync(new Author { Name = "George R.R. Martin" });
            var authorTolkien = await authorDao.AddAsync(new Author { Name = "J.R.R. Tolkien" });

            var publisherBantam = await publisherDao.AddAsync(new Publisher { Name = "Bantam Spectra" });
            var publisherAllen = await publisherDao.AddAsync(new Publisher { Name = "Allen & Unwin" });

            var bookGOT = await bookDao.AddAsync(new Book
            {
                Title = "A Game of Thrones",
                Description = "The first book in the A Song of Ice and Fire series.",
                PublicationYear = 1996,
                PublisherId = publisherBantam.Id,
                CreatedAt = DateTime.UtcNow
            });

            var bookHobbit = await bookDao.AddAsync(new Book
            {
                Title = "The Hobbit",
                Description = "A fantasy novel and children's book.",
                PublicationYear = 1937,
                PublisherId = publisherAllen.Id,
                CreatedAt = DateTime.UtcNow
            });

            await bookAuthorDao.AddAsync(new BookAuthor { BookId = bookGOT.Id, AuthorId = authorMartin.Id });
            await bookAuthorDao.AddAsync(new BookAuthor { BookId = bookHobbit.Id, AuthorId = authorTolkien.Id });

            Console.WriteLine("Database seeding complete.");
        }
    }
}