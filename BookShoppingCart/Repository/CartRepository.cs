
using BookShoppingCart.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookShoppingCart.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<int> AddItem(int bookId, int qty)
        {
            string userId = GetUserId();
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();
                // cart detail section
                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is not null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var book = _db.Books.Find(bookId);
                    cartItem = new CartDetail
                    {
                        BookId = bookId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = book.Price  // it is a new line after update
                    };
                    _db.CartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }


        public async Task<int> RemoveItem(int bookId)
        {
            //using var transaction = _db.Database.BeginTransaction();
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("user is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("Invalid cart");
                // cart detail section
                var cartItem = _db.CartDetails
                                  .FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);
                if (cartItem is null)
                    throw new Exception("Not items in cart");
                else if (cartItem.Quantity == 1)
                    _db.CartDetails.Remove(cartItem);
                else
                    cartItem.Quantity = cartItem.Quantity - 1;
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new Exception("Invalid userid");
            var shoppingCart = await _db.ShoppingCarts
                                  .Include(a => a.CartDetails)
                                  .ThenInclude(a => a.Book)
                                  .ThenInclude(a => a.Genre)
                                  .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return shoppingCart;

        }
        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        public async Task<int> GetCartItemCount(string userId="")
        {
            if (!string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }
            
            var data = await (from cart in _db.ShoppingCarts
                              join cartDetail in _db.CartDetails
                              on cart.Id equals cartDetail.ShoppingCartId
                              where cartDetail.ShoppingCartId == cart.Id
                              select new { cartDetail.Id }
                        ).ToListAsync();
            return data.Count;
        }
        public async Task<int> GetCartItemCount()
        {
            var userId = "";
            if (!string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }
            var data = await (from cart in _db.ShoppingCarts
                              join cartDetail in _db.CartDetails
                              on cart.Id equals cartDetail.ShoppingCartId
                              select new { cartDetail.Id }
                        ).ToListAsync();
            return data.Count;
        }

        public async Task<bool> DoCheckout()
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                // logic
                // move data from cartDetail to order and order detail then we will remove cart detail
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged-in");
                var cart = await GetCart(userId);
                if (cart is null)
                    throw new Exception("Invalid cart");
                var cartDetail = _db.CartDetails
                                    .Where(a => a.ShoppingCartId == cart.Id).ToList();
                if (cartDetail.Count == 0)
                    throw new Exception("Cart is empty");
                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    OrderStatusId = 1//pending
                };
                _db.Orders.Add(order);
                _db.SaveChanges();
                foreach (var item in cartDetail)
                {
                    var orderDetail = new OrderDetails
                    {
                        BookId = item.BookId,
                        OrderId = order.Id,
                        Quentity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);
                }
                _db.SaveChanges();

                // removing the cartdetails
                _db.CartDetails.RemoveRange(cartDetail);
                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }


    }
}



//using BookShoppingCart.Interfaces;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;

//namespace BookShoppingCart.Repository
//{
//    public class CartRepository : ICartRepository
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        public CartRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor
//            , UserManager<IdentityUser> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//            _httpContextAccessor = httpContextAccessor;

//        } 

//        public async Task<int> AddItem(int bookId, int qty)
//        {
//            string userId = GetUserId();
//            using var transaction = _context.Database.BeginTransaction();
//            try
//            {
//                if (string.IsNullOrEmpty(userId))
//                {
//                    throw new Exception("user is not logged-in");
//                }
//                var cart = await GetCart(userId);
//                if (cart is null)
//                {
//                    cart = new ShoppingCart
//                    {
//                        UserId = userId,
//                    };
//                    _context.ShoppingCarts.Add(cart);
//                }
//                _context.SaveChanges();
//                // cart details
//                var cartItem = _context.CartDetails
//                    .FirstOrDefault(x => x.ShoppingCartId == cart.Id && x.BookId == bookId);
//                if (cartItem is not null)
//                {
//                    cartItem.Quantity += qty;
//                }
//                else
//                {
//                    cartItem = new CartDetail
//                    {
//                        BookId = bookId,
//                        ShoppingCartId = cart.Id,
//                        Quantity = qty,

//                    };
//                    _context.CartDetails.Add(cartItem);

//                }
//                _context.SaveChanges();
//                transaction.Commit();
//            }
//            catch (Exception ex)
//            {

//            }
//            var cartItemCount = await GetCartItemCount(userId);
//            return cartItemCount;
//        }
//        public async Task<int> RemoveItem(int bookId)
//        {
//            string userId = GetUserId();
//            //using var transaction = _context.Database.BeginTransaction();
//            try
//            {

//                if (string.IsNullOrEmpty(userId))
//                {
//                    throw new Exception("user is not logged-in");
//                }
//                var cart = await GetCart(userId);
//                if (cart is null)
//                {
//                    throw new Exception("Cart is Empty");
//                }
//                _context.SaveChanges();
//                // cart details
//                var cartItem = _context.CartDetails.FirstOrDefault(x => x.ShoppingCartId == cart.Id && x.BookId == bookId);
//                if (cartItem is null)
//                {
//                    throw new Exception("No Item In Cart");
//                }
//                else if (cartItem.Quantity == 1)
//                {
//                    _context.CartDetails.Remove(cartItem);
//                }
//                else
//                {
//                    cartItem.Quantity = cartItem.Quantity - 1;
//                }
//                _context.SaveChanges();
//                //transaction.Commit();

//            }
//            catch (Exception ex)
//            {

//            }
//            var cartItemCount = await GetCartItemCount(userId);
//            return cartItemCount;
//        }
//        public async Task<ShoppingCart> GetUserCart()
//        {
//            var userId = GetUserId();
//            if (userId == null)
//                throw new Exception("Invalid userid");
//            var shoppingCart = await _context.ShoppingCarts
//                                     .Include(a => a.CartDetails)
//                                     .ThenInclude(a => a.Book)
//                                     .ThenInclude(a => a.Genre)
//                                     .Where(a => a.UserId == userId).FirstOrDefaultAsync();
//            return shoppingCart;

//        } 
//        public async Task<ShoppingCart> GetCart(string userId)
//        {
//            var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
//            return cart;
//        }
//        public async Task<int> GetCartItemCount(string userId = "")
//        {
//            if (!string.IsNullOrEmpty(userId))
//            {
//                userId = GetUserId();
//            }
//            //var data = await (from cart in _context.ShoppingCarts
//            //                  join cartDetail in _context.CartDetails
//            //                  on cart.Id equals cartDetail.ShoppingCartId
//            //                  select new { cartDetail.Id }
//            //            ).ToListAsync();
//            var data = await (from cart in _context.ShoppingCarts
//                              join cartDetail in _context.CartDetails
//                              on cart.Id equals cartDetail.ShoppingCartId
//                              select new { cartDetail.Id }
//                        ).ToListAsync();
//            return data.Count;
//        }
//        public async Task<int> GetCartItemCount()
//        {
//            var userId = "";
//            if (!string.IsNullOrEmpty(userId))
//            {
//                userId = GetUserId();
//            }
//            var data = await (from cart in _context.ShoppingCarts
//                              join cartDetail in _context.CartDetails
//                              on cart.Id equals cartDetail.ShoppingCartId
//                              select new { cartDetail.Id }
//                        ).ToListAsync();
//            return data.Count;
//        }
//        private string GetUserId()
//        {
//            var principal = _httpContextAccessor.HttpContext.User;
//            string userId = _userManager.GetUserId(principal);
//            return userId;
//        }


//    }
//}
