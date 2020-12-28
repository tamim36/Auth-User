using Dtos;
using Dtos.ExternalProviderDtos;
using Dtos.UserDto;
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
    }
}
