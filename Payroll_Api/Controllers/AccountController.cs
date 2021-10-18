using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Payroll_Api.APIModels;
using Payroll_Api.APIRequestModels;
using Payroll_Api.APIRequestModels.User;
using Payroll_Api.Exceptions;
using Payroll_Api.Identity;
using Services.Cache;
using Services.Enums;
using Services.Helpers;
using Services.Validations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Payroll_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _appSettings = appSettings;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            ApplicationUser dbUser = await this._userManager.FindByNameAsync(registerRequest.Username);
            if (dbUser == null)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    FullName = registerRequest.FullName,
                    UserName = registerRequest.Username,
                    Email = registerRequest.Email,
                    PhoneNumber = registerRequest.MobileNumber,
                    IsDeleted = false
                };
                IdentityResult result = await this._userManager.CreateAsync(user, registerRequest.Password);
                if (result.Succeeded)
                {
                    IdentityResult roleResult = await this._userManager.AddToRoleAsync(user, UserRoles.User.ToString());
                    if (roleResult.Succeeded)
                    {
                        ApplicationUser newUser = await this._userManager.FindByNameAsync(registerRequest.Username);
                        try
                        {
                            basicResponse.Data = newUser.UserName;
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }
                    else
                        throw new PayrollException(ErrorMessages.USER_NOT_ADDED_TO_ROLE);
                }
                else
                    basicResponse.ErrorMessage = ResponseMessage.USER_NOT_CREATED.ToString();
            }
            else
                basicResponse.ErrorMessage = ResponseMessage.USER_ALREADY_EXIST.ToString();

            return Ok(basicResponse);
        }

        //[Authorize]
        [HttpPost]
        [Route("create-role")]
        public async Task<IActionResult> CreateRole(AddRoleRequest addRoleRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            bool isExist = await this._roleManager.RoleExistsAsync(addRoleRequest.RoleName);
            if (!isExist)
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = addRoleRequest.RoleName
                };
                IdentityResult result = await this._roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    basicResponse.Data = true;
                }
                else
                {
                    if (result.Errors != null)
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            throw new PayrollException(error.Description);
                        }
                    }
                }
            }
            else
                throw new PayrollException(ErrorMessages.ROLE_ALREADY_EXIST);

            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            ApplicationUser user = await this._userManager.FindByEmailAsync(loginRequest.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, false, false);
                if (result.Succeeded)
                {
                    IList<string> roles = await this._userManager.GetRolesAsync(user);
                    basicResponse.Data = _GenerateJSONWebToken(user, roles);
                }
                else if (result.IsLockedOut)
                {
                    basicResponse.ErrorMessage = ResponseMessage.USER_IS_DISABLED.ToString();
                }
                else
                {
                    basicResponse.ErrorMessage = ResponseMessage.USER_INVALID_USERNAME_PASSWORD.ToString();
                }
            }
            else
            {
                basicResponse.ErrorMessage = ResponseMessage.USER_INVALID_USERNAME_PASSWORD.ToString();
            }
            return Ok(basicResponse);
        }

        [Authorize]
        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword(LoginRequest loginRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            //ApplicationUser user = await this._userManager.FindByNameAsync(loginRequest.Email);
            //if (user != null)
            //{
            //    var result = await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, false, false);
            //    if (result.Succeeded)
            //    {
            //        IList<string> roles = await this._userManager.GetRolesAsync(user);
            //        basicResponse.Data = _GenerateJSONWebToken(user, roles);
            //    }
            //    else if (result.IsLockedOut)
            //    {
            //        basicResponse.ErrorMessage = ResponseMessage.USER_IS_DISABLED.ToString();
            //    }
            //    else
            //    {
            //        basicResponse.ErrorMessage = ResponseMessage.USER_INVALID_USERNAME_PASSWORD.ToString();
            //    }
            //}
            //else
            //{
            //    basicResponse.ErrorMessage = ResponseMessage.USER_INVALID_USERNAME_PASSWORD.ToString();
            //}
            return Ok(basicResponse);
        }
        #region Private Methods
        private TokenResponse _GenerateJSONWebToken(ApplicationUser applicationUser, IList<string> roles)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Value.JwtKey));
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, applicationUser.UserName),
                    new Claim(ClaimTypes.Email, applicationUser.Email),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim(ClaimTypes.NameIdentifier , applicationUser.Id)
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_appSettings.Value.JwtExpireTime)),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
                IssuedAt = DateTime.UtcNow,
                Issuer = _appSettings.Value.JwtIssuer,
                Audience = _appSettings.Value.JwtIssuer
            };

            SecurityToken jwtToken = _jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            TokenResponse tokenResponse = new TokenResponse()
            {
                auth_token = token,
                expiration_time = jwtToken.ValidTo.ToString(Constants.MobileDateTimeFormat),
                issue_time = jwtToken.ValidFrom.ToString(Constants.MobileDateTimeFormat),
                expires_in = (jwtToken.ValidTo - jwtToken.ValidFrom).TotalSeconds,
                role = roles.FirstOrDefault()
            };
            return tokenResponse;
        }
        #endregion
    }
}
