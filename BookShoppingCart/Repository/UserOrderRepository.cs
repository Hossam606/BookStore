using BookShoppingCart.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShoppingCart.Repository
{
    public class UserOrderRepository: IUserOrderRepository
    {
        private readonly ApplicationDbContext _userOrder;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        public UserOrderRepository(ApplicationDbContext userOrder,
                                   IHttpContextAccessor httpContextAccessor
                                   ,UserManager<IdentityUser> userManager)
        {
                _userOrder= userOrder;
            _httpContextAccessor= httpContextAccessor;
            _userManager= userManager;
        }
        public async Task<IEnumerable<Order>> UserOrders()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User is not logged-in");
            }
            var order = await _userOrder.Orders.Include(x => x.OrderStatus)
                                               .Include(x=>x.OrderDetails)
                                               .ThenInclude(x=>x.Book)
                                               .ThenInclude(x => x.Genre) 
                                               .Where(a => a.UserId == userId)
                                               .ToListAsync();
            return order;
        }
        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
    
}
