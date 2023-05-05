using BookShoppingCart.Data;
using BookShoppingCart.Interfaces;
using BookShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookShoppingCart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRepository _bookRepository;

        public HomeController(ILogger<HomeController> logger, IRepository bookRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository; 
        }
          

        public async Task<IActionResult> Index(string sterm = "", int genreId = 0)
        {
            var books = await _bookRepository.GetBooks(sterm, genreId);
            var genres = await _bookRepository.Genres();
            BookDisplayModel bookModel =new BookDisplayModel
            {
                Books = books,
                Genres = genres,
                STerm=sterm,
                GenderId=genreId,
            };
            return View(bookModel);
        }
        public IActionResult Privacy()
        {
           
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}