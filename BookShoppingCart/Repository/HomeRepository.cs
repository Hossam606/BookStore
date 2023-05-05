using BookShoppingCart.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace BookShoppingCart.Repository
{
    public class HomeRepository : IRepository  
    {
        private readonly ApplicationDbContext _context;
        public HomeRepository(ApplicationDbContext db)
        {
            _context = db;  
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _context.Genres.ToListAsync();
        }

        public IEnumerable<Book> GetAll() 
        {
            return _context.Books.ToList();
        }
        public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();

            // search by title of book
            
            IEnumerable<Book> books = await (from book in _context.Books
                                             join genre in _context.Genres
                                             on book.GenreId equals genre.Id
                                             where string.IsNullOrWhiteSpace(sTerm) || (book != null && book.BookName.ToLower().StartsWith(sTerm))
                                             select new Book
                                             {
                                                 Id = book.Id,
                                                 Image = book.Image,
                                                 AuthiorName = book.AuthiorName,
                                                 BookName = book.BookName,
                                                 GenreId = book.GenreId,
                                                 Price = book.Price,
                                                 GenreName = book.Genre.GenreName,

                                             }
                         ).ToListAsync();

            // search by Kind
            if (genreId > 0)
            {
                books = books.Where(a => a.GenreId == genreId).ToList();
            }
            
            return books;

        }
    }
   
}
