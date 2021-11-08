using aTES.Identity.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace aTES.Identity.Models.Account
{
    public class UpdateRoleModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Roles Role { get; set; }
    }
}
