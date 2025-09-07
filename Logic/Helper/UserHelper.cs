using Core.Db;
using Core.DTOs;
using Core.Model;
using Logic.IHelper;
using Microsoft.AspNetCore.Identity;


namespace Logic.Helper
{
    public class UserHelper : IUserHelper
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserHelper(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<ApplicationUser> Registration(ApplicationUserDTO applicationUserViewModel)
        {
            try
            {
                if (applicationUserViewModel != null)
                {
                    var existingUser = await _userManager.FindByEmailAsync(applicationUserViewModel.Email).ConfigureAwait(false);
                    if (existingUser != null)
                    {
                        return null;
                    }

                    var newAppUser = new ApplicationUser
                    {
                        FirstName = applicationUserViewModel.FirstName,
                        LastName = applicationUserViewModel.LastName,
                        UserName = applicationUserViewModel.Email,
                        Email = applicationUserViewModel.Email,

                    };
                    var newResult = await _userManager.CreateAsync(newAppUser, applicationUserViewModel.Password);
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
