using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;

namespace FundooNotesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager manager;

        public UsersController(IUserManager manager)
        {
            this.manager = manager;
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
           
    }
}
