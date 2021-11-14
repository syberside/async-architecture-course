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
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, AccountingService accountingService)
        {
            _logger = logger;
            _accountingService = accountingService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var userId = CurrentUserPublicId();
            var (balance, log) = await _accountingService.GetProfileData(userId);
            return View(new AuditLogViewModel
            {
                CurrentBalance = balance,
                Log = log,
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
