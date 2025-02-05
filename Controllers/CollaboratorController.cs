using CommonLayer.Models;
using System.Threading.Tasks;
using System;
using ManagerLayer.Interface;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RepositoryLayer.Context;
using System.Linq;
using RepositoryLayer.Entity;
using RepositoryLayer.Migrations;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using FundooNotesApp.Helper;
using NLog;
using ILogger = NLog.ILogger;

namespace FundooNotesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorManager manager;
        private readonly FunDooDBContext context;
        private readonly IBus bus;
        private readonly IDistributedCache cache;
        private readonly ILogger<CollaboratorController> logger;

        public CollaboratorController(ICollaboratorManager manager, FunDooDBContext context, IBus bus, IDistributedCache cache, ILogger<CollaboratorController> logger)
        {
            this.manager = manager;
            this.context = context;
            this.bus = bus;
            this.cache = cache;
            this.logger = logger;
        }

        [Authorize]
        [HttpPost("AddCollab")]
        public async Task<IActionResult> AddCollaborator(CollaboratorModel model)
        {
            try
            {
                if (manager.MailExist(model.Email))
                {
                    string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                    string email = User.Claims.FirstOrDefault(c => c.Type == "Email").Value;
                    Collaborator response = manager.AddCollaboration(Convert.ToInt32(data), model);
                    SendForCollaborate send = new SendForCollaborate();
                    send.SendMail(model.Email, model.NotesId, email);
                    Uri uri = new Uri("rabbitmq://localhost/FundooNotesEmailQueue");
                    var endPoint = await bus.GetSendEndpoint(uri);
                    await endPoint.Send(model);

                    return Ok(new ResponseModel<string> { Success = true, Message = "Mail sent successfully", Data = endPoint.ToString() });

                }
                    return BadRequest(new ResponseModel<string> { Success = false, Message = "Please provide valid email" });
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetCollaborators")]
        public IActionResult GetAllCollabs()
        {
            try
            { 
                var response = manager.GetAllCollaborators();
                logger.LogInformation("Fetched {Count} collaborators", response.Count);

                return Ok(new ResponseModel<List<Collaborator>> { Success = true, Message = "Fetched all collaborators", Data = response });
            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<List<Collaborator>> { Success = false, Message = "Unable to find user/notes" });
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected error: {Error}", ex.ToString());
                return StatusCode(500, new ResponseModel<List<Collaborator>>{Success = false,Message = "An unexpected error occurred. Please try again later."});
            }
        }

        [HttpGet("UsingRedisCacheCollaborator")]

        public async Task<IActionResult> GetAllCollaboratorsUsingRedisCache()
        {
            var cacheKey = "CollabList";
            string SerializedCollabLst;
            var CollabList = new List<Collaborator>();
            var RedisCollabList = await cache.GetAsync(cacheKey);
            if (RedisCollabList != null)
            {
                SerializedCollabLst = Encoding.UTF8.GetString(RedisCollabList);
                CollabList = JsonConvert.DeserializeObject<List<Collaborator>>(SerializedCollabLst);
            }

            else
            {
                CollabList = context.Collaborators.ToList();
                SerializedCollabLst = JsonConvert.SerializeObject(CollabList);
                RedisCollabList = Encoding.UTF8.GetBytes(SerializedCollabLst);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(25))
                    .SetSlidingExpiration(TimeSpan.FromSeconds(5));
                await cache.SetAsync(cacheKey, RedisCollabList, options);
            }
            return Ok(CollabList);

        }
        [Authorize]
        [HttpDelete("DeleteCollaborator")]
        public IActionResult DeleteCollab(int CollaboratorId, int NotesId)
        {
            try
            {
                string data = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                var response = manager.DeleteCollaborator(Convert.ToInt32(data), CollaboratorId, NotesId);

                logger.LogInformation(response.ToString());
                return Ok(new ResponseModel<bool> { Success = true, Message = "Collaborator Deleted", Data = response });

            }
            catch (AppException ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(new ResponseModel<bool> { Success = false, Message = "Unable to find user/notes" });
            }
        }
    }
}
        
