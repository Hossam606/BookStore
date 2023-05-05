namespace BookShoppingCart.Repository
{
    public class BookRepository
    {
        private readonly ApplicationDbContext _context;
        public BookRepository(ApplicationDbContext context)
        {
            _context= context;
        }

        public async Task AddBook(Book book)
        {
            await _context.Set<Book>().AddAsync(book);
            await _context.SaveChangesAsync(); 
        }
    }
}
