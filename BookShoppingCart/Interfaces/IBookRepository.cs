namespace BookShoppingCart.Interfaces
{
    public interface IBookRepository
    {
        Task AddBook(Book book);
    }
}
