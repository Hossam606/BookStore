using BookShoppingCart.Models;

namespace BookShoppingCart.Interfaces
{
    public interface IRepository
    {
        public Task<IEnumerable<Book>> GetBooks (string sTerm = "", int genreId = 0);
        public IEnumerable<Book> GetAll();
        public Task<IEnumerable<Genre>> Genres();
    }
}
