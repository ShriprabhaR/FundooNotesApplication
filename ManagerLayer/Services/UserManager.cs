using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLayer.Models;
using ManagerLayer.Interface;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace ManagerLayer.Services
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository user;
        private readonly FunDooDBContext context;

        public UserManager(IUserRepository user, FunDooDBContext context)
        {
            this.user = user;
            this.context = context;
        }

        public Users Registration(RegisterModel model)
        {
            return user.Registration(model);
        }

        public string Login(LoginModel login)
        {
            return user.Login(login);
        }

        public ForgotPasswordModel ForgotPassword(string email)
        {
            return user.ForgotPassword(email);
        }

        public bool Reset(string email, ResetPasswordModel model)
        {
            return user.Reset(email, model);
        }

        public bool TestEmail(string email)
        {
            var checkMail = this.context.Users.FirstOrDefault(x => x.Email == email);

            if (checkMail!=null)
            {
                return true;
            }
            return false;
        }

    }
}
