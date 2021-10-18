using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll_Api.APIRequestModels.User
{
    public class AddRoleRequest
    {
        [Required]
        public string RoleName { get; set; }
    }
}
