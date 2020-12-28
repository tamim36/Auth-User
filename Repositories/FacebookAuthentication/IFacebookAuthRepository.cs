using Models.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.FacebookAuthentication
{
    public interface IFacebookAuthRepository
    {
        Task<ServiceResponse<string>> LoginWithFacebook(string access_token);
    }
}
