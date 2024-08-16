using Caro.Data;
using Caro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Caro.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public CartController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User); 
            var carts = _context.ShoppingCarts
                .Include(c => c.Product)
                    .ThenInclude(p => p.Images) .Include(e=>e.ApplicationUser)
                .Where(c => c.ApplicationUser.Id == userId)
                .ToList();
            if (carts == null || !carts.Any())
            {
                TempData["EmptyCartMessage"] = "Your cart is empty. Please add some products to the cart.";

                return RedirectToAction("Products", "Shop");
            }
            return View(carts);
        }

        [HttpPost]
       
        public  IActionResult AddToCart(ShoppingCart model  ,int?id, int quantity = 1 ) {

            var product =  _context.Products
                          .Include(p => p.Sizes)
                          .Include(p => p.Images)
                          .FirstOrDefault(p => p.Id == id); 
            if (product == null) {
                return NotFound();
            }
            for(int i =0; i<product.Sizes.Count;++i)
            {
                var size = product.Sizes[i];
                if (model.Size == size.Size)
                {
                    product.Sizes.Remove(size);
                    size.Quantity -= quantity;
                    product.Sizes.Add(size);
                }
            }
            var userId = _userManager.GetUserId(User);
            var carts = _context.ShoppingCarts
                .Include(c => c.Product)
                    .ThenInclude(p => p.Images) // Include images
                .Where(c => c.ApplicationUser.Id == userId)
                .ToList();
            bool exist = false;
            foreach (var cart in carts) {
                if (cart.ProductId == product.Id && model.Size==cart.Size)
                {
                    
                    exist = true;
                    model=cart;
                    model.Count += quantity;
                    _context.ShoppingCarts.Update(model);
                }
            }
            if(!exist) {
                model.Count = quantity;
                model.Product = product;
                model.ProductId = product.Id;
                model.ApplicationUserId = _userManager.GetUserAsync(User).Result.Id;
                model.ApplicationUser = _userManager.GetUserAsync(User).Result;
                model.Id = 0;
                _context.ShoppingCarts.Add(model);

            }
            _context.Products.Update(product);
            _context.SaveChanges();
            return RedirectToAction("Products","Shop");
        }
        public IActionResult RemoveCart(int? id)
        {
            var cart = _context.ShoppingCarts.Include(e=>e.Product).ThenInclude(e=>e.Sizes).FirstOrDefault(c => c.Id == id);
            if (cart == null) {
                return NotFound();
                
            }
            cart.Product.Sizes.FirstOrDefault(e => e.Size == cart.Size).Quantity += cart.Count;
            _context.Products.Update(cart.Product);
            _context.ShoppingCarts.Remove(cart);
            _context.SaveChanges();
            var userId = _userManager.GetUserId(User); // Use async version for better performance
            var carts = _context.ShoppingCarts
                .Include(c => c.Product)
                    .ThenInclude(p => p.Images) // Include images
                .Where(c => c.ApplicationUser.Id == userId)
                .ToList();
            return View("Index",carts);
        }

    }
}
