using Caro.Data;
using Caro.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Caro.Controllers
{
    
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ShopController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
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

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Sizes = product.Sizes.Select(s => new ProductSizeViewModel
                {
                    Id = s.Id,
                    Size = s.Size,
                    Quantity = s.Quantity,
                    
                }).ToList(),
                Images = product.Images.Select(i => new ProductImageViewModel
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    AltText = i.AltText
                }).ToList()
            };
            ViewBag.Products=_context.Products.Include(p=>p.Sizes).Include(p=>p.Images);
            return View(model);

        }
    }
}
