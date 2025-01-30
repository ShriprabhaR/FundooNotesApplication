using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CommonLayer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Migrations;

namespace RepositoryLayer.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly FunDooDBContext context;
        private readonly IConfiguration configuration;

        public UserRepository(FunDooDBContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public Users Registration(RegisterModel model)
        {
            Users users = new Users();
            users.FirstName = model.FirstName;
            users.LastName = model.LastName;
            users.DOB = model.DOB;
            users.Gender = model.Gender;
            users.Email = model.Email;
            users.Password = EncodePassword(model.Password);
            context.Users.Add(users);
            context.SaveChanges();
            return users;
        }

        public static string EncodePassword(string password)
        {
            try
            {
                byte[] encData = new byte[password.Length];
                encData = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        
        
        public string Login(LoginModel login)
        {
            var checkMailExist = this.context.Users.FirstOrDefault(a=> a.Email == login.Email && a.Password== EncodePassword(login.Password));
            if (checkMailExist != null)
            {
                string token = GenerateToken(checkMailExist.UserId, checkMailExist.Email);
                return token;
            }
            return null;
        }

        private string GenerateToken(int UserId, string EmailId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("UserId", UserId.ToString()),
                new Claim("Email", EmailId)
            };
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public ForgotPasswordModel ForgotPassword(string email)
        {

            Users user = context.Users.ToList().Find(a => a.Email == email);
            if (user != null)
            {
                ForgotPasswordModel forgotPasswordModel = new ForgotPasswordModel();
                forgotPasswordModel.UserId = user.UserId;
                forgotPasswordModel.Email = user.Email;
                forgotPasswordModel.Token = GenerateToken(user.UserId, user.Email);
                return forgotPasswordModel;
            }
            else
            {
                throw new Exception("User not exist for this email");
            }
        }

        public bool Reset(string email, ResetPasswordModel model)
        {
            Users user = context.Users.FirstOrDefault(a => a.Email == email);
            if (user != null)
            {
                if (model.Password == model.ConfirmPassword)
                {
                    user.Password = EncodePassword(model.ConfirmPassword);
                    context.Users.Update(user);
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

    }
}
