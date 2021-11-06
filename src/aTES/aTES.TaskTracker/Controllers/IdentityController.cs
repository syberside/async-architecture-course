using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace aTES.Identity.Controllers
{
    [Route("identity")]
    public class IdentityController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
