using System.Threading.Tasks;
using System;
using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace FundooNotesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager manager;
        private readonly IBus bus;

        public UsersController(IUserManager manager, IBus bus)
        {
            this.manager = manager;
            this.bus = bus;
        }
        [HttpPost]
        [Route("Reg")]

        public IActionResult Register(RegisterModel model)
        {
            var checkMail = manager.TestEmail(model.Email);
            if (checkMail)
            {
                return BadRequest(new ResponseModel<bool> { Success = true, Message = "Mail already exist", Data = checkMail });
            }
            else
            {
                var result = manager.Registration(model);
                if (result != null)
                {
                    return Ok(new ResponseModel<Users> { Success = true, Message = "Register Successful", Data = result });
                }
                else
                {
                    return BadRequest(new ResponseModel<Users> { Success = false, Message = "Register failed" });
                }
            }
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult UserLogin(LoginModel model)
        {
            var response = manager.Login(model);
            if (response != null)
            {
                return Ok(new ResponseModel<string> { Success = true,Message="Logged in successfully", Data= response });
            }
            return BadRequest(new ResponseModel<string> { Success= false, Message="login failed", Data=response });
        }


        [HttpGet("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                ForgotPasswordModel forgotPasswordModel = manager.ForgotPassword(email);
                Send send = new Send();
                send.SendMail(forgotPasswordModel.Email, forgotPasswordModel.Token);
                Uri uri = new Uri("rabbitmq://localhost/FundooNotesEmailQueue");
                var endPoint = await bus.GetSendEndpoint(uri);
                await endPoint.Send(forgotPasswordModel);
                if (endPoint != null)
                {
                    return Ok(new ResponseModel<string> { Success = true, Message = "Mail sent successfully", Data = endPoint.ToString() });
                }
                else
                {
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Please provide valid email" });
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }


        [Authorize]
        [HttpPost("ResetPassword")]
        public IActionResult Reset(ResetPasswordModel model)
        {
            string data = User.Claims.FirstOrDefault(c => c.Type == "Email").Value;
            var response = manager.Reset(data, model);
            if (response != false)
            {
                return Ok(new ResponseModel<string> { Success = true, Message = "Password updated", Data = response.ToString() });


            }
            else
            {
                return BadRequest(new ResponseModel<string> { Success = false, Message = "password not updated" });
            }
        }

    }
}
