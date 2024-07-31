using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Caro.ViewModels;

namespace Caro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoleFormViewModel role)
        {
            if (!ModelState.IsValid)
            
                return View("Index", await _roleManager.Roles.ToListAsync());
            
            
                var roleex = await _roleManager.RoleExistsAsync(role.Name);
                if (roleex)
                {
                    ModelState.AddModelError("Name", "role is exist");
                    return View("Index", await _roleManager.Roles.ToListAsync());

                }
                else
                {
                    await _roleManager.CreateAsync(new IdentityRole(role.Name.Trim()));
                return RedirectToAction(nameof(Index));
                }

            

        }
    }
}
