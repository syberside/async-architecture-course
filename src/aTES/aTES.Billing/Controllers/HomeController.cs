using aTES.Billing.Domain;
using aTES.Billing.Models;
using aTES.Billing.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace aTES.Billing.Controllers
{
    public class HomeController : Controller
    {
        private readonly AccountingService _accountingService;
        private readonly UserService _userService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, AccountingService accountingService, UserService userService)
        {
            _logger = logger;
            _accountingService = accountingService;
            _userService = userService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var userId = CurrentUserPublicId();
            var (balance, log) = await _accountingService.GetProfileData(userId);
            var role = await GetCurrentUserRole();
            long? todayTotal = null;
            if (role == Roles.Admin || role == Roles.Acounter)
            {
                todayTotal = await _accountingService.GetTodayTotal();
            }

            return View(new AuditLogViewModel
            {
                CurrentBalance = balance,
                Log = log,
                TodayTotal = todayTotal,
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private string CurrentUserPublicId()
        {
            return User.Claims.First(x => x.Type == "sub").Value;
        }

        private async Task<Roles> GetCurrentUserRole()
        {
            var role = await _userService.GetRole(CurrentUserPublicId());
            return role;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
