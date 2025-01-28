using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using RepositoryLayer.Migrations;

namespace RepositoryLayer.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly FunDooDBContext context;

        public UserRepository(FunDooDBContext context)
        {
            this.context = context;
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
                return "Login successful";
            }
            return null;
        }



    }
}
