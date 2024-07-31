using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Caro.Models;
using Caro.ViewModels;
using System.Threading.Tasks;
using System.Linq;

namespace Caro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            // Use await properly instead of .Result
            var users = await _userManager.Users.ToListAsync();

            var userViewModels = new List<UserViewModel>();

            // Fetch roles asynchronously in a loop
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return View(userViewModels);
        }

        public async Task<IActionResult> ManageUser(string? Id)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user != null)
            {
                var roles = await _roleManager.Roles.ToListAsync();
                var userRoles = await _userManager.GetRolesAsync(user);

                var viewModel = new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = roles.Select(role => new RoleCheckBoxViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        IsSelected = userRoles.Contains(role.Name)
                    }).ToList()
                };

                return View(viewModel);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUser(UserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = model.Roles.Where(r => r.IsSelected && !currentRoles.Contains(r.RoleName)).Select(r => r.RoleName);
            var rolesToRemove = model.Roles.Where(r => !r.IsSelected && currentRoles.Contains(r.RoleName)).Select(r => r.RoleName);

            var addRolesTask = Task.WhenAll(rolesToAdd.Select(role => _userManager.AddToRoleAsync(user, role)));
            var removeRolesTask = Task.WhenAll(rolesToRemove.Select(role => _userManager.RemoveFromRoleAsync(user, role)));

            await Task.WhenAll(addRolesTask, removeRolesTask);

            return RedirectToAction(nameof(Index));
        }
    }
}
