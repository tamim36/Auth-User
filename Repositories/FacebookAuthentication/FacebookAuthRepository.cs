using DataContracts.FacebookContracts;
using Microsoft.EntityFrameworkCore;
using Models.Responses;
using Models.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.FacebookAuthentication
{
    public class FacebookAuthRepository : IFacebookAuthRepository
    {
        private const string TokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
        private const string UserInfoUrl = "https://graph.facebook.com/me?access_token={0}&fields=id,name,gender,birthday,email,picture,first_name,last_name";
        private readonly IHttpClientFactory httpClientFactory;
        private readonly DataContext context;
        private readonly IAuthRepository authRepo;

        public FacebookAuthRepository(IHttpClientFactory httpClientFactory, DataContext context, IAuthRepository authRepo)
        {
            this.httpClientFactory = httpClientFactory;
            this.context = context;
            this.authRepo = authRepo;
        }

        public async Task<ServiceResponse<string>> LoginWithFacebook(string access_token)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            var validationTokenResult = await ValidateAccessTokenAsync(access_token);
            if (!validationTokenResult.Data.IsValid)
            {
                response.Success = false;
                response.Message = "Invalid Facebook Token";
            }

            var userInfo = await GetUserInfoAsync(access_token);
            User user = await context.Users.FirstOrDefaultAsync(x => x.Email == userInfo.Email);
            if(user == null)
            {
                user = new User { 
                Email = userInfo.Email,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Username = userInfo.Name,
                Created = DateTime.Now
                };

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                response.Data = authRepo.CreateToken(user);
                response.Message = "User Registration Successfully.";
                return response;
            }

            response.Data = authRepo.CreateToken(user);
            response.Message = "User Login Successfully.";
            return response;
        }

        private async Task<FacebookTokenValidation> ValidateAccessTokenAsync(string accessToken)
        {
            string appId = "427165374989527";
            string appSecret = "eab5764c39082eab12a8856c81e606b4";
            string formattedUrl = string.Format(TokenValidationUrl, accessToken, appId, appSecret);

            var result = await httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();
            var response = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookTokenValidation>(response);
        }

        private async Task<FacebookUserInfo> GetUserInfoAsync(string accessToken)
        {
            string formattedUrl = string.Format(UserInfoUrl, accessToken);

            var result = await httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();
            var response = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookUserInfo>(response);
        }
    }
}
