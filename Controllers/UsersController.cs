using System.Threading.Tasks;
using System;
using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using RepositoryLayer.Context;
using System.Collections.Generic;
using RepositoryLayer.Migrations;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using FundooNotesApp.Helper;

namespace FundooNotesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly FunDooDBContext context;
        private readonly IUserManager manager;
        private readonly IBus bus;
        private readonly IDistributedCache distributedCache;
        private readonly ILogger<UsersController> logger;

        public UsersController(IUserManager manager, IBus bus, FunDooDBContext context, IDistributedCache distributedCache, ILogger<UsersController> logger)
        {
            this.manager = manager;
            this.bus = bus;
            this.context = context;
            this.distributedCache = distributedCache;
            this.logger = logger;
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
            try
            {
                var response = manager.Login(model);
                logger.LogInformation(response.ToString());
                return Ok(new ResponseModel<string> { Success = true, Message = "Logged in successfully", Data = response });
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<string> { Success = false, Message = "login failed"});
            }
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
            catch (AppException ex)
            {
                throw ex;

            }
        }


        [Authorize]
        [HttpPost("ResetPassword")]
        public IActionResult Reset(ResetPasswordModel model)
        {
            try
            {
                string data = User.Claims.FirstOrDefault(c => c.Type == "Email").Value;
                var response = manager.Reset(data, model);
                logger.LogInformation(response.ToString());
                return Ok(new ResponseModel<string> { Success = true, Message = "Password updated", Data = response.ToString() });
            }
            catch(AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<string> { Success = false, Message = "password not updated" });
            }
        }





        //Get all users
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {   
            List<Users> userList = new List<Users>();
            var users = context.Users.Select(x => x);
           
            if (users != null)
            { 
                foreach(Users user in users)
                {
                    userList.Add(user);
                }
                int count = userList.Count;
                return Ok(new ResponseModel<List<Users>> { Success = true, Message = "Data retrieved successfully", Data = userList });
            }
            else
            {
                return BadRequest(new ResponseModel<List<Users>> { Success = false, Message = "Can't retrieve data" });
            }

        }


        //Find a user by ID
        [HttpGet("GetUserById")]
        public IActionResult FindUserById(int UserId)
        {
            var user = context.Users.FirstOrDefault(x => x.UserId == UserId);
            if (user != null)
            {
                return Ok(new ResponseModel<Users> { Success = true, Message = "User found", Data = user });
            }
            else
            {
                return BadRequest(new ResponseModel<Users> { Success = false, Message = "User not found" });
            }

        }


        //Get users whose name starts with 'A'
        [HttpGet("NameStartsWithA")]
        public IActionResult GetUsersStartingWithA()
        {
            var users = context.Users.Where(x => x.FirstName.StartsWith("A")).ToList();

            if (users != null)
            {
                return Ok(new ResponseModel<List<Users>>{Success = true,Message = "Users found", Data = users});
            }
            else
            {
                return BadRequest(new ResponseModel<List<Users>>{Success = false,Message = "No users found"});

            }
            
        }
        //Count the total number of users
        [HttpGet("UserCount")]
        public IActionResult GetUserCount()
        {
            int userCount = context.Users.Count();
            if (userCount > 0)
            {
                return Ok(new ResponseModel<int> { Success = true, Message = "Total user count retrieved", Data = userCount });
            }
            else
            {
                return BadRequest(new ResponseModel<int> { Success = false, Message = "There is no users", Data = userCount });
            }
        }

        //Get users ordered by name (ascending & descending)
        [HttpGet("OrderUsers")]
        public IActionResult GetUsersOrderedByName()
        {
            var usersAsc = context.Users.OrderBy(x => x.FirstName).ToList();

            var usersDesc = context.Users.OrderByDescending(x => x.FirstName).ToList();

            if (usersAsc != null  && usersDesc != null)
            {
                return Ok(new ResponseModel<object> { Success = true, Message = "Users ordered by name", Data = new { usersAsc, usersDesc } });

            }
            else
            {
                return BadRequest(new ResponseModel<object> { Success = false, Message = "No users found" });
            }
        }

        //Get the average age of users
        [HttpGet("AvgAge")]
        public IActionResult GetAverageAge()
        {
            var users = context.Users.ToList();

            if (users != null)
            {
                var averageAge = users.Select(x => DateTime.Now.Year - x.DOB.Year).Average();

                return Ok(new ResponseModel<double> { Success = true, Message = "Average age calculated successfully", Data = averageAge });

            }
            else
            {
                return BadRequest(new ResponseModel<double> { Success = false, Message = "No users found to calculate average age" });
            }

            
        }

        //Get the oldest and youngest user age
        [HttpGet("OldestAndYoungest")]
        public IActionResult GetOldestAndYoungestAge()
        {
            var users = context.Users.ToList();

            var ages = users.Select(user => DateTime.Now.Year - user.DOB.Year).ToList();

            var oldestAge = ages.Max();
            var youngestAge = ages.Min();

            if (ages != null)
            {

                return Ok(new ResponseModel<object> { Success = true, Message = "Oldest and youngest age calculated successfully", Data = new { oldestAge, youngestAge } });
            }
            else
            {
                return BadRequest(new ResponseModel<object> { Success = false, Message = "Ages not found" });
            }

     
        }

        [HttpGet("UsingRedisCacheUser")]

        public async Task<IActionResult> GetAllUsersUsingRedisCache()
        {
            var cacheKey = "UsersList";
            string SerializedUserLst;
            var UsersList = new List<Users>();
            var RedisUserList = await distributedCache.GetAsync(cacheKey);
            if (RedisUserList != null)
            {
                SerializedUserLst = Encoding.UTF8.GetString(RedisUserList);
                UsersList = JsonConvert.DeserializeObject<List<Users>>(SerializedUserLst);
            }

            else
            {
                UsersList = context.Users.ToList();
                SerializedUserLst = JsonConvert.SerializeObject(UsersList);
                RedisUserList = Encoding.UTF8.GetBytes(SerializedUserLst);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(25))
                    .SetSlidingExpiration(TimeSpan.FromSeconds(5));
                await distributedCache.SetAsync(cacheKey, RedisUserList, options);
            }
            return Ok(UsersList);

        }

        [HttpPost("Register")]
        public IActionResult Registered(RegisterModel model)
        {
            try
            {
                var ifEmailExist = manager.TestEmail(model.Email);
                if (ifEmailExist)
                {
                    logger.LogWarning("The EmailId Already Exist");
                    return Ok(new { success = false, message = "The EmailId Already Exist" });
                }
                var resUser = manager.Registration(model);
                if (resUser != null)
                {
                    logger.LogInformation("Registeration Successfull");
                    return Created("User Added Successfully", new { success = true, data = resUser });
                }
                else
                {
                    logger.LogError("Registeration Unsuccessfull");
                    return BadRequest(new { success = false, message = "Something Went Wrong" });
                }
            }
            catch (AppException ex)
            {
                logger.LogCritical(ex, " Exception Thrown...");
                return NotFound(new { success = false, message = ex.Message });
            }
        }


    }
}
