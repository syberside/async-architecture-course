using aTES.Analytics.Domain;
using aTES.Analytics.Models;
using aTES.Analytics.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace aTES.Analytics.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UsersService _usersService;

        public HomeController(ILogger<HomeController> logger, UsersService usersService)
        {
            _logger = logger;
            _usersService = usersService;
        }

        public async Task<IActionResult> Index(DashboardType type = DashboardType.MostExpensiveByDay)
        {
            if (await GetCurrentUserRole() != Roles.Admin)
            {
                return Forbid();
            }

            var deptorsCount = await _usersService.CountDeptors();
            var incomeSize = await _usersService.GetBalanceTotal();
            var mostExpensiveTaskCost = await _usersService.GetMostExpensiveTaskCost(type);
            var vm = new DashboardViewModel
            {
                Type = type,
                DeptorsCount = deptorsCount,
                IncomeSize = incomeSize,
                MostExpensiveTaskCost = mostExpensiveTaskCost,
                GeneratedAt = DateTime.Now,
            };

            return View(vm);
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

        private string CurrentUserPublicId()
        {
            return User.Claims.First(x => x.Type == "sub").Value;
        }

        private async Task<Roles> GetCurrentUserRole()
        {
            var role = await _usersService.GetRole(CurrentUserPublicId());
            return role;
        }
    }
}