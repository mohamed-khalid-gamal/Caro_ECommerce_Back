using Caro.Models;
using Caro.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using Caro.Data;
namespace Caro.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<IActionResult> Dash()
        {
            return View();
        }
        // Display the list of products
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

        // Display the product creation form
        public IActionResult Create()
        {
            return View(new ProductViewModel());
        }

        // Handle product creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Sizes = model.Sizes.Select(s => new ProductSize
                    {
                        Size = s.Size,
                        Quantity = s.Quantity,

                    }).ToList()
                };
                if(model.ImageFiles == null)
                {
                    ModelState.AddModelError("ImageFiles", "Must upload at least one photo");
                }
                // Save images
                if (model.ImageFiles != null && model.ImageFiles.Count > 0)
                {
                    foreach (var imageFile in model.ImageFiles)
                    {
                        var imagePath = await SaveImageAsync(imageFile);
                        if (imagePath != null)
                        {
                            product.Images.Add(new ProductImage
                            {
                                ImageUrl = imagePath,
                                AltText = $"{model.Name} Image"
                            });
                        }
                    }
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET Edit
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

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

            return View(model);
        }

        // POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Products
                        .Include(p => p.Sizes)
                        .Include(p => p.Images)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (product == null)
                    {
                        return NotFound();
                    }

                    product.Name = model.Name;
                    product.Description = model.Description;
                    // Manage sizes
                    var existingSizeIds = model.Sizes.Select(s => s.Id).ToList();
                    var sizesToRemove = product.Sizes.Where(s => !existingSizeIds.Contains(s.Id)).ToList();

                    foreach (var size in sizesToRemove)
                    {
                        _context.ProductSizes.Remove(size);
                    }
                    
                    if (!string.IsNullOrEmpty(model.ImageToRemove))
                    {
                        var AllImageToRemove = model.ImageToRemove.Split(',');
                        if (AllImageToRemove.Length == product.Images.Count && model.ImageFiles==null)
                        {
                            ModelState.AddModelError("ImageFiles", "Must be at least one image");
                            return View(model);
                        }
                        foreach (var imageToRemove in AllImageToRemove)
                        {

                            DeleteImageAsync(imageToRemove);
                            var ImageToremoveFromDatabase = await _context.ProductImages.SingleOrDefaultAsync(m => m.ImageUrl == imageToRemove);
                            _context.ProductImages.Remove(ImageToremoveFromDatabase);
                            _context.SaveChanges();
                        }
                    }

                    foreach (var sizeModel in model.Sizes)
                    {
                        var existingSize = product.Sizes.FirstOrDefault(s => s.Id == sizeModel.Id);
                        if (existingSize != null)
                        {
                            existingSize.Size = sizeModel.Size;
                            existingSize.Quantity = sizeModel.Quantity;

                        }
                        else
                        {
                            product.Sizes.Add(new ProductSize
                            {
                                Size = sizeModel.Size,
                                Quantity = sizeModel.Quantity,
                            });
                        }
                    }

                    foreach (var imageFile in model.ImageFiles ?? Enumerable.Empty<IFormFile>())
                    {
                        var imagePath = await SaveImageAsync(imageFile);
                        if (imagePath != null)
                        {
                            product.Images.Add(new ProductImage
                            {
                                ImageUrl = imagePath,
                                AltText = $"{model.Name} Image"
                            });
                        }
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // Display the product deletion confirmation
        // GET Delete
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.Sizes)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                _context.ProductSizes.RemoveRange(product.Sizes);
                _context.ProductImages.RemoveRange(product.Images);

                foreach (var image in product.Images)
                {
                    var fullPath = Path.Combine(_hostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        // Save the uploaded image to a specific path
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/uploads/" + uniqueFileName;
        }

        private async Task<bool> DeleteImageAsync(string ImageUrl)
        {

            if (string.IsNullOrEmpty(ImageUrl))
            {
                return false;
            }
            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, ImageUrl.TrimStart('/').Replace('/', '\\'));



            if (!System.IO.File.Exists(uploadsFolder))
            {
                return false;
            }


            try
            {



                System.IO.File.Delete(uploadsFolder);
                return true;
            }
            catch (Exception)
            {
                // Handle any exceptions here, possibly logging them or rethrowing
                return false;
            }
        }
    }

}
