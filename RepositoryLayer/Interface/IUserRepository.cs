using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IUserRepository
    {
        public Users Registration(RegisterModel model);
        public string Login(LoginModel login);

        public ForgotPasswordModel ForgotPassword(string email);
        public bool Reset(string email, ResetPasswordModel model);

    }
}
