using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;

namespace ManagerLayer.Interface
{
    public interface IUserManager
    {
        public Users Registration(RegisterModel model);
        public string Login(LoginModel login);
        public bool TestEmail(string email);

        public ForgotPasswordModel ForgotPassword(string email);

        public bool Reset(string email, ResetPasswordModel model);
    }
}
