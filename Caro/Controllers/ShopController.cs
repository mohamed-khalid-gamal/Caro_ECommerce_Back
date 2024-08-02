using Caro.Data;
using Caro.Models;
using Caro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Caro.Controllers
{
    
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public ShopController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
               .Include(p => p.Sizes)
               .Include(p => p.Images)
               .ToListAsync();

            var productViewModels = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Sizes = p.Sizes.Select(s => new ProductSizeViewModel
                {
                    Id = s.Id,
                    Size = s.Size,
                    Quantity = s.Quantity,
                    
                }).ToList(),
                Images = p.Images.Select(i => new ProductImageViewModel
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    AltText = i.AltText
                }).ToList()
            }).ToList();
            return View(productViewModels);
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public async Task<IActionResult> Blogs(int PageNo = 1)
        {
            var blogs = await _context.Blogs.ToListAsync();
            int NoOfRecordsPerPage = 3;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(blogs.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;  // Corrected variable name
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;

            blogs = blogs.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return View(blogs);
            
        }
        public async Task<IActionResult> Products(int PageNo = 1)
        {
			var products = await _context.Products
			  .Include(p => p.Sizes)
			  .Include(p => p.Images)
			  .ToListAsync();

			var productViewModels = products.Select(p => new ProductViewModel
			{
				Id = p.Id,
				Name = p.Name,
				Description = p.Description,
                Price = p.Price,
				Sizes = p.Sizes.Select(s => new ProductSizeViewModel
				{
					Id = s.Id,
					Size = s.Size,
					Quantity = s.Quantity,
					
				}).ToList(),
				Images = p.Images.Select(i => new ProductImageViewModel
				{
					Id = i.Id,
					ImageUrl = i.ImageUrl,
					AltText = i.AltText
				}).ToList()
			}).ToList();
            int NoOfRecordsPerPage = 3;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(productViewModels.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;  // Corrected variable name
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;

            productViewModels = productViewModels.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return View(productViewModels);

        }
        public IActionResult Blog()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> SingleProduct(int? Id)
        {
            var product = await _context.Products
               .Include(p => p.Sizes)
               .Include(p => p.Images)
               .FirstOrDefaultAsync(p => p.Id == Id);

            if (product == null)
            {
                return NotFound();
            }
            ShoppingCart model = new ShoppingCart()
            {
                
                Product = product,
                Count = 1,
                ProductId = product.Id,
                ApplicationUser = await _userManager.GetUserAsync(User),
                ApplicationUserId =  _userManager.GetUserAsync(User).Result.Id
            };
            ViewBag.Products=_context.Products.Include(p=>p.Sizes).Include(p=>p.Images);
            return View(model);

        }
    }
}
