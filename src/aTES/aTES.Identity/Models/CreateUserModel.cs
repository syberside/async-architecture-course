using aTES.Identity.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace aTES.Identity.Models.Account
{
    public class CreateUserModel
    {

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public Roles Role { get; set; }

        public IEnumerable<SelectListItem> DropDownItems(Roles? selected)
        {
            foreach (var value in new[] { Roles.RegularPopug, Roles.Admin, })
            {
                yield return new SelectListItem
                {
                    Text = value.ToString(),
                    Value = ((int)value).ToString(),
                    Selected = selected == value,
                };
            }
        }
    }
}
