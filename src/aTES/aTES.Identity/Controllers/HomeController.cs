using aTES.Identity.Domain;
using aTES.Identity.Helpers;
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
            var role = await GetCurrentUserRole();
            if (role.CanAdministerUsers())
            {
                ViewData["ShowUsers"] = true;
            }
            if (role == Roles.SuperUser)
            {
                ViewData["CanTriggerStreaming"] = true;
            }
            return View(new CreateUserModel());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserManagementAsync(CreateUserModel model)
        {
            var role = await GetCurrentUserRole();
            if (!role.CanAdministerUsers())
            {
                return Forbid();
            }
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateRole(UpdateRoleModel model)
        {
            var role = await GetCurrentUserRole();
            if (!role.CanAdministerUsers())
            {
                return Forbid();
            }
            await _usersStore.UpdateRole(model.Id, model.Role);
            return RedirectToAction("UserManagement");
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


        private async Task<Roles> GetCurrentUserRole()
        {
            //TODO: replace with policy and read from claims
            var username = User.GetUserName();
            var user = await _usersStore.GetByUsername(username);
            return user.Role;
        }

        [HttpPost]
        public async Task<IActionResult> StreamUserData(StreamDataModel model)
        {
            var role = await GetCurrentUserRole();
            if (role != Roles.SuperUser)
            {
                return Forbid();
            }
            await _usersStore.StreamUser(model.Id);
            return RedirectToAction("UserManagement");
        }
    }
}
