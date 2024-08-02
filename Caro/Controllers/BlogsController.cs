using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Caro.Data;
using Caro.Utility;
using Caro.Models;
using Microsoft.AspNetCore.Identity;
using Caro.ViewModels;
using Microsoft.Extensions.Hosting;
namespace Caro.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BlogsController(ApplicationDbContext context , UserManager<ApplicationUser> userManager , SignInManager<ApplicationUser> signInManager , IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Blogs.ToListAsync());
        }

        

        // GET: Blogs/Create
        public IActionResult Create()
        {
            
            return View(new BlogViewModel());
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( BlogViewModel model , IFormFile? file)
        {
            if (file==null)
            {
                ModelState.AddModelError("file", "Must Upload Image");
                return View(model);
            }
            var imagePath = await SaveImageAsync(file);
           
            Blog blog = new Blog()
            {
                Title = model.Title,
                Description = model.Description,
                Date = DateTime.Now,
                Comments = null,
                ShortDescription = model.ShortDescription,
                Image = imagePath
            };
           
          
            if (ModelState.IsValid)
            {
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            var model = new BlogViewModel()
            {
                Title = blog.Title,
                Description = blog.Description,
                Id = blog.Id,
                Comments = blog.Comments,
                ShortDescription = blog.ShortDescription,
                ExistingImage = blog.Image
              
            };

            return View(model);
        }


        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogViewModel model,IFormFile?file)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            

            if (ModelState.IsValid)
            {
                var blog = await _context.Blogs.FindAsync(id);
                if (blog == null)
                {
                    return NotFound();
                }
                if (file != null)
                {
                    var path = await SaveImageAsync(file);
                    blog.Image = path;
                }

                blog.Title = model.Title;
                blog.Description = model.Description;
                blog.ShortDescription= model.ShortDescription;
                

                _context.Update(blog);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            model.ExistingImage = _context.Blogs.FirstOrDefault(e => e.Id == id).Image;
            return View(model);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Blogs
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
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
    }
    

}
