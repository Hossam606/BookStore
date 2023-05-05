using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShoppingCart.Models
{
    [Table("Book")]
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Please Enter Title Of Book"),Display(Name ="BookTitle")]
        [MaxLength(45)]
        public string? BookName { get; set; }
        [Required(ErrorMessage = "Please Enter Title Of Book"), Display(Name = "AuthorName")]
        [MaxLength(45)]
        public string? AuthiorName { get; set; }
        public double Price { get; set; }
        public String? Image { get; set; }

        [Required ,Display(Name ="Genre of Book")]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public List<CartDetail> CartDetails { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
        [NotMapped]
        public string GenreName { get; set; }
    }
}
