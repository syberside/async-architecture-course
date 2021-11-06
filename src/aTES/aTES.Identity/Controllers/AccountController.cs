using aTES.Identity.Models.Account;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace aTES.Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly TestUserStore _users;
        private readonly IEventService _events;

        public AccountController(IIdentityServerInteractionService interaction, TestUserStore users, IEventService events)
        {
            _interaction = interaction;
            _users = users;
            _events = events;
        }

        [HttpGet]
        public ViewResult Login(string returnUrl)
        {
            var vm = BuildLoginViewModelAsync(returnUrl);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (button != "login")
            {
                if (context != null)
                {
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                // validate username/password against in-memory store
                if (_users.ValidateCredentials(model.Username, model.Password))
                {
                    var user = _users.FindByUsername(model.Username);
                    var @event = new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username, clientId: context?.Client.ClientId);
                    await _events.RaiseAsync(@event);

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

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
                ModelState.AddModelError(string.Empty, "invalid credentials");
            }

            // something went wrong, show form with error
            var vm = BuildLoginViewModelAsync(model);
            return View(vm);
        }


        private LoginViewModel BuildLoginViewModelAsync(string returnUrl)
        {
            // this is meant to short circuit the UI and only trigger the one external IdP
            var vm = new LoginViewModel
            {
                ReturnUrl = returnUrl,
            };

            return vm;
        }

        private LoginViewModel BuildLoginViewModelAsync(LoginViewModel model)
        {
            var vm = BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }
    }
}
