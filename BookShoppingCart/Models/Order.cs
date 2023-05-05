using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShoppingCart.Models
{
    [Table("Order")]
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        [Required]
        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public bool IsDeleted { get; set; } = false;
        public List<OrderDetails> OrderDetails { get; set; }  
    }
}
