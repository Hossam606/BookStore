using BookShoppingCart.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCart.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;
        public BookController(IBookRepository bookRepository)
        {
            _bookRepository= bookRepository;
        }
        public ActionResult Create()
        {
            return View();
        }
        //public async Task <IActionResult> Create([Bind("")])
        //{
        //    var book = _bookRepository.AddBook();
        //    return View();
        //}
    }
}
