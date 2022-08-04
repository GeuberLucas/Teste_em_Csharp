using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Teste_Csharp
{
    public class NotContainValidator<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            var username = await manager.GetUserNameAsync(user);

            if (username == password)
                return IdentityResult.Failed(new IdentityError { Description = "a senha nao pode ser igual a antiga " });

            if(password.Contains("password"))
               return IdentityResult.Failed(new IdentityError { Description = "a senha nao pode ser igual" });

            return IdentityResult.Success;
        }
    }
}
