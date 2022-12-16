using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PWCExamService.Common;
using PWCExamService.Data;
using PWCExamService.Data.UnitOfWork;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PWCExamService.Managers
{
    public interface IAuthManager
    {
        Task<BaseResponseEntity<TokenResponseEntity>> Login(UserEntity request);
        Task<BaseResponseEntity<SingleResponseEntity<bool>>> Register(UserEntity user);
    }
    public class AuthManager : IAuthManager
    {
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork UoW;
        public AuthManager(IConfiguration configuration, IUnitOfWork uoW)
        {
            this.configuration = configuration;
            UoW = uoW;
        }

        public async Task<BaseResponseEntity<SingleResponseEntity<bool>>> Register(UserEntity request)
        {
            var result = new BaseResponseEntity<SingleResponseEntity<bool>>();
            try
            {
                var user = UoW.users.Find(x => x.Username == request.username).Result.FirstOrDefault();
                if (user == null)
                {
                    CreatePasswordHash(request.password, out byte[] passwordHash, out byte[] passwordSalt);
                    var fullHash = string.Format("{0}|{1}", Convert.ToBase64String(passwordHash), Convert.ToBase64String(passwordSalt));

                    await UoW.users.Insert(new Users
                    {
                        Username = request.username,
                        Password = Encryptor.EncryptString(configuration["Configurations:UPEncrypterKey"].ToString(), fullHash)
                    });
                    UoW.Save();

                    result.Code = 200;
                    result.Data = new SingleResponseEntity<bool> { result = true };
                }
                else
                {
                    result.Code = 400;
                    result.Message = "Username was already used";
                    result.Data = new SingleResponseEntity<bool> { result = false };
                }
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
                result.Data = new SingleResponseEntity<bool> { result = false };
            }

            return result;
        }
        public async Task<BaseResponseEntity<TokenResponseEntity>> Login(UserEntity request)
        {
            var result = new BaseResponseEntity<TokenResponseEntity>();

            try
            {
                var user = UoW.users.Find(x => x.Username == request.username).Result.FirstOrDefault();
                var userFullHash = Encryptor.DecryptString(configuration["Configurations:UPEncrypterKey"], user.Password);

                if (user.Username == request.username)
                {
                    if (VerifyPasswordHash(request.password, Convert.FromBase64String(userFullHash.Split('|')[0]), Convert.FromBase64String(userFullHash.Split('|')[1])))
                    {
                        result.Code = 200;
                        result.ErrorId = 0;
                        result.Data = new TokenResponseEntity { Token = await CreateToken(user.Id), TokenType = "Bearer", TokenExpiresIn = 7200 };
                    }
                    else
                    {
                        result.Code = 400;
                        result.ErrorId = 001;
                        result.Message = "Wrong password";
                    }
                }
                else
                {
                    result.Code = 400;
                    result.ErrorId = 002;
                    result.Message = "User not found";
                }
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.ErrorId = 003;
                result.Message = ex.Message;
            }


            return result;
        }

        #region PrivateMethods
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private async Task<string> CreateToken(int userId)
        {
            var payload = UoW.users.GetById(userId).Result;

            List<Claim> claims = new List<Claim>
            {
                new Claim("fullname", payload.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Configurations:UPTokenKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddSeconds(3600),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        #endregion
    }
}
