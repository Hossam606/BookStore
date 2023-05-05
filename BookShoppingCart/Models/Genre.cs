using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookShoppingCart.Models
{
    [Table("Genre")]
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Enter Title Of Book"), Display(Name = "Genre")]
        [MaxLength(45)]
        public string GenreName { get; set; }
        public List<Book> Books { get; set; }
    }
}
