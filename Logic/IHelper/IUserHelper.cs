using Core.DTOs;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IHelper
{
    public interface IUserHelper
    {
        Task<ApplicationUser> Registration(ApplicationUserDTO applicationUserViewModel);
    }
}
