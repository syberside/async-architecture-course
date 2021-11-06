using aTES.Identity.Models;
using aTES.Identity.Models.Account;
using aTES.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace aTES.Identity.Controllers
{
    public class HomeController : Controller
    {
        private readonly UsersStore _usersStore;

        public HomeController(UsersStore usersStore)
        {
            _usersStore = usersStore;
        }

        public IActionResult IndexAsync()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> UserManagementAsync()
        {
            var userName = User.GetUserName();
            var user = await _usersStore.GetByUsername(userName);
            if (user.Role == Domain.Roles.Admin)
            {
                ViewData["ShowUsers"] = true;
            }
            return View(new CreateUserModel());
        }

        [Authorize]

        [HttpPost]
        public async Task<IActionResult> UserManagementAsync(CreateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return await UserManagementAsync();
            }

            var alreadyExistUser = await _usersStore.FindByUsername(model.Username);
            if (alreadyExistUser != null)
            {
                ModelState.AddModelError("user already exists", "User with the same name already exists");
                return await UserManagementAsync();
            }
            await _usersStore.Register(model.Username, model.Password, model.Role);
            return await UserManagementAsync();
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
