using Dtos;
using Dtos.ExternalProviderDtos;
using Dtos.UserDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Requests;
using Models.Responses;
using Models.Users;
using Repositories;
using Repositories.FacebookAuthentication;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebService.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepo;
        private readonly IMailService mailService;
        private readonly IFacebookAuthRepository facebookAuth;

        public AuthController(IAuthRepository authRepo, IMailService mailService, IFacebookAuthRepository facebookAuth)
        {
            this.authRepo = authRepo;
            this.mailService = mailService;
            this.facebookAuth = facebookAuth;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMail(MailRequestDto req)
        {
            try
            {
                await mailService.SendEmailAsync(req.Mail, req.Subject, req.Body);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            ServiceResponse<int> response = await authRepo.Register(
                new User { Username = request.Username, Email = request.Email }, request.Password
            );
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            ServiceResponse<string> response = await authRepo.Login(request.Email, request.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            SetTokenToCookie(response.Message);
            return Ok(response);
        }

        [HttpPost("Refresh-Token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            ServiceResponse<string> response = await authRepo.RefreshToken(refreshToken);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            SetTokenToCookie(response.Message);
            return Ok(response);
        }

        [HttpPost("Revoke-Token")]
        public async Task<IActionResult> RevokeToken(RevokeTokenDto request)
        {
            var token = request.Token ?? Request.Cookies["refreshToken"];
            ServiceResponse<string> response = await authRepo.RevokeToken(token);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto request)
        {
            ServiceResponse<string> response = await authRepo.ForgotPassword(request.Email);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto request)
        {
            ServiceResponse<string> response = await authRepo.ResetPassword(request.Token, request.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("facebook")]
        public async Task<IActionResult> FacebookLogin(FacebookLoginDto request)
        {
            ServiceResponse<string> response = await facebookAuth.LoginWithFacebook(request.AccessToken);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpPost("Set-Password")]
        public async Task<IActionResult> SetPassword(SetPasswordDto request)
        {
            int id = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            ServiceResponse<string> response = await authRepo.SetOrChangePassword(userId: id ,oldPassword: null, newPassword: request.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [Authorize]
        [HttpPost("Change-Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto request)
        {
            int id = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            ServiceResponse<string> response = await authRepo.SetOrChangePassword(userId: id, oldPassword: request.OldPassword, newPassword: request.NewPassword);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        public void SetTokenToCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append(
                "refreshToken", token, cookieOptions);
        }
    }
}
