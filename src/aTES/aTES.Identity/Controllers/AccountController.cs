using aTES.Identity.Models.Account;
using aTES.Identity.Services;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace aTES.Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _identityService;
        private readonly UsersStore _usersStore;

        public AccountController(IIdentityServerInteractionService interaction, UsersStore userStore)
        {
            _identityService = interaction;
            _usersStore = userStore;
        }

        [HttpGet]
        public ViewResult Login(string returnUrl)
        {
            var vm = new LoginViewModel
            {
                ReturnUrl = returnUrl,
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string button)
        {
            var context = await _identityService.GetAuthorizationContextAsync(model.ReturnUrl);

            if (button != "login")
            {
                if (context != null)
                {
                    await _identityService.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return Redirect("~/");
                }
            }

            if (!ModelState.IsValid)
            {
                var vm = new LoginViewModel
                {
                    ReturnUrl = model.ReturnUrl,
                    Username = model.Username,
                    RememberLogin = model.RememberLogin,
                };
                return View(vm);
            }

            // validate username/password against in-memory store
            var credsAreValid = await _usersStore.ValidateCredentials(model.Username, model.Password);
            if (!credsAreValid)
            {
                ModelState.AddModelError(string.Empty, "invalid credentials");
                var vm = new LoginViewModel
                {
                    ReturnUrl = model.ReturnUrl,
                    Username = model.Username,
                    RememberLogin = model.RememberLogin,
                };
                return View(vm);

            }

            var user = await _usersStore.FindByUsername(model.Username);
            AuthenticationProperties props = null;
            if (model.RememberLogin)
            {
                props = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1),
                };
            };

            // issue authentication cookie with subject ID and username
            var isuser = new IdentityServerUser(user.SubjectId)
            {
                DisplayName = user.Username
            };

            await HttpContext.SignInAsync(isuser, props);

            if (context != null)
            {
                return Redirect(model.ReturnUrl);
            }

            // request for a local page
            if (Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            else if (string.IsNullOrEmpty(model.ReturnUrl))
            {
                return Redirect("~/");
            }
            else
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            return await DoLogout(logoutId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoLogout(string logoutId)
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                await HttpContext.SignOutAsync();
            }

            var logout = await _identityService.GetLogoutContextAsync(logoutId);
            if (logout?.PostLogoutRedirectUri != null)
            {
                return Redirect(logout.PostLogoutRedirectUri);
            }


            return View("LoggedOut", logoutId);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
