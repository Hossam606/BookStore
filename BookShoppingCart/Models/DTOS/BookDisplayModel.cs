namespace BookShoppingCart.Models.DTOS
{
    public class BookDisplayModel
    {
        public IEnumerable<Book> Books { get; set;} 
        public IEnumerable<Genre> Genres { get; set;}
        public int GenderId { get; set; } = 0; 
        public string STerm { get; set; } = "";
    }
}
