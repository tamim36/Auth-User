﻿using Models.Responses;
using Models.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<int>> Register(User user, string password);
        Task<ServiceResponse<string>> Login(string email, string password);
        Task<ServiceResponse<string>> ForgotPassword(string email);
        Task<ServiceResponse<string>> ResetPassword(string token, string password);
        Task<bool> EmailExists(string email);
        Task<ServiceResponse<string>> SetOrChangePassword(int userId, string oldPassword, string newPassword);
        string CreateToken(User user);
    }
}
