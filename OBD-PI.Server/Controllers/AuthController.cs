using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OBDPI.Server.Data.Dtos;
using OBDPI.Server.Data.Models;
using OBDPI.Server.Filters;

namespace OBDPI.Server.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfigurationSection _configSecuritySection;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configSecuritySection = config.GetSection("Security");
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            var user = Mapper.Map<User>(userDto);

            await _userManager.CreateAsync(user, userDto.Password);
            return Ok();
        }

        [Route("login")]
        [HttpPost]
        public async Task<object> Login([FromBody] UserDto userDto)
        {
            var result = await _signInManager.PasswordSignInAsync(userDto.Username, userDto.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(u => u.UserName == userDto.Username);

                if (appUser is null) return Unauthorized("No such user.");
                if (appUser.IsVerified == false) return Unauthorized("User is not yet Verified.");

                return GenerateJwtToken(appUser);
            }

            return Unauthorized("Username or password are not correct.");
        }

        [ClaimAuth("Root", "True")]
        [Route("verify")]
        [HttpPost]
        public async Task<object> Verify([FromBody] UserDto userDto)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(userDto.Username, userDto.Password, isPersistent: false, lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(u => u.UserName == userDto.Username);

                if (appUser is null) return Unauthorized("No such user.");
                if (appUser.IsVerified) return BadRequest("User is already Verified.");

                appUser.IsVerified = true;
                var updateResult = await _userManager.UpdateAsync(appUser);

                if (updateResult.Succeeded) return Ok();
                return BadRequest("Verification failed");
            }

            return Unauthorized("Username or password are not correct.");
        }

        private object GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtCustomClaims.Verified.ToString(), user.IsVerified.ToString()),
                new Claim(JwtCustomClaims.Root.ToString(), $"{user.UserName.Equals("Administrator")}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configSecuritySection["JwtKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configSecuritySection["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configSecuritySection["JwtIssuer"],
                _configSecuritySection["JwtAudience"],
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
